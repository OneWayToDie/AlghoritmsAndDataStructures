using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using AlghoritmsAndDataStructures.Helpers;

namespace AlghoritmsAndDataStructures.Views
{
	public partial class Cube3DWindow : Window
	{
		private double _edge;
		private double _faceArea;
		private double _totalSurface;
		private double _volume;
		private double _baseDistance;
		private Point _lastMousePos;
		private ModelVisual3D _cubeContainer;
		private ModelVisual3D _axesContainer;

		private Color _currentColor = Colors.Blue;
		private double _currentOpacity = 1.0;

		private DispatcherTimer _rotationTimer;
		private bool _isRotating = false;
		private double _rotationAngle = 0;
		private Vector3D _rotationAxis = new Vector3D(0, 1, 0);
		private Button _selectedAxisButton = null;

		// Для подсветки грани
		private GeometryModel3D _highlightedModel;
		private DiffuseMaterial _originalMaterial;
		private DiffuseMaterial _highlightMaterial;

		public Cube3DWindow(double edge, double faceArea, double totalSurface, double volume)
		{
			InitializeComponent();
			_edge = edge;
			_faceArea = faceArea;
			_totalSurface = totalSurface;
			_volume = volume;

			_rotationTimer = new DispatcherTimer();
			_rotationTimer.Interval = TimeSpan.FromMilliseconds(20);
			_rotationTimer.Tick += RotationTimer_Tick;

			_highlightMaterial = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(180, 255, 255, 100)));

			this.Loaded += (s, e) =>
			{
				BuildCube();
				BuildAxes();
				UpdateEdgeLabel();
				UpdateInfoPanel();
				AdjustCameraToCube();
				ZoomSlider.Value = 50;
				OpacitySlider.Value = 100;
				HighlightAxisButton(AxisYButton);
				_selectedAxisButton = AxisYButton;
			};
		}

		private void BuildCube()
		{
			var scene = SceneVisual;
			scene.Children.Clear();

			_cubeContainer = new ModelVisual3D();
			_axesContainer = new ModelVisual3D();
			scene.Children.Add(_cubeContainer);
			scene.Children.Add(_axesContainer);

			// Материал граней
			SolidColorBrush faceBrush = new SolidColorBrush(_currentColor) { Opacity = _currentOpacity };
			DiffuseMaterial faceMaterial = new DiffuseMaterial(faceBrush);

			// Материал рёбер — белый
			SolidColorBrush edgeBrush = new SolidColorBrush(Colors.White) { Opacity = _currentOpacity * 0.9 };
			DiffuseMaterial edgeMaterial = new DiffuseMaterial(edgeBrush);

			double s = _edge;
			Point3D[] vertices = new Point3D[]
			{
				new Point3D(-s/2, -s/2, -s/2), new Point3D(s/2, -s/2, -s/2),
				new Point3D(s/2, s/2, -s/2), new Point3D(-s/2, s/2, -s/2),
				new Point3D(-s/2, -s/2, s/2), new Point3D(s/2, -s/2, s/2),
				new Point3D(s/2, s/2, s/2), new Point3D(-s/2, s/2, s/2)
			};

			int[][] faces = new int[][]
			{
				new int[] {0,1,2,3}, new int[] {4,7,6,5},
				new int[] {0,4,5,1}, new int[] {3,2,6,7},
				new int[] {0,3,7,4}, new int[] {1,5,6,2}
			};

			for (int f = 0; f < faces.Length; f++)
			{
				MeshGeometry3D mesh = new MeshGeometry3D();
				int[] face = faces[f];
				mesh.Positions.Add(vertices[face[0]]);
				mesh.Positions.Add(vertices[face[1]]);
				mesh.Positions.Add(vertices[face[2]]);
				mesh.Positions.Add(vertices[face[3]]);
				mesh.TriangleIndices = new Int32Collection(new int[] { 0, 1, 2, 0, 2, 3 });

				Vector3D normal = Vector3D.CrossProduct(
					vertices[face[1]] - vertices[face[0]],
					vertices[face[2]] - vertices[face[0]]
				);
				normal.Normalize();
				mesh.Normals = new Vector3DCollection(new Vector3D[] { normal, normal, normal, normal });

				var model = new GeometryModel3D(mesh, faceMaterial);
				model.BackMaterial = faceMaterial;

				var visual = new ModelVisual3D();
				visual.Content = model;
				_cubeContainer.Children.Add(visual);
			}

			// РЁБРА
			int[][] edgeIndices = new int[][]
			{
				new int[] {0,1}, new int[] {1,2}, new int[] {2,3}, new int[] {3,0},
				new int[] {4,5}, new int[] {5,6}, new int[] {6,7}, new int[] {7,4},
				new int[] {0,4}, new int[] {1,5}, new int[] {2,6}, new int[] {3,7}
			};
			double thickness = 0.05;

			foreach (var edge in edgeIndices)
			{
				Point3D p1 = vertices[edge[0]];
				Point3D p2 = vertices[edge[1]];
				Model3D edgeModel = CreateEdgeModel(p1, p2, edgeMaterial, thickness);
				if (edgeModel != null)
				{
					var visual = new ModelVisual3D();
					visual.Content = edgeModel;
					_cubeContainer.Children.Add(visual);
				}
			}

			// Восстанавливаем вращение
			if (_rotationAngle != 0)
			{
				_cubeContainer.Transform = new RotateTransform3D(
					new AxisAngleRotation3D(_rotationAxis, _rotationAngle * 180 / Math.PI)
				);
			}
		}

		private Model3D CreateEdgeModel(Point3D p1, Point3D p2, DiffuseMaterial material, double thickness)
		{
			Vector3D dir = p2 - p1;
			double length = dir.Length;
			if (length < 0.001) return null;
			dir.Normalize();

			Vector3D up = (Math.Abs(dir.Y) < 0.9) ? new Vector3D(0, 1, 0) : new Vector3D(1, 0, 0);
			Vector3D right = Vector3D.CrossProduct(dir, up);
			right.Normalize();
			up = Vector3D.CrossProduct(right, dir);
			up.Normalize();

			double half = thickness / 2;
			Vector3D offset1 = right * half + up * half;
			Vector3D offset2 = -right * half + up * half;
			Vector3D offset3 = -right * half - up * half;
			Vector3D offset4 = right * half - up * half;

			Point3D[] corners = new Point3D[8];
			corners[0] = p1 + offset1;
			corners[1] = p1 + offset2;
			corners[2] = p1 + offset3;
			corners[3] = p1 + offset4;
			corners[4] = p2 + offset1;
			corners[5] = p2 + offset2;
			corners[6] = p2 + offset3;
			corners[7] = p2 + offset4;

			MeshGeometry3D mesh = new MeshGeometry3D();
			foreach (var pt in corners) mesh.Positions.Add(pt);

			int[] triIndices = new int[]
			{
				0,1,5, 0,5,4,
				1,2,6, 1,6,5,
				2,3,7, 2,7,6,
				3,0,4, 3,4,7,
				0,3,2, 0,2,1,
				4,5,6, 4,6,7
			};
			mesh.TriangleIndices = new Int32Collection(triIndices);
			mesh.Normals = null;

			GeometryModel3D model = new GeometryModel3D(mesh, material);
			model.BackMaterial = material;
			return model;
		}

		private void BuildAxes()
		{
			if (_axesContainer == null) return;
			_axesContainer.Children.Clear();

			double axisLength = _edge * 1.8;
			double shaftRadius = 0.025;
			double coneRadius = 0.08;
			double coneHeight = 0.25;

			Vector3D[] directions = new Vector3D[]
			{
				new Vector3D(1, 0, 0),
				new Vector3D(0, 1, 0),
				new Vector3D(0, 0, 1)
			};
			Color[] colors = new Color[]
			{
				Color.FromRgb(255, 80, 80),
				Color.FromRgb(80, 255, 80),
				Color.FromRgb(80, 80, 255)
			};

			for (int i = 0; i < 3; i++)
			{
				Vector3D dir = directions[i];
				Color color = colors[i];
				Point3D start = new Point3D(0, 0, 0);
				Point3D end = start + dir * axisLength;

				SolidColorBrush brush = new SolidColorBrush(color);
				DiffuseMaterial material = new DiffuseMaterial(brush);

				MeshGeometry3D shaftMesh = CreateCylinderMesh(start, end, shaftRadius, 8);
				if (shaftMesh != null)
				{
					GeometryModel3D shaftModel = new GeometryModel3D(shaftMesh, material);
					shaftModel.BackMaterial = material;
					var visual = new ModelVisual3D();
					visual.Content = shaftModel;
					_axesContainer.Children.Add(visual);
				}

				Point3D coneStart = end - dir * coneHeight;
				Point3D coneEnd = end;
				MeshGeometry3D coneMesh = CreateConeMesh(coneStart, coneEnd, coneRadius, 8);
				if (coneMesh != null)
				{
					GeometryModel3D coneModel = new GeometryModel3D(coneMesh, material);
					coneModel.BackMaterial = material;
					var visual = new ModelVisual3D();
					visual.Content = coneModel;
					_axesContainer.Children.Add(visual);
				}
			}
		}

		// ---------- ВСПОМОГАТЕЛЬНЫЕ 3D-ПРИМИТИВЫ ----------
		private MeshGeometry3D CreateCylinderMesh(Point3D p1, Point3D p2, double radius, int segments = 8)
		{
			Vector3D dir = p2 - p1;
			double length = dir.Length;
			if (length < 0.001) return null;
			dir.Normalize();

			Vector3D up = (Math.Abs(dir.Y) < 0.9) ? new Vector3D(0, 1, 0) : new Vector3D(1, 0, 0);
			Vector3D right = Vector3D.CrossProduct(dir, up);
			right.Normalize();
			up = Vector3D.CrossProduct(right, dir);
			up.Normalize();

			MeshGeometry3D mesh = new MeshGeometry3D();
			List<Point3D> vertices = new List<Point3D>();
			List<int> triangles = new List<int>();

			for (int i = 0; i <= segments; i++)
			{
				double angle = 2 * Math.PI * i / segments;
				Vector3D offset = right * Math.Cos(angle) * radius + up * Math.Sin(angle) * radius;
				vertices.Add(p1 + offset);
				vertices.Add(p2 + offset);
			}

			for (int i = 0; i < segments; i++)
			{
				int i0 = i * 2;
				int i1 = i * 2 + 1;
				int i2 = (i + 1) * 2;
				int i3 = (i + 1) * 2 + 1;
				triangles.Add(i0); triangles.Add(i2); triangles.Add(i1);
				triangles.Add(i2); triangles.Add(i3); triangles.Add(i1);
			}

			int centerIndex1 = vertices.Count;
			vertices.Add(p1);
			for (int i = 0; i < segments; i++)
			{
				int i0 = i * 2;
				int i1 = (i + 1) * 2;
				triangles.Add(i0); triangles.Add(i1); triangles.Add(centerIndex1);
			}

			int centerIndex2 = vertices.Count;
			vertices.Add(p2);
			for (int i = 0; i < segments; i++)
			{
				int i0 = i * 2 + 1;
				int i1 = (i + 1) * 2 + 1;
				triangles.Add(i0); triangles.Add(centerIndex2); triangles.Add(i1);
			}

			foreach (var v in vertices) mesh.Positions.Add(v);
			foreach (int idx in triangles) mesh.TriangleIndices.Add(idx);

			return mesh;
		}

		private MeshGeometry3D CreateConeMesh(Point3D baseCenter, Point3D tip, double radius, int segments = 8)
		{
			Vector3D dir = tip - baseCenter;
			double height = dir.Length;
			if (height < 0.001) return null;
			dir.Normalize();

			Vector3D up = (Math.Abs(dir.Y) < 0.9) ? new Vector3D(0, 1, 0) : new Vector3D(1, 0, 0);
			Vector3D right = Vector3D.CrossProduct(dir, up);
			right.Normalize();
			up = Vector3D.CrossProduct(right, dir);
			up.Normalize();

			MeshGeometry3D mesh = new MeshGeometry3D();
			List<Point3D> vertices = new List<Point3D>();
			List<int> triangles = new List<int>();

			for (int i = 0; i <= segments; i++)
			{
				double angle = 2 * Math.PI * i / segments;
				Vector3D offset = right * Math.Cos(angle) * radius + up * Math.Sin(angle) * radius;
				vertices.Add(baseCenter + offset);
			}
			int tipIndex = vertices.Count;
			vertices.Add(tip);

			for (int i = 0; i < segments; i++)
			{
				int i0 = i;
				int i1 = (i + 1);
				triangles.Add(i0); triangles.Add(i1); triangles.Add(tipIndex);
			}

			int centerIndex = vertices.Count;
			vertices.Add(baseCenter);
			for (int i = 0; i < segments; i++)
			{
				int i0 = i;
				int i1 = (i + 1);
				triangles.Add(i0); triangles.Add(centerIndex); triangles.Add(i1);
			}

			foreach (var v in vertices) mesh.Positions.Add(v);
			foreach (int idx in triangles) mesh.TriangleIndices.Add(idx);

			return mesh;
		}

		// ---------- ИЗМЕНЕНИЕ ЦВЕТА И ПРОЗРАЧНОСТИ ----------
		private void SetCubeColor(Color color)
		{
			_currentColor = color;
			BuildCube();
			BuildAxes();
			if (_selectedAxisButton != null)
				HighlightAxisButton(_selectedAxisButton);
		}

		private void SetCubeOpacity(double opacity)
		{
			_currentOpacity = opacity;
			BuildCube();
			BuildAxes();
			if (_selectedAxisButton != null)
				HighlightAxisButton(_selectedAxisButton);
		}

		private void ColorButton_Click(object sender, RoutedEventArgs e)
		{
			var btn = sender as Button;
			if (btn == null) return;
			var brush = btn.Background as SolidColorBrush;
			if (brush != null)
			{
				SetCubeColor(brush.Color);
			}
		}

		private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (OpacityValueText == null) return;
			double val = e.NewValue;
			OpacityValueText.Text = val.ToString("0");
			double opacity = val / 100.0;
			SetCubeOpacity(opacity);
		}

		// ---------- КАМЕРА И МАСШТАБ ----------
		private void AdjustCameraToCube()
		{
			_baseDistance = _edge * 3 + 5;
			UpdateZoom(ZoomSlider.Value);
		}

		private void UpdateZoom(double zoomValue)
		{
			if (Camera == null) return;
			double scale = Math.Pow(2, (zoomValue - 50) / 50.0);
			double distance = _baseDistance / scale;
			if (distance < 0.5) distance = 0.5;
			Camera.Position = new Point3D(distance, distance, distance);
			Camera.LookDirection = new Vector3D(-distance, -distance, -distance);
			Camera.UpDirection = new Vector3D(0, 1, 0);
		}

		private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (ZoomValueText == null || Camera == null) return;
			double val = e.NewValue;
			ZoomValueText.Text = val.ToString("0");
			UpdateZoom(val);
		}

		private void UpdateEdgeLabel()
		{
			EdgeLabel.Text = string.Format("{0:F2}", _edge);
		}

		private void UpdateInfoPanel()
		{
			FaceAreaText.Text = string.Format("{0:F2}", _faceArea);
			TotalSurfaceText.Text = string.Format("{0:F2}", _totalSurface);
			VolumeText.Text = string.Format("{0:F2}", _volume);
		}

		// ---------- СБРОС ВИДА ----------
		private void ResetButton_Click(object sender, RoutedEventArgs e)
		{
			_rotationAngle = 0;
			if (_cubeContainer != null)
			{
				_cubeContainer.Transform = Transform3D.Identity;
			}
			AdjustCameraToCube();
			if (_isRotating)
			{
				_rotationTimer.Stop();
				_isRotating = false;
				RotateButton.Content = "🔄";
			}
		}

		// ---------- АВТОВРАЩЕНИЕ ----------
		private void RotationTimer_Tick(object sender, EventArgs e)
		{
			if (_cubeContainer == null) return;
			_rotationAngle += 0.02;
			_cubeContainer.Transform = new RotateTransform3D(
				new AxisAngleRotation3D(_rotationAxis, _rotationAngle * 180 / Math.PI)
			);
		}

		private void RotateButton_Click(object sender, RoutedEventArgs e)
		{
			_isRotating = !_isRotating;
			if (_isRotating)
			{
				_rotationTimer.Start();
				RotateButton.Content = "⏸";
			}
			else
			{
				_rotationTimer.Stop();
				RotateButton.Content = "🔄";
			}
		}

		// ---------- ВЫБОР ОСИ ВРАЩЕНИЯ ----------
		private void AxisButton_Click(object sender, RoutedEventArgs e)
		{
			Button btn = sender as Button;
			if (btn == null) return;
			switch (btn.Content.ToString())
			{
				case "X": _rotationAxis = new Vector3D(1, 0, 0); break;
				case "Y": _rotationAxis = new Vector3D(0, 1, 0); break;
				case "Z": _rotationAxis = new Vector3D(0, 0, 1); break;
				default: return;
			}
			if (_isRotating)
			{
				_rotationAngle = 0;
			}
			HighlightAxisButton(btn);
			_selectedAxisButton = btn;
		}

		private void HighlightAxisButton(Button selected)
		{
			AxisXButton.Background = System.Windows.Media.Brushes.Transparent;
			AxisYButton.Background = System.Windows.Media.Brushes.Transparent;
			AxisZButton.Background = System.Windows.Media.Brushes.Transparent;
			selected.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White) { Opacity = 0.3 };
		}

		// ---------- УПРАВЛЕНИЕ КАМЕРОЙ МЫШЬЮ ----------
		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				_lastMousePos = e.GetPosition(this);
				this.MouseMove += OnMouseMove;
				this.MouseUp += OnMouseUp;
			}
		}

		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			var pos = e.GetPosition(this);
			double dx = pos.X - _lastMousePos.X;
			double dy = pos.Y - _lastMousePos.Y;
			_lastMousePos = pos;

			double angle = Math.Sqrt(dx * dx + dy * dy) * 0.3;
			if (angle > 0.01)
			{
				Vector3D axis = new Vector3D(dy, dx, 0);
				Quaternion q = new Quaternion(axis, angle);
				Vector3D posVec = new Vector3D(Camera.Position.X, Camera.Position.Y, Camera.Position.Z);
				Quaternion qPos = new Quaternion(posVec.X, posVec.Y, posVec.Z, 0);
				Quaternion qConj = new Quaternion(-q.X, -q.Y, -q.Z, q.W);
				Quaternion rotatedQ = q * qPos * qConj;
				Vector3D rotatedPos = new Vector3D(rotatedQ.X, rotatedQ.Y, rotatedQ.Z);
				Camera.Position = new Point3D(rotatedPos.X, rotatedPos.Y, rotatedPos.Z);
				Camera.LookDirection = -rotatedPos;
				Camera.UpDirection = new Vector3D(0, 1, 0);
			}
		}

		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			this.MouseMove -= OnMouseMove;
			this.MouseUp -= OnMouseUp;
		}

		// ---------- ОБРАБОТЧИКИ ЗАГОЛОВКА ----------
		private void CaptionBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 1) this.DragMove();
		}

		private void MinimizeButton_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;
		private void MaximizeButton_Click(object sender, RoutedEventArgs e) => this.WindowState = (this.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
		private void CloseButton_Click(object sender, RoutedEventArgs e) => this.Close();
	}
}
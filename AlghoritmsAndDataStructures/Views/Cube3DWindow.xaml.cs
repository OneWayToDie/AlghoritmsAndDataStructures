using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace AlghoritmsAndDataStructures.Views
{
	public partial class Cube3DWindow : Window
	{
		private double _edge;
		private Point _lastMousePos;
		private double _baseDistance;

		public Cube3DWindow(double edge)
		{
			InitializeComponent();
			_edge = edge;
			BuildCube();
			UpdateEdgeLabel();
			AdjustCameraToCube();
			ZoomSlider.Value = 50; // Стартовый масштаб
		}

		private void BuildCube()
		{
			var scene = SceneVisual;
			scene.Children.Clear();

			var material = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(200, 70, 130, 255)));

			MeshGeometry3D mesh = new MeshGeometry3D();
			double s = _edge;
			// Центрируем куб в (0,0,0)
			Point3D[] vertices = new Point3D[]
			{
				new Point3D(-s/2, -s/2, -s/2), new Point3D(s/2, -s/2, -s/2),
				new Point3D(s/2, s/2, -s/2), new Point3D(-s/2, s/2, -s/2),
				new Point3D(-s/2, -s/2, s/2), new Point3D(s/2, -s/2, s/2),
				new Point3D(s/2, s/2, s/2), new Point3D(-s/2, s/2, s/2)
			};

			int[] indices = new int[]
			{
				0,1,2, 0,2,3,
				4,6,5, 4,7,6,
				0,3,7, 0,7,4,
				1,5,6, 1,6,2,
				0,4,5, 0,5,1,
				3,2,6, 3,6,7
			};

			foreach (var v in vertices) mesh.Positions.Add(v);
			foreach (int i in indices) mesh.TriangleIndices.Add(i);

			var model = new GeometryModel3D(mesh, material);
			model.BackMaterial = material;

			var visual = new ModelVisual3D();
			visual.Content = model;
			scene.Children.Add(visual);
		}

		private void AdjustCameraToCube()
		{
			_baseDistance = _edge * 3 + 5;
			UpdateZoom(ZoomSlider.Value);
		}

		private void UpdateZoom(double zoomValue)
		{
			// zoomValue от 0 до 100, при 50 масштаб = 1
			double scale = Math.Pow(2, (zoomValue - 50) / 50.0);
			double distance = _baseDistance / scale;
			// Ограничиваем, чтобы камера не ушла слишком далеко или близко
			if (distance < 0.5) distance = 0.5;
			Camera.Position = new Point3D(distance, distance, distance);
			Camera.LookDirection = new Vector3D(-distance, -distance, -distance);
			Camera.UpDirection = new Vector3D(0, 1, 0);
		}

		private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			// Защита от вызова до завершения инициализации
			if (ZoomValueText == null || Camera == null)
				return;

			double val = e.NewValue;
			ZoomValueText.Text = val.ToString("0");
			UpdateZoom(val);
		}

		private void UpdateEdgeLabel()
		{
			EdgeLabel.Text = string.Format("{0:F2}", _edge);
		}

		private void CaptionBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 1)
				this.DragMove();
		}

		private void MinimizeButton_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;
		private void MaximizeButton_Click(object sender, RoutedEventArgs e) => this.WindowState = (this.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
		private void CloseButton_Click(object sender, RoutedEventArgs e) => this.Close();

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

			double angle = Math.Sqrt(dx * dx + dy * dy) * 0.2;
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
	}
}
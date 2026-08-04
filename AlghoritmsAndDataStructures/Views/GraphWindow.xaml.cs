using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AlghoritmsAndDataStructures.Core.Calculators;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;

namespace AlghoritmsAndDataStructures.Views
{
	public partial class GraphWindow : Window, INotifyPropertyChanged
	{
		private double _x;
		private double _r;
		private readonly bool _isDarkTheme;
		private PlotModel _currentModel;
		private List<DataPoint> _allPoints;
		private LineSeries _animatedSeries;
		private DispatcherTimer _animationTimer;
		private int _currentPointIndex;
		private const int PointsPerStep = 30;

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public double X
		{
			get => _x;
			set
			{
				if (_x != value)
				{
					_x = value;
					OnPropertyChanged();
				}
			}
		}

		public double R
		{
			get => _r;
			set
			{
				if (_r != value)
				{
					_r = value;
					OnPropertyChanged();
				}
			}
		}

		public GraphWindow(double x, double r, bool isDarkTheme)
		{
			InitializeComponent();
			_x = x;
			_r = r;
			_isDarkTheme = isDarkTheme;
			Owner = Application.Current.MainWindow;
			DataContext = this;
			BuildPlot();
		}

		private void BuildPlot()
		{
			StopAnimation();

			// --- Проверка на недопустимые значения R ---
			if (_r == 5 || _r == 8)
			{
				var errorModel = new PlotModel
				{
					Title = "Ошибка: R не может быть равен 5 или 8 (деление на ноль).",
					TitleColor = OxyColors.Red,
					Background = _isDarkTheme ? OxyColors.Black : OxyColors.White
				};
				var errorXAxis = new LinearAxis { Position = AxisPosition.Bottom, Minimum = -10, Maximum = 10 };
				var errorYAxis = new LinearAxis { Position = AxisPosition.Left, Minimum = -10, Maximum = 10 };
				errorModel.Axes.Add(errorXAxis);
				errorModel.Axes.Add(errorYAxis);
				_currentModel = errorModel;
				PlotView.Model = errorModel;
				return;
			}

			// --- Динамический диапазон X ---
			double xMinBase = Math.Min(-10, -_r - 2);
			double xMaxBase = Math.Max(11, _r + 2);
			double xMinUser = _x - 2;
			double xMaxUser = _x + 2;
			double xMin = Math.Min(xMinBase, xMinUser);
			double xMax = Math.Max(xMaxBase, xMaxUser);

			double range = xMax - xMin;
			double step = 0.01;
			if (range > 50) step = 0.05;
			if (range > 200) step = 0.1;
			if (range > 500) step = 0.5;
			if (range > 1000) step = 1.0;

			// --- Сбор точек ---
			var points = new List<DataPoint>();
			double yMin = double.MaxValue, yMax = double.MinValue;
			for (double xVal = xMin; xVal <= xMax; xVal += step)
			{
				string err;
				var yVal = GraphCalculator.Compute(xVal, _r, out err);
				if (yVal.HasValue && !double.IsNaN(yVal.Value) && !double.IsInfinity(yVal.Value))
				{
					points.Add(new DataPoint(xVal, yVal.Value));
					if (yVal.Value < yMin) yMin = yVal.Value;
					if (yVal.Value > yMax) yMax = yVal.Value;
				}
			}

			if (points.Count == 0)
			{
				var errorModel = new PlotModel
				{
					Title = "Ошибка: не удалось построить график (проверьте R).",
					TitleColor = OxyColors.Red,
					Background = _isDarkTheme ? OxyColors.Black : OxyColors.White
				};
				var errorXAxis = new LinearAxis { Position = AxisPosition.Bottom, Minimum = -10, Maximum = 10 };
				var errorYAxis = new LinearAxis { Position = AxisPosition.Left, Minimum = -10, Maximum = 10 };
				errorModel.Axes.Add(errorXAxis);
				errorModel.Axes.Add(errorYAxis);
				_currentModel = errorModel;
				PlotView.Model = errorModel;
				return;
			}

			_allPoints = points;

			// --- Оси ---
			var axisColor = _isDarkTheme ? OxyColors.White : OxyColors.Black;
			var gridColor = _isDarkTheme ? OxyColors.Gray : OxyColors.LightGray;

			var xAxis = new LinearAxis
			{
				Position = AxisPosition.Bottom,
				Title = string.Format("X (диапазон: [{0:F1}, {1:F1}])", xMin, xMax),
				Minimum = xMin,
				Maximum = xMax,
				TickStyle = TickStyle.Outside,
				AxislineStyle = LineStyle.Solid,
				AxislineColor = axisColor,
				TextColor = axisColor,
				TitleColor = axisColor,
				MajorGridlineStyle = LineStyle.Dot,
				MajorGridlineColor = gridColor,
				MinorGridlineStyle = LineStyle.None
			};

			double yPadding = (yMax - yMin) * 0.1;
			if (yPadding < 0.5) yPadding = 0.5;
			double yAxisMin = yMin - yPadding;
			double yAxisMax = yMax + yPadding;

			var yAxis = new LinearAxis
			{
				Position = AxisPosition.Left,
				Title = "Y",
				Minimum = yAxisMin,
				Maximum = yAxisMax,
				TickStyle = TickStyle.Outside,
				AxislineStyle = LineStyle.Solid,
				AxislineColor = axisColor,
				TextColor = axisColor,
				TitleColor = axisColor,
				MajorGridlineStyle = LineStyle.Dot,
				MajorGridlineColor = gridColor,
				MinorGridlineStyle = LineStyle.None
			};

			// --- Модель ---
			var model = new PlotModel
			{
				Title = string.Format("График функции при R = {0:F2}", _r),
				TitleColor = _isDarkTheme ? OxyColors.White : OxyColors.Black,
				Background = _isDarkTheme ? OxyColors.Black : OxyColors.White,
				PlotAreaBackground = _isDarkTheme ? OxyColors.Black : OxyColors.White
			};

			// --- Анимируемая кривая ---
			var curveColor = _isDarkTheme ? OxyColors.DodgerBlue : OxyColors.DarkBlue;
			_animatedSeries = new LineSeries
			{
				Color = curveColor,
				StrokeThickness = 2,
				LineStyle = LineStyle.Solid
			};

			// --- Границы дуги ---
			var borderColor = _isDarkTheme ? OxyColors.Orange : OxyColors.DarkOrange;
			var leftBorder = new LineSeries
			{
				Color = borderColor,
				StrokeThickness = 1,
				LineStyle = LineStyle.Dash
			};
			leftBorder.Points.Add(new DataPoint(-_r, yAxisMin));
			leftBorder.Points.Add(new DataPoint(-_r, yAxisMax));

			var rightBorder = new LineSeries
			{
				Color = borderColor,
				StrokeThickness = 1,
				LineStyle = LineStyle.Dash
			};
			rightBorder.Points.Add(new DataPoint(_r, yAxisMin));
			rightBorder.Points.Add(new DataPoint(_r, yAxisMax));

			// --- Маркер и аннотация ---
			var markerSeries = new ScatterSeries
			{
				MarkerType = MarkerType.Circle,
				MarkerSize = 10,
				MarkerFill = OxyColors.Red,
				MarkerStroke = OxyColors.White,
				MarkerStrokeThickness = 2
			};
			string errUser;
			var userY = GraphCalculator.Compute(_x, _r, out errUser);
			if (userY.HasValue && !double.IsNaN(userY.Value) && !double.IsInfinity(userY.Value))
			{
				markerSeries.Points.Add(new ScatterPoint(_x, userY.Value));

				var userLine = new LineSeries
				{
					Color = OxyColors.Red,
					StrokeThickness = 1,
					LineStyle = LineStyle.Dash
				};
				userLine.Points.Add(new DataPoint(_x, yAxisMin));
				userLine.Points.Add(new DataPoint(_x, yAxisMax));
				model.Series.Add(userLine);

				var annotation = new TextAnnotation
				{
					Text = string.Format("({0:F2}; {1:F2})", _x, userY.Value),
					TextColor = OxyColors.Red,
					FontSize = 12,
					FontWeight = 700,
					Stroke = OxyColors.White,
					StrokeThickness = 1,
					Background = OxyColor.FromArgb(180, 0, 0, 0),
					TextPosition = new DataPoint(_x, userY.Value + 2.0)
				};
				model.Annotations.Add(annotation);
			}

			model.Axes.Add(xAxis);
			model.Axes.Add(yAxis);
			model.Series.Add(_animatedSeries);
			model.Series.Add(leftBorder);
			model.Series.Add(rightBorder);
			model.Series.Add(markerSeries);

			_currentModel = model;
			PlotView.Model = model;
			StartAnimation();
		}

		// ---------------------- АНИМАЦИЯ ----------------------
		private void StartAnimation()
		{
			if (_allPoints == null || _allPoints.Count == 0)
				return;

			_currentPointIndex = 0;
			_animatedSeries.Points.Clear();

			_animationTimer = new DispatcherTimer();
			_animationTimer.Interval = TimeSpan.FromMilliseconds(30);
			_animationTimer.Tick += OnAnimationTick;
			_animationTimer.Start();
		}

		private void OnAnimationTick(object sender, EventArgs e)
		{
			if (_allPoints == null || _animatedSeries == null)
			{
				StopAnimation();
				return;
			}

			int remaining = _allPoints.Count - _currentPointIndex;
			int toAdd = Math.Min(PointsPerStep, remaining);

			for (int i = 0; i < toAdd; i++)
			{
				_animatedSeries.Points.Add(_allPoints[_currentPointIndex + i]);
			}
			_currentPointIndex += toAdd;

			PlotView.InvalidatePlot(true);

			if (_currentPointIndex >= _allPoints.Count)
			{
				StopAnimation();
			}
		}

		private void StopAnimation()
		{
			if (_animationTimer != null)
			{
				_animationTimer.Stop();
				_animationTimer.Tick -= OnAnimationTick;
				_animationTimer = null;
			}
		}

		// ---------------------- ОБНОВЛЕНИЕ ГРАФИКА ----------------------
		private void UpdateButton_Click(object sender, RoutedEventArgs e)
		{
			if (double.TryParse(InputX.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double newX) &&
				double.TryParse(InputR.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double newR))
			{
				X = newX;
				R = newR;
				BuildPlot();
			}
			else
			{
				if (!double.IsNaN(X) && !double.IsInfinity(X) && !double.IsNaN(R) && !double.IsInfinity(R) && R > 0 && R != 5 && R != 8)
				{
					BuildPlot();
				}
				else
				{
					MessageBox.Show("Введите корректные числовые значения для X и R.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			}
		}

		private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
		{
			UpdateButton_Click(sender, null);
		}

		// ---------------------- СОХРАНЕНИЕ PNG ----------------------
		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			if (_currentModel == null) return;

			var dialog = new SaveFileDialog
			{
				Filter = "PNG files (*.png)|*.png",
				DefaultExt = ".png",
				FileName = string.Format("graph_R{0:F2}_X{1:F2}", _r, _x)
			};
			if (dialog.ShowDialog() == true)
			{
				using (var stream = File.Create(dialog.FileName))
				{
					var exporter = new PngExporter { Width = 800, Height = 500 };
					exporter.Export(_currentModel, stream);
				}
				MessageBox.Show("График сохранён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		// ---------------------- КОПИРОВАНИЕ В БУФЕР ----------------------
		private void CopyButton_Click(object sender, RoutedEventArgs e)
		{
			if (_currentModel == null) return;

			try
			{
				var exporter = new PngExporter { Width = 800, Height = 500 };
				using (var stream = new MemoryStream())
				{
					exporter.Export(_currentModel, stream);
					var bitmap = new BitmapImage();
					bitmap.BeginInit();
					bitmap.StreamSource = stream;
					bitmap.CacheOption = BitmapCacheOption.OnLoad;
					bitmap.EndInit();
					Clipboard.SetImage(bitmap);
					MessageBox.Show("График скопирован в буфер обмена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format("Ошибка копирования: {0}", ex.Message), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		// ---------------------- ОБРАБОТЧИКИ ЗАГОЛОВКА ----------------------
		private void CaptionBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 1)
				this.DragMove();
		}

		private void MinimizeButton_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}

		private void MaximizeButton_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = (this.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			StopAnimation();
			this.Close();
		}
	}
}
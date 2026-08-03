using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AlghoritmsAndDataStructures.Core.Calculators;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;

namespace AlghoritmsAndDataStructures.Views
{
	public partial class GraphWindow : Window
	{
		private double _x;
		private double _r;
		private readonly bool _isDarkTheme;
		private PlotModel _currentModel;

		public double X
		{
			get => _x;
			set => _x = value;
		}

		public double R
		{
			get => _r;
			set => _r = value;
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
			var model = new PlotModel
			{
				Title = $"График функции при R = {_r:F2}",
				TitleColor = _isDarkTheme ? OxyColors.White : OxyColors.Black,
				Background = _isDarkTheme ? OxyColors.Black : OxyColors.White,
				PlotAreaBackground = _isDarkTheme ? OxyColors.Black : OxyColors.White
			};

			// Динамический диапазон X
			double xMinBase = Math.Min(-10, -_r - 2);
			double xMaxBase = Math.Max(11, _r + 2);
			double xMinUser = _x - 2;
			double xMaxUser = _x + 2;
			double xMin = Math.Min(xMinBase, xMinUser);
			double xMax = Math.Max(xMaxBase, xMaxUser);

			// --- Автоматический шаг (оптимизация) ---
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

			// --- Оси ---
			var axisColor = _isDarkTheme ? OxyColors.White : OxyColors.Black;
			var gridColor = _isDarkTheme ? OxyColors.Gray : OxyColors.LightGray;

			var xAxis = new LinearAxis
			{
				Position = AxisPosition.Bottom,
				Title = $"X (диапазон: [{xMin:F1}, {xMax:F1}])",
				Minimum = xMin,
				Maximum = xMax,
				TickStyle = TickStyle.Outside,
				AxislineStyle = LineStyle.Solid,
				AxislineColor = axisColor,
				TextColor = axisColor,
				TitleColor = axisColor,
				MajorGridlineStyle = LineStyle.Dot,
				MajorGridlineColor = gridColor,
				MinorGridlineStyle = LineStyle.None // отключаем частую сетку для скорости
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

			// --- Основная кривая ---
			// --- Основная кривая ---
			var curveColor = _isDarkTheme ? OxyColors.DodgerBlue : OxyColors.DarkBlue;
			var series = new LineSeries
			{
				Color = curveColor,
				StrokeThickness = 2,
				LineStyle = LineStyle.Solid
			};
			series.Points.AddRange(points); // вместо Points = points

			// --- Границы дуги (-R и R) ---
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

				// Вертикальная линия для X
				var userLine = new LineSeries
				{
					Color = OxyColors.Red,
					StrokeThickness = 1,
					LineStyle = LineStyle.Dash
				};
				userLine.Points.Add(new DataPoint(_x, yAxisMin));
				userLine.Points.Add(new DataPoint(_x, yAxisMax));
				model.Series.Add(userLine);

				// Аннотация с координатами
				var annotation = new TextAnnotation
				{
					Text = $"({_x:F2}; {userY.Value:F2})",
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

			// --- Сборка модели ---
			model.Axes.Add(xAxis);
			model.Axes.Add(yAxis);
			model.Series.Add(series);
			model.Series.Add(leftBorder);
			model.Series.Add(rightBorder);
			model.Series.Add(markerSeries);

			_currentModel = model;
			PlotView.Model = model;
		}

		// --- Обработчики ---
		private void UpdateButton_Click(object sender, RoutedEventArgs e)
		{
			if (double.TryParse(InputX.Text, out double newX) && double.TryParse(InputR.Text, out double newR))
			{
				_x = newX;
				_r = newR;
				BuildPlot();
			}
			else
			{
				MessageBox.Show("Введите корректные числовые значения для X и R.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			if (_currentModel == null) return;

			var dialog = new SaveFileDialog
			{
				Filter = "PNG files (*.png)|*.png",
				DefaultExt = ".png",
				FileName = $"graph_R{_r:F2}_X{_x:F2}"
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
				MessageBox.Show($"Ошибка копирования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		// Заголовок
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
			this.Close();
		}
	}
}
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AlghoritmsAndDataStructures.Views
{
	public partial class FractionVisualizationWindow : Window
	{
		private int _m = 10;
		private int _n = 3;
		private DispatcherTimer _animationTimer;
		private double _currentAngle = 0;
		private double _targetAngle = 0;

		public FractionVisualizationWindow(int m, int n)
		{
			InitializeComponent();
			_m = m;
			_n = n;
			SliderM.Value = _m;
			SliderN.Value = _n;
			TextM.Text = _m.ToString();
			TextN.Text = _n.ToString();
			this.Loaded += (s, e) => UpdateVisualization();
		}

		private void UpdateVisualization()
		{
			if (_n == 0) return;

			int integerPart = _m / _n;
			int remainder = _m % _n;
			double fraction = (double)remainder / _n;

			int integerLastDigit = integerPart % 10;
			int fractionFirstDigit = (remainder * 10) / _n;

			ResultIntegerPart.Text = $"Целая часть: {integerPart}";
			ResultFractionPart.Text = $"Дробная часть: {fraction:F3}";
			ResultIntegerLastDigit.Text = $"Младшая цифра целой части: {integerLastDigit}";
			ResultFractionFirstDigit.Text = $"Старшая цифра дробной части: {fractionFirstDigit}";

			DisplayIntegerLastDigit.Text = integerLastDigit.ToString();
			DisplayFractionFirstDigit.Text = fractionFirstDigit.ToString();

			_targetAngle = 360 * fraction;
			_currentAngle = 0;
			if (_animationTimer != null)
			{
				_animationTimer.Stop();
				_animationTimer.Tick -= AnimationTick;
			}
			_animationTimer = new DispatcherTimer();
			_animationTimer.Interval = TimeSpan.FromMilliseconds(20);
			_animationTimer.Tick += AnimationTick;
			_animationTimer.Start();
		}

		private void AnimationTick(object sender, EventArgs e)
		{
			if (_currentAngle < _targetAngle)
			{
				_currentAngle += Math.Min(10, _targetAngle - _currentAngle);
				double drawAngle = Math.Min(_currentAngle, 359.99);
				DrawCircle(drawAngle);
			}
			else
			{
				_animationTimer.Stop();
				double drawAngle = Math.Min(_targetAngle, 359.99);
				DrawCircle(drawAngle);
			}
		}

		private void DrawCircle(double angle)
		{
			DrawingCanvas.Children.Clear();

			double centerX = DrawingCanvas.Width / 2;
			double centerY = DrawingCanvas.Height / 2;
			double radius = Math.Min(DrawingCanvas.Width, DrawingCanvas.Height) / 2 - 30;

			Ellipse backgroundEllipse = new Ellipse
			{
				Width = radius * 2,
				Height = radius * 2,
				Stroke = new SolidColorBrush(Color.FromRgb(100, 100, 100)),
				StrokeThickness = 2,
				Fill = new SolidColorBrush(Color.FromArgb(50, 50, 50, 50))
			};
			Canvas.SetLeft(backgroundEllipse, centerX - radius);
			Canvas.SetTop(backgroundEllipse, centerY - radius);
			DrawingCanvas.Children.Add(backgroundEllipse);

			if (angle <= 0) return;

			PointCollection points = new PointCollection();
			points.Add(new Point(centerX, centerY));
			int segments = 80;
			double maxAngle = Math.Min(angle, 359.99);
			for (int i = 0; i <= segments; i++)
			{
				double a = (maxAngle / segments) * i * Math.PI / 180;
				double x = centerX + radius * Math.Cos(a);
				double y = centerY - radius * Math.Sin(a);
				points.Add(new Point(x, y));
			}

			Polygon polygon = new Polygon
			{
				Points = points,
				Fill = new SolidColorBrush(Color.FromRgb(70, 130, 255)),
				Stroke = new SolidColorBrush(Color.FromRgb(70, 130, 255)),
				StrokeThickness = 1
			};
			DrawingCanvas.Children.Add(polygon);

			TextBlock textBlock = new TextBlock
			{
				Text = $"{_m / _n}",
				FontSize = 32,
				FontWeight = FontWeights.Bold,
				Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255)),
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			Canvas.SetLeft(textBlock, centerX - 20);
			Canvas.SetTop(textBlock, centerY - 20);
			DrawingCanvas.Children.Add(textBlock);

			TextBlock fractionLabel = new TextBlock
			{
				Text = $"{_m % _n}/{_n}",
				FontSize = 14,
				Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			Canvas.SetLeft(fractionLabel, centerX - 25);
			Canvas.SetTop(fractionLabel, centerY + 25);
			DrawingCanvas.Children.Add(fractionLabel);
		}

		private void SliderM_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!IsLoaded) return;
			_m = (int)e.NewValue;
			TextM.Text = _m.ToString();
			UpdateVisualization();
		}

		private void SliderN_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (!IsLoaded) return;
			int newN = (int)e.NewValue;
			if (newN == 0) newN = 1;
			_n = newN;
			TextN.Text = _n.ToString();
			UpdateVisualization();
		}

		// Обработчики заголовка
		private void CaptionBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 1) this.DragMove();
		}

		private void MinimizeButton_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;
		private void MaximizeButton_Click(object sender, RoutedEventArgs e) => this.WindowState = (this.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
		private void CloseButton_Click(object sender, RoutedEventArgs e) => this.Close();

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && this.WindowState == WindowState.Normal)
				this.DragMove();
		}
	}
}
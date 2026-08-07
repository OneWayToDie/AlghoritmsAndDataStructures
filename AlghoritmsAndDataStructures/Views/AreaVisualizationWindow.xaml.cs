using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using AlghoritmsAndDataStructures.Core.Calculators;

namespace AlghoritmsAndDataStructures.Views
{
	public partial class AreaVisualizationWindow : Window
	{
		private double _x = 0, _y = 0, _a = 4, _b = 3, _r = 5;

		public AreaVisualizationWindow(double x, double y, double a, double b, double r)
		{
			InitializeComponent();
			_x = x; _y = y; _a = a; _b = b; _r = r;
			InputX.Text = x.ToString("F1");
			InputY.Text = y.ToString("F1");
			InputA.Text = a.ToString("F1");
			InputB.Text = b.ToString("F1");
			InputR.Text = r.ToString("F1");
			SliderA.Value = a;
			SliderB.Value = b;
			SliderR.Value = r;
			DrawCanvas();
			UpdateResult();
		}

		private void DrawCanvas()
		{
			var canvas = MapCanvas;
			canvas.Children.Clear();

			double width = canvas.Width;
			double height = canvas.Height;
			double centerX = width / 2;
			double centerY = height / 2;
			double maxVal = Math.Max(Math.Max(_a, _b), _r) * 1.2;
			if (maxVal < 1) maxVal = 1;
			double scale = Math.Min((width / 2) / maxVal, (height / 2) / maxVal);

			Func<double, double, Point> toPixel = (wx, wy) =>
				new Point(centerX + wx * scale, centerY - wy * scale);

			DrawAxis(canvas, toPixel, maxVal);
			DrawRectangle(canvas, toPixel, _a, _b);
			DrawCircle(canvas, toPixel, _r);
			DrawShadedAreas(canvas, toPixel, _a, _b, _r);
			DrawPoint(canvas, toPixel, _x, _y);
		}

		private void DrawAxis(Canvas canvas, Func<double, double, Point> toPixel, double maxVal)
		{
			var axisColor = new SolidColorBrush(Colors.LightGray);
			double axisLength = maxVal * 1.1;

			var lineX = new Line();
			var p1 = toPixel(-axisLength, 0);
			var p2 = toPixel(axisLength, 0);
			lineX.X1 = p1.X; lineX.Y1 = p1.Y;
			lineX.X2 = p2.X; lineX.Y2 = p2.Y;
			lineX.Stroke = axisColor;
			lineX.StrokeThickness = 1;
			canvas.Children.Add(lineX);

			var lineY = new Line();
			var p3 = toPixel(0, -axisLength);
			var p4 = toPixel(0, axisLength);
			lineY.X1 = p3.X; lineY.Y1 = p3.Y;
			lineY.X2 = p4.X; lineY.Y2 = p4.Y;
			lineY.Stroke = axisColor;
			lineY.StrokeThickness = 1;
			canvas.Children.Add(lineY);

			AddTick(canvas, toPixel, _a, 0, "a");
			AddTick(canvas, toPixel, -_a, 0, "-a");
			AddTick(canvas, toPixel, 0, _b, "b");
			AddTick(canvas, toPixel, 0, -_b, "-b");
			AddTick(canvas, toPixel, _r, 0, "R");
			AddTick(canvas, toPixel, -_r, 0, "-R");
		}

		private void AddTick(Canvas canvas, Func<double, double, Point> toPixel, double x, double y, string label)
		{
			var pos = toPixel(x, y);
			var tb = new TextBlock() { Text = label, Foreground = new SolidColorBrush(Colors.LightGray), FontSize = 10 };
			Canvas.SetLeft(tb, pos.X + 3);
			Canvas.SetTop(tb, pos.Y - 6);
			canvas.Children.Add(tb);
		}

		private void DrawRectangle(Canvas canvas, Func<double, double, Point> toPixel, double a, double b)
		{
			var p1 = toPixel(-a, -b);
			var p2 = toPixel(a, b);
			var rect = new Rectangle()
			{
				Stroke = new SolidColorBrush(Colors.Gray),
				StrokeThickness = 1,
				StrokeDashArray = new DoubleCollection() { 4, 2 },
				Fill = Brushes.Transparent
			};
			Canvas.SetLeft(rect, p1.X);
			Canvas.SetTop(rect, p2.Y);
			rect.Width = p2.X - p1.X;
			rect.Height = p1.Y - p2.Y;
			canvas.Children.Add(rect);
		}

		private void DrawCircle(Canvas canvas, Func<double, double, Point> toPixel, double r)
		{
			var center = toPixel(0, 0);
			var edge = toPixel(r, 0);
			double radius = edge.X - center.X;
			var ellipse = new Ellipse()
			{
				Stroke = new SolidColorBrush(Colors.Gray),
				StrokeThickness = 1,
				StrokeDashArray = new DoubleCollection() { 4, 2 },
				Fill = Brushes.Transparent,
				Width = radius * 2,
				Height = radius * 2
			};
			Canvas.SetLeft(ellipse, center.X - radius);
			Canvas.SetTop(ellipse, center.Y - radius);
			canvas.Children.Add(ellipse);
		}

		private void DrawShadedAreas(Canvas canvas, Func<double, double, Point> toPixel, double a, double b, double r)
		{
			Point center = toPixel(0, 0);
			double radiusPx = Math.Abs(toPixel(r, 0).X - center.X);

			// Левая нижняя область (III квадрант) — внутри круга
			PathGeometry leftLowerGeometry = new PathGeometry();
			PathFigure leftFigure = new PathFigure();
			leftFigure.StartPoint = toPixel(-a, 0);
			leftFigure.Segments.Add(new LineSegment(toPixel(0, 0), true));
			leftFigure.Segments.Add(new LineSegment(toPixel(0, -b), true));
			leftFigure.Segments.Add(new LineSegment(toPixel(-a, -b), true));
			leftFigure.IsClosed = true;
			leftLowerGeometry.Figures.Add(leftFigure);

			// Правая верхняя область (I квадрант) — снаружи круга
			PathGeometry rightUpperGeometry = new PathGeometry();
			PathFigure rightFigure = new PathFigure();
			rightFigure.StartPoint = toPixel(0, 0);
			rightFigure.Segments.Add(new LineSegment(toPixel(a, 0), true));
			rightFigure.Segments.Add(new LineSegment(toPixel(a, b), true));
			rightFigure.Segments.Add(new LineSegment(toPixel(0, b), true));
			rightFigure.IsClosed = true;
			rightUpperGeometry.Figures.Add(rightFigure);

			EllipseGeometry circleGeometry = new EllipseGeometry(center, radiusPx, radiusPx);

			// Левая нижняя: пересечение с кругом
			Geometry leftCombined = Geometry.Combine(leftLowerGeometry, circleGeometry, GeometryCombineMode.Intersect, null);
			Path leftPath = new Path
			{
				Fill = new SolidColorBrush(Colors.Gray) { Opacity = 0.4 },
				Data = leftCombined
			};
			canvas.Children.Add(leftPath);

			// Правая верхняя: вычитание круга
			Geometry rightCombined = Geometry.Combine(rightUpperGeometry, circleGeometry, GeometryCombineMode.Exclude, null);
			Path rightPath = new Path
			{
				Fill = new SolidColorBrush(Colors.Gray) { Opacity = 0.4 },
				Data = rightCombined
			};
			canvas.Children.Add(rightPath);
		}

		private void DrawPoint(Canvas canvas, Func<double, double, Point> toPixel, double x, double y)
		{
			var pos = toPixel(x, y);
			var dot = new Ellipse()
			{
				Width = 10,
				Height = 10,
				Fill = new SolidColorBrush(Colors.Red),
				Stroke = new SolidColorBrush(Colors.White),
				StrokeThickness = 2
			};
			Canvas.SetLeft(dot, pos.X - 5);
			Canvas.SetTop(dot, pos.Y - 5);
			canvas.Children.Add(dot);
		}

		private void UpdateResult()
		{
			if (ResultText == null) return;
			string msg;
			bool ok = AreaChecker.Check(_x, _y, _a, _b, _r, out msg);
			ResultText.Text = msg;
			ResultText.Foreground = ok ? new SolidColorBrush(Colors.LimeGreen) : new SolidColorBrush(Colors.Red);
		}

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (SliderA == null || SliderB == null || SliderR == null ||
				InputA == null || InputB == null || InputR == null) return;

			_a = SliderA.Value;
			_b = SliderB.Value;
			_r = SliderR.Value;
			InputA.Text = _a.ToString("F1");
			InputB.Text = _b.ToString("F1");
			InputR.Text = _r.ToString("F1");
			DrawCanvas();
			UpdateResult();
		}

		private void UpdateButton_Click(object sender, RoutedEventArgs e)
		{
			if (InputX == null || InputY == null || InputA == null || InputB == null || InputR == null) return;

			double.TryParse(InputX.Text, out _x);
			double.TryParse(InputY.Text, out _y);
			double.TryParse(InputA.Text, out _a);
			double.TryParse(InputB.Text, out _b);
			double.TryParse(InputR.Text, out _r);
			SliderA.Value = _a;
			SliderB.Value = _b;
			SliderR.Value = _r;
			DrawCanvas();
			UpdateResult();
		}

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
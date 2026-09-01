using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using AlghoritmsAndDataStructures.Core.Calculators;

namespace AlghoritmsAndDataStructures.Views
{
	public partial class QuickSortVisualizationWindow : Window
	{
		private readonly List<QuickSortStep> _steps;
		private int _currentIndex;
		private readonly int _totalSteps;
		private DispatcherTimer _timer;
		private const double PaddingLeft = 40;
		private const double PaddingRight = 40;
		private const double PaddingTop = 64;
		private const double PaddingBottom = 24;
		private const int MaxBarCount = 40;

		public QuickSortVisualizationWindow(int[] array)
		{
			InitializeComponent();
			_steps = QuickSortCalculator.TraceSort(array);
			_totalSteps = _steps.Count;
			_currentIndex = 0;

			_timer = new DispatcherTimer();
			_timer.Tick += Timer_Tick;

			SizeChanged += (s, e) => RenderStep(_currentIndex);
			Loaded += (s, e) => RenderStep(_currentIndex);

			PlayButton.Content = "⏸ Пауза";
			StartAnimation();
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			if (_currentIndex >= _totalSteps - 1)
			{
				StopAnimation();
				return;
			}
			_currentIndex++;
			RenderStep(_currentIndex);
		}

		private void StartAnimation()
		{
			double interval = Math.Max(520 - SpeedSlider.Value * 10, 20);
			_timer.Interval = TimeSpan.FromMilliseconds(interval);
			_timer.Start();
			PlayButton.Content = "⏸ Пауза";
		}

		private void StopAnimation()
		{
			_timer.Stop();
			PlayButton.Content = "▶ Авто";
		}

		private void RenderStep(int index)
		{
			SortCanvas.Children.Clear();

			var step = _steps[index];
			var values = step.Array;

			double width = SortCanvas.ActualWidth;
			double height = SortCanvas.ActualHeight;
			if (width <= 0 || height <= 0)
			{
				Dispatcher.BeginInvoke(new Action(() => RenderStep(_currentIndex)), DispatcherPriority.Background);
				return;
			}

			double plotWidth = width - PaddingLeft - PaddingRight;
			double plotHeight = height - PaddingTop - PaddingBottom;
			if (plotWidth <= 0 || plotHeight <= 0) return;

			int count = values.Length;
			bool hasNegative = false;
			int maxAbs = 0;
			for (int i = 0; i < count; i++)
			{
				if (values[i] < 0) hasNegative = true;
				if (Math.Abs(values[i]) > maxAbs) maxAbs = Math.Abs(values[i]);
			}
			if (maxAbs == 0) maxAbs = 1;

			double zeroY = hasNegative ? PaddingTop + plotHeight / 2 : PaddingTop + plotHeight;
			double scale = hasNegative ? (plotHeight / 2) / maxAbs : plotHeight / maxAbs;

			double colWidth = plotWidth / count;
			double barWidth = Math.Max(colWidth * 0.7, 2);

			// Нулевая линия при наличии отрицательных
			if (hasNegative)
			{
				Line zero = new Line
				{
					X1 = PaddingLeft,
					Y1 = zeroY,
					X2 = PaddingLeft + plotWidth,
					Y2 = zeroY,
					Stroke = new SolidColorBrush(Colors.Gray),
					StrokeThickness = 1,
					StrokeDashArray = new DoubleCollection { 3, 3 }
				};
				SortCanvas.Children.Add(zero);
			}

			// Столбцы
			for (int i = 0; i < count; i++)
			{
				int value = values[i];
				double barHeight = Math.Abs(value) * scale;
				if (barHeight < 1 && value != 0) barHeight = 1;

				double top;
				if (hasNegative)
				{
					top = value >= 0 ? zeroY - barHeight : zeroY;
				}
				else
				{
					top = zeroY - barHeight;
				}

				double left = PaddingLeft + i * colWidth;

				Color barColor = (value >= 0) ? Colors.DodgerBlue : Colors.Orange;

				// Подсветка: опорный > сравниваемый j > индекс i
				if (i == step.PivotIndex && step.PivotIndex >= 0)
					barColor = Colors.Red;
				else if (i == step.IndexB && step.IndexB >= 0)
					barColor = Colors.Gold;
				else if (i == step.IndexA && step.IndexA >= 0)
					barColor = Colors.MediumSpringGreen;

				Rectangle rect = new Rectangle
				{
					Width = barWidth,
					Height = barHeight,
					Fill = new SolidColorBrush(barColor),
					Stroke = new SolidColorBrush(Colors.White),
					StrokeThickness = 1
				};
				Canvas.SetLeft(rect, left + (colWidth - barWidth) / 2);
				Canvas.SetTop(rect, top);
				SortCanvas.Children.Add(rect);

				// Значение (зависит от ширины столбца)
				if (count <= MaxBarCount && colWidth > 5)
				{
					double fontSize = Math.Min(Math.Max(colWidth * 0.55, 9), 13);
					string text = value.ToString();
					double naturalWidth = text.Length * fontSize * 0.62;
					double naturalHeight = fontSize * 1.3;

					TextBlock tb = new TextBlock
					{
						Text = text,
						Foreground = new SolidColorBrush(Colors.LightGray),
						FontSize = fontSize,
						FontWeight = FontWeights.Bold,
						RenderTransformOrigin = new Point(0.5, 0.5),
						RenderTransform = new RotateTransform(-90)
					};

					double centerX = PaddingLeft + i * colWidth + colWidth / 2;
					double labelY;
					if (hasNegative && value < 0)
						labelY = top + barHeight + 6;
					else
						labelY = top - 8 - naturalWidth;

					Canvas.SetLeft(tb, centerX - naturalWidth / 2);
					Canvas.SetTop(tb, labelY);
					SortCanvas.Children.Add(tb);
				}
			}

			// Границы текущего подмассива [Low..High]
			if (step.High > step.Low && step.Type != QuickSortStepType.Initial && step.Type != QuickSortStepType.Complete)
			{
				double pxLow = PaddingLeft + step.Low * colWidth + colWidth / 2;
				double pxHigh = PaddingLeft + step.High * colWidth + colWidth / 2;
				double bracketY = PaddingTop - 22;

				Line tops = new Line
				{
					X1 = pxLow,
					Y1 = bracketY,
					X2 = pxHigh,
					Y2 = bracketY,
					Stroke = new SolidColorBrush(Color.FromRgb(255, 100, 100)),
					StrokeThickness = 2
				};
				Line leftEdge = new Line
				{
					X1 = pxLow,
					Y1 = bracketY - 6,
					X2 = pxLow,
					Y2 = bracketY + 6,
					Stroke = new SolidColorBrush(Color.FromRgb(255, 100, 100)),
					StrokeThickness = 2
				};
				Line rightEdge = new Line
				{
					X1 = pxHigh,
					Y1 = bracketY - 6,
					X2 = pxHigh,
					Y2 = bracketY + 6,
					Stroke = new SolidColorBrush(Color.FromRgb(255, 100, 100)),
					StrokeThickness = 2
				};
				SortCanvas.Children.Add(tops);
				SortCanvas.Children.Add(leftEdge);
				SortCanvas.Children.Add(rightEdge);

				TextBlock range = new TextBlock
				{
					Text = $"подмассив [{step.Low}..{step.High}]",
					Foreground = new SolidColorBrush(Color.FromRgb(255, 150, 150)),
					FontSize = 12,
					FontWeight = FontWeights.Bold
				};
				double rangeWidth = Math.Abs(pxHigh - pxLow);
				Canvas.SetLeft(range, (pxLow + pxHigh) / 2 - rangeWidth / 2);
				Canvas.SetTop(range, bracketY + 8);
				SortCanvas.Children.Add(range);
			}

			DescriptionText.Text = step.Description;
			ComparisonsText.Text = step.Comparisons.ToString();
			SwapsText.Text = step.Swaps.ToString();
			StepText.Text = $"{_currentIndex + 1} / {_totalSteps}";
		}

		// ===== Управление =====
		private void FirstButton_Click(object sender, RoutedEventArgs e)
		{
			StopAnimation();
			_currentIndex = 0;
			RenderStep(_currentIndex);
		}

		private void StepBackButton_Click(object sender, RoutedEventArgs e)
		{
			StopAnimation();
			if (_currentIndex > 0) _currentIndex--;
			RenderStep(_currentIndex);
		}

		private void StepForwardButton_Click(object sender, RoutedEventArgs e)
		{
			StopAnimation();
			if (_currentIndex < _totalSteps - 1) _currentIndex++;
			RenderStep(_currentIndex);
		}

		private void PlayButton_Click(object sender, RoutedEventArgs e)
		{
			if (_timer.IsEnabled)
			{
				StopAnimation();
				return;
			}
			if (_currentIndex >= _totalSteps - 1)
				_currentIndex = 0;
			StartAnimation();
		}

		private void PauseButton_Click(object sender, RoutedEventArgs e)
		{
			StopAnimation();
		}

		private void ResetButton_Click(object sender, RoutedEventArgs e)
		{
			StopAnimation();
			_currentIndex = 0;
			RenderStep(_currentIndex);
			StartAnimation();
		}

		// ===== Заголовок окна =====
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
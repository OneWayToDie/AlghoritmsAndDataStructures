using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AlghoritmsAndDataStructures.Views.Controls
{
	public enum HistogramMode
	{
		Series,
		Average
	}

	public partial class HistogramControl : UserControl
	{
		public static readonly DependencyProperty DataProperty =
			DependencyProperty.Register("Data", typeof(IEnumerable), typeof(HistogramControl),
				new PropertyMetadata(null, OnDataChanged));

		public static readonly DependencyProperty ModeProperty =
			DependencyProperty.Register("Mode", typeof(HistogramMode), typeof(HistogramControl),
				new PropertyMetadata(HistogramMode.Series, OnDataChanged));

		public static readonly DependencyProperty AverageValueProperty =
			DependencyProperty.Register("AverageValue", typeof(double?), typeof(HistogramControl),
				new PropertyMetadata(null, OnDataChanged));

		public IEnumerable Data
		{
			get => (IEnumerable)GetValue(DataProperty);
			set => SetValue(DataProperty, value);
		}

		public HistogramMode Mode
		{
			get => (HistogramMode)GetValue(ModeProperty);
			set => SetValue(ModeProperty, value);
		}

		public double? AverageValue
		{
			get => (double?)GetValue(AverageValueProperty);
			set => SetValue(AverageValueProperty, value);
		}

		private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((HistogramControl)d).Render();
		}

		private DispatcherTimer _animationTimer;
		private List<double> _displayItems;
		private List<double> _allItems;
		private int _currentIndex;
		private double _maxValue;
		private const double Padding = 60;
		private const int MaxDisplayCount = 25;

		public HistogramControl()
		{
			InitializeComponent();
			_animationTimer = new DispatcherTimer();
			_animationTimer.Interval = TimeSpan.FromMilliseconds(50);
			_animationTimer.Tick += AnimationTimer_Tick;
			this.SizeChanged += (s, e) => Render();
			this.Loaded += (s, e) => Render();
		}

		public void Render()
		{
			DrawingCanvas.Children.Clear();
			_animationTimer.Stop();

			if (Data == null) return;
			_allItems = Data.Cast<object>().Select(x => Convert.ToDouble(x)).ToList();
			if (_allItems.Count == 0) return;

			double width = DrawingCanvas.ActualWidth;
			double height = DrawingCanvas.ActualHeight;
			if (width <= 0 || height <= 0)
			{
				width = this.ActualWidth - 20;
				height = this.ActualHeight - 20;
				if (width <= 0 || height <= 0)
				{
					Dispatcher.BeginInvoke(new Action(Render), DispatcherPriority.Background);
					return;
				}
				DrawingCanvas.Width = width;
				DrawingCanvas.Height = height;
				DrawingCanvas.UpdateLayout();
			}

			// Прореживание
			if (_allItems.Count > MaxDisplayCount)
			{
				_displayItems = new List<double>();
				int step = _allItems.Count / MaxDisplayCount;
				for (int i = 0; i < _allItems.Count; i += step)
				{
					_displayItems.Add(_allItems[i]);
				}
				if (_displayItems.Last() != _allItems.Last())
					_displayItems.Add(_allItems.Last());
			}
			else
			{
				_displayItems = _allItems.ToList();
			}

			_maxValue = _displayItems.Max();
			if (Mode == HistogramMode.Average && AverageValue.HasValue)
				_maxValue = Math.Max(_maxValue, AverageValue.Value);
			if (_maxValue == 0) _maxValue = 1;
			_maxValue *= 1.1;

			if (Mode == HistogramMode.Series && _displayItems.Count <= 50)
			{
				_currentIndex = 0;
				DrawingCanvas.Children.Clear();
				_animationTimer.Start();
			}
			else
			{
				DrawAll();
				DrawAverageLine();
			}
		}

		private void AnimationTimer_Tick(object sender, EventArgs e)
		{
			if (_currentIndex < _displayItems.Count)
			{
				DrawItem(_currentIndex);
				_currentIndex++;
			}
			else
			{
				_animationTimer.Stop();
				DrawAverageLine();
			}
		}

		private void DrawAll()
		{
			for (int i = 0; i < _displayItems.Count; i++)
				DrawItem(i);
		}

		private void DrawItem(int index)
		{
			var canvas = DrawingCanvas;
			double width = canvas.ActualWidth - Padding * 2;
			double height = canvas.ActualHeight - Padding * 2;
			if (width <= 0 || height <= 0) return;

			double colWidth = width / _displayItems.Count;
			double left = Padding + index * colWidth;
			double barHeight = (_displayItems[index] / _maxValue) * height;

			// Столбец
			Rectangle rect = new Rectangle
			{
				Width = Math.Max(colWidth * 0.8, 2),
				Height = Math.Max(barHeight, 1),
				Fill = new SolidColorBrush(Colors.DodgerBlue),
				Stroke = new SolidColorBrush(Colors.White),
				StrokeThickness = 1
			};
			Canvas.SetLeft(rect, left + colWidth * 0.1);
			Canvas.SetTop(rect, canvas.ActualHeight - Padding - barHeight);
			canvas.Children.Add(rect);

			// Вертикальная подпись снизу — всегда для выборки (до 30 элементов)
			if (Mode == HistogramMode.Series)
			{
				// Показываем подписи, если отображаемых элементов не более 30
				// (это всегда верно после прореживания, т.к. MaxDisplayCount=25)
				bool showLabel = _displayItems.Count <= 30;

				if (showLabel)
				{
					TextBlock tb = new TextBlock
					{
						Text = _displayItems[index].ToString("F2"),
						Foreground = new SolidColorBrush(Colors.LightGray),
						FontSize = 12,
						FontWeight = FontWeights.Bold,
						HorizontalAlignment = HorizontalAlignment.Center,
						RenderTransform = new RotateTransform(-90)
					};
					Canvas.SetLeft(tb, left + colWidth * 0.5 - 14);
					Canvas.SetTop(tb, canvas.ActualHeight - Padding + 28);
					canvas.Children.Add(tb);
				}
			}
		}

		private void DrawAverageLine()
		{
			if (Mode != HistogramMode.Average || !AverageValue.HasValue) return;
			var canvas = DrawingCanvas;
			double width = canvas.ActualWidth - Padding * 2;
			double height = canvas.ActualHeight - Padding * 2;
			if (width <= 0 || height <= 0) return;

			double yPos = canvas.ActualHeight - Padding - (AverageValue.Value / _maxValue) * height;
			if (yPos < 0) yPos = 0;
			if (yPos > canvas.ActualHeight - Padding) yPos = canvas.ActualHeight - Padding;

			Line line = new Line
			{
				X1 = Padding,
				Y1 = yPos,
				X2 = canvas.ActualWidth - Padding,
				Y2 = yPos,
				Stroke = new SolidColorBrush(Colors.Red),
				StrokeThickness = 2,
				StrokeDashArray = new DoubleCollection { 5, 3 }
			};
			canvas.Children.Add(line);

			TextBlock tb = new TextBlock
			{
				Text = $"Ср: {AverageValue.Value:F2}",
				Foreground = new SolidColorBrush(Colors.White),
				FontWeight = FontWeights.Bold,
				FontSize = 12,
				Background = new SolidColorBrush(Color.FromArgb(150, 0, 0, 0))
			};
			Canvas.SetLeft(tb, canvas.ActualWidth - Padding - 80);
			Canvas.SetTop(tb, yPos - 10);
			canvas.Children.Add(tb);
		}
	}
}
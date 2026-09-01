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

		public static readonly DependencyProperty HighlightIndicesProperty =
			DependencyProperty.Register("HighlightIndices", typeof(IEnumerable<int>), typeof(HistogramControl),
				new PropertyMetadata(null, OnHighlightOrSpecialChanged));

		public static readonly DependencyProperty SpecialIndicesProperty =
			DependencyProperty.Register("SpecialIndices", typeof(IEnumerable<int>), typeof(HistogramControl),
				new PropertyMetadata(null, OnHighlightOrSpecialChanged));

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

		public IEnumerable<int> HighlightIndices
		{
			get => (IEnumerable<int>)GetValue(HighlightIndicesProperty);
			set => SetValue(HighlightIndicesProperty, value);
		}

		public IEnumerable<int> SpecialIndices
		{
			get => (IEnumerable<int>)GetValue(SpecialIndicesProperty);
			set => SetValue(SpecialIndicesProperty, value);
		}

		private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((HistogramControl)d).Render();
		}

		private static void OnHighlightOrSpecialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((HistogramControl)d).Render();
		}

		private DispatcherTimer _animationTimer;
		private List<double> _displayItems;
		private List<double> _allItems;
		private int _currentIndex;
		private double _maxValue;
		private double _minValue;
		private bool _hasNegative;
		private double _zeroY;
		private const double Padding = 80;
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

			// Определяем min и max
			_maxValue = _displayItems.Max();
			_minValue = _displayItems.Min();
			_hasNegative = _minValue < 0;

			// Если есть отрицательные, считаем максимальный модуль для масштаба
			double absMax = _hasNegative ? Math.Max(Math.Abs(_minValue), Math.Abs(_maxValue)) : _maxValue;
			if (absMax == 0) absMax = 1;

			// Для режима Average учитываем AverageValue
			if (Mode == HistogramMode.Average && AverageValue.HasValue)
			{
				double avg = AverageValue.Value;
				if (avg < 0 && avg < _minValue) _minValue = avg;
				if (avg > 0 && avg > _maxValue) _maxValue = avg;
				absMax = Math.Max(Math.Abs(_minValue), Math.Abs(_maxValue));
				if (absMax == 0) absMax = 1;
			}

			// Рассчитываем масштаб и нулевую линию
			double scaleFactor = 0;
			if (_hasNegative)
				scaleFactor = (height - Padding * 2) / (absMax * 2);
			else
				scaleFactor = (height - Padding * 2) / absMax;

			if (double.IsInfinity(scaleFactor) || double.IsNaN(scaleFactor)) scaleFactor = 1;

			_zeroY = _hasNegative ? (height - Padding) - ((-_minValue) * scaleFactor) : height - Padding;

			// Для совместимости со старыми работами: если нет отрицательных, используем старую логику (столбцы от нижнего края)
			if (!_hasNegative)
			{
				_maxValue = _displayItems.Max();
				if (Mode == HistogramMode.Average && AverageValue.HasValue)
					_maxValue = Math.Max(_maxValue, AverageValue.Value);
				if (_maxValue == 0) _maxValue = 1;
				_maxValue *= 1.1;
			}

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

			// Рисуем нулевую линию, если есть отрицательные
			if (_hasNegative)
				DrawZeroLine();
		}

		private void DrawZeroLine()
		{
			var canvas = DrawingCanvas;
			Line zeroLine = new Line
			{
				X1 = Padding,
				Y1 = _zeroY,
				X2 = canvas.ActualWidth - Padding,
				Y2 = _zeroY,
				Stroke = new SolidColorBrush(Colors.Gray),
				StrokeThickness = 1,
				StrokeDashArray = new DoubleCollection { 3, 3 }
			};
			canvas.Children.Add(zeroLine);
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
			double value = _displayItems[index];

			double barHeight, top;
			Color barColor = Colors.DodgerBlue;

			if (_hasNegative)
			{
				double absMax = Math.Max(Math.Abs(_minValue), Math.Abs(_maxValue));
				if (absMax == 0) absMax = 1;
				double scale = (height) / (absMax * 2);
				barHeight = Math.Abs(value) * scale;
				if (value >= 0)
				{
					top = _zeroY - barHeight;
					barColor = Colors.DodgerBlue;
				}
				else
				{
					top = _zeroY;
					barColor = Colors.Orange; // отрицательные — оранжевые
				}
			}
			else
			{
				barHeight = (value / _maxValue) * height;
				top = canvas.ActualHeight - Padding - barHeight;
				barColor = Colors.DodgerBlue;
			}

			// Корректировка минимальной высоты для видимости
			if (barHeight < 1) barHeight = 1;

			// Столбец
			Rectangle rect = new Rectangle
			{
				Width = Math.Max(colWidth * 0.8, 2),
				Height = barHeight,
				Fill = new SolidColorBrush(barColor),
				Stroke = new SolidColorBrush(Colors.White),
				StrokeThickness = 1
			};

			// Подсветка нечётных позиций
			if (HighlightIndices != null && HighlightIndices.Contains(index))
			{
				rect.Fill = new SolidColorBrush(Colors.Gold);
			}

			// Особый маркер для специальных индексов
			if (SpecialIndices != null && SpecialIndices.Contains(index))
			{
				rect.Stroke = new SolidColorBrush(Colors.Red);
				rect.StrokeThickness = 3;
			}

			Canvas.SetLeft(rect, left + colWidth * 0.1);
			Canvas.SetTop(rect, top);
			canvas.Children.Add(rect);

			// Подписи значений (над/под столбцами, центрированы по столбцу)
			if (Mode == HistogramMode.Series && _displayItems.Count <= 30 && colWidth > 4)
			{
				double fontSize = Math.Min(Math.Max(colWidth * 0.55, 9), 14);
				double gap = Math.Min(colWidth * 0.3, 8);

				string text = value.ToString("F2");
				double naturalWidth = text.Length * fontSize * 0.62;
				double naturalHeight = fontSize * 1.3;

				TextBlock tb = new TextBlock
				{
					Text = text,
					Foreground = new SolidColorBrush(Colors.LightGray),
					FontSize = fontSize,
					FontWeight = FontWeights.Bold,
					RenderTransformOrigin = new System.Windows.Point(0.5, 0.5),
					RenderTransform = new RotateTransform(-90)
				};

				double barCenterX = left + colWidth * 0.5;

				double labelY;
				if (_hasNegative && value < 0)
					labelY = top + barHeight + gap;
				else
					labelY = top - gap - naturalWidth;

				Canvas.SetLeft(tb, barCenterX - naturalWidth / 2);
				Canvas.SetTop(tb, labelY);
				canvas.Children.Add(tb);
			}
		}

		private void DrawAverageLine()
		{
			if (Mode != HistogramMode.Average || !AverageValue.HasValue) return;
			var canvas = DrawingCanvas;
			double width = canvas.ActualWidth - Padding * 2;
			double height = canvas.ActualHeight - Padding * 2;
			if (width <= 0 || height <= 0) return;

			double yPos;
			double avg = AverageValue.Value;
			if (_hasNegative)
			{
				double absMax = Math.Max(Math.Abs(_minValue), Math.Abs(_maxValue));
				if (absMax == 0) absMax = 1;
				double scale = height / (absMax * 2);
				yPos = _zeroY - avg * scale;
			}
			else
			{
				if (_maxValue == 0) _maxValue = 1;
				yPos = canvas.ActualHeight - Padding - (avg / _maxValue) * height;
			}

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
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using AlghoritmsAndDataStructures.Core.Calculators;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace AlghoritmsAndDataStructures.Views
{
	public partial class ConvergenceWindow : Window
	{
		private readonly double _x;
		private readonly double _eps;
		private readonly double _exact;
		private DispatcherTimer _timer;
		private List<double> _partialSums;
		private int _currentIndex;
		private PlotModel _plotModel;
		private LineSeries _series;
		private LineSeries _exactLine;

		public ConvergenceWindow(double x, double eps)
		{
			InitializeComponent();
			_x = x;
			_eps = eps;
			_exact = Math.Exp(-x);
			ExactValueText.Text = _exact.ToString("F6");

			BuildPlot();
			PrepareData();

			_timer = new DispatcherTimer();
			_timer.Interval = TimeSpan.FromMilliseconds(100);
			_timer.Tick += Timer_Tick;

			// Начинаем анимацию автоматически
			StartAnimation();
		}

		private void BuildPlot()
		{
			_plotModel = new PlotModel
			{
				Title = $"Сходимость ряда для x = {_x:F3}",
				TitleColor = OxyColors.White,
				Background = OxyColors.Black
			};

			// Оси
			var xAxis = new LinearAxis
			{
				Position = AxisPosition.Bottom,
				Title = "N (количество членов)",
				TextColor = OxyColors.White,
				TitleColor = OxyColors.White,
				MajorGridlineStyle = LineStyle.Dot,
				MajorGridlineColor = OxyColors.Gray
			};
			var yAxis = new LinearAxis
			{
				Position = AxisPosition.Left,
				Title = "Сумма ряда",
				TextColor = OxyColors.White,
				TitleColor = OxyColors.White,
				MajorGridlineStyle = LineStyle.Dot,
				MajorGridlineColor = OxyColors.Gray
			};
			_plotModel.Axes.Add(xAxis);
			_plotModel.Axes.Add(yAxis);

			// Серия для частичных сумм
			_series = new LineSeries
			{
				Color = OxyColors.DodgerBlue,
				StrokeThickness = 2,
				MarkerType = MarkerType.Circle,
				MarkerSize = 4,
				MarkerFill = OxyColors.DodgerBlue
			};
			_plotModel.Series.Add(_series);

			// Линия точного значения (горизонтальная)
			_exactLine = new LineSeries
			{
				Color = OxyColors.Red,
				StrokeThickness = 2,
				LineStyle = LineStyle.Dash,
				MarkerType = MarkerType.None
			};
			_plotModel.Series.Add(_exactLine);

			PlotView.Model = _plotModel;
		}

		private void PrepareData()
		{
			// Вычисляем частичные суммы для всех членов, пока погрешность не станет меньше eps
			_partialSums = new List<double>();
			double sum = 0;
			double term = 1.0;
			int n = 0;
			_partialSums.Add(sum); // N=0

			while (true)
			{
				n++;
				term *= (-_x) / n;
				sum += term;
				_partialSums.Add(sum);
				if (Math.Abs(sum - _exact) < _eps || n > 50)
					break;
			}

			_currentIndex = 0;
			_series.Points.Clear();
			_exactLine.Points.Clear();

			// Добавляем линию точного значения на весь диапазон
			_exactLine.Points.Add(new DataPoint(0, _exact));
			_exactLine.Points.Add(new DataPoint(_partialSums.Count - 1, _exact));

			// Добавляем первую точку (N=0)
			AddPoint(0);
			UpdateInfo(0);
		}

		private void AddPoint(int index)
		{
			if (index >= _partialSums.Count) return;
			double x = index;
			double y = _partialSums[index];
			_series.Points.Add(new DataPoint(x, y));
			PlotView.InvalidatePlot(true);
		}

		private void UpdateInfo(int index)
		{
			if (index >= _partialSums.Count) return;
			double sum = _partialSums[index];
			SumValueText.Text = sum.ToString("F6");
			TermsText.Text = index.ToString();
			double error = Math.Abs(sum - _exact);
			ErrorText.Text = error.ToString("E4");
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			_currentIndex++;
			if (_currentIndex >= _partialSums.Count)
			{
				StopAnimation();
				return;
			}
			AddPoint(_currentIndex);
			UpdateInfo(_currentIndex);
		}

		private void StartAnimation()
		{
			_currentIndex = 0;
			_series.Points.Clear();
			AddPoint(0);
			UpdateInfo(0);
			_timer.Start();
		}

		private void StopAnimation()
		{
			_timer.Stop();
		}

		private void ResetAnimation()
		{
			StopAnimation();
			StartAnimation();
		}

		// Обработчики кнопок
		private void PlayButton_Click(object sender, RoutedEventArgs e)
		{
			StartAnimation();
		}

		private void ResetButton_Click(object sender, RoutedEventArgs e)
		{
			ResetAnimation();
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
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using AlghoritmsAndDataStructures.Core.Calculators;
using AlghoritmsAndDataStructures.Helpers;
using AlghoritmsAndDataStructures.Models;
using AlghoritmsAndDataStructures.ViewModels.Base;
using AlghoritmsAndDataStructures.Views;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class SeriesTaskViewModel : BaseTaskViewModel
	{
		private string _aStr = "";
		private string _bStr = "";
		private string _dxStr = "";
		private string _epsStr = "";
		private string _resultMessage = "";
		private ObservableCollection<SeriesResult> _results = new ObservableCollection<SeriesResult>();

		public string AStr
		{
			get => _aStr;
			set { _aStr = value; OnPropertyChanged(nameof(AStr)); }
		}

		public string BStr
		{
			get => _bStr;
			set { _bStr = value; OnPropertyChanged(nameof(BStr)); }
		}

		public string DxStr
		{
			get => _dxStr;
			set { _dxStr = value; OnPropertyChanged(nameof(DxStr)); }
		}

		public string EpsStr
		{
			get => _epsStr;
			set { _epsStr = value; OnPropertyChanged(nameof(EpsStr)); }
		}

		public string ResultMessage
		{
			get => _resultMessage;
			private set { _resultMessage = value; OnPropertyChanged(nameof(ResultMessage)); }
		}

		public ObservableCollection<SeriesResult> Results
		{
			get => _results;
			private set { _results = value; OnPropertyChanged(nameof(Results)); }
		}

		public ICommand ShowSolutionCommand { get; }
		public ICommand ShowHistoryCommand { get; }
		public ICommand GenerateCommand { get; }
		public ICommand ShowConvergenceCommand { get; }

		public override string Title => "Задача: разложение e^(-x) в ряд";
		public override string HistoryKey => "SeriesTask";

		public SeriesTaskViewModel()
		{
			ShowSolutionCommand = new RelayCommand(ExecuteShowSolution);
			ShowHistoryCommand = new RelayCommand(ExecuteShowHistory);
			GenerateCommand = new RelayCommand(ExecuteGenerate);
			ShowConvergenceCommand = new RelayCommand(ExecuteShowConvergence);
		}

		private void ExecuteShowConvergence(object parameter)
		{
			if (Results.Count == 0)
			{
				MessageBox.Show("Сначала выполните вычисление, чтобы получить данные.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			// Берём первое значение x из таблицы (можно выбрать любое)
			var first = Results.FirstOrDefault();
			if (first == null) return;

			var window = new ConvergenceWindow(first.X, double.Parse(EpsStr));
			window.ShowDialog();
		}

		private void ExecuteGenerate(object parameter)
		{
			var rand = new Random();
			// A: от -10 до 10
			double a = rand.Next(-10, 11) + rand.NextDouble();
			// B: от A+1 до A+6
			double b = a + rand.Next(1, 7) + rand.NextDouble();
			// dx: от 0.1 до 2.0
			double dx = 0.1 + rand.NextDouble() * 1.9;
			// eps: от 1e-7 до 1e-3
			double eps = Math.Pow(10, -rand.Next(3, 8)) * (0.5 + rand.NextDouble() * 1.5);

			AStr = a.ToString("F2");
			BStr = b.ToString("F2");
			DxStr = dx.ToString("F3");
			EpsStr = eps.ToString("E2");
		}

		protected override void ExecuteCompute(object parameter)
		{
			// Парсим значения
			if (!double.TryParse(AStr, out double a) || !double.TryParse(BStr, out double b) ||
				!double.TryParse(DxStr, out double dx) || !double.TryParse(EpsStr, out double eps))
			{
				ResultMessage = "Ошибка: все поля должны быть заполнены корректными числами.";
				return;
			}

			if (a > b || dx <= 0 || eps <= 0)
			{
				ResultMessage = "Ошибка: проверьте параметры (A<=B, dx>0, eps>0).";
				return;
			}

			Results.Clear();
			int steps = (int)((b - a) / dx) + 1;
			var list = new System.Collections.Generic.List<SeriesResult>();
			for (int i = 0; i < steps; i++)
			{
				double x = a + i * dx;
				if (x > b) break;
				var (sum, terms) = SeriesCalculator.ComputeExpSeries(x, eps);
				list.Add(new SeriesResult { X = x, Sum = sum, Terms = terms });
			}
			Results = new ObservableCollection<SeriesResult>(list.OrderBy(r => r.X));
			OnPropertyChanged(nameof(Results));

			ResultMessage = $"Таблица построена для {Results.Count} точек. Точность eps = {eps:E2}.";
			AddHistoryEntry($"A={a}, B={b}, dx={dx}, eps={eps}, точек={Results.Count}");
		}

		private void ExecuteShowSolution(object parameter)
		{
			if (Results.Count == 0)
			{
				System.Windows.MessageBox.Show("Сначала выполните вычисление.", "Информация", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
				return;
			}
			var first = Results.FirstOrDefault();
			if (first == null) return;
			var steps = $"Для x = {first.X:F3}, точное значение e^(-x) = {Math.Exp(-first.X):F6}\n" +
						$"Сумма ряда: {first.Sum:F6}, членов: {first.Terms}, погрешность: {first.Error:F8}";
			var window = new SolutionWindow(steps);
			window.ShowDialog();
		}

		private void ExecuteShowHistory(object parameter)
		{
			var window = new HistoryWindow(this);
			window.ShowDialog();
		}
	}
}
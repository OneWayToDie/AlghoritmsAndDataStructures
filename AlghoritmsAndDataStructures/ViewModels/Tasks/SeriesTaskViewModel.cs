using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using AlghoritmsAndDataStructures.Core.Calculators;
using AlghoritmsAndDataStructures.Helpers;
using AlghoritmsAndDataStructures.Properties;
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
		private int _seed;
		private double _lastEps = 1e-6;

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

		public int Seed
		{
			get => _seed;
			private set { _seed = value; OnPropertyChanged(nameof(Seed)); }
		}

		public ICommand ShowSolutionCommand { get; }
		public ICommand ShowHistoryCommand { get; }
		public ICommand GenerateCommand { get; }
		public ICommand PasteSeedCommand { get; }
		public ICommand ShowConvergenceCommand { get; }
		public ICommand ExportCommand { get; }

		public override string Title => "Задача: разложение e^(-x) в ряд";
		public override string HistoryKey => "SeriesTask";

		public SeriesTaskViewModel()
		{
			ShowSolutionCommand = new RelayCommand(ExecuteShowSolution);
			ShowHistoryCommand = new RelayCommand(ExecuteShowHistory);
			GenerateCommand = new RelayCommand(ExecuteGenerate);
			PasteSeedCommand = new RelayCommand(ExecutePasteSeed);
			ShowConvergenceCommand = new RelayCommand(ExecuteShowConvergence);
			ExportCommand = new RelayCommand(ExecuteExport);
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

			var window = new ConvergenceWindow(first.X, ParseEpsOrDefault());
			window.ShowDialog();
		}

		private void ExecuteGenerate(object parameter)
		{
			Seed = new Random().Next(1, int.MaxValue);
			Regenerate(Seed);
		}

		private void ExecutePasteSeed(object parameter)
		{
			string clip = Clipboard.ContainsText() ? Clipboard.GetText().Trim() : "";
			if (int.TryParse(clip, out int seed) && seed > 0)
			{
				Seed = seed;
				Regenerate(seed);
				return;
			}
			MessageBox.Show(
				"В буфере обмена нет корректного кода набора. Скопируйте его из истории (кнопка «Скопировать код набора»).",
				"Информация", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void Regenerate(int seed)
		{
			var rand = new Random(seed);
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

			_lastEps = eps;

			Results.Clear();
			double rawSteps = (b - a) / dx;
			if (double.IsNaN(rawSteps) || double.IsInfinity(rawSteps) || rawSteps <= 0)
			{
				ResultMessage = "Ошибка: невозможно вычислить количество точек (проверьте A, B, dx).";
				return;
			}

			int steps = (int)rawSteps + 1;
			int maxPoints = Settings.Default.MaxSeriesPoints;
			if (maxPoints < 100) maxPoints = 5000;
			if (steps > maxPoints)
			{
				ResultMessage = $"Ошибка: шаг dx слишком мал — получится более {maxPoints} точек.";
				return;
			}

			var list = new System.Collections.Generic.List<SeriesResult>();
			for (int i = 0; i < steps; i++)
			{
				double x = a + i * dx;
				if (x > b) break;
				var (sum, terms) = SeriesCalculator.ComputeExpSeries(x, eps);
				if (double.IsNaN(sum) || double.IsInfinity(sum))
				{
					ResultMessage = $"Ошибка: переполнение при вычислении в точке x = {x:F3} (значение слишком велико).";
					return;
				}
				list.Add(new SeriesResult { X = x, Sum = sum, Terms = terms });
			}
			Results = new ObservableCollection<SeriesResult>(list.OrderBy(r => r.X));
			OnPropertyChanged(nameof(Results));

			ResultMessage = $"Таблица построена для {Results.Count} точек. Точность eps = {eps:E2}.";
			AddHistoryEntry($"A={a}, B={b}, dx={dx}, eps={eps}, точек={Results.Count}; код={Seed}");
		}

		private double ParseEpsOrDefault()
		{
			if (double.TryParse(EpsStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double eps) && eps > 0)
				return eps;
			if (double.TryParse(EpsStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out eps) && eps > 0)
				return eps;
			return _lastEps > 0 ? _lastEps : 1e-6;
		}

		private void ExecuteShowSolution(object parameter)
		{
			if (Results.Count == 0)
			{
				System.Windows.MessageBox.Show("Сначала выполните вычисление.", "Информация", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
				return;
			}

			var sb = new System.Text.StringBuilder();
			sb.AppendLine($"Точность eps = {ParseEpsOrDefault():E2}\n");

			foreach (var r in Results)
			{
				double exact = Math.Exp(-r.X);
				sb.AppendLine(string.Format("x = {0:F3}:  сумма = {1:F6},  членов = {2},  точное = {3:F6},  погрешность = {4:E2}",
					r.X, r.Sum, r.Terms, exact, r.Error));
			}

			var window = new SolutionWindow(sb.ToString());
			window.ShowDialog();
		}

		private void ExecuteShowHistory(object parameter)
		{
			var window = new HistoryWindow(this);
			window.ShowDialog();
		}

		private void ExecuteExport(object parameter)
		{
			if (Results.Count == 0)
			{
				MessageBox.Show("Сначала выполните вычисление, чтобы получить данные.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			var headers = new[] { "X", "Sum", "Terms", "Exact", "Error" };
			var rows = Results.Select(r => new[]
			{
				r.X.ToString("F4", CultureInfo.InvariantCulture),
				r.Sum.ToString("F6", CultureInfo.InvariantCulture),
				r.Terms.ToString(),
				r.Exact.ToString("F6", CultureInfo.InvariantCulture),
				r.Error.ToString("E4", CultureInfo.InvariantCulture)
			});
			CsvExporter.Save(headers, rows, "series_exp_table");
		}
	}
}
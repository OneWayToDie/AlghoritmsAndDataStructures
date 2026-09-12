using System;
using System.Windows.Input;
using AlghoritmsAndDataStructures.Core.Calculators;
using AlghoritmsAndDataStructures.Helpers;
using AlghoritmsAndDataStructures.ViewModels.Base;
using AlghoritmsAndDataStructures.Views;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class GraphTaskViewModel : BaseTaskViewModel
	{
		private double _x = 0.0;
		private double _r = 3.0;
		private int _seed;

		public override string HistoryKey => "Graph";

		public double X
		{
			get => _x;
			set
			{
				_x = value;
				OnPropertyChanged(nameof(X));
			}
		}

		public double R
		{
			get => _r;
			set
			{
				_r = value;
				OnPropertyChanged(nameof(R));
			}
		}

		public int Seed
		{
			get => _seed;
			private set { _seed = value; OnPropertyChanged(nameof(Seed)); }
		}

		public ICommand ShowGraphCommand { get; }
		public ICommand ShowSolutionCommand { get; }
		public ICommand ShowHistoryCommand { get; }
		public ICommand GenerateCommand { get; }
		public ICommand PasteSeedCommand { get; }

		public override string Title => "ПР 2: вычисление функции по графику";

		public GraphTaskViewModel()
		{
			ShowGraphCommand = new RelayCommand(ExecuteShowGraph, CanShowGraph);
			ShowSolutionCommand = new RelayCommand(ExecuteShowSolution);
			ShowHistoryCommand = new RelayCommand(ExecuteShowHistory);
			GenerateCommand = new RelayCommand(ExecuteGenerate);
			PasteSeedCommand = new RelayCommand(ExecutePasteSeed);
		}

		protected override void ExecuteCompute(object parameter)
		{
			string error;
			var result = GraphCalculator.Compute(X, R, out error);

			if (result.HasValue)
			{
				ResultText = string.Format("Y = {0:F4}", result.Value);
				AddHistoryEntry(WithCode(string.Format("X={0:F2}, R={1:F2} → Y = {2:F4}", X, R, result.Value), Seed));
			}
			else
			{
				ResultText = string.Format("Ошибка: {0}", error);
			}
		}

		private void ExecuteGenerate(object parameter)
		{
			Seed = new Random().Next(1, int.MaxValue);
			Regenerate(Seed);
		}

		private void ExecutePasteSeed(object parameter)
		{
			string clip = System.Windows.Clipboard.ContainsText() ? System.Windows.Clipboard.GetText().Trim() : "";
			if (int.TryParse(clip, out int seed) && seed > 0)
			{
				Seed = seed;
				Regenerate(seed);
				return;
			}
			System.Windows.MessageBox.Show(
				"В буфере обмена нет корректного кода набора. Скопируйте его из истории (кнопка «Скопировать код набора»).",
				"Информация", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
		}

		private void Regenerate(int seed)
		{
			var rand = new Random(seed);
			double r;
			do
			{
				r = 0.5 + rand.NextDouble() * 4.5;
			} while (Math.Abs(r - 5.0) < 0.05);
			R = Math.Round(r, 2);
			X = Math.Round(rand.Next(-8, 9) + rand.NextDouble(), 2);
		}

		private void ExecuteShowSolution(object parameter)
		{
			string error;
			var result = GraphCalculator.Compute(X, R, out error);
			if (!result.HasValue)
			{
				System.Windows.MessageBox.Show(error, "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
				return;
			}

			string branch;
			if (X <= -5)
			{
				branch = string.Format("X = {0:F2} <= -5 → горизонтальный участок", X);
			}
			else if (X <= -R)
			{
				double slope = 3.0 / (5.0 - R);
				branch = string.Format(
					"-5 < X = {0:F2} <= -R = {1:F2} → левая наклонная прямая\nY = (3/(5-R))*(X+R) = {2:F4}*({3:F2}) = {4:F4}",
					X, -R, slope, X + R, result.Value);
			}
			else if (X <= R)
			{
				branch = string.Format(
					"-R = {0:F2} < X = {1:F2} <= R = {2:F2} → дуга окружности x^2 + y^2 = R^2\nY = sqrt(R^2 - X^2) = sqrt({3:F2} - {4:F2}) = {5:F4}",
					-R, X, R, R * R, X * X, result.Value);
			}
			else if (X <= 8)
			{
				double slope = 3.0 / (8.0 - R);
				branch = string.Format(
					"R = {0:F2} < X = {1:F2} <= 8 → правая наклонная прямая\nY = (3/(8-R))*(X-R) = {2:F4}*({3:F2}) = {4:F4}",
					R, X, slope, X - R, result.Value);
			}
			else
			{
				branch = string.Format("X = {0:F2} > 8 → горизонтальный участок", X);
			}

			var steps =
				"Вычисление функции по графику\n\n" +
				string.Format("Входные данные: X = {0:F2}, R = {1:F2}\n", X, R) +
				string.Format("Проверка: R = {0:F2} > 0, R < 5 — допустимо.\n\n", R) +
				"Выбранная ветка:\n" + branch + "\n\n" +
				string.Format("Результат: Y = {0:F4}", result.Value);

			var window = new SolutionWindow(steps);
			window.ShowDialog();
		}

		private void ExecuteShowHistory(object parameter)
		{
			var window = new HistoryWindow(this);
			window.ShowDialog();
		}

		private void ExecuteShowGraph(object parameter)
		{
			bool isDark = App.IsDarkTheme;
			var graphWindow = new GraphWindow(X, R, isDark);
			graphWindow.ShowDialog();
		}

		private bool CanShowGraph(object parameter)
		{
			string err;
			var test = GraphCalculator.Compute(X, R, out err);
			return test.HasValue;
		}
	}
}
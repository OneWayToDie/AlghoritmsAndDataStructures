using System;
using System.Windows.Input;
using AlghoritmsAndDataStructures.Core.Calculators;
using AlghoritmsAndDataStructures.ViewModels.Base;
using AlghoritmsAndDataStructures.Views;
using AlghoritmsAndDataStructures.Helpers;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class FractionTaskViewModel : BaseTaskViewModel
	{
		private int _m = 0;
		private int _n = 1;
		private int _integerLastDigit;
		private int _fractionFirstDigit;
		private string _calculationSteps = string.Empty;
		private int _seed;
		public override string HistoryKey => "Fraction";



		public int M
		{
			get => _m;
			set { _m = value; OnPropertyChanged(nameof(M)); }
		}

		public int N
		{
			get => _n;
			set { _n = value; OnPropertyChanged(nameof(N)); }
		}

		public int IntegerLastDigit
		{
			get => _integerLastDigit;
			private set { _integerLastDigit = value; OnPropertyChanged(nameof(IntegerLastDigit)); }
		}

		public int FractionFirstDigit
		{
			get => _fractionFirstDigit;
			private set { _fractionFirstDigit = value; OnPropertyChanged(nameof(FractionFirstDigit)); }
		}

		public string CalculationSteps
		{
			get => _calculationSteps;
			private set { _calculationSteps = value; OnPropertyChanged(nameof(CalculationSteps)); }
		}

		public int Seed
		{
			get => _seed;
			private set { _seed = value; OnPropertyChanged(nameof(Seed)); }
		}

		public override string Title => "ПР 1: Дробь M/N";

		protected override void ExecuteCompute(object parameter)
		{
			if (N <= 0)
			{
				IntegerLastDigit = 0;
				FractionFirstDigit = 0;
				ResultText = "Ошибка: N должно быть больше 0.";
				CalculationSteps = string.Empty;
				return;
			}

			var result = FractionCalculator.Compute(M, N);
			IntegerLastDigit = result.IntegerLastDigit;
			FractionFirstDigit = result.FractionFirstDigit;
			ResultText = "Вычислено успешно.";

			int integerPart = M / N;
			int remainder = M % N;

			CalculationSteps =
				$"Формулы:\n" +
				$"{M} / {N} = {integerPart} целых, остаток {remainder}\n" +
				$"Старшая цифра дробной части: ({Math.Abs(remainder)} * 10) / {N} = {FractionFirstDigit}\n" +
				$"Младшая цифра целой части: {integerPart} % 10 = {IntegerLastDigit}";

			string historyEntry = WithCode($"M={M}, N={N} → целая(мл.):{IntegerLastDigit}, дробная(ст.):{FractionFirstDigit}", Seed);
			AddHistoryEntry(historyEntry);
		}

		private void ExecuteGenerate(object parameter)
		{
			Seed = new System.Random().Next(1, int.MaxValue);
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
			var rand = new System.Random(seed);
			M = rand.Next(1, 1000);
			N = rand.Next(1, 100);
		}

		public ICommand ShowVisualizationCommand { get; }
		public ICommand ShowSolutionCommand { get; }
		public ICommand ShowHistoryCommand { get; }
		public ICommand GenerateCommand { get; }
		public ICommand PasteSeedCommand { get; }

		public FractionTaskViewModel()
		{
			ShowVisualizationCommand = new RelayCommand(ExecuteShowVisualization);
			ShowSolutionCommand = new RelayCommand(ExecuteShowSolution);
			ShowHistoryCommand = new RelayCommand(ExecuteShowHistory);
			GenerateCommand = new RelayCommand(ExecuteGenerate);
			PasteSeedCommand = new RelayCommand(ExecutePasteSeed);
		}

		private void ExecuteShowVisualization(object parameter)
		{
			var window = new FractionVisualizationWindow(M, N);
			window.ShowDialog();
		}

		private void ExecuteShowSolution(object parameter)
		{
			if (string.IsNullOrEmpty(CalculationSteps))
			{
				System.Windows.MessageBox.Show("Сначала выполните вычисление.", "Информация", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
				return;
			}
			var window = new SolutionWindow(CalculationSteps);
			window.ShowDialog();
		}

		private void ExecuteShowHistory(object parameter)
		{
			var window = new HistoryWindow(this);
			window.ShowDialog();
		}
	}
}
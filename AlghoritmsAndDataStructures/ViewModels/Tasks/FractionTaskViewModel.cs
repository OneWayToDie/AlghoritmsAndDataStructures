using AlghoritmsAndDataStructures.Core.Calculators;
using AlghoritmsAndDataStructures.ViewModels.Base;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class FractionTaskViewModel : BaseTaskViewModel
	{
		private int _m = 0;
		private int _n = 1;
		private int _integerLastDigit;
		private int _fractionFirstDigit;
		private string _calculationSteps = string.Empty;
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

		public override string Title => "Задача 2: Дробь M/N";

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
				$"Младшая цифра целой части: {integerPart} % 10 = {IntegerLastDigit}\n" +
				$"Старшая цифра дробной части: ({remainder} * 10) / {N} = {FractionFirstDigit}";

			string historyEntry = $"M={M}, N={N} → целая(мл.):{IntegerLastDigit}, дробная(ст.):{FractionFirstDigit}";
			AddHistoryEntry(historyEntry);
		}
	}
}
namespace AlghoritmsAndDataStructures.Core.Calculators
{
	public static class FractionCalculator
	{
		public static FractionResult Compute(int m, int n)
		{
			if (n <= 0)
				return new FractionResult(0, 0, false);

			int integerPart = m / n;
			int remainder = m % n;

			int lastDigit = integerPart % 10;
			if (integerPart < 0) lastDigit = -lastDigit;

			int firstFractionDigit = (remainder * 10) / n;

			return new FractionResult(lastDigit, firstFractionDigit, true);
		}
	}

	public class FractionResult
	{
		public int IntegerLastDigit { get; }
		public int FractionFirstDigit { get; }
		public bool Success { get; }

		public FractionResult(int integerLastDigit, int fractionFirstDigit, bool success)
		{
			IntegerLastDigit = integerLastDigit;
			FractionFirstDigit = fractionFirstDigit;
			Success = success;
		}
	}
}

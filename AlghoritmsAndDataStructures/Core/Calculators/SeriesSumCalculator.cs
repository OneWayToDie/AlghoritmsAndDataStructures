namespace AlghoritmsAndDataStructures.Core.Calculators
{
	public static class SeriesSumCalculator
	{
		public static double ComputeSum(int n)
		{
			if (n < 2) return 0;
			double sum = 0;
			for (int k = 1; k <= n; k++)
				sum += (double)k / (k + 1);
			return sum;
		}
	}
}

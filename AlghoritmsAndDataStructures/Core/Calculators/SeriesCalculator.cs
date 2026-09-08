using System;

namespace AlghoritmsAndDataStructures.Core.Calculators
{
	public static class SeriesCalculator
	{
		private const int MaxTerms = 10000;

		public static (double sum, int terms) ComputeExpSeries(double x, double eps)
		{
			if (eps <= 0) eps = 1e-6;

			double sum = 1.0;
			double term = 1.0;
			int n = 0;
			while (Math.Abs(term) > eps && n < MaxTerms)
			{
				n++;
				term *= (-x) / n;
				if (double.IsNaN(term) || double.IsInfinity(term))
				{
					sum = double.NaN;
					break;
				}
				sum += term;
				if (double.IsNaN(sum) || double.IsInfinity(sum))
				{
					sum = double.NaN;
					break;
				}
			}
			return (sum, n + 1);
		}
	}
}

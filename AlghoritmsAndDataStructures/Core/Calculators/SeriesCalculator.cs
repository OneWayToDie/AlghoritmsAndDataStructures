using System;

namespace AlghoritmsAndDataStructures.Core.Calculators
{
	public static class SeriesCalculator
	{
		public static (double sum, int terms) ComputeExpSeries(double x, double eps)
		{
			if (eps <= 0) eps = 1e-6;

			double sum = 1.0;
			double term = 1.0;
			int n = 0;
			while (Math.Abs(term) > eps)
			{
				n++;
				term *= (-x) / n;
				sum += term;
			}
			return (sum, n + 1);
		}
	}
}

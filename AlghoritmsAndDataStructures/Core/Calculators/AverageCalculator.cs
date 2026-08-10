using System;
using System.Collections.Generic;
using System.Linq;

namespace AlghoritmsAndDataStructures.Core.Calculators
{
	public static class AverageCalculator
	{
		public static (List<int> threeDigitNumbers, double? average, string message) ComputeAverage(string input)
		{
			var numbers = input.Split(new[] { ',', ' ', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries)
							   .Select(s => int.TryParse(s, out int val) ? (int?)val : null)
							   .Where(x => x.HasValue)
							   .Select(x => x.Value)
							   .ToList();

			var threeDigit = numbers.Where(x => x >= 100 && x <= 999).ToList();

			if (threeDigit.Count == 0)
				return (threeDigit, null, "Трёхзначных чисел нет в последовательности.");

			double avg = threeDigit.Average();
			return (threeDigit, avg, $"Найдено {threeDigit.Count} трёхзначных чисел. Среднее: {avg:F2}");
		}
	}
}

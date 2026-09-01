using System;
using System.Collections.Generic;
using System.Linq;

namespace AlghoritmsAndDataStructures.Core.Calculators
{
	public static class NeighborhoodAverageCalculator
	{
		public static double[] ComputeAverages(int[] arr)
		{
			if (arr.Length < 2)
				return arr.Select(x => (double)x).ToArray();

			var result = new double[arr.Length];

			// Первый элемент: среднее с правым соседом
			result[0] = Math.Round((arr[0] + arr[1]) / 2.0, 2);

			// Средние элементы: среднее с двумя соседями
			for (int i = 1; i < arr.Length - 1; i++)
			{
				result[i] = Math.Round((arr[i - 1] + arr[i] + arr[i + 1]) / 3.0, 2);
			}

			// Последний элемент: среднее с левым соседом
			result[arr.Length - 1] = Math.Round((arr[arr.Length - 2] + arr[arr.Length - 1]) / 2.0, 2);

			return result;
		}

		public static string GetComputationSteps(int[] original, double[] result)
		{
			var steps = new List<string>();
			steps.Add("Исходный массив: [" + string.Join(", ", original) + "]");
			steps.Add("Размер массива: " + original.Length);
			steps.Add("");

			for (int i = 0; i < original.Length; i++)
			{
				string formula;
				if (original.Length == 1)
				{
					formula = string.Format("arr[{0}] = {1} (единственный элемент)", i, original[i]);
				}
				else if (i == 0)
				{
					formula = string.Format("arr[{0}] = ({1} + {2}) / 2 = {3}",
						i, original[i], original[i + 1], result[i]);
				}
				else if (i == original.Length - 1)
				{
					formula = string.Format("arr[{0}] = ({1} + {2}) / 2 = {3}",
						i, original[i - 1], original[i], result[i]);
				}
				else
				{
					formula = string.Format("arr[{0}] = ({1} + {2} + {3}) / 3 = {4}",
						i, original[i - 1], original[i], original[i + 1], result[i]);
				}
				steps.Add(formula);
			}

			steps.Add("");
			steps.Add("Результат: [" + string.Join(", ", result.Select(x => x.ToString("F2"))) + "]");
			return string.Join("\n", steps);
		}
	}
}

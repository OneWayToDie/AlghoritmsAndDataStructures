using System;
using System.Collections.Generic;
using System.Linq;

namespace AlghoritmsAndDataStructures.Core.Calculators
{
	public static class ArrayProcessor
	{
		public static (double average, double[] modifiedArray) ProcessArray(int[] inputArray)
		{
			if (inputArray.Length != 12)
				throw new ArgumentException("Массив должен содержать ровно 12 элементов.");

			// Шаг 1: элементы на нечётных позициях (индексы 1, 3, 5, 7, 9, 11)
			var oddPositions = new List<double>();
			for (int i = 0; i < inputArray.Length; i++)
			{
				if (i % 2 == 1)
					oddPositions.Add(inputArray[i]);
			}

			// Шаг 2: среднее арифметическое (округлить до сотых)
			double average = Math.Round(oddPositions.Average(), 2);

			// Шаг 3: заменить элементы, кратные 3, на среднее (с сохранением дробной части)
			var modified = new double[inputArray.Length];
			for (int i = 0; i < inputArray.Length; i++)
			{
				if (inputArray[i] % 3 == 0)
					modified[i] = average;   // сохраняем дробную часть
				else
					modified[i] = inputArray[i];
			}

			return (average, modified);
		}
	}
}
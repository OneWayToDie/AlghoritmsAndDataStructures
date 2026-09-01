using System;
using System.Collections.Generic;
using System.Linq;

namespace AlghoritmsAndDataStructures.Core.Calculators
{
	public static class QuickSortCalculator
	{
		private static readonly Random _rand = new Random();

		public static int[] GenerateRandomArray(int count, int min, int max)
		{
			var arr = new int[count];
			for (int i = 0; i < count; i++)
				arr[i] = _rand.Next(min, max + 1);
			return arr;
		}

		public static void SortDescending(int[] arr, int low, int high)
		{
			if (low < high)
			{
				int pivotIndex = Partition(arr, low, high);
				SortDescending(arr, low, pivotIndex - 1);
				SortDescending(arr, pivotIndex + 1, high);
			}
		}

		private static int Partition(int[] arr, int low, int high)
		{
			int pivot = arr[high];
			int i = low - 1;

			for (int j = low; j < high; j++)
			{
				if (arr[j] >= pivot)
				{
					i++;
					Swap(arr, i, j);
				}
			}

			Swap(arr, i + 1, high);
			return i + 1;
		}

		private static void Swap(int[] arr, int a, int b)
		{
			int temp = arr[a];
			arr[a] = arr[b];
			arr[b] = temp;
		}

		public static string GetSortSteps(int[] original, int[] sorted)
		{
			var steps = new List<string>();
			steps.Add("Исходный массив: [" + string.Join(", ", original) + "]");
			steps.Add("Количество элементов: " + original.Length);
			steps.Add("");
			stepQuickSort(original.ToArray(), 0, original.Length - 1, steps);
			steps.Add("");
			steps.Add("Результат сортировки (по убыванию): [" + string.Join(", ", sorted) + "]");
			return string.Join("\n", steps);
		}

		private static void stepQuickSort(int[] arr, int low, int high, List<string> steps)
		{
			if (low < high)
			{
				int pivotIndex = Partition(arr, low, high);
				steps.Add(string.Format("Опорный элемент: arr[{0}] = {1}. После разбиения: [{2}]",
					pivotIndex, arr[pivotIndex], string.Join(", ", arr)));
				stepQuickSort(arr, low, pivotIndex - 1, steps);
				stepQuickSort(arr, pivotIndex + 1, high, steps);
			}
		}
	}
}

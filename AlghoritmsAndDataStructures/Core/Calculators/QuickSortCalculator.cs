using System;
using System.Collections.Generic;
using System.Linq;

namespace AlghoritmsAndDataStructures.Core.Calculators
{
	public enum QuickSortStepType
	{
		Initial,
		PivotSelected,
		Compare,
		Swap,
		PivotPlaced,
		Complete
	}

	public class QuickSortStep
	{
		public int[] Array { get; set; }          // снимок массива на этом шаге
		public QuickSortStepType Type { get; set; }
		public int IndexA { get; set; }           // индекс i (граница разбиения) или -1
		public int IndexB { get; set; }           // индекс j (сравниваемый) или -1
		public int PivotIndex { get; set; }       // позиция опорного элемента
		public int Low { get; set; }              // границы текущего подмассива
		public int High { get; set; }
		public int Comparisons { get; set; }      // накопительные счётчики
		public int Swaps { get; set; }
		public string Description { get; set; }
	}

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
			int comparisons = 0;
			int swaps = 0;
			SortDescendingCounted(arr, low, high, ref comparisons, ref swaps);
		}

		public static (int[] sorted, int comparisons, int swaps) SortWithMetrics(int[] arr)
		{
			var clone = (int[])arr.Clone();
			int comparisons = 0;
			int swaps = 0;
			SortDescendingCounted(clone, 0, clone.Length - 1, ref comparisons, ref swaps);
			return (clone, comparisons, swaps);
		}

		private static void SortDescendingCounted(int[] arr, int low, int high,
			ref int comparisons, ref int swaps)
		{
			if (low < high)
			{
				int pivotIndex = PartitionCounted(arr, low, high, ref comparisons, ref swaps);
				SortDescendingCounted(arr, low, pivotIndex - 1, ref comparisons, ref swaps);
				SortDescendingCounted(arr, pivotIndex + 1, high, ref comparisons, ref swaps);
			}
		}

		private static int PartitionCounted(int[] arr, int low, int high,
			ref int comparisons, ref int swaps)
		{
			int pivot = arr[high];
			int i = low - 1;

			for (int j = low; j < high; j++)
			{
				comparisons++;
				if (arr[j] >= pivot)
				{
					i++;
					Swap(arr, i, j);
					swaps++;
				}
			}

			Swap(arr, i + 1, high);
			swaps++;
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

			int comparisons = 0;
			int swaps = 0;
			stepQuickSort(original.ToArray(), 0, original.Length - 1, steps, ref comparisons, ref swaps);

			steps.Add("");
			steps.Add("Результат сортировки (по убыванию): [" + string.Join(", ", sorted) + "]");
			steps.Add(string.Format("Всего выполнено: {0} сравнений, {1} перестановок.", comparisons, swaps));
			return string.Join("\n", steps);
		}

		private static void stepQuickSort(int[] arr, int low, int high, List<string> steps,
			ref int comparisons, ref int swaps)
		{
			if (low < high)
			{
				int pivotIndex = PartitionCounted(arr, low, high, ref comparisons, ref swaps);
				steps.Add(string.Format("Опорный элемент: arr[{0}] = {1}. После разбиения: [{2}]",
					pivotIndex, arr[pivotIndex], string.Join(", ", arr)));
				stepQuickSort(arr, low, pivotIndex - 1, steps, ref comparisons, ref swaps);
				stepQuickSort(arr, pivotIndex + 1, high, steps, ref comparisons, ref swaps);
			}
		}

		/// <summary>
		/// Выполняет быструю сортировку по убыванию с записью всех шагов
		/// для визуализации. Исходный массив не изменяется.
		/// </summary>
		public static List<QuickSortStep> TraceSort(int[] original)
		{
			var arr = (int[])original.Clone();
			var steps = new List<QuickSortStep>();
			int comparisons = 0;
			int swaps = 0;

			AddStep(steps, arr, QuickSortStepType.Initial, -1, -1, -1, 0, arr.Length - 1,
				comparisons, swaps, $"Исходный массив из {arr.Length} элементов. Начало сортировки по убыванию.");

			TraceQuickSort(arr, 0, arr.Length - 1, steps, ref comparisons, ref swaps);

			AddStep(steps, arr, QuickSortStepType.Complete, -1, -1, -1, 0, arr.Length - 1,
				comparisons, swaps, $"Сортировка завершена. Итог: {comparisons} сравнений, {swaps} перестановок.");

			return steps;
		}

		private static void TraceQuickSort(int[] arr, int low, int high, List<QuickSortStep> steps,
			ref int comparisons, ref int swaps)
		{
			if (low >= high) return;

			int pivotIndex = TracePartition(arr, low, high, steps, ref comparisons, ref swaps);
			TraceQuickSort(arr, low, pivotIndex - 1, steps, ref comparisons, ref swaps);
			TraceQuickSort(arr, pivotIndex + 1, high, steps, ref comparisons, ref swaps);
		}

		private static int TracePartition(int[] arr, int low, int high, List<QuickSortStep> steps,
			ref int comparisons, ref int swaps)
		{
			int pivot = arr[high];
			int i = low - 1;

			AddStep(steps, arr, QuickSortStepType.PivotSelected, i, -1, high, low, high,
				comparisons, swaps,
				$"Разбиение подмассива [{low}..{high}]. Опорный элемент: arr[{high}] = {pivot}.");

			for (int j = low; j < high; j++)
			{
				comparisons++;
				AddStep(steps, arr, QuickSortStepType.Compare, j, i, high, low, high,
					comparisons, swaps,
					$"Сравнение: arr[{j}] = {arr[j]} {(arr[j] >= pivot ? ">= опорного" : "< опорного")} {pivot}.");

				if (arr[j] >= pivot)
				{
					i++;
					Swap(arr, i, j);
					swaps++;
					AddStep(steps, arr, QuickSortStepType.Swap, i, j, high, low, high,
						comparisons, swaps,
						$"Обмен: arr[{i}] ({arr[i]}) ↔ arr[{j}] ({arr[j]}).");
				}
			}

			Swap(arr, i + 1, high);
			swaps++;

			AddStep(steps, arr, QuickSortStepType.PivotPlaced, i + 1, -1, i + 1, low, high,
				comparisons, swaps,
				$"Опорный элемент встал на позицию [{i + 1}] = {arr[i + 1]}.");

			return i + 1;
		}

		private static void AddStep(List<QuickSortStep> steps, int[] arr, QuickSortStepType type,
			int indexA, int indexB, int pivotIndex, int low, int high,
			int comparisons, int swaps, string description)
		{
			steps.Add(new QuickSortStep
			{
				Array = (int[])arr.Clone(),
				Type = type,
				IndexA = indexA,
				IndexB = indexB,
				PivotIndex = pivotIndex,
				Low = low,
				High = high,
				Comparisons = comparisons,
				Swaps = swaps,
				Description = description
			});
		}
	}
}

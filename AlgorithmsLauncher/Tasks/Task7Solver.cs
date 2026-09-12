using System;
using System.Diagnostics;
using System.Linq;
using AlghoritmsAndDataStructures.Core.Calculators;

namespace AlgorithmsLauncher.Tasks
{
	public static class Task7Solver
	{
		public static void Run()
		{
			while (true)
			{
				ConsoleUI.Clear();
				ConsoleUI.Header("ПРАКТИЧЕСКАЯ РАБОТА 7");
				ConsoleUI.MenuItem(1, "Быстрая сортировка массива по убыванию", "\u2B07\uFE0F");
				ConsoleUI.MenuItem(0, "Назад в меню", "\u21A9\uFE0F");
				Console.WriteLine();
				if (ConsoleUI.AskChoice(0, 1) == 0) return;
				Solve();
			}
		}

		private static void Solve()
		{
			ConsoleUI.Clear();
			ConsoleUI.Header("7 — БЫСТРАЯ СОРТИРОВКА");
			ConsoleUI.Condition(
				"Отсортировать массив целых чисел по убыванию методом быстрой сортировки\n" +
				"(опорный элемент — последний элемент подмассива, Хоара с разбиением Ломуто).");

			int[] arr = AskArray();
			if (arr == null) return;

			var sw = Stopwatch.StartNew();
			int[] sorted = QuickSortCalculator.SortWithMetrics(arr).sorted;
			sw.Stop();

			Console.WriteLine();
			ConsoleUI.Step("Шаги сортировки:");
			ConsoleUI.Block(QuickSortCalculator.GetSortSteps(arr, sorted));
			ConsoleUI.Good("Ответ: отсортированный массив: [" + string.Join(", ", sorted) + "].");
			Console.WriteLine();
			ConsoleUI.PrintBarChart(arr, "Исходный массив");
			ConsoleUI.PrintBarChart(sorted, "Результат сортировки (по убыванию)");
			ConsoleUI.Hint("Время вычисления: " + ConsoleUI.FormatTime(sw.Elapsed));
			HistoryStorage.Save(new HistoryEntry
			{
				Timestamp = DateTime.Now, TaskName = "ПР 7 — Сортировка",
				InputSummary = $"{arr.Length} элементов",
				ResultSummary = $"[{string.Join(",", sorted.Take(5))}{(sorted.Length > 5 ? ",..." : "")}]",
				Elapsed = ConsoleUI.FormatTime(sw.Elapsed)
			});
			ConsoleUI.Pause();
		}

		private static int[] AskArray()
		{
			int mode = ConsoleUI.AskMode();
			if (mode == 0) return null;
			if (mode == 2)
			{
				int seed = ConsoleUI.AskSeed();
				int[] arr = QuickSortCalculator.GenerateRandomArray(25, -20, 20, seed);
				ConsoleUI.Hint("Сгенерировано: 25 элементов из [-20; 20]");
				return arr;
			}
			return ConsoleUI.ReadIntArray("Введите от 5 до 40 целых чисел:", 5, 40);
		}
	}
}
using System;
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
				ConsoleUI.MenuItem(1, "Быстрая сортировка массива по убыванию");
				ConsoleUI.MenuItem(0, "Назад в меню");
				Console.WriteLine();
				int choice = ConsoleUI.AskChoice(0, 1);
				if (choice == 0) return;
				Solve();
			}
		}

		private static void Solve()
		{
			ConsoleUI.Clear();
			ConsoleUI.Header("7 — БЫСТРАЯ СОРТИРОВКА", ConsoleColor.Cyan);
			ConsoleUI.Condition(
				"Отсортировать массив целых чисел по убыванию методом быстрой сортировки\n" +
				"(опорный элемент — последний элемент подмассива, Хоара с разбиением Ломуто).");

			int mode = ConsoleUI.AskMode();
			if (mode == 0) return;

			int[] arr;
			if (mode == 2)
			{
				int seed = ConsoleUI.AskSeed();
				arr = QuickSortCalculator.GenerateRandomArray(25, -20, 20, seed);
				ConsoleUI.Hint("Сгенерировано: 25 элементов из [-20; 20]");
			}
			else
			{
				arr = ConsoleUI.ReadIntArray("Введите от 5 до 40 целых чисел:", 5, 40);
			}

			int[] sorted = QuickSortCalculator.SortWithMetrics(arr).sorted;

			Console.WriteLine();
			ConsoleUI.Step("Шаги сортировки:");
			ConsoleUI.Block(QuickSortCalculator.GetSortSteps(arr, sorted));
			ConsoleUI.Good("Ответ: отсортированный массив: [" + string.Join(", ", sorted) + "].");
			ConsoleUI.Pause();
		}
	}
}
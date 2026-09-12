using System;
using System.Diagnostics;
using System.Linq;
using AlghoritmsAndDataStructures.Core.Calculators;

namespace AlgorithmsLauncher.Tasks
{
	public static class Task8Solver
	{
		public static void Run()
		{
			while (true)
			{
				ConsoleUI.Clear();
				ConsoleUI.Header("ПРАКТИЧЕСКАЯ РАБОТА 8");
				ConsoleUI.MenuItem(1, "Среднее арифметическое соседей", "\uD83D\uDD04");
				ConsoleUI.MenuItem(0, "Назад в меню", "\u21A9\uFE0F");
				Console.WriteLine();
				int choice = ConsoleUI.AskChoice(0, 1);
				if (choice == 0) return;
				Solve();
			}
		}

		private static void Solve()
		{
			ConsoleUI.Clear();
			ConsoleUI.Header("8 — СРЕДНЕЕ АРИФМЕТИЧЕСКОЕ СОСЕДЕЙ");
			ConsoleUI.Condition(
				"Заменить каждый элемент массива средним арифметическим его соседей:\n" +
				"первый элемент — среднее с правым соседом, внутренние — с двумя соседями,\n" +
				"последний — среднее с левым соседом. Результат округляется до сотых.");

			int mode = ConsoleUI.AskMode();
			if (mode == 0) return;

			int[] arr;
			if (mode == 2)
			{
				int seed = ConsoleUI.AskSeed();
				Random rnd = new Random(seed);
				int count = rnd.Next(5, 13);
				arr = new int[count];
				for (int i = 0; i < count; i++) arr[i] = rnd.Next(-20, 21);
				ConsoleUI.Hint($"Сгенерировано: {count} элементов из [-20; 20]");
			}
			else
			{
				arr = ConsoleUI.ReadIntArray("Введите целые числа (от 2 до 30):", 2, 30);
			}

			var sw = Stopwatch.StartNew();
			double[] result = NeighborhoodAverageCalculator.ComputeAverages(arr);
			sw.Stop();

			Console.WriteLine();
			ConsoleUI.Step("Шаги вычисления:");
			ConsoleUI.Block(NeighborhoodAverageCalculator.GetComputationSteps(arr, result));
			ConsoleUI.Good("Ответ: массив результатов (округление до сотых): [" +
				string.Join(", ", result.Select(x => x.ToString("F2")).ToArray()) + "].");
			Console.WriteLine();
			ConsoleUI.PrintBarChart(arr, "Исходный массив");
			ConsoleUI.PrintBarChart(result, "Результат (среднее арифметическое соседей)");
			ConsoleUI.Hint("Время вычисления: " + ConsoleUI.FormatTime(sw.Elapsed));
			HistoryStorage.Save(new HistoryEntry
			{
				Timestamp = DateTime.Now, TaskName = "ПР 8 — Соседи",
				InputSummary = $"{arr.Length} элементов",
				ResultSummary = $"[{string.Join(",", result.Select(x => x.ToString("0.#")).Take(5))}{(result.Length > 5 ? ",..." : "")}]",
				Elapsed = ConsoleUI.FormatTime(sw.Elapsed)
			});
			ConsoleUI.Pause();
		}
	}
}
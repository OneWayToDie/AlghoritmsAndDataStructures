using System;
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
				ConsoleUI.MenuItem(1, "Среднее арифметическое соседей");
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
			ConsoleUI.Header("8 — СРЕДНЕЕ АРИФМЕТИЧЕСКОЕ СОСЕДЕЙ", ConsoleColor.Cyan);
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

			double[] result = NeighborhoodAverageCalculator.ComputeAverages(arr);

			Console.WriteLine();
			ConsoleUI.Step("Шаги вычисления:");
			ConsoleUI.Block(NeighborhoodAverageCalculator.GetComputationSteps(arr, result));
			ConsoleUI.Good("Ответ: массив результатов (округление до сотых): [" +
				string.Join(", ", result.Select(x => x.ToString("F2")).ToArray()) + "].");
			ConsoleUI.Pause();
		}
	}
}
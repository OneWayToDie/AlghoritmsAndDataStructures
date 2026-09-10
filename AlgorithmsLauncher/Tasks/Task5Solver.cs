using System;
using System.Collections.Generic;
using System.Linq;
using AlghoritmsAndDataStructures.Core.Calculators;

namespace AlgorithmsLauncher.Tasks
{
	public static class Task5Solver
	{
		public static void Run()
		{
			while (true)
			{
				ConsoleUI.Clear();
				ConsoleUI.Header("ПРАКТИЧЕСКАЯ РАБОТА 5");
				ConsoleUI.MenuItem(1, "Массив: среднее на нечётных местах");
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
			ConsoleUI.Header("5 — СРЕДНЕЕ НА НЕЧЁТНЫХ МЕСТАХ", ConsoleColor.Cyan);
			ConsoleUI.Condition(
				"Дан массив A(N), N = 12. Найти среднее арифметическое элементов, стоящих на\n" +
				"нечётных позициях (1, 3, 5, 7, 9, 11), и заменить этим средним элементы, кратные 3.");

			int mode = ConsoleUI.AskMode();
			if (mode == 0) return;

			int[] arr;
			if (mode == 2)
			{
				int seed = ConsoleUI.AskSeed();
				Random rnd = new Random(seed);
				arr = Enumerable.Range(0, 12).Select(_ => rnd.Next(-20, 21)).ToArray();
				ConsoleUI.Hint("Сгенерировано: 12 элементов из [-20; 20]");
			}
			else
			{
				arr = ConsoleUI.ReadIntArray("Введите ровно 12 целых чисел:", 12, 12);
			}

			(double average, double[] modified) = ArrayProcessor.ProcessArray(arr);

			Console.WriteLine();
			ConsoleUI.Step("Исходный массив: " + ArrayText(arr));
			ConsoleUI.Step("Элементы на нечётных позициях (позиции 1, 3, 5, 7, 9, 11):");

			var oddPositions = new List<double>();
			for (int i = 0; i < arr.Length; i += 2)
			{
				oddPositions.Add(arr[i]);
				ConsoleUI.Step($"  позиция {i + 1}: {arr[i]}");
			}

			ConsoleUI.Step($"Среднее арифметическое = ({string.Join(" + ", oddPositions)}) / 6 = {average:0.##}");

			ConsoleUI.Step("Замена элементов, кратных 3, на среднее:");
			bool any = false;
			for (int i = 0; i < arr.Length; i++)
			{
				if (arr[i] % 3 == 0)
				{
					any = true;
					ConsoleUI.Step($"  {arr[i]} (позиция {i + 1}) \u2192 {average:0.##}");
				}
			}
			if (!any)
				ConsoleUI.Step("  кратных 3 нет — массив не изменяется");

			Console.WriteLine();
			ConsoleUI.Good("Итоговый массив: " + ArrayText(modified));
			ConsoleUI.Pause();
		}

		private static string ArrayText(int[] a) => "[" + string.Join(", ", a) + "]";

		private static string ArrayText(double[] a) =>
			"[" + string.Join(", ", a.Select(x => x.ToString("0.##")).ToArray()) + "]";
	}
}
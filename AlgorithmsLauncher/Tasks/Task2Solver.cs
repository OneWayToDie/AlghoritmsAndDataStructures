using System;
using System.Diagnostics;
using AlghoritmsAndDataStructures.Core.Calculators;

namespace AlgorithmsLauncher.Tasks
{
	public static class Task2Solver
	{
		public static void Run()
		{
			while (true)
			{
				ConsoleUI.Clear();
				ConsoleUI.Header("ПРАКТИЧЕСКАЯ РАБОТА 2");
				ConsoleUI.MenuItem(1, "Вычислить значение функции по графику");
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
			ConsoleUI.Header("2 — ФУНКЦИЯ, ЗАДАННАЯ ГРАФИКОМ", ConsoleColor.Cyan);
			ConsoleUI.Condition(
				"Вычислить значение функции, заданной графиком. Параметр R вводится (0 < R < 5):\n" +
				"  x ≤ -5              →  y = -3\n" +
				"  -5 < x ≤ -R         →  y = 3·(x + R) / (5 - R)\n" +
				"  -R < x ≤ R          →  y = √(R² - x²)\n" +
				"  R < x ≤ 8           →  y = 3·(x - R) / (8 - R)\n" +
				"  x > 8               →  y = 3");

			int mode = ConsoleUI.AskMode();
			if (mode == 0) return;

			double r, x;
			if (mode == 2)
			{
				int seed = ConsoleUI.AskSeed();
				Random rnd = new Random(seed);
				r = Math.Round(0.5 + rnd.NextDouble() * 4.5, 2);
				x = Math.Round(rnd.Next(-8, 9) + rnd.NextDouble(), 2);
				ConsoleUI.Hint($"Сгенерировано: R = {r:0.##}, X = {x:0.##}");
			}
			else
			{
				r = ConsoleUI.ReadDouble("Введите R (0 < R < 5):", 0.0001, 4.9999).Value;
				x = ConsoleUI.ReadDouble("Введите X:", -1e9, 1e9).Value;
			}

			var sw = Stopwatch.StartNew();
			double? result = GraphCalculator.Compute(x, r, out string errorMessage);
			sw.Stop();
			if (result == null)
			{
				ConsoleUI.Error(errorMessage);
				ConsoleUI.Pause();
				return;
			}
			double y = result.Value;

			Console.WriteLine();
			ConsoleUI.Info($"Входные данные: X = {x:0.##}, R = {r:0.##}");
			string branch;
			if (x <= -5)
				branch = $"x = {x:0.##} ≤ -5 → прямая: y = -3";
			else if (x <= -r)
				branch = $"-5 < x = {x:0.##} ≤ -R → наклонная: y = 3·(x + R) / (5 - R) = 3·({x:0.##} + {r:0.##}) / (5 - {r:0.##})";
			else if (x <= r)
				branch = $"-R < x = {x:0.##} ≤ R → полуокружность: y = √(R² - x²) = √({r:0.##}² - {x:0.##}²)";
			else if (x <= 8)
				branch = $"R < x = {x:0.##} ≤ 8 → наклонная: y = 3·(x - R) / (8 - R) = 3·({x:0.##} - {r:0.##}) / (8 - {r:0.##})";
			else
				branch = $"x = {x:0.##} > 8 → прямая: y = 3";
			ConsoleUI.Step(branch);
			Console.WriteLine();
			ConsoleUI.Good($"Ответ: при X = {x:0.##}, R = {r:0.##} значение функции Y = {y:0.###}.");
			ConsoleUI.Hint("Время вычисления: " + ConsoleUI.FormatTime(sw.Elapsed));
			HistoryStorage.Save(new HistoryEntry
			{
				Timestamp = DateTime.Now, TaskName = "ПР 2 — Функция",
				InputSummary = $"x={x:0.##}, R={r:0.##}",
				ResultSummary = $"y={y:0.###}",
				Elapsed = ConsoleUI.FormatTime(sw.Elapsed)
			});
			ConsoleUI.Pause();
		}
	}
}
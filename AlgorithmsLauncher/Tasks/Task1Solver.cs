using System;
using System.Diagnostics;
using AlghoritmsAndDataStructures.Core.Calculators;

namespace AlgorithmsLauncher.Tasks
{
	public static class Task1Solver
	{
		public static void Run()
		{
			while (true)
			{
				ConsoleUI.Clear();
				ConsoleUI.Header("ПРАКТИЧЕСКАЯ РАБОТА 1");
				ConsoleUI.MenuItem(1, "Куб — площадь грани, полная поверхность, объём");
				ConsoleUI.MenuItem(2, "Дробь M/N — цифры целой и дробной части");
				ConsoleUI.MenuItem(0, "Назад в меню");
				Console.WriteLine();
				int choice = ConsoleUI.AskChoice(0, 2);
				if (choice == 0) return;
				if (choice == 1) SolveCube();
				else SolveFraction();
			}
		}

		private static void SolveCube()
		{
			ConsoleUI.Clear();
			ConsoleUI.Header("1.1 — КУБ", ConsoleColor.Cyan);
			ConsoleUI.Condition(
				"Дана грань куба a. Найти площадь грани Sгр, площадь полной поверхности Sполн и объём V:\n" +
				"Sгр = a², Sполн = 6·a², V = a³.");

			int mode = ConsoleUI.AskMode();
			if (mode == 0) return;

			double a;
			if (mode == 2)
			{
				int seed = ConsoleUI.AskSeed();
				a = Math.Round(1.0 + new Random(seed).NextDouble() * 9.0, 2);
				ConsoleUI.Hint($"Сгенерировано: a = {a:0.##}");
			}
			else
			{
				a = ConsoleUI.ReadDouble("Введите сторону куба a (a > 0):", 0.0001, 1e9).Value;
			}

			var sw = Stopwatch.StartNew();
			CubeResult r = CubeCalculator.Compute(a);
			sw.Stop();
			if (!r.Success)
			{
				ConsoleUI.Error("Некорректный ввод: сторону куба нужно задать положительным числом.");
				ConsoleUI.Pause();
				return;
			}

			Console.WriteLine();
			ConsoleUI.Step($"Sгр   = a²        = {a:0.##}²       = {r.FaceArea:0.###}");
			ConsoleUI.Step($"Sполн = 6·a²      = 6 × {r.FaceArea:0.###}  = {r.TotalSurface:0.###}");
			ConsoleUI.Step($"V     = a³        = {a:0.##}³       = {r.Volume:0.###}");
			Console.WriteLine();
			ConsoleUI.Good($"Ответ: Sгр = {r.FaceArea:0.###}; Sполн = {r.TotalSurface:0.###}; V = {r.Volume:0.###}.");
			ConsoleUI.Hint("Время вычисления: " + ConsoleUI.FormatTime(sw.Elapsed));
			HistoryStorage.Save(new HistoryEntry
			{
				Timestamp = DateTime.Now, TaskName = "ПР 1.1 — Куб",
				InputSummary = $"a = {a:0.##}",
				ResultSummary = $"Sгр={r.FaceArea:0.##}, V={r.Volume:0.##}",
				Elapsed = ConsoleUI.FormatTime(sw.Elapsed)
			});
			ConsoleUI.Pause();
		}

		private static void SolveFraction()
		{
			ConsoleUI.Clear();
			ConsoleUI.Header("1.2 — ДРОБЬ M/N", ConsoleColor.Cyan);
			ConsoleUI.Condition(
				"Дана дробь M/N (M, N — натуральные числа). Найти младшую цифру целой части дроби\n" +
				"и старшую цифру дробной части.");

			int mode = ConsoleUI.AskMode();
			if (mode == 0) return;

			int m, n;
			if (mode == 2)
			{
				int seed = ConsoleUI.AskSeed();
				Random rnd = new Random(seed);
				m = rnd.Next(1, 1000);
				n = rnd.Next(1, 100);
				ConsoleUI.Hint($"Сгенерировано: M = {m}, N = {n}");
			}
			else
			{
				m = ConsoleUI.ReadInt("Введите M (числитель, натуральное):", 1, int.MaxValue).Value;
				n = ConsoleUI.ReadInt("Введите N (знаменатель, натуральное):", 1, int.MaxValue).Value;
			}

			var sw = Stopwatch.StartNew();
			FractionResult res = FractionCalculator.Compute(m, n);
			sw.Stop();
			if (!res.Success)
			{
				ConsoleUI.Error("Некорректный ввод: знаменатель N должен быть положительным числом.");
				ConsoleUI.Pause();
				return;
			}

			int integerPart = m / n;
			int remainder = m % n;
			int seniorDigit = (remainder * 10) / n;

			Console.WriteLine();
			ConsoleUI.Step($"M / N = {m} / {n}: целая часть = {integerPart}, остаток = {remainder}");
			ConsoleUI.Step($"Младшая цифра целой части: {integerPart} → {res.IntegerLastDigit}  (равна {integerPart} % 10)");
			ConsoleUI.Step($"Старшая цифра дробной части: {remainder}×10 / {n} = {seniorDigit}");
			Console.WriteLine();
			ConsoleUI.Good($"Ответ: младшая цифра целой части = {res.IntegerLastDigit}; старшая цифра дробной части = {res.FractionFirstDigit}.");
			ConsoleUI.Hint("Время вычисления: " + ConsoleUI.FormatTime(sw.Elapsed));
			HistoryStorage.Save(new HistoryEntry
			{
				Timestamp = DateTime.Now, TaskName = "ПР 1.2 — Дробь",
				InputSummary = $"M={m}, N={n}",
				ResultSummary = $"целая={res.IntegerLastDigit}, дробная={res.FractionFirstDigit}",
				Elapsed = ConsoleUI.FormatTime(sw.Elapsed)
			});
			ConsoleUI.Pause();
		}
	}
}
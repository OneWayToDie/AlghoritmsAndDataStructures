using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AlghoritmsAndDataStructures.Core.Calculators;

namespace AlgorithmsLauncher.Tasks
{
	public static class Task4Solver
	{
		public static void Run()
		{
			while (true)
			{
				ConsoleUI.Clear();
				ConsoleUI.Header("ПРАКТИЧЕСКАЯ РАБОТА 4");
				ConsoleUI.MenuItem(1, "Сумма ряда", "\u2211");
				ConsoleUI.MenuItem(2, "Среднее арифметическое трёхзначных чисел", "\uD83D\uDCCA");
				ConsoleUI.MenuItem(0, "Назад в меню", "\u21A9\uFE0F");
				Console.WriteLine();
				int choice = ConsoleUI.AskChoice(0, 2);
				if (choice == 0) return;
				if (choice == 1) SolveSeriesSum();
				else SolveAverage();
			}
		}

		private static void SolveSeriesSum()
		{
			ConsoleUI.Clear();
			ConsoleUI.Header("4.1 — СУММА РЯДА");
			ConsoleUI.Condition(
				"Найти сумму ряда: S = Σ k/(k+1), где k = 1..n.\n" +
				"Точное значение: S = n - (H(n+1) - 1), где H(m) — гармоническое число. В задаче сумма находится последовательно.");

			int mode = ConsoleUI.AskMode();
			if (mode == 0) return;

			int n;
			if (mode == 2)
			{
				int seed = ConsoleUI.AskSeed();
				n = new Random(seed).Next(5, 21);
				ConsoleUI.Hint($"Сгенерировано: n = {n}");
			}
			else
			{
				n = ConsoleUI.ReadInt("Введите n (n >= 1):", 1, 5000).Value;
			}

			var sw = Stopwatch.StartNew();
			double total = SeriesSumCalculator.ComputeSum(n);
			sw.Stop();
			Console.WriteLine();
			ConsoleUI.Step("Слагаемые ряда k/(k+1):");

			const int maxVisible = 18;
			for (int k = 1; k <= n; k++)
			{
				bool show = n <= maxVisible || k <= 12 || k > n - 3;
				if (!show && k == 13) ConsoleUI.Hint("  ... (промежуточные слагаемые пропущены)");
				if (show)
				{
					double term = (double)k / (k + 1);
					ConsoleUI.Step($"  {k,4} / {k + 1}  =  {term:0.########}");
				}
			}

			Console.WriteLine();
			ConsoleUI.Step($"Сумма {n} слагаемых = {total:0.########}");
			Console.WriteLine();
			ConsoleUI.Good($"Ответ: S = {total:0.########}.");
			ConsoleUI.Hint("Время вычисления: " + ConsoleUI.FormatTime(sw.Elapsed));
			HistoryStorage.Save(new HistoryEntry
			{
				Timestamp = DateTime.Now, TaskName = "ПР 4.1 — Сумма ряда",
				InputSummary = $"n={n}",
				ResultSummary = $"S={total:0.######}",
				Elapsed = ConsoleUI.FormatTime(sw.Elapsed)
			});
			ConsoleUI.Pause();
		}

		private static void SolveAverage()
		{
			ConsoleUI.Clear();
			ConsoleUI.Header("4.2 — СРЕДНЕЕ ТРЁХЗНАЧНЫХ ЧИСЕЛ");
			ConsoleUI.Condition(
				"В последовательности целых чисел найти количество трёхзначных чисел и их среднее арифметическое.");

			int mode = ConsoleUI.AskMode();
			if (mode == 0) return;

			string input;
			if (mode == 2)
			{
				int seed = ConsoleUI.AskSeed();
				Random rnd = new Random(seed);
				int count = rnd.Next(6, 13);
				var numbers = new List<int>(count);
				numbers.Add(rnd.Next(100, 1000));
				while (numbers.Count < count)
					numbers.Add(rnd.Next(-999, 1000));
				input = string.Join(", ", numbers);
				ConsoleUI.Hint($"Сгенерировано: {count} чисел");
			}
			else
			{
				input = ConsoleUI.ReadInput("Введите целые числа через пробел/запятую:");
			}

			var nums = input.Split(new[] { ',', ' ', ';', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries)
							.Select(t => int.TryParse(t, out int v) ? (int?)v : null)
							.Where(v => v.HasValue)
							.Select(v => v.Value)
							.ToList();
			if (nums.Count == 0)
			{
				ConsoleUI.Error("Не введено ни одного числа.");
				ConsoleUI.Pause();
				return;
			}

			Console.WriteLine();
			ConsoleUI.Info("Последовательность: " + string.Join(", ", nums));

			var sw = Stopwatch.StartNew();
			var (threeDigit, average, _) = AverageCalculator.ComputeAverage(input);
			sw.Stop();
			if (threeDigit.Count == 0)
			{
				ConsoleUI.Warn("Трёхзначных чисел нет в последовательности.");
				ConsoleUI.Pause();
				return;
			}

			ConsoleUI.Step("Трёхзначные числа: " + string.Join(", ", threeDigit));
			string sumExpr = string.Join(" + ", threeDigit);
			ConsoleUI.Step($"Среднее = ({sumExpr}) / {threeDigit.Count} = {average:0.###}");
			Console.WriteLine();
			ConsoleUI.Good($"Ответ: найдено {threeDigit.Count} трёхзначных чисел, среднее арифметическое = {average:0.###}.");
			ConsoleUI.Hint("Время вычисления: " + ConsoleUI.FormatTime(sw.Elapsed));
			HistoryStorage.Save(new HistoryEntry
			{
				Timestamp = DateTime.Now, TaskName = "ПР 4.2 — Среднее",
				InputSummary = $"{nums.Count} чисел",
				ResultSummary = $"{threeDigit.Count} трёхзнач., ср.={average:0.##}",
				Elapsed = ConsoleUI.FormatTime(sw.Elapsed)
			});
			ConsoleUI.Pause();
		}
	}
}
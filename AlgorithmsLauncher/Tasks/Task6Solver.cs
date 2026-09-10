using System;
using AlghoritmsAndDataStructures.Core.Calculators;

namespace AlgorithmsLauncher.Tasks
{
	public static class Task6Solver
	{
		private const int MaxPoints = 5000;

		public static void Run()
		{
			while (true)
			{
				ConsoleUI.Clear();
				ConsoleUI.Header("ПРАКТИЧЕСКАЯ РАБОТА 6");
				ConsoleUI.MenuItem(1, "Сумма ряда e^(-x) на интервале");
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
			ConsoleUI.Header("6 — СУММА РЯДА e^(-x)", ConsoleColor.Cyan);
			ConsoleUI.Condition(
				"На интервале [A; B] с шагом Dx вычислить значение суммы ряда\n" +
				"e^(-x) = 1 - x + x²/2! - x³/3! + ... с точностью eps и сравнить с точным значением e^(-x).");

			int mode = ConsoleUI.AskMode();
			if (mode == 0) return;

			double a, b, dx, eps;
			if (mode == 2)
			{
				int seed = ConsoleUI.AskSeed();
				Random rnd = new Random(seed);
				a = Math.Round(-3.0 + rnd.NextDouble() * 5.0, 2);
				b = Math.Round(a + 0.5 + rnd.Next(1, 5), 2);
				dx = Math.Round(0.1 + rnd.NextDouble() * 0.9, 2);
				eps = Math.Pow(10, -rnd.Next(3, 7));
				ConsoleUI.Hint($"Сгенерировано: A = {a:0.##}, B = {b:0.##}, Dx = {dx:0.##}, eps = {eps:0.0E+0}");
			}
			else
			{
				a = ConsoleUI.ReadDouble("A (левая граница интервала):", -50, 50).Value;
				b = ConsoleUI.ReadDouble("B (правая граница, B ≥ A):", a, 50).Value;
				dx = ConsoleUI.ReadDouble("Шаг Dx (Dx > 0):", 0.000000001, 10).Value;
				eps = ConsoleUI.ReadDouble("Точность eps (eps > 0):", 0.000000000001, 0.1).Value;
			}

			int points = (int)Math.Floor((b - a) / dx) + 1;
			if (points > MaxPoints)
			{
				ConsoleUI.Warn($"Слишком много точек ({points}). Будет выведено не более {MaxPoints} точек.");
				points = MaxPoints;
			}

			PrintExpansion(a, eps);

			Console.WriteLine();
			PrintTableHeader();

			bool showAll = points <= 20;
			int skipMarker = 0;
			ConsoleColor white = ConsoleColor.White;
			for (int i = 0; i < points; i++)
			{
				double x = a + i * dx;
				bool show = showAll || i < 12 || i >= points - 4;
				if (!show && skipMarker != 1)
				{
					skipMarker = 1;
					Console.ForegroundColor = ConsoleColor.DarkGray;
					Console.WriteLine("  ...");
					Console.ResetColor();
				}
				if (show)
				{
					(double sum, int terms) = SeriesCalculator.ComputeExpSeries(x, eps);
					PrintRow(x, sum, terms, white);
				}
			}

			Console.WriteLine();
			ConsoleUI.Good("Сумма ряда вычислена для всех точек интервала.");
			ConsoleUI.Pause();
		}

		private static void PrintExpansion(double x, double eps)
		{
			Console.WriteLine();
			ConsoleUI.Info($"Шаги вычисления для первой точки x = {x:0.###} (точность {eps:0.0E+0}):");
			ConsoleUI.Step("Члены ряда e^(-x) = 1 - x + x²/2! - x³/3! + ...:");
			ConsoleUI.Step("  k=0: член = 1        → S = 1");

			const int maxTerms = 12;
			double sum = 1.0;
			double term = 1.0;
			for (int k = 1; k <= maxTerms; k++)
			{
				term *= (-x) / k;
				sum += term;
				ConsoleUI.Step($"  k={k}: член = {term:0.######}  → S = {sum:0.######}");
				if (Math.Abs(term) <= eps)
				{
					ConsoleUI.Hint($"  |член| = {Math.Abs(term):0.0E+0} ≤ eps — суммирование останавливается.");
					return;
				}
			}
		}

		private static void PrintTableHeader()
		{
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(string.Format("  {0,-9}  {1,-13}  {2,-7}  {3,-13}  {4,-13}",
				"x", "Сумма ряда", "Членов", "e^(-x)", "Погрешность"));
			ConsoleUI.Rule();
		}

		private static void PrintRow(double x, double sum, int terms, ConsoleColor color)
		{
			double exact = Math.Exp(-x);
			double error = double.IsNaN(sum) ? double.NaN : Math.Abs(sum - exact);
			Console.ForegroundColor = color;
			Console.WriteLine(string.Format("  {0,-9}  {1,-13}  {2,-7}  {3,-13}  {4,-13}",
				x.ToString("0.000"),
				sum.ToString("0.00000000"),
				terms.ToString(),
				exact.ToString("0.00000000"),
				error.ToString("0.00000000")));
			Console.ResetColor();
		}
	}
}
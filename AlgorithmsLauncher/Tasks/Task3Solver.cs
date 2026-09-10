using System;
using AlghoritmsAndDataStructures.Core.Calculators;

namespace AlgorithmsLauncher.Tasks
{
	public static class Task3Solver
	{
		public static void Run()
		{
			while (true)
			{
				ConsoleUI.Clear();
				ConsoleUI.Header("ПРАКТИЧЕСКАЯ РАБОТА 3");
				ConsoleUI.MenuItem(1, "Проверить попадание точки в область");
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
			ConsoleUI.Header("3 — ПОПАДАНИЕ ТОЧКИ В ОБЛАСТЬ", ConsoleColor.Cyan);
			ConsoleUI.Condition(
				"Определить, попадает ли точка с координатами (x, y) в заштрихованную область.\n" +
				"Область: третий квадрант — внутри прямоугольника a×b И внутри окружности r;\n" +
				"первый квадрант — внутри прямоугольника a×b И снаружи окружности r.");

			int mode = ConsoleUI.AskMode();
			if (mode == 0) return;

			double a, b, r, x, y;
			if (mode == 2)
			{
				int seed = ConsoleUI.AskSeed();
				Random rnd = new Random(seed);
				a = Math.Round(1 + rnd.NextDouble() * 9, 2);
				b = Math.Round(1 + rnd.NextDouble() * 9, 2);
				r = Math.Round(1 + rnd.NextDouble() * 9, 2);
				x = Math.Round(rnd.Next(-10, 11) + rnd.NextDouble(), 2);
				y = Math.Round(rnd.Next(-10, 11) + rnd.NextDouble(), 2);
				ConsoleUI.Hint($"Сгенерировано: a = {a:0.##}, b = {b:0.##}, r = {r:0.##}, x = {x:0.##}, y = {y:0.##}");
			}
			else
			{
				a = ConsoleUI.ReadDouble("a (ширина прямоугольника, a > 0):", 0.0001, 1e6).Value;
				b = ConsoleUI.ReadDouble("b (высота прямоугольника, b > 0):", 0.0001, 1e6).Value;
				r = ConsoleUI.ReadDouble("r (радиус окружности, r > 0):", 0.0001, 1e6).Value;
				x = ConsoleUI.ReadDouble("x точки:", -1e9, 1e9).Value;
				y = ConsoleUI.ReadDouble("y точки:", -1e9, 1e9).Value;
			}

			bool inside = AreaChecker.Check(x, y, a, b, r, out string message);

			string tf(bool v) => v ? "истина" : "ложь";
			bool zoneLeftCond = x <= 0 && y <= 0;
			bool zoneLeftRect = zoneLeftCond && x >= -a && y >= -b;
			bool inCircle = x * x + y * y <= r * r;
			bool zoneLeft = zoneLeftRect && inCircle;

			bool zoneRightCond = x >= 0 && y >= 0;
			bool zoneRightRect = zoneRightCond && x <= a && y <= b;
			bool outCircle = x * x + y * y >= r * r;
			bool zoneRight = zoneRightRect && outCircle;

			Console.WriteLine();
			ConsoleUI.Info($"Точка ({x:0.##}; {y:0.##}), прямоугольник {a:0.##}×{b:0.##}, окружность r = {r:0.##}.");
			Console.WriteLine();
			ConsoleUI.Step("Зона A (III квадрант): внутри прямоугольника и внутри окружности");
			ConsoleUI.Step($"  x ≤ 0 и y ≤ 0 ........... {tf(zoneLeftCond)}");
			ConsoleUI.Step($"  |x| ≤ a и |y| ≤ b ....... {tf(zoneLeftRect)}");
			ConsoleUI.Step($"  x² + y² ≤ r² ............ {tf(inCircle)}");
			ConsoleUI.Step($"  → зона A: {tf(zoneLeft)}");
			Console.WriteLine();
			ConsoleUI.Step("Зона B (I квадрант): внутри прямоугольника и снаружи окружности");
			ConsoleUI.Step($"  x ≥ 0 и y ≥ 0 ........... {tf(zoneRightCond)}");
			ConsoleUI.Step($"  x ≤ a и y ≤ b ........... {tf(zoneRightRect)}");
			ConsoleUI.Step($"  x² + y² ≥ r² ............ {tf(outCircle)}");
			ConsoleUI.Step($"  → зона B: {tf(zoneRight)}");
			Console.WriteLine();
			ConsoleUI.Good(message);
			ConsoleUI.Pause();
		}
	}
}
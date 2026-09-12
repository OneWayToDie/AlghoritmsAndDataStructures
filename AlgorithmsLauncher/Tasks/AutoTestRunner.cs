using System;
using System.Linq;
using AlghoritmsAndDataStructures.Core.Calculators;

namespace AlgorithmsLauncher.Tasks
{
	public static class AutoTestRunner
	{
		private static int _passed;
		private static int _failed;

		public static void Run()
		{
			_passed = 0;
			_failed = 0;

			ConsoleUI.Clear();
			ConsoleUI.Header("АВТОТЕСТЫ ЗАДАЧ");
			Console.WriteLine();

			TestTask1Cube();
			TestTask1Fraction();
			TestTask2Graph();
			TestTask3Area();
			TestTask4SeriesSum();
			TestTask4Average();
			TestTask5Array();
			TestTask6ExpSeries();
			TestTask7QuickSort();
			TestTask8Neighbors();

			Console.WriteLine();
			ConsoleUI.Rule();
			if (_failed == 0)
				ConsoleUI.Good($"ИТОГО: {_passed}/{_passed + _failed} тестов пройдено.");
			else
			{
				ConsoleUI.Warn($"ИТОГО: {_passed}/{_passed + _failed} пройдено, {_failed} провалено.");
			}
			ConsoleUI.Pause();
		}

		private static void Pass(string text)
		{
			_passed++;
			ConsoleUI.Good(text);
		}

		private static void Fail(string text)
		{
			_failed++;
			ConsoleUI.Error(text);
		}

		private static bool Eq(double actual, double expected, double eps = 1e-6)
		{
			return Math.Abs(actual - expected) < eps;
		}

		private static bool ArrEq(double[] actual, double[] expected, double eps = 1e-6)
		{
			if (actual.Length != expected.Length) return false;
			for (int i = 0; i < actual.Length; i++)
				if (!Eq(actual[i], expected[i], eps)) return false;
			return true;
		}

		private static bool ArrEq(int[] actual, int[] expected)
		{
			if (actual.Length != expected.Length) return false;
			for (int i = 0; i < actual.Length; i++)
				if (actual[i] != expected[i]) return false;
			return true;
		}

		private static void TestTask1Cube()
		{
			ConsoleUI.Info("Задача 1.1 — Куб");

			var r1 = CubeCalculator.Compute(5);
			if (r1.Success && Eq(r1.FaceArea, 25) && Eq(r1.TotalSurface, 150) && Eq(r1.Volume, 125))
				Pass("  a=5:   Sгр=25, Sполн=150, V=125");
			else
				Fail($"  a=5:   ожидается Sгр=25, Sполн=150, V=125, получено {r1.FaceArea}/{r1.TotalSurface}/{r1.Volume}");

			var r2 = CubeCalculator.Compute(1);
			if (r2.Success && Eq(r2.FaceArea, 1) && Eq(r2.TotalSurface, 6) && Eq(r2.Volume, 1))
				Pass("  a=1:   Sгр=1, Sполн=6, V=1");
			else
				Fail($"  a=1:   ожидается Sгр=1, Sполн=6, V=1");

			var r3 = CubeCalculator.Compute(-1);
			if (!r3.Success)
				Pass("  a=-1:  отказ вычисления (корректно)");
			else
				Fail("  a=-1:  ожидается отказ вычисления");

			Console.WriteLine();
		}

		private static void TestTask1Fraction()
		{
			ConsoleUI.Info("Задача 1.2 — Дробь M/N");

			var f1 = FractionCalculator.Compute(7, 3);
			if (f1.Success && f1.IntegerLastDigit == 2 && f1.FractionFirstDigit == 3)
				Pass("  M=7/N=3:   целая=2, дробная=3");
			else
				Fail($"  M=7/N=3:   ожидается (2,3), получено ({f1.IntegerLastDigit},{f1.FractionFirstDigit})");

			var f2 = FractionCalculator.Compute(10, 4);
			if (f2.Success && f2.IntegerLastDigit == 2 && f2.FractionFirstDigit == 5)
				Pass("  M=10/N=4:  целая=2, дробная=5");
			else
				Fail($"  M=10/N=4:  ожидается (2,5), получено ({f2.IntegerLastDigit},{f2.FractionFirstDigit})");

			var f3 = FractionCalculator.Compute(7, 0);
			if (!f3.Success)
				Pass("  N=0:       отказ вычисления (корректно)");
			else
				Fail("  N=0:       ожидается отказ вычисления");

			Console.WriteLine();
		}

		private static void TestTask2Graph()
		{
			ConsoleUI.Info("Задача 2 — Функция, заданная графиком");

			string err;
			double? y;

			y = GraphCalculator.Compute(0, 3, out err);
			if (y.HasValue && Eq(y.Value, 3.0) && err == null)
				Pass("  x=0, R=3:   y=3 (√9)");
			else
				Fail($"  x=0, R=3:   ожидается y=3, получено {y}");

			y = GraphCalculator.Compute(-6, 3, out err);
			if (y.HasValue && Eq(y.Value, -3.0) && err == null)
				Pass("  x=-6, R=3:  y=-3 (прямая)");
			else
				Fail($"  x=-6, R=3:  ожидается y=-3, получено {y}");

			y = GraphCalculator.Compute(4, 3, out err);
			double expected2 = 3.0 * (4 - 3) / (8 - 3);
			if (y.HasValue && Eq(y.Value, expected2) && err == null)
				Pass($"  x=4, R=3:   y={expected2:0.###} (наклонная)");
			else
				Fail($"  x=4, R=3:   ожидается y={expected2:0.###}, получено {y}");

			y = GraphCalculator.Compute(0, 5, out err);
			if (!y.HasValue && err != null)
				Pass("  R=5:        ошибка (корректно)");
			else
				Fail("  R=5:        ожидается ошибка");

			Console.WriteLine();
		}

		private static void TestTask3Area()
		{
			ConsoleUI.Info("Задача 3 — Попадание точки в область");

			string msg;

			bool inside1 = AreaChecker.Check(-1, -1, 2, 2, 1.5, out msg);
			if (inside1)
				Pass("  (-1;-1), a=2,b=2,r=1.5:  внутри (зона A)");
			else
				Fail($"  (-1;-1), a=2,b=2,r=1.5:  ожидается «внутри», получено «снаружи»");

			bool inside2 = AreaChecker.Check(2, 2, 3, 3, 1.5, out msg);
			if (inside2)
				Pass("  (2;2), a=3,b=3,r=1.5:    внутри (зона B)");
			else
				Fail($"  (2;2), a=3,b=3,r=1.5:    ожидается «внутри», получено «снаружи»");

			bool inside3 = AreaChecker.Check(5, 5, 2, 2, 1.5, out msg);
			if (!inside3)
				Pass("  (5;5), a=2,b=2,r=1.5:    снаружи");
			else
				Fail($"  (5;5), a=2,b=2,r=1.5:    ожидается «снаружи»");

			bool inside4 = AreaChecker.Check(1, 1, -1, 2, 1.5, out msg);
			if (!inside4 && msg.Contains("Ошибка"))
				Pass("  a=-1:                     ошибка (корректно)");
			else
				Fail("  a=-1:                     ожидается ошибка");

			Console.WriteLine();
		}

		private static void TestTask4SeriesSum()
		{
			ConsoleUI.Info("Задача 4.1 — Сумма ряда Σ k/(k+1)");

			double s1 = SeriesSumCalculator.ComputeSum(5);
			double expected1 = 1.0 / 2 + 2.0 / 3 + 3.0 / 4 + 4.0 / 5 + 5.0 / 6;
			if (Eq(s1, expected1))
				Pass($"  n=5:   S={s1:0.######}");
			else
				Fail($"  n=5:   ожидается S={expected1:0.######}, получено S={s1:0.######}");

			double s2 = SeriesSumCalculator.ComputeSum(2);
			double expected2 = 1.0 / 2 + 2.0 / 3;
			if (Eq(s2, expected2))
				Pass($"  n=2:   S={s2:0.######}");
			else
				Fail($"  n=2:   ожидается S={expected2:0.######}, получено S={s2:0.######}");

			double s3 = SeriesSumCalculator.ComputeSum(100);
			if (s3 > 0)
				Pass($"  n=100: S={s3:0.##} (> 0)");
			else
				Fail($"  n=100: ожидается S > 0, получено S={s3}");

			Console.WriteLine();
		}

		private static void TestTask4Average()
		{
			ConsoleUI.Info("Задача 4.2 — Среднее арифметическое трёхзначных чисел");

			var a1 = AverageCalculator.ComputeAverage("123 45 678 -999 1000");
			if (a1.average.HasValue && Eq(a1.average.Value, -66.0) && a1.threeDigitNumbers.Count == 3)
				Pass("  [123 45 678 -999 1000]:  три трёхзначных (123,678,-999), ср.=-66");
			else
				Fail($"  [123 45 678 -999 1000]:  ожидается ср.=-66, получено {a1.average}");

			var a2 = AverageCalculator.ComputeAverage("1 22 333 4444 555 66");
			if (a2.average.HasValue && Eq(a2.average.Value, 444.0) && a2.threeDigitNumbers.Count == 2)
				Pass("  [1 22 333 4444 555 66]:    два трёхзначных (333,555), ср.=444");
			else
				Fail($"  [1 22 333 4444 555 66]:    ожидается ср.=444, получено {a2.average}");

			var a3 = AverageCalculator.ComputeAverage("1 2 3");
			if (!a3.average.HasValue && a3.threeDigitNumbers.Count == 0)
				Pass("  [1 2 3]:                   трёхзначных чисел нет (корректно)");
			else
				Fail($"  [1 2 3]:                   ожидается отсутствие трёхзначных, получено {a3.average}");

			var a4 = AverageCalculator.ComputeAverage("-999 -100");
			if (a4.average.HasValue && Eq(a4.average.Value, -549.5) && a4.threeDigitNumbers.Count == 2)
				Pass("  [-999 -100]:               два трёхзначных (-999,-100), ср.=-549.5");
			else
				Fail($"  [-999 -100]:               ожидается ср.=-549.5, получено {a4.average}");

			Console.WriteLine();
		}

		private static void TestTask5Array()
		{
			ConsoleUI.Info("Задача 5 — Массив: среднее на нечётных местах");

			var a1 = ArrayProcessor.ProcessArray(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
			double[] expected1 = { 1, 2, 6, 4, 5, 6, 7, 8, 6, 10, 11, 6 };
			if (Eq(a1.average, 6.0) && ArrEq(a1.modifiedArray, expected1))
				Pass("  [1..12]:     ср.=6, замена %3 → [1,2,6,4,5,6,7,8,6,10,11,6]");
			else
			{
				string got = "[" + string.Join(",", a1.modifiedArray.Select(x => x.ToString("0.##"))) + "]";
				Fail($"  [1..12]:     ожидается ср.=6, [{string.Join(",", expected1)}], получено ср.={a1.average}, {got}");
			}

			var a2 = ArrayProcessor.ProcessArray(new[] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 });
			if (Eq(a2.average, 3.0) && a2.modifiedArray.All(x => Eq(x, 3.0)))
				Pass("  [3×12]:      ср.=3, все заменены на 3");
			else
				Fail("  [3×12]:      ожидается ср.=3, все элементы = 3");

			Console.WriteLine();
		}

		private static void TestTask6ExpSeries()
		{
			ConsoleUI.Info("Задача 6 — Ряд e^(-x)");

			var r1 = SeriesCalculator.ComputeExpSeries(0, 0.001);
			if (Eq(r1.sum, 1.0))
				Pass($"  x=0,  eps=0.001:  сумма={r1.sum:0.######}, слагаемых={r1.terms}");
			else
				Fail($"  x=0,  eps=0.001:  ожидается сумма≈1, получено сумма={r1.sum}");

			var r2 = SeriesCalculator.ComputeExpSeries(1, 0.001);
			double exact2 = Math.Exp(-1);
			if (Eq(r2.sum, exact2, 0.01))
				Pass($"  x=1,  eps=0.001:  сумма={r2.sum:0.######} (≈e⁻¹={exact2:0.######})");
			else
				Fail($"  x=1,  eps=0.001:  ожидается сумма≈{exact2:0.######}, получено сумма={r2.sum}");

			var r3 = SeriesCalculator.ComputeExpSeries(-2, 0.001);
			double exact3 = Math.Exp(2);
			if (Eq(r3.sum, exact3, 0.01))
				Pass($"  x=-2, eps=0.001:  сумма={r3.sum:0.######} (≈e²={exact3:0.######})");
			else
				Fail($"  x=-2, eps=0.001:  ожидается сумма≈{exact3:0.######}, получено сумма={r3.sum}");

			Console.WriteLine();
		}

		private static void TestTask7QuickSort()
		{
			ConsoleUI.Info("Задача 7 — Быстрая сортировка (по убыванию)");

			var s1 = QuickSortCalculator.SortWithMetrics(new[] { 3, 1, 4, 1, 5, 9 });
			if (ArrEq(s1.sorted, new[] { 9, 5, 4, 3, 1, 1 }))
				Pass("  [3,1,4,1,5,9]  → [9,5,4,3,1,1]");
			else
				Fail($"  [3,1,4,1,5,9]  → [{string.Join(",", s1.sorted)}] (ожидается [9,5,4,3,1,1])");

			var s2 = QuickSortCalculator.SortWithMetrics(new[] { 5, 5, 5 });
			if (ArrEq(s2.sorted, new[] { 5, 5, 5 }))
				Pass("  [5,5,5]        → [5,5,5]");
			else
				Fail($"  [5,5,5]        → [{string.Join(",", s2.sorted)}]");

			var s3 = QuickSortCalculator.SortWithMetrics(new int[0]);
			if (s3.sorted.Length == 0)
				Pass("  []             → [] (пустой)");
			else
				Fail($"  []             → [{string.Join(",", s3.sorted)}] (ожидается [])");

			Console.WriteLine();
		}

		private static void TestTask8Neighbors()
		{
			ConsoleUI.Info("Задача 8 — Среднее арифметическое соседей");

			var n1 = NeighborhoodAverageCalculator.ComputeAverages(new[] { 1, 2, 3, 4, 5 });
			double[] exp1 = { 1.5, 2.0, 3.0, 4.0, 4.5 };
			if (ArrEq(n1, exp1))
				Pass("  [1,2,3,4,5]  → [1.5, 2, 3, 4, 4.5]");
			else
				Fail($"  [1,2,3,4,5]  → [{string.Join(",", n1.Select(x => x.ToString("0.#")))}] (ожидается [1.5,2,3,4,4.5])");

			var n2 = NeighborhoodAverageCalculator.ComputeAverages(new[] { 10, 20 });
			double[] exp2 = { 15.0, 15.0 };
			if (ArrEq(n2, exp2))
				Pass("  [10,20]       → [15, 15]");
			else
				Fail($"  [10,20]       → [{string.Join(",", n2)}]");

			var n3 = NeighborhoodAverageCalculator.ComputeAverages(new[] { 7 });
			if (n3.Length == 1 && Eq(n3[0], 7.0))
				Pass("  [7]           → [7] (единственный элемент)");
			else
				Fail($"  [7]           → [{string.Join(",", n3)}]");

			Console.WriteLine();
		}
	}
}

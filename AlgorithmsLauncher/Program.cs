using System;
using System.Text;

namespace AlgorithmsLauncher
{
	internal static class Program
	{
		private static void Main()
		{
			Console.OutputEncoding = Encoding.UTF8;
			Console.Title = "Алгоритмы и структуры данных — лаунчер";
			ConsoleUI.Detect();

			if (!ConsoleUI.Unicode)
				ConsoleUI.Warn("Для лучшего отображения запускайте программу из Windows Terminal.");
			ConsoleUI.Hint("Закрыть окно консоли можно в любой момент.");
			ConsoleUI.Pause();

			while (true)
			{
				ConsoleUI.Clear();
				ShowMainMenu();
				int choice = ConsoleUI.AskChoice(0, 3);
				switch (choice)
				{
					case 1: LauncherHelper.LaunchWpf(); ConsoleUI.Pause(); break;
					case 2: RunTasksMenu(); break;
					case 3: ShowAbout(); ConsoleUI.Pause(); break;
					default: return;
				}
			}
		}

		private static void ShowMainMenu()
		{
			ConsoleUI.Header("ЛАУНЧЕР «АЛГОРИТМЫ И СТРУКТУРЫ ДАННЫХ»");
			ConsoleUI.MenuItem(1, "Запустить графическую версию (WPF)");
			ConsoleUI.MenuItem(2, "Решения задач в консоли");
			ConsoleUI.MenuItem(3, "О программе");
			ConsoleUI.MenuItem(0, "Выход");
			Console.WriteLine();
		}

		private static void RunTasksMenu()
		{
			while (true)
			{
				ConsoleUI.Clear();
				ConsoleUI.Header("РЕШЕНИЯ ЗАДАЧ В КОНСОЛИ");
				ConsoleUI.MenuItem(1, "ПР 1 — Куб и дробь M/N");
				ConsoleUI.MenuItem(2, "ПР 2 — Функция, заданная графиком");
				ConsoleUI.MenuItem(3, "ПР 3 — Попадание точки в область");
				ConsoleUI.MenuItem(4, "ПР 4 — Сумма ряда и среднее трёхзначных");
				ConsoleUI.MenuItem(5, "ПР 5 — Массив: среднее на нечётных местах");
				ConsoleUI.MenuItem(6, "ПР 6 — Сумма ряда e^(-x)");
				ConsoleUI.MenuItem(7, "ПР 7 — Быстрая сортировка");
				ConsoleUI.MenuItem(8, "ПР 8 — Среднее арифметическое соседей");
				ConsoleUI.MenuItem(0, "Назад в меню");
				Console.WriteLine();
				int choice = ConsoleUI.AskChoice(0, 8);
				if (choice == 0) return;
				switch (choice)
				{
					case 1: Tasks.Task1Solver.Run(); break;
					case 2: Tasks.Task2Solver.Run(); break;
					case 3: Tasks.Task3Solver.Run(); break;
					case 4: Tasks.Task4Solver.Run(); break;
					case 5: Tasks.Task5Solver.Run(); break;
					case 6: Tasks.Task6Solver.Run(); break;
					case 7: Tasks.Task7Solver.Run(); break;
					case 8: Tasks.Task8Solver.Run(); break;
				}
			}
		}

		private static void ShowAbout()
		{
			ConsoleUI.Clear();
			ConsoleUI.Header("О ПРОГРАММЕ");
			ConsoleUI.Info("Консольный лаунчер учебного проекта «Алгоритмы и структуры данных».");
			ConsoleUI.Info("Содержит пошаговые решения восьми практических работ и запускает");
			ConsoleUI.Info("графическую версию приложения (WPF).");
			Console.WriteLine();
			ConsoleUI.Info("Работы:");
			ConsoleUI.Step("1 — куб: площадь грани, полная поверхность, объём");
			ConsoleUI.Step("   дробь M/N: младшая цифра целой и старшая цифра дробной части");
			ConsoleUI.Step("2 — функция, заданная графиком (параметр R)");
			ConsoleUI.Step("3 — попадание точки в заштрихованную область");
			ConsoleUI.Step("4 — сумма ряда Σ k/(k+1); среднее трёхзначных чисел");
			ConsoleUI.Step("5 — среднее элементов на нечётных позициях, замена кратных 3");
			ConsoleUI.Step("6 — ряд e^(-x) с заданной точностью");
			ConsoleUI.Step("7 — быстрая сортировка по убыванию");
			ConsoleUI.Step("8 — замена элемента средним арифметическим соседей");
			Console.WriteLine();
			ConsoleUI.Hint("Логика вычислений общая с WPF-версией приложения.");
		}
	}
}
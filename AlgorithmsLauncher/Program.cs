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
			_ = Settings.Current;
			ConsoleUI.Detect();

			if (!ConsoleUI.Minimal)
			{
				if (!ConsoleUI.Unicode)
					ConsoleUI.Warn("Для лучшего отображения запускайте программу из Windows Terminal.");
				ConsoleUI.Hint("Закрыть окно консоли можно в любой момент.");
				ConsoleUI.Pause();
			}

			while (true)
			{
				ConsoleUI.Clear();
				if (ConsoleUI.Minimal)
				{
					if (!RunMinimalMainMenu()) return;
					continue;
				}
				ShowMainMenu();
				int choice = ConsoleUI.AskChoice(0, 6);
				switch (choice)
				{
					case 1: LauncherHelper.LaunchWpf(); ConsoleUI.Pause(); break;
					case 2: RunTasksMenu(); break;
					case 3: Tasks.AutoTestRunner.Run(); break;
					case 4: Tasks.HistoryViewer.Run(); break;
					case 5: ShowAbout(); ConsoleUI.Pause(); break;
					case 6: RunSettingsMenu(); break;
					default: return;
				}
			}
		}

		private static void ShowMainMenu()
		{
			ConsoleUI.Header("ЛАУНЧЕР «АЛГОРИТМЫ И СТРУКТУРЫ ДАННЫХ»");
			ConsoleUI.MenuItem(1, "Запустить графическую версию (WPF)", "\uD83D\uDE80");
			ConsoleUI.MenuItem(2, "Решения задач в консоли", "\u2699\uFE0F");
			ConsoleUI.MenuItem(3, "Автотесты всех задач", "\uD83E\uDDEA");
			ConsoleUI.MenuItem(4, "История запусков", "\uD83D\uDD58");
			ConsoleUI.MenuItem(5, "О программе", "\u2139\uFE0F");
			ConsoleUI.MenuItem(6, "Настройки", "\uD83D\uDEE0\uFE0F");
			ConsoleUI.MenuItem(0, "Выход", "\u2630");
			Console.WriteLine();
		}

		private static void RunTasksMenu()
		{
			while (true)
			{
				ConsoleUI.Clear();
				ConsoleUI.Header("РЕШЕНИЯ ЗАДАЧ В КОНСОЛИ");
				ConsoleUI.MenuItem(1, "ПР 1 — Куб и дробь M/N", "\uD83E\uDDCA");
				ConsoleUI.MenuItem(2, "ПР 2 — Функция, заданная графиком", "\uD83D\uDCC8");
				ConsoleUI.MenuItem(3, "ПР 3 — Попадание точки в область", "\uD83D\uDCD0");
				ConsoleUI.MenuItem(4, "ПР 4 — Сумма ряда и среднее трёхзначных", "\u2211");
				ConsoleUI.MenuItem(5, "ПР 5 — Массив: среднее на нечётных местах", "\uD83D\uDCCB");
				ConsoleUI.MenuItem(6, "ПР 6 — Сумма ряда e^(-x)", "\u2211");
				ConsoleUI.MenuItem(7, "ПР 7 — Быстрая сортировка", "\uD83D\uDD00");
				ConsoleUI.MenuItem(8, "ПР 8 — Среднее арифметическое соседей", "\uD83D\uDD04");
				ConsoleUI.MenuItem(0, "Назад в меню", "\u21A9\uFE0F");
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

		private static void RunSettingsMenu()
		{
			while (true)
			{
				ConsoleUI.Clear();
				ConsoleUI.Header("НАСТРОЙКИ");
				if (ConsoleUI.Minimal)
				{
					ConsoleUI.MenuItem(1, "Минимальная консоль");
					ConsoleUI.MenuItem(0, "Назад в меню", "\u21A9\uFE0F");
					Console.WriteLine();
					int choice = ConsoleUI.AskChoice(0, 1);
					if (choice == 0) return;
					RunMinimalConsoleMenu();
				}
				else
				{
					ConsoleUI.MenuItem(1, "Темы оформления", "\uD83D\uDD8C\uFE0F");
					ConsoleUI.MenuItem(2, "Иконки меню", "\u2728");
					ConsoleUI.MenuItem(3, "Скорость анимации", "\u23E9");
					ConsoleUI.MenuItem(4, "Минимальная консоль", "\uD83D\uDEE0\uFE0F");
					ConsoleUI.MenuItem(0, "Назад в меню", "\u21A9\uFE0F");
					Console.WriteLine();
					int choice = ConsoleUI.AskChoice(0, 4);
					switch (choice)
					{
						case 1: RunThemesMenu(); break;
						case 2: RunIconsMenu(); break;
						case 3: RunAnimationMenu(); break;
						case 4: RunMinimalConsoleMenu(); break;
						default: return;
					}
				}
			}
		}

		private static void RunMinimalConsoleMenu()
		{
			while (true)
			{
				ConsoleUI.Clear();
				ConsoleUI.Header("МИНИМАЛЬНАЯ КОНСОЛЬ");
				Console.WriteLine();
				ConsoleUI.OptionRow(1, "Включена — только задачи и настройки, без оформления", Settings.Current.Minimal);
				ConsoleUI.OptionRow(2, "Выключена — полноценный лаунчер", !Settings.Current.Minimal);
				Console.WriteLine();
				ConsoleUI.MenuItem(0, "Назад", "\u21A9\uFE0F");
				Console.WriteLine();
				int choice = ConsoleUI.AskChoice(0, 2);
				if (choice == 0) return;
				Settings.Current.Minimal = choice == 1;
				Settings.Current.Save();
			}
		}

		private static bool RunMinimalMainMenu()
		{
			while (true)
			{
				if (!ConsoleUI.Minimal) return true;
				ConsoleUI.Clear();
				ConsoleUI.Header("АЛГОРИТМЫ И СТРУКТУРЫ ДАННЫХ");
				ConsoleUI.MenuItem(1, "Решения задач");
				ConsoleUI.MenuItem(2, "Настройки");
				ConsoleUI.MenuItem(0, "Выход");
				Console.WriteLine();
				int choice = ConsoleUI.AskChoice(0, 2);
				switch (choice)
				{
					case 1: RunTasksMenu(); break;
					case 2: RunSettingsMenu(); break;
					default: return false;
				}
			}
		}

		private static void RunIconsMenu()
		{
			while (true)
			{
				ConsoleUI.Clear();
				ConsoleUI.Header("ИКОНКИ МЕНЮ");
				Console.WriteLine();
				IconStyle current = Settings.Current.IconStyle;
				ConsoleUI.OptionRow(1, "Эмодзи (Windows Terminal)", current == IconStyle.Emoji);
				ConsoleUI.OptionRow(2, "Строгие символы (conhost)", current == IconStyle.Strict);
				ConsoleUI.OptionRow(3, "Выключены", current == IconStyle.Off);
				Console.WriteLine();
				ConsoleUI.MenuItem(0, "Назад в меню", "\u21A9\uFE0F");
				Console.WriteLine();
				int choice = ConsoleUI.AskChoice(0, 3);
				if (choice == 0) return;
				Settings.Current.IconStyle = (IconStyle)(choice - 1);
				Settings.Current.Save();
			}
		}

		private static void RunAnimationMenu()
		{
			while (true)
			{
				ConsoleUI.Clear();
				ConsoleUI.Header("СКОРОСТЬ АНИМАЦИИ");
				Console.WriteLine();
				AnimationSpeed current = Settings.Current.AnimationSpeed;
				ConsoleUI.OptionRow(1, "Выключены", current == AnimationSpeed.Off);
				ConsoleUI.OptionRow(2, "Медленно", current == AnimationSpeed.Slow);
				ConsoleUI.OptionRow(3, "Нормально", current == AnimationSpeed.Normal);
				Console.WriteLine();
				ConsoleUI.MenuItem(0, "Назад в меню", "\u21A9\uFE0F");
				Console.WriteLine();
				int choice = ConsoleUI.AskChoice(0, 3);
				if (choice == 0) return;
				Settings.Current.AnimationSpeed = (AnimationSpeed)(choice - 1);
				Settings.Current.Save();
			}
		}

		private static void RunThemesMenu()
		{
			while (true)
			{
				ConsoleUI.Clear();
				ConsoleUI.Header("ТЕМЫ ОФОРМЛЕНИЯ");
				Console.WriteLine();
				string[] keys = ThemePalettes.Names;
				string current = ThemePalettes.Get(Settings.Current.Theme).DisplayName;
				for (int i = 0; i < keys.Length; i++)
					ConsoleUI.ThemeOption(i + 1, ThemePalettes.DisplayNames[i], string.Equals(current, ThemePalettes.DisplayNames[i], StringComparison.OrdinalIgnoreCase));
				Console.WriteLine();
				ConsoleUI.MenuItem(0, "Назад в меню", "\u21A9\uFE0F");
				Console.WriteLine();
				int choice = ConsoleUI.AskChoice(0, keys.Length);
				if (choice == 0) return;
				Settings.Current.Theme = keys[choice - 1];
				Settings.Current.Save();
			}
		}

		private static void ShowAbout()
		{
			ConsoleUI.Clear();
			ConsoleUI.Header("О ПРОГРАММЕ");
			ConsoleUI.Info("Консольный лаунчер учебного проекта по предмету «Алгоритмы и структуры данных».");
			ConsoleUI.Info("Реализовано решение восьми практических работ и графическую версию приложения на WPF.");
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
using System;
using System.Collections.Generic;

namespace AlgorithmsLauncher.Tasks
{
	public static class HistoryViewer
	{
		public static void Run()
		{
			while (true)
			{
				ConsoleUI.Clear();
				ConsoleUI.Header("ИСТОРИЯ ЗАПУСКОВ");

				var entries = HistoryStorage.Load();
				if (entries.Count == 0)
				{
					ConsoleUI.Warn("История пуста.");
					ConsoleUI.Pause();
					return;
				}

				int maxTask = 0, maxRes = 0;
				foreach (var e in entries)
				{
					if (e.TaskName.Length > maxTask) maxTask = e.TaskName.Length;
					if (e.ResultSummary.Length > maxRes) maxRes = e.ResultSummary.Length;
				}
				if (maxTask < 10) maxTask = 10;
				if (maxRes < 10) maxRes = 10;
				if (maxTask > 24) maxTask = 24;
				if (maxRes > 28) maxRes = 28;

				int numW = entries.Count.ToString().Length;
				if (numW < 2) numW = 2;

				int dateW = DateTime.Now.ToString("dd.MM.yyyy  HH:mm:ss").Length;
				int elapsedW = 9;

				Console.WriteLine();
				Console.ForegroundColor = ConsoleUI.Pal.Borders;
				string headerNum = "".PadLeft(numW);
				string headerLine = "  " + headerNum + " \u2502 "
					+ "Дата и время".PadRight(dateW) + " \u2502 "
					+ "Задача".PadRight(maxTask) + " \u2502 "
					+ "Результат".PadRight(maxRes) + " \u2502 "
					+ "Время";
				Console.WriteLine(headerLine);

				string rule = "  " + new string('\u2500', numW) + "\u2500\u253C\u2500"
					+ new string('\u2500', dateW) + "\u2500\u253C\u2500"
					+ new string('\u2500', maxTask) + "\u2500\u253C\u2500"
					+ new string('\u2500', maxRes) + "\u2500\u253C\u2500"
					+ new string('\u2500', elapsedW);
				Console.WriteLine(rule);
				Console.ResetColor();

				for (int i = 0; i < entries.Count; i++)
				{
					var e = entries[i];
					string num = (i + 1).ToString().PadLeft(numW);
					string date = e.Timestamp.ToString("dd.MM.yyyy  HH:mm:ss");
					string task = Truncate(e.TaskName, maxTask).PadRight(maxTask);
					string res = Truncate(e.ResultSummary, maxRes).PadRight(maxRes);
					string elapsed = (e.Elapsed ?? "").PadRight(elapsedW);

					Console.ForegroundColor = ConsoleUI.Pal.Borders;
					Console.Write("  " + num + " \u2502 ");
					Console.ResetColor();
					Console.ForegroundColor = ConsoleUI.Pal.Info;
					Console.Write(date);
					Console.ResetColor();
					Console.ForegroundColor = ConsoleUI.Pal.Borders;
					Console.Write(" \u2502 ");
					Console.ResetColor();
					Console.ForegroundColor = ConsoleUI.Pal.Accent;
					Console.Write(task);
					Console.ResetColor();
					Console.ForegroundColor = ConsoleUI.Pal.Borders;
					Console.Write(" \u2502 ");
					Console.ResetColor();
					Console.ForegroundColor = ConsoleUI.Pal.Good;
					Console.Write(res);
					Console.ResetColor();
					Console.ForegroundColor = ConsoleUI.Pal.Borders;
					Console.Write(" \u2502 ");
					Console.ResetColor();
					Console.WriteLine(elapsed);
				}

				Console.WriteLine();
				ConsoleUI.MenuItem(1, "Очистить историю", "\uD83D\uDDD1\uFE0F");
				ConsoleUI.MenuItem(0, "Назад", "\u21A9\uFE0F");
				Console.WriteLine();
				int choice = ConsoleUI.AskChoice(0, 1);
				if (choice == 0) return;
				if (choice == 1)
				{
					HistoryStorage.Clear();
					ConsoleUI.Good("История очищена.");
					ConsoleUI.Pause();
				}
			}
		}

		private static string Truncate(string s, int max)
		{
			if (s == null) return "";
			return s.Length <= max ? s : s.Substring(0, max - 1) + "\u2026";
		}
	}
}

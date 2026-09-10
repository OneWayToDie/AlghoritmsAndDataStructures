using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace AlgorithmsLauncher
{
	/// <summary>
	/// Консольный интерфейс: рамки, цвета, ввод.
	/// Unicode-вывод (рамки ╔═╗ и эмодзи) включается в Windows Terminal,
	/// в cmd/conhost используется упрощённый ASCII-вариант.
	/// </summary>
	public static class ConsoleUI
	{
		/// <summary>true — расширенный вывод (рамки и эмодзи), false — упрощённый (cmd).</summary>
		public static bool Unicode { get; private set; }

		/// <summary>Определяет стиль оформления один раз при старте.</summary>
		public static void Detect()
		{
			if (IsWindowsTerminal())
			{
				Unicode = true;
				return;
			}
			Unicode = ProbeEmojiSupport();
		}

		private static bool IsWindowsTerminal()
		{
			try { return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WT_SESSION")); }
			catch { return false; }
		}

		private static bool ProbeEmojiSupport()
		{
			try
			{
				int before = Console.CursorLeft;
				Console.Write("\u26A1");
				int after = Console.CursorLeft;
				Console.Write("  ");
				Console.SetCursorPosition(before, Console.CursorTop);
				return after - before >= 2;
			}
			catch { return false; }
		}

		public static int UsableWidth()
		{
			try { return Math.Max(20, Console.WindowWidth - 2); }
			catch { return 78; }
		}

		private static string Center(string s, int width)
		{
			if (width <= s.Length) return s;
			int left = (width - s.Length) / 2;
			return new string(' ', left) + s + new string(' ', width - s.Length - left);
		}

		/// <summary>Заголовок в рамке. По умолчанию рамка зелёная, для задач — голубая.</summary>
		public static void Header(string title, ConsoleColor frameColor = ConsoleColor.Green)
		{
			int width = Math.Max(56, Math.Min(UsableWidth(), title.Length + 8));
			bool u = Unicode;
			Console.ForegroundColor = frameColor;
			Console.WriteLine((u ? "\u2554" : "+") + new string(u ? '\u2550' : '=', width) + (u ? "\u2557" : "+"));
			Console.WriteLine((u ? "\u2551" : "|") + Center(title, width) + (u ? "\u2551" : "|"));
			Console.WriteLine((u ? "\u255A" : "+") + new string(u ? '\u2550' : '=', width) + (u ? "\u255D" : "+"));
			Console.ResetColor();
			Console.WriteLine();
		}

		/// <summary>Блок «УСЛОВИЕ ЗАДАЧИ».</summary>
		public static void Condition(string text)
		{
			int width = UsableWidth();
			string prefix = Unicode ? "  \u25B8 " : "  * ";
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine((Unicode ? "\u2500\u2500 " : "-- ") + "УСЛОВИЕ ЗАДАЧИ " + new string(Unicode ? '\u2500' : '-', Math.Max(10, width - 24)));
			Console.ResetColor();
			Console.WriteLine();
			foreach (string line in text.Replace("\r", "").Split('\n'))
				Console.WriteLine(prefix + line);
			Console.WriteLine();
		}

		public static void Rule()
		{
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine(new string(Unicode ? '\u2500' : '-', Math.Max(40, UsableWidth())));
			Console.ResetColor();
		}

		public static void MenuItem(int key, string text)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write(Unicode ? $"   {key,2} \u2192 " : $"   {key,2} - ");
			Console.ResetColor();
			Console.WriteLine(text);
		}

		public static void Step(string text) => Print(Unicode ? "   \u25B8 " : "   > ", text, ConsoleColor.Cyan);
		public static void Info(string text)   => Print("  ", text, ConsoleColor.Gray);
		public static void Hint(string text)   => Print("  ", text, ConsoleColor.DarkGray);
		public static void Good(string text)   => Print(Unicode ? "  \u2713 " : "  OK ", text, ConsoleColor.Green);
		public static void Warn(string text)   => Print(Unicode ? "  \u26A0 " : "  !! ", text, ConsoleColor.Yellow);
		public static void Error(string text)  => Print(Unicode ? "  \u2715 " : "  XX ", text, ConsoleColor.Red);

		private static void Print(string prefix, string text, ConsoleColor color)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(prefix + text);
			Console.ResetColor();
		}

		/// <summary>Печатает многострочный блок текста как есть (шаги вычислений).</summary>
		public static void Block(string text)
		{
			foreach (string line in text.Replace("\r", "").Split('\n'))
			{
				if (line.Length == 0) Console.WriteLine();
				else Console.WriteLine("  " + line);
			}
			Console.WriteLine();
		}

		public static string ReadInput(string prompt)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.Write(prompt + " ");
			Console.ResetColor();
			return Console.ReadLine()?.Trim() ?? "";
		}

		public static int AskChoice(int min, int max)
		{
			while (true)
			{
				string s = ReadInput("Ваш выбор:");
				if (int.TryParse(s, out int v) && v >= min && v <= max) return v;
				Warn($"Введите число от {min} до {max}.");
			}
		}

		// '/r/n' or empty first characters — internal
		public static void Clear()
		{
			try { Console.Clear(); }
			catch (System.IO.IOException) { Console.WriteLine(); }
			catch (Exception) { Console.WriteLine(); }
		}

		/// <summary>Пустой ввод → null.</summary>
		public static int? ReadInt(string prompt, int min, int max)
		{
			while (true)
			{
				string s = ReadInput(prompt);
				if (s.Length == 0) return null;
				if (int.TryParse(s, out int v) && v >= min && v <= max) return v;
				Warn($"Некорректный ввод. Введите целое число от {min} до {max}.");
			}
		}

		/// <summary>Принимает и точку, и запятую как разделитель. Пустой ввод → null.</summary>
		public static double? ReadDouble(string prompt, double min, double max)
		{
			while (true)
			{
				string s = ReadInput(prompt);
				if (s.Length == 0) return null;
				string norm = s.Replace(',', '.').Trim();
				if (double.TryParse(norm, NumberStyles.Float, CultureInfo.InvariantCulture, out double v) && v >= min && v <= max)
					return v;
				Warn($"Некорректный ввод. Введите число от {min:0.####} до {max:0.####}.");
			}
		}

		public static int[] ReadIntArray(string prompt, int minCount, int maxCount)
		{
			while (true)
			{
				string s = ReadInput(prompt);
				var nums = s.Split(new[] { ',', ' ', ';', '\t' }, StringSplitOptions.RemoveEmptyEntries)
							.Select(t => int.TryParse(t, out int v) ? (int?)v : null)
							.Where(v => v.HasValue)
							.Select(v => v.Value)
							.ToArray();
				if (nums.Length >= minCount && nums.Length <= maxCount) return nums;
				Warn($"Введите от {minCount} до {maxCount} целых чисел через пробел или запятую (сейчас {nums.Length}).");
			}
		}

		/// <summary>Режим ввода для задач: 1 — вручную, 2 — сгенерировать, 0 — назад.</summary>
		public static int AskMode()
		{
			Console.WriteLine();
			MenuItem(1, "Ввести данные вручную");
			MenuItem(2, "Сгенерировать пример (seed; пусто — случайно)");
			MenuItem(0, "Назад");
			return AskChoice(0, 2);
		}

		public static int AskSeed()
		{
			string s = ReadInput("Введите seed (пусто — случайный):");
			if (s.Length == 0) return Environment.TickCount & int.MaxValue;
			if (int.TryParse(s, out int seed)) return seed;
			Warn("Требуется целое число. Использую случайный seed.");
			return Environment.TickCount & int.MaxValue;
		}

		public static void Pause()
		{
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine(Unicode ? "  \u2500\u2500 Нажмите Enter, чтобы продолжить \u2500\u2500" : "  -- Нажмите Enter, чтобы продолжить --");
			Console.ResetColor();
			Console.ReadLine();
		}

		/// <summary>Горизонтальная гистограмма для массива целых чисел в виде таблицы с рамками.</summary>
		public static void PrintBarChart(int[] arr, string title)
		{
			if (arr == null || arr.Length == 0) return;
			string[] labels = new string[arr.Length];
			string[] valStrs = new string[arr.Length];
			double[] vals = new double[arr.Length];
			for (int i = 0; i < arr.Length; i++)
			{
				labels[i] = "[" + i + "]";
				valStrs[i] = arr[i].ToString();
				vals[i] = arr[i];
			}
			PrintBarChartTable(labels, valStrs, vals, title);
		}

		/// <summary>Горизонтальная гистограмма для массива вещественных чисел в виде таблицы с рамками.</summary>
		public static void PrintBarChart(double[] arr, string title, string format = "0.##")
		{
			if (arr == null || arr.Length == 0) return;
			string[] labels = new string[arr.Length];
			string[] valStrs = new string[arr.Length];
			double[] vals = new double[arr.Length];
			for (int i = 0; i < arr.Length; i++)
			{
				labels[i] = "[" + i + "]";
				valStrs[i] = arr[i].ToString(format);
				vals[i] = arr[i];
			}
			PrintBarChartTable(labels, valStrs, vals, title);
		}

		private static void PrintBarChartTable(string[] labels, string[] valStrs, double[] vals, string title)
		{
			if (!Unicode) return;
			bool u = Unicode;
			char h  = u ? '\u2500' : '-';
			char tl = u ? '\u250C' : '+';
			char tr = u ? '\u2510' : '+';
			char bl = u ? '\u2514' : '+';
			char br = u ? '\u2518' : '+';
			char vr = u ? '\u2502' : '|';
			char lt = u ? '\u251C' : '+';
			char rt = u ? '\u2524' : '+';
			char bc = u ? '\u2588' : '#';

			int innerWidth = Math.Max(40, UsableWidth() - 2);

			double maxAbs = 0;
			foreach (double v in vals) { double a = Math.Abs(v); if (a > maxAbs) maxAbs = a; }
			if (maxAbs < 1e-12) maxAbs = 1;

			int maxLblLen = 1;
			foreach (string l in labels) { if (l.Length > maxLblLen) maxLblLen = l.Length; }

			int maxValLen = 1;
			foreach (string s in valStrs) { if (s.Length > maxValLen) maxValLen = s.Length; }

			int contentPrefix = 2 + maxLblLen + 1 + maxValLen + 2;
			int barWidth = innerWidth - contentPrefix;
			if (barWidth < 10) barWidth = 10;

			string countWord = ElemWord(vals.Length);

			string titleText = " " + title + " (" + vals.Length + " " + countWord + ") ";
			int dashes = innerWidth - titleText.Length;
			if (dashes < 4) dashes = 4;
			int leftDash = dashes / 2;
			int rightDash = dashes - leftDash;

			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.Write(tl + new string(h, leftDash));
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write(titleText);
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.Write(new string(h, rightDash) + tr);
			Console.WriteLine();
			Console.ResetColor();

			WriteDataRow(vr, null, innerWidth);

			for (int i = 0; i < vals.Length; i++)
			{
				int barLen = (int)Math.Round(Math.Abs(vals[i]) / maxAbs * barWidth);
				if (barLen > barWidth) barLen = barWidth;

				ConsoleColor barColor = vals[i] > 0 ? ConsoleColor.Green
									  : vals[i] < 0 ? ConsoleColor.Red
									  : ConsoleColor.Yellow;

				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.Write(vr);
				Console.ResetColor();

				Console.Write("  " + labels[i].PadRight(maxLblLen) + " " + valStrs[i].PadLeft(maxValLen) + "  ");

				Console.ForegroundColor = barColor;
				Console.Write(new string(bc, barLen));
				Console.ResetColor();

				Console.Write(new string(' ', barWidth - barLen));
				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.WriteLine(vr);
				Console.ResetColor();

				if (i < vals.Length - 1)
				{
					Console.ForegroundColor = ConsoleColor.DarkGray;
					Console.WriteLine(lt + new string(h, innerWidth) + rt);
					Console.ResetColor();
				}
			}

			WriteDataRow(vr, null, innerWidth);

			string scale = BuildScaleLine(barWidth, maxAbs);
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.Write(vr);
			Console.ResetColor();
			Console.Write(new string(' ', contentPrefix));
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.Write(scale);
			int tail = innerWidth - contentPrefix - scale.Length;
			if (tail > 0) Console.Write(new string(' ', tail));
			Console.WriteLine(vr);
			Console.ResetColor();

			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine(bl + new string(h, innerWidth) + br);
			Console.ResetColor();
			Console.WriteLine();
		}

		private static void WriteDataRow(char vr, string content, int innerWidth)
		{
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.Write(vr);
			Console.ResetColor();
			int len = content?.Length ?? 0;
			if (content != null) Console.Write(content);
			Console.Write(new string(' ', innerWidth - len));
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine(vr);
			Console.ResetColor();
		}

		private static string BuildScaleLine(int barWidth, double maxAbs)
		{
			if (barWidth < 6) return "0" + new string(Unicode ? '\u2500' : '-', Math.Max(0, barWidth - 1));

			int intMax = (int)Math.Ceiling(maxAbs);
			int step = intMax <= 5 ? 1 : intMax <= 10 ? 2 : intMax <= 25 ? 5 : intMax <= 50 ? 10 : 20;

			var marks = new System.Collections.Generic.List<int>();
			for (int v = 0; v <= intMax; v += step) marks.Add(v);
			if (marks[marks.Count - 1] < intMax) marks.Add(intMax);

			char fill = Unicode ? '\u2500' : '-';
			char[] line = new char[barWidth];
			for (int i = 0; i < barWidth; i++) line[i] = fill;

			foreach (int mark in marks)
			{
				int pos = (int)Math.Round((double)mark / maxAbs * (barWidth - 1));
				string s = mark.ToString();
				int start = Math.Max(0, Math.Min(pos - s.Length / 2, barWidth - s.Length));
				for (int j = 0; j < s.Length && start + j < barWidth; j++)
					line[start + j] = ' ';
				for (int j = 0; j < s.Length && start + j < barWidth; j++)
					line[start + j] = s[j];
			}

			return new string(line);
		}

		public static string FormatTime(TimeSpan ts)
		{
			if (ts.TotalMilliseconds < 1) return $"{ts.Ticks / 10.0:F0} мкс";
			if (ts.TotalSeconds < 1) return $"{ts.TotalMilliseconds:F2} мс";
			return $"{ts.TotalSeconds:F2} сек";
		}

		private static string ElemWord(int n)
		{
			int m10 = n % 10, m100 = n % 100;
			if (m10 == 1 && m100 != 11) return "элемент";
			if (m10 >= 2 && m10 <= 4 && (m100 < 12 || m100 > 14)) return "элемента";
			return "элементов";
		}
	}
}

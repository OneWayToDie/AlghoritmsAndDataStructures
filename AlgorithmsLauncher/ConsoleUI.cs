using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

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

		/// <summary>Палитра активного оформления (плоская в минимальном режиме).</summary>
		public static ThemePalette Pal => Minimal ? Plain : ThemePalettes.Get(Settings.Current.Theme);

		private static bool _animationsSkipped;

		/// <summary>true — режим минимальной консоли (без оформления).</summary>
		public static bool Minimal => Settings.Current.Minimal;

		private static readonly ThemePalette Plain = new ThemePalette
		{
			Name = "Plain",
			Accent = ConsoleColor.Gray,
			Menu = ConsoleColor.Gray,
			Info = ConsoleColor.Gray,
			Hint = ConsoleColor.DarkGray,
			Good = ConsoleColor.Gray,
			Warn = ConsoleColor.Gray,
			Error = ConsoleColor.Gray,
			Borders = ConsoleColor.Gray,
			BarPlus = ConsoleColor.Gray,
			BarMinus = ConsoleColor.Gray,
			BarZero = ConsoleColor.Gray
		};

		/// <summary>
		/// true — анимации включены: только Windows Terminal (Unicode), не перенаправленный вывод
		/// и скорость из настроек не равна Off.
		/// </summary>
		public static bool AnimationsEnabled
		{
			get
			{
				if (_animationsSkipped || Minimal) return false;
				try { if (Console.IsOutputRedirected) return false; } catch { return false; }
				if (!Unicode) return false;
				return Settings.Current.AnimationSpeed != AnimationSpeed.Off;
			}
		}

		private static int LineAnimationDelayMs
		{
			get
			{
				switch (Settings.Current.AnimationSpeed)
				{
					case AnimationSpeed.Slow: return 45;
					case AnimationSpeed.Normal: return 16;
					default: return 0;
				}
			}
		}

		private static int WipeAnimationDelayMs
		{
			get
			{
				switch (Settings.Current.AnimationSpeed)
				{
					case AnimationSpeed.Slow: return 8;
					case AnimationSpeed.Normal: return 3;
					default: return 0;
				}
			}
		}

		/// <summary>Пауза между строками меню. Esc прерывает и отключает анимации до конца сессии.</summary>
		private static void AnimateSleep(int delayMs)
		{
			if (delayMs <= 0 || !AnimationsEnabled) return;
			int total = 0;
			while (total < delayMs)
			{
				try
				{
					while (Console.KeyAvailable)
					{
						ConsoleKey key = Console.ReadKey(true).Key;
						if (key == ConsoleKey.Escape) { _animationsSkipped = true; return; }
					}
				}
				catch { return; }
				Thread.Sleep(Math.Min(20, delayMs - total));
				total += 20;
			}
		}

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

		/// <summary>Заголовок. В минимальном режиме — одна строка без рамки.</summary>
		public static void Header(string title)
		{
			if (Minimal)
			{
				Console.WriteLine(title);
				Console.WriteLine();
				return;
			}
			int width = Math.Max(56, Math.Min(UsableWidth(), title.Length + 8));
			bool u = Unicode;
			Console.ForegroundColor = Pal.Accent;
			Console.Write(u ? "\u2554" : "+");
			if (AnimationsEnabled)
			{
				int step = WipeAnimationDelayMs;
				for (int i = 0; i < width; i++)
				{
					Console.Write(u ? '\u2550' : '=');
					AnimateSleep(step);
				}
			}
			else
			{
				Console.Write(new string(u ? '\u2550' : '=', width));
			}
			Console.WriteLine(u ? "\u2557" : "+");
			Console.WriteLine((u ? "\u2551" : "|") + Center(title, width) + (u ? "\u2551" : "|"));
			Console.WriteLine((u ? "\u255A" : "+") + new string(u ? '\u2550' : '=', width) + (u ? "\u255D" : "+"));
			Console.ResetColor();
			Console.WriteLine();
		}

		/// <summary>Блок «УСЛОВИЕ ЗАДАЧИ».</summary>
		public static void Condition(string text)
		{
			if (Minimal)
			{
				Console.WriteLine("УСЛОВИЕ ЗАДАЧИ:");
				foreach (string line in text.Replace("\r", "").Split('\n'))
					Console.WriteLine("  " + line);
				Console.WriteLine();
				return;
			}
			int width = UsableWidth();
			string prefix = Unicode ? "  \u25B8 " : "  * ";
			Console.ForegroundColor = Pal.Accent;
			Console.WriteLine((Unicode ? "\u2500\u2500 " : "-- ") + "УСЛОВИЕ ЗАДАЧИ " + new string(Unicode ? '\u2500' : '-', Math.Max(10, width - 24)));
			Console.ResetColor();
			Console.WriteLine();
			foreach (string line in text.Replace("\r", "").Split('\n'))
				Console.WriteLine(prefix + line);
			Console.WriteLine();
		}

		public static void Rule()
		{
			if (Minimal) return;
			Console.ForegroundColor = Pal.Borders;
			Console.WriteLine(new string(Unicode ? '\u2500' : '-', Math.Max(40, UsableWidth())));
			Console.ResetColor();
		}

		public static void MenuItem(int key, string text, string icon = null)
		{
			if (Minimal)
			{
				Console.WriteLine($"  {key}. {text}");
				return;
			}
			Console.ForegroundColor = Pal.Menu;
			Console.Write(Unicode ? $"   {key,2} \u2192 " : $"   {key,2} - ");
			Console.ResetColor();
			Console.Write(RenderIcon(icon));
			Console.WriteLine(text);
			AnimateSleep(LineAnimationDelayMs);
		}

		/// <summary>
		/// Иконка пункта меню по настройке IconStyle:
		/// Emoji — эмодзи (в Windows Terminal), Strict — строгие символы, Off — без иконок.
		/// </summary>
		private static string RenderIcon(string icon)
		{
			if (Minimal) return "";
			IconStyle style = Settings.Current.IconStyle;
			if (style == IconStyle.Off || string.IsNullOrEmpty(icon) || !Unicode)
				return "";
			if (style == IconStyle.Strict)
				return "\u25AA ";
			return icon + " ";
		}

		/// <summary>Строка темы в подменю «Оформление»: маркер текущей + полоска-предпросмотр.</summary>
		public static void ThemeOption(int key, string name, bool isCurrent)
		{
			if (Minimal)
			{
				Console.WriteLine($"  {key}. {(isCurrent ? "OK " : "   ")}{name}");
				return;
			}
			Console.ForegroundColor = Pal.Menu;
			Console.Write(Unicode ? $"   {key,2} \u2192 " : $"   {key,2} - ");
			Console.ResetColor();
			string marker = isCurrent ? (Unicode ? "\u2713" : "OK") : "  ";
			Console.ForegroundColor = isCurrent ? Pal.Good : ConsoleColor.DarkGray;
			Console.Write(marker + " ");
			Console.ResetColor();
			Console.ForegroundColor = Pal.Hint;
			Console.Write(name.PadRight(12));
			Console.ResetColor();
			Console.Write(" ");
			ThemePalettes.Preview(ThemePalettes.Get(name));
			Console.WriteLine();
		}

		/// <summary>Строка-вариант в подменю настроек с маркером текущего значения.</summary>
		public static void OptionRow(int key, string label, bool isCurrent)
		{
			if (Minimal)
			{
				Console.WriteLine($"  {key}. {(isCurrent ? "OK " : "   ")}{label}");
				return;
			}
			Console.ForegroundColor = Pal.Menu;
			Console.Write(Unicode ? $"   {key,2} \u2192 " : $"   {key,2} - ");
			Console.ResetColor();
			string marker = isCurrent ? (Unicode ? "\u2713" : "OK") : "  ";
			Console.ForegroundColor = isCurrent ? Pal.Good : ConsoleColor.DarkGray;
			Console.Write(marker + " ");
			Console.ResetColor();
			Console.WriteLine(label);
		}

		public static void Step(string text) => Print((Minimal || !Unicode) ? "   > " : "   \u25B8 ", text, Pal.Accent);
		public static void Info(string text)   => Print("  ", text, Pal.Info);
		public static void Hint(string text)   => Print("  ", text, Pal.Hint);
		public static void Good(string text)   => Print(Minimal || !Unicode ? "  " : "  \u2713 ", text, Pal.Good);
		public static void Warn(string text)   => Print(Minimal || !Unicode ? "  Внимание: " : "  \u26A0 ", text, Pal.Warn);
		public static void Error(string text)  => Print(Minimal || !Unicode ? "  Ошибка: " : "  \u2715 ", text, Pal.Error);

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
			Console.ForegroundColor = Pal.Menu;
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
			MenuItem(1, "Ввести данные вручную", "\u270D\uFE0F");
			MenuItem(2, "Сгенерировать пример (seed; пусто — случайно)", "\uD83C\uDFB2");
			MenuItem(0, "Назад", "\u21A9\uFE0F");
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
			if (Console.IsOutputRedirected) return;
			Console.WriteLine();
			if (Minimal)
			{
				Console.WriteLine("  -- Нажмите Enter, чтобы продолжить --");
			}
			else
			{
				Console.ForegroundColor = Pal.Borders;
				Console.WriteLine(Unicode ? "  \u2500\u2500 Нажмите Enter, чтобы продолжить \u2500\u2500" : "  -- Нажмите Enter, чтобы продолжить --");
				Console.ResetColor();
			}
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
			if (Minimal || vals == null) return;
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

			Console.ForegroundColor = Pal.Borders;
			Console.Write(tl + new string(h, leftDash));
			Console.ForegroundColor = Pal.Accent;
			Console.Write(titleText);
			Console.ForegroundColor = Pal.Borders;
			Console.Write(new string(h, rightDash) + tr);
			Console.WriteLine();
			Console.ResetColor();

			WriteDataRow(vr, null, innerWidth);

			for (int i = 0; i < vals.Length; i++)
			{
				int barLen = (int)Math.Round(Math.Abs(vals[i]) / maxAbs * barWidth);
				if (barLen > barWidth) barLen = barWidth;

				ConsoleColor barColor = vals[i] > 0 ? Pal.BarPlus
									  : vals[i] < 0 ? Pal.BarMinus
									  : Pal.BarZero;

				Console.ForegroundColor = Pal.Borders;
				Console.Write(vr);
				Console.ResetColor();

				Console.Write("  " + labels[i].PadRight(maxLblLen) + " " + valStrs[i].PadLeft(maxValLen) + "  ");

				Console.ForegroundColor = barColor;
				Console.Write(new string(bc, barLen));
				Console.ResetColor();

				Console.Write(new string(' ', barWidth - barLen));
				Console.ForegroundColor = Pal.Borders;
				Console.WriteLine(vr);
				Console.ResetColor();

				if (i < vals.Length - 1)
				{
					Console.ForegroundColor = Pal.Borders;
					Console.WriteLine(lt + new string(h, innerWidth) + rt);
					Console.ResetColor();
				}
			}

			WriteDataRow(vr, null, innerWidth);

			string scale = BuildScaleLine(barWidth, maxAbs);
			Console.ForegroundColor = Pal.Borders;
			Console.Write(vr);
			Console.ResetColor();
			Console.Write(new string(' ', contentPrefix));
			Console.ForegroundColor = Pal.Borders;
			Console.Write(scale);
			int tail = innerWidth - contentPrefix - scale.Length;
			if (tail > 0) Console.Write(new string(' ', tail));
			Console.WriteLine(vr);
			Console.ResetColor();

			Console.ForegroundColor = Pal.Borders;
			Console.WriteLine(bl + new string(h, innerWidth) + br);
			Console.ResetColor();
			Console.WriteLine();
		}

		private static void WriteDataRow(char vr, string content, int innerWidth)
		{
			Console.ForegroundColor = Pal.Borders;
			Console.Write(vr);
			Console.ResetColor();
			int len = content?.Length ?? 0;
			if (content != null) Console.Write(content);
			Console.Write(new string(' ', innerWidth - len));
			Console.ForegroundColor = Pal.Borders;
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

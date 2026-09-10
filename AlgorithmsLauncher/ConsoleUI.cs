using System;
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
	}
}
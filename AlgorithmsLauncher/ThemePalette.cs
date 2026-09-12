using System;
using System.Collections.Generic;

namespace AlgorithmsLauncher
{
	/// <summary>
	/// Семантическая палитра консоли. Все цвета ConsoleUI берутся из слотов,
	/// а не напрямую из ConsoleColor — смена темы меняет оформление мгновенно.
	/// </summary>
	public class ThemePalette
	{
		public string Name { get; set; } = "Classic";
		public string DisplayName { get; set; } = "Классическая";
		public ConsoleColor Accent { get; set; } = ConsoleColor.Green;   // рамки заголовков, подчёркивания, шаги
		public ConsoleColor Menu { get; set; } = ConsoleColor.Yellow;    // ключи пунктов меню, приглашения ввода
		public ConsoleColor Info { get; set; } = ConsoleColor.Gray;      // основной текст
		public ConsoleColor Hint { get; set; } = ConsoleColor.DarkGray;  // второстепенный текст
		public ConsoleColor Good { get; set; } = ConsoleColor.Green;     // успех, галочки
		public ConsoleColor Warn { get; set; } = ConsoleColor.Yellow;    // предупреждения
		public ConsoleColor Error { get; set; } = ConsoleColor.Red;      // ошибки
		public ConsoleColor Borders { get; set; } = ConsoleColor.DarkGray; // линии-разделители, рамки схем
		public ConsoleColor BarPlus { get; set; } = ConsoleColor.Green;    // столбцы > 0
		public ConsoleColor BarMinus { get; set; } = ConsoleColor.Red;     // столбцы < 0
		public ConsoleColor BarZero { get; set; } = ConsoleColor.Yellow;   // столбцы = 0
	}

	/// <summary>Реестр тем оформления консоли.</summary>
	public static class ThemePalettes
	{
		private static readonly Dictionary<string, ThemePalette> Registry =
			new Dictionary<string, ThemePalette>(StringComparer.OrdinalIgnoreCase)
			{
				{ "Classic", new ThemePalette { Name = "Classic", DisplayName = "Классическая" } },

				{ "Hacker", new ThemePalette
					{
						Name = "Hacker",
						DisplayName = "Хакерская",
						Accent = ConsoleColor.Green,
						Menu = ConsoleColor.Green,
						Info = ConsoleColor.Green,
						Hint = ConsoleColor.DarkGreen,
						Good = ConsoleColor.Green,
						Warn = ConsoleColor.DarkYellow,
						Error = ConsoleColor.Red,
						Borders = ConsoleColor.DarkGreen,
						BarPlus = ConsoleColor.Green,
						BarMinus = ConsoleColor.DarkRed,
						BarZero = ConsoleColor.Gray
					} },

				{ "Night", new ThemePalette
					{
						Name = "Night",
						DisplayName = "Ночная",
						Accent = ConsoleColor.Cyan,
						Menu = ConsoleColor.Cyan,
						Info = ConsoleColor.Gray,
						Hint = ConsoleColor.DarkGray,
						Good = ConsoleColor.Green,
						Warn = ConsoleColor.DarkYellow,
						Error = ConsoleColor.Red,
						Borders = ConsoleColor.DarkGray,
						BarPlus = ConsoleColor.Cyan,
						BarMinus = ConsoleColor.Red,
						BarZero = ConsoleColor.DarkGray
					} },

				{ "Sunset", new ThemePalette
					{
						Name = "Sunset",
						DisplayName = "Закат",
						Accent = ConsoleColor.DarkYellow,
						Menu = ConsoleColor.Yellow,
						Info = ConsoleColor.Gray,
						Hint = ConsoleColor.DarkGray,
						Good = ConsoleColor.Green,
						Warn = ConsoleColor.DarkYellow,
						Error = ConsoleColor.Red,
						Borders = ConsoleColor.DarkGray,
						BarPlus = ConsoleColor.DarkYellow,
						BarMinus = ConsoleColor.Red,
						BarZero = ConsoleColor.Yellow
					} },

				{ "Light", new ThemePalette
					{
						Name = "Light",
						DisplayName = "Светлая",
						Accent = ConsoleColor.DarkBlue,
						Menu = ConsoleColor.DarkBlue,
						Info = ConsoleColor.DarkGray,
						Hint = ConsoleColor.Gray,
						Good = ConsoleColor.DarkGreen,
						Warn = ConsoleColor.DarkYellow,
						Error = ConsoleColor.DarkRed,
						Borders = ConsoleColor.DarkGray,
						BarPlus = ConsoleColor.DarkGreen,
						BarMinus = ConsoleColor.DarkRed,
						BarZero = ConsoleColor.DarkYellow
					} }
			};

		/// <summary>Названия тем (ключи файла настроек) в порядке их отображения в меню.</summary>
		public static string[] Names { get; } =
		{
			"Classic", "Hacker", "Night", "Sunset", "Light"
		};

		/// <summary>Русские названия тем для отображения в меню (тот же порядок, что и Keys).</summary>
		public static string[] DisplayNames { get; } =
		{
			"Классическая", "Хакерская", "Ночная", "Закат", "Светлая"
		};

		public static ThemePalette Get(string name)
		{
			if (name != null && Registry.TryGetValue(name, out var palette))
				return palette;
			return Registry["Classic"];
		}

		/// <summary>Цветовая полоска-предпросмотр темы.</summary>
		public static void Preview(ThemePalette palette)
		{
			ConsoleColor[] strip = { palette.Accent, palette.Menu, palette.Info, palette.Hint,
									 palette.Good, palette.Warn, palette.Error, palette.Borders };
			char block = ConsoleUI.Unicode ? '\u2588' : '#';
			foreach (ConsoleColor c in strip)
			{
				Console.ForegroundColor = c;
				Console.Write(new string(block, 2));
			}
			Console.ResetColor();
		}
	}
}
using System;
using System.Windows;

namespace AlghoritmsAndDataStructures.Helpers
{
	public static class ThemeManager
	{
		public static void Apply(string themeName)
		{
			if (string.IsNullOrEmpty(themeName))
				return;

			var app = Application.Current;
			if (app == null)
				return;

			var newDict = new ResourceDictionary();
			newDict.Source = new Uri($"Resources/Themes/{themeName}.xaml", UriKind.Relative);

			app.Resources.MergedDictionaries.Clear();
			app.Resources.MergedDictionaries.Add(newDict);

			App.IsDarkTheme = IsDark(themeName);
		}

		public static bool IsDark(string themeName)
		{
			return string.IsNullOrEmpty(themeName) ||
				!themeName.StartsWith("Light", StringComparison.OrdinalIgnoreCase);
		}
	}
}
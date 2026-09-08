using System.Linq;
using System.Windows;
using AlghoritmsAndDataStructures.Helpers;
using AlghoritmsAndDataStructures.Properties;

namespace AlghoritmsAndDataStructures
{
	public partial class App : Application
	{
		public static bool IsDarkTheme { get; set; } = true;

		protected override void OnStartup(StartupEventArgs e)
		{
			ThemeManager.Apply(Settings.Default.Theme);
			base.OnStartup(e);
		}
	}
}
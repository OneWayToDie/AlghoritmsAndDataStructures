using System;
using System.Diagnostics;
using System.IO;

namespace AlgorithmsLauncher
{
	public static class LauncherHelper
	{
		private const string AppExeName = "AlghoritmsAndDataStructures.exe";

		public static void LaunchWpf()
		{
			if (IsAlreadyRunning())
			{
				ConsoleUI.Warn("Графическое приложение уже запущено.");
				return;
			}

			string path = FindAppExe();
			if (path == null)
			{
				ConsoleUI.Error("Не найден файл " + AppExeName + ".");
				ConsoleUI.Hint("Ожидается рядом с лаунчером либо в ..\\..\\AlghoritmsAndDataStructures\\bin\\{Debug|Release}.");
				return;
			}

			try
			{
				var psi = new ProcessStartInfo { FileName = path, WorkingDirectory = Path.GetDirectoryName(path) };
				Process.Start(psi);
				ConsoleUI.Good("Графическое приложение запущено.");
			}
			catch (Exception ex)
			{
				ConsoleUI.Error("Не удалось запустить приложение: " + ex.Message);
			}
		}

		private static bool IsAlreadyRunning()
		{
			try { return Process.GetProcessesByName("AlghoritmsAndDataStructures").Length > 0; }
			catch { return false; }
		}

		private static string FindAppExe()
		{
			string baseDir = AppDomain.CurrentDomain.BaseDirectory;
			string[] candidates =
			{
				Path.Combine(baseDir, AppExeName),
				Path.Combine(baseDir, @"..\..\..\AlghoritmsAndDataStructures\bin\Release", AppExeName),
				Path.Combine(baseDir, @"..\..\..\AlghoritmsAndDataStructures\bin\Debug", AppExeName)
			};
			foreach (string c in candidates)
			{
				try
				{
					string full = Path.GetFullPath(c);
					if (File.Exists(full)) return full;
				}
				catch { }
			}
			return null;
		}
	}
}
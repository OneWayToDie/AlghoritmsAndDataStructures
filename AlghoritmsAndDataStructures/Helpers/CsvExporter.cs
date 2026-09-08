using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace AlghoritmsAndDataStructures.Helpers
{
	public static class CsvExporter
	{
		public static bool Save(string[] headers, IEnumerable<string[]> rows, string defaultFileName)
		{
			if (headers == null || headers.Length == 0)
				return false;

			var dialog = new SaveFileDialog
			{
				Filter = "CSV files (*.csv)|*.csv",
				DefaultExt = ".csv",
				FileName = defaultFileName
			};

			if (dialog.ShowDialog() != true)
				return false;

			try
			{
				using (var writer = new StreamWriter(dialog.FileName, false, new UTF8Encoding(true)))
				{
					writer.WriteLine(JoinRow(headers));
					if (rows != null)
					{
						foreach (var row in rows)
						{
							if (row != null && row.Length > 0)
								writer.WriteLine(JoinRow(row));
						}
					}
				}
				MessageBox.Show("Файл сохранён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
				return true;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
		}

		private static string JoinRow(string[] fields)
		{
			var sb = new StringBuilder();
			for (int i = 0; i < fields.Length; i++)
			{
				if (i > 0) sb.Append(';');
				sb.Append(Escape(fields[i]));
			}
			return sb.ToString();
		}

		private static string Escape(string field)
		{
			if (field == null) return "";
			if (field.IndexOfAny(new[] { ';', '"', '\r', '\n' }) < 0)
				return field;
			return "\"" + field.Replace("\"", "\"\"") + "\"";
		}
	}
}
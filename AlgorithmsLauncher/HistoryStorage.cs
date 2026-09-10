using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace AlgorithmsLauncher
{
	public static class HistoryStorage
	{
		private const int MaxEntries = 50;

		private static readonly string FilePath =
			Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "history.json");

		public static void Save(HistoryEntry entry)
		{
			try
			{
				var list = Load();
				list.Insert(0, entry);
				if (list.Count > MaxEntries)
					list.RemoveRange(MaxEntries, list.Count - MaxEntries);
				string json = JsonConvert.SerializeObject(list, Formatting.Indented);
				File.WriteAllText(FilePath, json);
			}
			catch { }
		}

		public static List<HistoryEntry> Load()
		{
			try
			{
				if (!File.Exists(FilePath)) return new List<HistoryEntry>();
				string json = File.ReadAllText(FilePath);
				var list = JsonConvert.DeserializeObject<List<HistoryEntry>>(json);
				return list ?? new List<HistoryEntry>();
			}
			catch { return new List<HistoryEntry>(); }
		}

		public static void Clear()
		{
			try { if (File.Exists(FilePath)) File.Delete(FilePath); }
			catch { }
		}
	}
}

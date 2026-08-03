using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace AlghoritmsAndDataStructures.Helpers
{
	public static class HistoryStorage
	{
		private static readonly string FilePath = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory,
			"history.json"
		);

		public static List<string> Load(string key)
		{
			try
			{
				if (!File.Exists(FilePath))
					return new List<string>();

				string json = File.ReadAllText(FilePath);
				var data = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
				if (data != null && data.TryGetValue(key, out var list))
					return list;
			}
			catch
			{
				// игнорируем ошибки
			}
			return new List<string>();
		}

		public static void Save(string key, List<string> history)
		{
			try
			{
				var data = new Dictionary<string, List<string>>();
				if (File.Exists(FilePath))
				{
					string json = File.ReadAllText(FilePath);
					data = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json)
						   ?? new Dictionary<string, List<string>>();
				}

				data[key] = history;

				string newJson = JsonConvert.SerializeObject(data, Formatting.Indented);
				File.WriteAllText(FilePath, newJson);
			}
			catch
			{
				// игнорируем ошибки сохранения
			}
		}
	}
}
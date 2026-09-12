using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AlgorithmsLauncher
{
	/// <summary>Стиль иконок пунктов меню.</summary>
	public enum IconStyle { Emoji, Strict, Off }

	/// <summary>Скорость анимаций меню.</summary>
	public enum AnimationSpeed { Off, Slow, Normal }

	/// <summary>
	/// База настроек лаунчера. Хранится в JSON рядом с exe (как history.json):
	/// Theme, IconStyle (иконки меню), AnimationSpeed (анимации текста).
	/// </summary>
	public class Settings
	{
		private static readonly string FilePath = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory,
			"settings.json"
		);

		private static Settings _current;
		private static readonly object _lock = new object();

		/// <summary>Актуальные настройки приложения (синглтон, ленивая загрузка).</summary>
		public static Settings Current
		{
			get
			{
				lock (_lock)
				{
					if (_current == null)
						_current = Load();
					return _current;
				}
			}
		}

		/// <summary>Имя консольной темы оформления (Classic/Hacker/Night/Sunset/Light).</summary>
		public string Theme { get; set; } = "Classic";

		/// <summary>Стиль иконок пунктов меню.</summary>
		[JsonConverter(typeof(StringEnumConverter))]
		public IconStyle IconStyle { get; set; } = IconStyle.Emoji;

		/// <summary>Скорость анимаций меню.</summary>
		[JsonConverter(typeof(StringEnumConverter))]
		public AnimationSpeed AnimationSpeed { get; set; } = AnimationSpeed.Normal;

		/// <summary>true — минимальная консоль (только решения задач + настройки, без оформления).</summary>
		public bool Minimal { get; set; }

		private static Settings Load()
		{
			try
			{
				if (File.Exists(FilePath))
				{
					string json = File.ReadAllText(FilePath);
					var settings = JsonConvert.DeserializeObject<Settings>(json);
					if (settings != null)
						return settings;
				}
			}
			catch
			{
				// битый файл — используем значения по умолчанию
			}
			return new Settings();
		}

		public void Save()
		{
			try
			{
				lock (_lock)
				{
					string json = JsonConvert.SerializeObject(this, Formatting.Indented);
					File.WriteAllText(FilePath, json);
				}
			}
			catch
			{
				// нет прав на запись — молча игнорируем
			}
		}
	}
}
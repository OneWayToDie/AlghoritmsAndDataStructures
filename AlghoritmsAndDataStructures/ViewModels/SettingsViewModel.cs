using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using AlghoritmsAndDataStructures.Helpers;
using AlghoritmsAndDataStructures.Properties;

namespace AlghoritmsAndDataStructures.ViewModels
{
	public class ThemeOption
	{
		public string Value { get; set; }
		public string Display { get; set; }
	}

	public class SettingsViewModel : INotifyPropertyChanged
	{
		private string _selectedTheme;
		private bool _historyEnabled;
		private string _historyLimitStr;
		private string _maxSeriesPointsStr;
		private string _maxSeriesSumNStr;

		public ObservableCollection<ThemeOption> Themes { get; } = new ObservableCollection<ThemeOption>
		{
			new ThemeOption { Value = "DarkYellow", Display = "Тёмная (золотая)" },
			new ThemeOption { Value = "DarkBlue", Display = "Тёмная (синяя)" },
			new ThemeOption { Value = "LightGreen", Display = "Светлая (зелёная)" },
			new ThemeOption { Value = "LightBlue", Display = "Светлая (голубая)" }
		};

		public event Action CloseRequested;

		public ICommand SaveCommand { get; }
		public ICommand CancelCommand { get; }
		public ICommand ClearAllHistoryCommand { get; }

		public string SelectedTheme
		{
			get => _selectedTheme;
			set { _selectedTheme = value; OnPropertyChanged(nameof(SelectedTheme)); }
		}

		public bool HistoryEnabled
		{
			get => _historyEnabled;
			set { _historyEnabled = value; OnPropertyChanged(nameof(HistoryEnabled)); }
		}

		public string HistoryLimitStr
		{
			get => _historyLimitStr;
			set { _historyLimitStr = value; OnPropertyChanged(nameof(HistoryLimitStr)); }
		}

		public string MaxSeriesPointsStr
		{
			get => _maxSeriesPointsStr;
			set { _maxSeriesPointsStr = value; OnPropertyChanged(nameof(MaxSeriesPointsStr)); }
		}

		public string MaxSeriesSumNStr
		{
			get => _maxSeriesSumNStr;
			set { _maxSeriesSumNStr = value; OnPropertyChanged(nameof(MaxSeriesSumNStr)); }
		}

		public SettingsViewModel()
		{
			SaveCommand = new RelayCommand(ExecuteSave);
			CancelCommand = new RelayCommand(_ => CloseRequested?.Invoke());
			ClearAllHistoryCommand = new RelayCommand(ExecuteClearAllHistory);

			string savedTheme = Settings.Default.Theme;
			bool themeValid = false;
			foreach (var theme in Themes)
			{
				if (string.Equals(theme.Value, savedTheme, StringComparison.OrdinalIgnoreCase))
					themeValid = true;
			}
			SelectedTheme = themeValid ? savedTheme : "DarkYellow";

			HistoryEnabled = Settings.Default.HistoryEnabled;
			HistoryLimitStr = Settings.Default.HistoryLimit.ToString();
			MaxSeriesPointsStr = Settings.Default.MaxSeriesPoints.ToString();
			MaxSeriesSumNStr = Settings.Default.MaxSeriesSumN.ToString();
		}

		private void ExecuteSave(object parameter)
		{
			int historyLimit = 20;
			if (!int.TryParse(HistoryLimitStr, out historyLimit) || historyLimit < 1)
				historyLimit = 20;
			if (historyLimit > 200)
				historyLimit = 200;

			int maxSeriesPoints = 5000;
			if (!int.TryParse(MaxSeriesPointsStr, out maxSeriesPoints) || maxSeriesPoints < 100)
				maxSeriesPoints = 5000;
			if (maxSeriesPoints > 10000)
				maxSeriesPoints = 10000;

			int maxSeriesSumN = 5000;
			if (!int.TryParse(MaxSeriesSumNStr, out maxSeriesSumN) || maxSeriesSumN < 100)
				maxSeriesSumN = 5000;
			if (maxSeriesSumN > 100000)
				maxSeriesSumN = 100000;

			Settings.Default.Theme = string.IsNullOrEmpty(SelectedTheme) ? "DarkYellow" : SelectedTheme;
			Settings.Default.HistoryEnabled = HistoryEnabled;
			Settings.Default.HistoryLimit = historyLimit;
			Settings.Default.MaxSeriesPoints = maxSeriesPoints;
			Settings.Default.MaxSeriesSumN = maxSeriesSumN;
			Settings.Default.Save();

			ThemeManager.Apply(Settings.Default.Theme);

			CloseRequested?.Invoke();
		}

		private void ExecuteClearAllHistory(object parameter)
		{
			var result = MessageBox.Show(
				"Удалить всю историю вычислений для всех задач?",
				"Очистка истории",
				MessageBoxButton.YesNo,
				MessageBoxImage.Question);
			if (result == MessageBoxResult.Yes)
			{
				HistoryStorage.ClearAll();
				MessageBox.Show("История очищена.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
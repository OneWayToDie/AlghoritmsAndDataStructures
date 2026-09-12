using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using AlghoritmsAndDataStructures.Helpers;
using AlghoritmsAndDataStructures.Models;
using AlghoritmsAndDataStructures.ViewModels.Base;
using AlghoritmsAndDataStructures.ViewModels.Tasks;
using AlghoritmsAndDataStructures.Views;

namespace AlghoritmsAndDataStructures.ViewModels
{
	public class MainViewModel
	{
		public ObservableCollection<WorkItem> Works { get; private set; }

		public ICommand OpenWorkCommand { get; private set; }
		public ICommand OpenSettingsCommand { get; private set; }
		public ICommand SwitchThemeCommand { get; private set; }

		public MainViewModel()
		{
			Works = new ObservableCollection<WorkItem>();

			OpenWorkCommand = new RelayCommand(ExecuteOpenWork);
			OpenSettingsCommand = new RelayCommand(ExecuteOpenSettings);
			SwitchThemeCommand = new RelayCommand(ExecuteSwitchTheme);

			// Заполняем список работ (8 штук)
			Works.Add(new WorkItem { Id = 1, Title = "Практическая работа 1", IconPath = "/Resources/Icons/practice1.ico", IsAvailable = true });
			Works.Add(new WorkItem { Id = 2, Title = "Практическая работа 2", IconPath = "/Resources/Icons/practice2.ico", IsAvailable = true });
			Works.Add(new WorkItem { Id = 3, Title = "Практическая работа 3", IconPath = "/Resources/Icons/practice3.ico", IsAvailable = true });
			Works.Add(new WorkItem { Id = 4, Title = "Практическая работа 4", IconPath = "/Resources/Icons/practice4.ico", IsAvailable = true });
			Works.Add(new WorkItem { Id = 5, Title = "Практическая работа 5", IconPath = "/Resources/Icons/practice5.ico", IsAvailable = true });
			Works.Add(new WorkItem { Id = 6, Title = "Практическая работа 6", IconPath = "/Resources/Icons/practice6.ico", IsAvailable = true });
			Works.Add(new WorkItem { Id = 7, Title = "Практическая работа 7", IconPath = "/Resources/Icons/practice7.ico", IsAvailable = true });
			Works.Add(new WorkItem { Id = 8, Title = "Практическая работа 8", IconPath = "/Resources/Icons/practice8.ico", IsAvailable = true });
		}

		private void ExecuteOpenWork(object parameter)
		{
			if (parameter is int id)
			{
				if (id == 1)
				{
					var vm = new Work1ViewModel();
					var taskWindow = new TaskWindow(vm);
					taskWindow.ShowDialog();
				}
				else if (id == 2)
				{
					var vm = new Work2ViewModel();
					var taskWindow = new TaskWindow(vm);
					taskWindow.ShowDialog();
				}
				else if (id == 3)
				{
					var vm = new Work3ViewModel();
					var taskWindow = new TaskWindow(vm);
					taskWindow.ShowDialog();
				}
				else if (id == 4)
				{
					var vm = new Work4ViewModel();
					var taskWindow = new TaskWindow(vm);
					taskWindow.ShowDialog();
				}
				else if (id == 5)
				{
					var vm = new Work5ViewModel();
					var taskWindow = new TaskWindow(vm);
					taskWindow.ShowDialog();
				}
			else if (id == 6)
			{
				var vm = new Work6ViewModel();
				var taskWindow = new TaskWindow(vm);
				taskWindow.ShowDialog();
			}
			else if (id == 7)
			{
				var vm = new Work7ViewModel();
				var taskWindow = new TaskWindow(vm);
				taskWindow.ShowDialog();
			}
			else if (id == 8)
			{
				var vm = new Work8ViewModel();
				var taskWindow = new TaskWindow(vm);
				taskWindow.ShowDialog();
			}
			else
			{
				MessageBox.Show($"Работа {id} будет добавлена позже.");
			}
			}
		}

		private void ExecuteOpenSettings(object parameter)
		{
			var settingsWindow = new SettingsWindow();
			settingsWindow.ShowDialog();
		}

		private void ExecuteSwitchTheme(object parameter)
		{
			string themeName = parameter as string;
			if (string.IsNullOrEmpty(themeName))
				return;

			ThemeManager.Apply(themeName);
		}
	}
}
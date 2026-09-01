using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AlghoritmsAndDataStructures.Core.Calculators;
using AlghoritmsAndDataStructures.Helpers;
using AlghoritmsAndDataStructures.ViewModels.Base;
using AlghoritmsAndDataStructures.Views;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public enum NeighborhoodDisplayMode
	{
		Original,
		Modified
	}

	public class NeighborhoodAverageTaskViewModel : BaseTaskViewModel
	{
		private string _inputArray = "";
		private string _resultMessage = "";
		private string _originalArrayDisplay = "";
		private string _modifiedArrayDisplay = "";
		private ObservableCollection<int> _originalItems = new ObservableCollection<int>();
		private ObservableCollection<double> _modifiedItems = new ObservableCollection<double>();
		private ObservableCollection<double> _currentItems = new ObservableCollection<double>();
		private NeighborhoodDisplayMode _displayMode = NeighborhoodDisplayMode.Original;

		public string InputArray
		{
			get => _inputArray;
			set { _inputArray = value; OnPropertyChanged(nameof(InputArray)); }
		}

		public string ResultMessage
		{
			get => _resultMessage;
			private set { _resultMessage = value; OnPropertyChanged(nameof(ResultMessage)); }
		}

		public string OriginalArrayDisplay
		{
			get => _originalArrayDisplay;
			private set { _originalArrayDisplay = value; OnPropertyChanged(nameof(OriginalArrayDisplay)); }
		}

		public string ModifiedArrayDisplay
		{
			get => _modifiedArrayDisplay;
			private set { _modifiedArrayDisplay = value; OnPropertyChanged(nameof(ModifiedArrayDisplay)); }
		}

		public ObservableCollection<int> OriginalItems
		{
			get => _originalItems;
			private set { _originalItems = value; OnPropertyChanged(nameof(OriginalItems)); }
		}

		public ObservableCollection<double> ModifiedItems
		{
			get => _modifiedItems;
			private set { _modifiedItems = value; OnPropertyChanged(nameof(ModifiedItems)); }
		}

		public ObservableCollection<double> CurrentItems
		{
			get => _currentItems;
			private set { _currentItems = value; OnPropertyChanged(nameof(CurrentItems)); }
		}

		public NeighborhoodDisplayMode DisplayMode
		{
			get => _displayMode;
			set
			{
				_displayMode = value;
				OnPropertyChanged(nameof(DisplayMode));
				UpdateCurrentItems();
			}
		}

		public ICommand ShowSolutionCommand { get; }
		public ICommand ShowHistoryCommand { get; }
		public ICommand GenerateArrayCommand { get; }
		public ICommand SwitchDisplayModeCommand { get; }

		public override string Title => "Задача: среднее арифметическое соседей";
		public override string HistoryKey => "NeighborhoodAverageTask";

		public NeighborhoodAverageTaskViewModel()
		{
			ShowSolutionCommand = new RelayCommand(ExecuteShowSolution);
			ShowHistoryCommand = new RelayCommand(ExecuteShowHistory);
			GenerateArrayCommand = new RelayCommand(ExecuteGenerateArray);
			SwitchDisplayModeCommand = new RelayCommand(ExecuteSwitchDisplayMode);
			InputArray = "";
		}

		private void ExecuteGenerateArray(object parameter)
		{
			var rand = new Random();
			int count = rand.Next(5, 13);
			var numbers = new int[count];
			for (int i = 0; i < count; i++)
				numbers[i] = rand.Next(-20, 21);
			InputArray = string.Join(", ", numbers);
		}

		protected override void ExecuteCompute(object parameter)
		{
			var numbers = InputArray.Split(new[] { ',', ' ', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries)
									.Select(s => int.TryParse(s, out int val) ? (int?)val : null)
									.Where(x => x.HasValue)
									.Select(x => x.Value)
									.ToList();

			if (numbers.Count < 2)
			{
				ResultMessage = "Ошибка: необходимо ввести минимум 2 целых числа.";
				return;
			}

			var inputArray = numbers.ToArray();
			var modifiedArray = NeighborhoodAverageCalculator.ComputeAverages(inputArray);

			OriginalItems.Clear();
			foreach (var item in inputArray)
				OriginalItems.Add(item);

			ModifiedItems.Clear();
			foreach (var item in modifiedArray)
				ModifiedItems.Add(item);

			OriginalArrayDisplay = string.Join("  ", inputArray);
			ModifiedArrayDisplay = string.Join("  ", modifiedArray.Select(x => x.ToString("F2")));

			ResultMessage = $"Массив из {inputArray.Length} элементов обработан. Каждый элемент заменён на среднее с соседями.";

			DisplayMode = NeighborhoodDisplayMode.Original;
			UpdateCurrentItems();

			AddHistoryEntry($"Исходный: {OriginalArrayDisplay} → Результат: {ModifiedArrayDisplay}");
		}

		private void UpdateCurrentItems()
		{
			var newItems = new ObservableCollection<double>();
			if (DisplayMode == NeighborhoodDisplayMode.Original)
			{
				foreach (var item in OriginalItems)
					newItems.Add(item);
			}
			else
			{
				foreach (var item in ModifiedItems)
					newItems.Add(item);
			}
			CurrentItems = newItems;
			OnPropertyChanged(nameof(CurrentItems));
		}

		private void ExecuteSwitchDisplayMode(object parameter)
		{
			if (parameter is string modeStr)
			{
				if (Enum.TryParse<NeighborhoodDisplayMode>(modeStr, out var mode))
				{
					DisplayMode = mode;
				}
			}
		}

		private void ExecuteShowSolution(object parameter)
		{
			if (OriginalItems.Count == 0)
			{
				System.Windows.MessageBox.Show("Сначала выполните вычисление.", "Информация", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
				return;
			}

			var inputArray = OriginalItems.ToArray();
			var modifiedArray = ModifiedItems.ToArray();
			var steps = NeighborhoodAverageCalculator.GetComputationSteps(inputArray, modifiedArray);

			var window = new SolutionWindow(steps);
			window.ShowDialog();
		}

		private void ExecuteShowHistory(object parameter)
		{
			var window = new HistoryWindow(this);
			window.ShowDialog();
		}
	}
}

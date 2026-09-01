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
	public enum QuickSortDisplayMode
	{
		Original,
		Sorted
	}

	public class QuickSortTaskViewModel : BaseTaskViewModel
	{
		private string _resultMessage = "";
		private string _originalArrayDisplay = "";
		private string _sortedArrayDisplay = "";
		private ObservableCollection<int> _originalItems = new ObservableCollection<int>();
		private ObservableCollection<int> _sortedItems = new ObservableCollection<int>();
		private ObservableCollection<double> _currentItems = new ObservableCollection<double>();
		private QuickSortDisplayMode _displayMode = QuickSortDisplayMode.Original;
		private int[] _originalArray = Array.Empty<int>();
		private int[] _sortedArray = Array.Empty<int>();

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

		public string SortedArrayDisplay
		{
			get => _sortedArrayDisplay;
			private set { _sortedArrayDisplay = value; OnPropertyChanged(nameof(SortedArrayDisplay)); }
		}

		public ObservableCollection<int> OriginalItems
		{
			get => _originalItems;
			private set { _originalItems = value; OnPropertyChanged(nameof(OriginalItems)); }
		}

		public ObservableCollection<int> SortedItems
		{
			get => _sortedItems;
			private set { _sortedItems = value; OnPropertyChanged(nameof(SortedItems)); }
		}

		public ObservableCollection<double> CurrentItems
		{
			get => _currentItems;
			private set { _currentItems = value; OnPropertyChanged(nameof(CurrentItems)); }
		}

		public QuickSortDisplayMode DisplayMode
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

		public override string Title => "Задача: быстрая сортировка массива";
		public override string HistoryKey => "QuickSortTask";

		public QuickSortTaskViewModel()
		{
			ShowSolutionCommand = new RelayCommand(ExecuteShowSolution);
			ShowHistoryCommand = new RelayCommand(ExecuteShowHistory);
			GenerateArrayCommand = new RelayCommand(ExecuteGenerateArray);
			SwitchDisplayModeCommand = new RelayCommand(ExecuteSwitchDisplayMode);
		}

		private void ExecuteGenerateArray(object parameter)
		{
			_originalArray = QuickSortCalculator.GenerateRandomArray(25, -20, 20);
			OriginalArrayDisplay = string.Join("  ", _originalArray);
			OriginalItems.Clear();
			foreach (var item in _originalArray)
				OriginalItems.Add(item);

			_sortedArray = new int[_originalArray.Length];
			Array.Copy(_originalArray, _sortedArray, _originalArray.Length);
			QuickSortCalculator.SortDescending(_sortedArray, 0, _sortedArray.Length - 1);
			SortedArrayDisplay = string.Join("  ", _sortedArray);
			SortedItems.Clear();
			foreach (var item in _sortedArray)
				SortedItems.Add(item);

			ResultMessage = $"Сгенерировано {OriginalItems.Count} случайных чисел. Нажмите «Вычислить» для сортировки.";

			DisplayMode = QuickSortDisplayMode.Original;
			UpdateCurrentItems();
		}

		protected override void ExecuteCompute(object parameter)
		{
			if (OriginalItems.Count == 0)
			{
				ExecuteGenerateArray(null);
			}

			_sortedArray = new int[_originalArray.Length];
			Array.Copy(_originalArray, _sortedArray, _originalArray.Length);
			QuickSortCalculator.SortDescending(_sortedArray, 0, _sortedArray.Length - 1);

			SortedArrayDisplay = string.Join("  ", _sortedArray);
			SortedItems.Clear();
			foreach (var item in _sortedArray)
				SortedItems.Add(item);

			ResultMessage = $"Массив из {OriginalItems.Count} элементов отсортирован по убыванию.";

			DisplayMode = QuickSortDisplayMode.Sorted;
			UpdateCurrentItems();

			AddHistoryEntry($"Исходный: {OriginalArrayDisplay} → Отсортированный: {SortedArrayDisplay}");
		}

		private void UpdateCurrentItems()
		{
			var newItems = new ObservableCollection<double>();
			if (DisplayMode == QuickSortDisplayMode.Original)
			{
				foreach (var item in OriginalItems)
					newItems.Add(item);
			}
			else
			{
				foreach (var item in SortedItems)
					newItems.Add(item);
			}
			CurrentItems = newItems;
			OnPropertyChanged(nameof(CurrentItems));
		}

		private void ExecuteSwitchDisplayMode(object parameter)
		{
			if (parameter is string modeStr)
			{
				if (Enum.TryParse<QuickSortDisplayMode>(modeStr, out var mode))
				{
					DisplayMode = mode;
				}
			}
		}

		private void ExecuteShowSolution(object parameter)
		{
			if (OriginalItems.Count == 0)
			{
				System.Windows.MessageBox.Show("Сначала сгенерируйте массив.", "Информация", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
				return;
			}

			var steps = QuickSortCalculator.GetSortSteps(_originalArray, _sortedArray);

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

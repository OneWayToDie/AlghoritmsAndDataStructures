using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AlghoritmsAndDataStructures.Core.Calculators;
using AlghoritmsAndDataStructures.Helpers;
using AlghoritmsAndDataStructures.ViewModels.Base;
using AlghoritmsAndDataStructures.Views;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class ArrayTaskViewModel : BaseTaskViewModel
	{
		private string _inputArray = "";
		private string _resultMessage = "";
		private string _originalArrayDisplay = "";
		private string _modifiedArrayDisplay = "";
		private double _averageValue;
		private ObservableCollection<int> _originalItems = new ObservableCollection<int>();
		private ObservableCollection<double> _modifiedItems = new ObservableCollection<double>(); // теперь double

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

		public double AverageValue
		{
			get => _averageValue;
			private set { _averageValue = value; OnPropertyChanged(nameof(AverageValue)); }
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

		public ICommand ShowSolutionCommand { get; }
		public ICommand ShowHistoryCommand { get; }
		public ICommand GenerateArrayCommand { get; }

		public override string Title => "Задача: обработка массива (вар. 4)";
		public override string HistoryKey => "ArrayTask";

		public ArrayTaskViewModel()
		{
			ShowSolutionCommand = new RelayCommand(ExecuteShowSolution);
			ShowHistoryCommand = new RelayCommand(ExecuteShowHistory);
			GenerateArrayCommand = new RelayCommand(ExecuteGenerateArray);
			InputArray = "";
		}

		private void ExecuteGenerateArray(object parameter)
		{
			var rand = new Random();
			var numbers = new int[12];
			for (int i = 0; i < 12; i++)
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

			if (numbers.Count != 12)
			{
				ResultMessage = "Ошибка: необходимо ввести ровно 12 целых чисел.";
				return;
			}

			var inputArray = numbers.ToArray();
			var (average, modifiedArray) = ArrayProcessor.ProcessArray(inputArray);
			AverageValue = average;

			OriginalItems.Clear();
			foreach (var item in inputArray)
				OriginalItems.Add(item);

			ModifiedItems.Clear();
			foreach (var item in modifiedArray)
				ModifiedItems.Add(item);

			OriginalArrayDisplay = string.Join("  ", inputArray);
			ModifiedArrayDisplay = string.Join("  ", modifiedArray.Select(x => x.ToString("F2")));

			ResultMessage = $"Среднее на нечётных позициях: {average:F2}. Заменены элементы, кратные 3.";

			AddHistoryEntry($"Исходный: {OriginalArrayDisplay} → Заменённый: {ModifiedArrayDisplay}, Среднее: {average:F2}");
		}

		private void ExecuteShowSolution(object parameter)
		{
			if (OriginalItems.Count == 0)
			{
				System.Windows.MessageBox.Show("Сначала выполните вычисление.", "Информация", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
				return;
			}

			var oddValues = OriginalItems.Where((x, i) => i % 2 == 1).Select(x => x.ToString());
			var modifiedDisplay = string.Join("  ", ModifiedItems.Select(x => x.ToString("F2")));

			var steps = $"Исходный массив: {OriginalArrayDisplay}\n\n" +
						$"Позиции с нечётным индексом (1, 3, 5, 7, 9, 11):\n" +
						$"{string.Join("  ", oddValues)}\n" +
						$"Среднее арифметическое: {AverageValue:F2}\n\n" +
						$"Элементы, кратные 3, заменены на среднее:\n" +
						$"Результат: {modifiedDisplay}";

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
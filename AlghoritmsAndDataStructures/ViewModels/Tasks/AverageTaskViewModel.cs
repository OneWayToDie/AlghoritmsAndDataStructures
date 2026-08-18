using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using AlghoritmsAndDataStructures.Core.Calculators;
using AlghoritmsAndDataStructures.Helpers;
using AlghoritmsAndDataStructures.ViewModels.Base;
using AlghoritmsAndDataStructures.Views;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class AverageTaskViewModel : BaseTaskViewModel
	{
		private string _inputNumbers = "";
		private string _resultDetail = "";
		private ObservableCollection<int> _threeDigitNumbers = new ObservableCollection<int>();
		private double? _averageValue;
		private string _solutionSteps = "";

		public string InputNumbers
		{
			get => _inputNumbers;
			set { _inputNumbers = value; OnPropertyChanged(nameof(InputNumbers)); }
		}

		public string ResultDetail
		{
			get => _resultDetail;
			private set { _resultDetail = value; OnPropertyChanged(nameof(ResultDetail)); }
		}

		public ObservableCollection<int> ThreeDigitNumbers
		{
			get => _threeDigitNumbers;
			private set { _threeDigitNumbers = value; OnPropertyChanged(nameof(ThreeDigitNumbers)); }
		}

		public string ThreeDigitString => string.Join(", ", ThreeDigitNumbers);

		public double? AverageValue
		{
			get => _averageValue;
			private set { _averageValue = value; OnPropertyChanged(nameof(AverageValue)); }
		}

		public ICommand ShowSolutionCommand { get; }
		public ICommand ShowHistoryCommand { get; }

		public override string Title => "Задача 2: Среднее трёхзначных";
		public override string HistoryKey => "Average";

		public AverageTaskViewModel()
		{
			ShowSolutionCommand = new RelayCommand(ExecuteShowSolution);
			ShowHistoryCommand = new RelayCommand(ExecuteShowHistory);
		}

		protected override void ExecuteCompute(object parameter)
		{
			var (threeDigit, avg, message) = AverageCalculator.ComputeAverage(InputNumbers);

			// Заменяем коллекцию новой
			ThreeDigitNumbers = new ObservableCollection<int>(threeDigit);
			OnPropertyChanged(nameof(ThreeDigitNumbers));
			OnPropertyChanged(nameof(ThreeDigitString));
			AverageValue = avg;
			OnPropertyChanged(nameof(AverageValue));

			if (avg.HasValue)
			{
				ResultText = $"Среднее: {avg.Value:F2}";
				ResultDetail = message;
			}
			else
			{
				ResultText = message;
				ResultDetail = "";
			}

			AddHistoryEntry($"Вход: {InputNumbers} → {ResultText}");
			OnPropertyChanged(nameof(ThreeDigitString));

			// Генерируем пошаговое решение
			var sb = new StringBuilder();
			sb.AppendLine("Входные числа: " + InputNumbers);
			sb.AppendLine();
			if (threeDigit.Count == 0)
			{
				sb.AppendLine("Трёхзначных чисел не найдено.");
				_solutionSteps = sb.ToString();
				return;
			}
			sb.AppendLine("Трёхзначные числа: " + string.Join(", ", threeDigit));
			sb.AppendLine($"Количество: {threeDigit.Count}");
			double sum = 0;
			foreach (var num in threeDigit)
				sum += num;
			sb.AppendLine($"Сумма: {sum}");
			sb.AppendLine($"Среднее: {sum} / {threeDigit.Count} = {avg.Value:F2}");
			_solutionSteps = sb.ToString();

			Debug.WriteLine($"=== ExecuteCompute ===");
			Debug.WriteLine($"ThreeDigitNumbers count: {ThreeDigitNumbers.Count}");
			Debug.WriteLine($"AverageValue: {AverageValue}");
		}

		private void ExecuteShowSolution(object parameter)
		{
			if (string.IsNullOrEmpty(_solutionSteps))
			{
				ExecuteCompute(null);
				if (string.IsNullOrEmpty(_solutionSteps))
				{
					System.Windows.MessageBox.Show("Сначала выполните вычисление.", "Информация", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
					return;
				}
			}
			var window = new SolutionWindow(_solutionSteps);
			window.ShowDialog();
		}

		private void ExecuteShowHistory(object parameter)
		{
			var window = new HistoryWindow(this);
			window.ShowDialog();
		}
	}
}
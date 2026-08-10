using System.Collections.ObjectModel;
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

		public ICommand ShowSolutionCommand { get; }

		public override string Title => "Задача 2: Среднее трёхзначных";
		public override string HistoryKey => "Average";

		public AverageTaskViewModel()
		{
			ShowSolutionCommand = new RelayCommand(ExecuteShowSolution);
		}

		protected override void ExecuteCompute(object parameter)
		{
			var (threeDigit, avg, message) = AverageCalculator.ComputeAverage(InputNumbers);

			ThreeDigitNumbers.Clear();
			foreach (var num in threeDigit)
				ThreeDigitNumbers.Add(num);

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
			{
				sum += num;
			}
			sb.AppendLine($"Сумма: {sum}");
			sb.AppendLine($"Среднее: {sum} / {threeDigit.Count} = {avg.Value:F2}");
			_solutionSteps = sb.ToString();
		}

		private void ExecuteShowSolution(object parameter)
		{
			if (string.IsNullOrEmpty(_solutionSteps))
			{
				// Если решение ещё не сгенерировано, вызываем вычисление
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
	}
}
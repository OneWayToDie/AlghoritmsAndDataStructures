using System.Text;
using System.Windows.Input;
using AlghoritmsAndDataStructures.Core.Calculators;
using AlghoritmsAndDataStructures.Helpers;
using AlghoritmsAndDataStructures.ViewModels.Base;
using AlghoritmsAndDataStructures.Views;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class SeriesSumTaskViewModel : BaseTaskViewModel
	{
		private int _n = 2;
		private double _sum = 0;
		private string _solutionSteps = "";

		public int N
		{
			get => _n;
			set { _n = value; OnPropertyChanged(nameof(N)); }
		}

		public double Sum
		{
			get => _sum;
			private set { _sum = value; OnPropertyChanged(nameof(Sum)); }
		}

		public ICommand ShowSolutionCommand { get; }

		public override string Title => "Задача 1: Сумма ряда";
		public override string HistoryKey => "SeriesSum";

		public SeriesSumTaskViewModel()
		{
			ShowSolutionCommand = new RelayCommand(ExecuteShowSolution);
		}

		protected override void ExecuteCompute(object parameter)
		{
			if (N < 2)
			{
				ResultText = "Ошибка: n должно быть больше 1.";
				Sum = 0;
				_solutionSteps = "";
				return;
			}

			Sum = SeriesSumCalculator.ComputeSum(N);
			ResultText = $"S = {Sum:F6}";
			AddHistoryEntry($"n={N}, S={Sum:F6}");

			// Генерируем пошаговое решение
			var sb = new StringBuilder();
			sb.AppendLine($"Сумма ряда: S = 1/2 + 2/3 + ... + {N}/({N}+1)\n");
			double currentSum = 0;
			for (int k = 1; k <= N; k++)
			{
				double term = (double)k / (k + 1);
				currentSum += term;
				sb.AppendLine($"Шаг {k}: {k}/({k + 1}) = {term:F6}, сумма = {currentSum:F6}");
			}
			sb.AppendLine($"\nИтоговая сумма: {Sum:F6}");
			_solutionSteps = sb.ToString();
		}

		private void ExecuteShowSolution(object parameter)
		{
			if (string.IsNullOrEmpty(_solutionSteps))
			{
				// Если решение ещё не сгенерировано, вызываем вычисление (или показываем сообщение)
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
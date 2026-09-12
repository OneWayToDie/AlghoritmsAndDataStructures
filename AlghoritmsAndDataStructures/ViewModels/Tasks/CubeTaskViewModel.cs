using System;
using System.Windows.Input;
using AlghoritmsAndDataStructures.Core.Calculators;
using AlghoritmsAndDataStructures.ViewModels.Base;
using AlghoritmsAndDataStructures.Helpers;        // <-- для RelayCommand
using AlghoritmsAndDataStructures.Views;          // <-- для Cube3DWindow

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class CubeTaskViewModel : BaseTaskViewModel
	{
		private double _edge = 0.0;
		private double _faceArea;
		private double _totalSurface;
		private double _volume;
		private string _calculationSteps = string.Empty;
		private int _seed;
		public override string HistoryKey => "Cube";

		public ICommand Show3DCubeCommand { get; }
		public ICommand ShowSolutionCommand { get; }
		public ICommand ShowHistoryCommand { get; }
		public ICommand GenerateCommand { get; }
		public ICommand PasteSeedCommand { get; }

		public int Seed
		{
			get => _seed;
			private set { _seed = value; OnPropertyChanged(nameof(Seed)); }
		}

		public CubeTaskViewModel()
		{
			Show3DCubeCommand = new RelayCommand(ExecuteShow3DCube);
			ShowSolutionCommand = new RelayCommand(ExecuteShowSolution);
			ShowHistoryCommand = new RelayCommand(ExecuteShowHistory);
			GenerateCommand = new RelayCommand(ExecuteGenerate);
			PasteSeedCommand = new RelayCommand(ExecutePasteSeed);
		}

		public double Edge
		{
			get => _edge;
			set { _edge = value; OnPropertyChanged(nameof(Edge)); }
		}

		public double FaceArea
		{
			get => _faceArea;
			private set { _faceArea = value; OnPropertyChanged(nameof(FaceArea)); }
		}

		public double TotalSurface
		{
			get => _totalSurface;
			private set { _totalSurface = value; OnPropertyChanged(nameof(TotalSurface)); }
		}

		public double Volume
		{
			get => _volume;
			private set { _volume = value; OnPropertyChanged(nameof(Volume)); }
		}

		public string CalculationSteps
		{
			get => _calculationSteps;
			private set { _calculationSteps = value; OnPropertyChanged(nameof(CalculationSteps)); }
		}

		public override string Title => "ПР 1: Куб";

		protected override void ExecuteCompute(object parameter)
		{
			if (Edge <= 0)
			{
				FaceArea = 0;
				TotalSurface = 0;
				Volume = 0;
				ResultText = "Ошибка: сторона должна быть положительной.";
				CalculationSteps = string.Empty;
				return;
			}

			var result = CubeCalculator.Compute(Edge);
			FaceArea = result.FaceArea;
			TotalSurface = result.TotalSurface;
			Volume = result.Volume;
			ResultText = "Вычислено успешно.";

			// Формируем пошаговый вывод (без интерполяции)
			CalculationSteps =
				"Формулы:\n" +
				string.Format("S_грани = a² = {0:F2}² = {1:F2}\n", Edge, FaceArea) +
				string.Format("S_полн = 6·a² = 6·{0:F2} = {1:F2}\n", FaceArea, TotalSurface) +
				string.Format("V = a³ = {0:F2}³ = {1:F2}", Edge, Volume);

			// Добавляем в историю
			string historyEntry = WithCode(
				string.Format("a={0:F2} → Sгр={1:F2}, Sп={2:F2}, V={3:F2}",
					Edge, FaceArea, TotalSurface, Volume),
				Seed);
			AddHistoryEntry(historyEntry);
		}

		private void ExecuteGenerate(object parameter)
		{
			Seed = new Random().Next(1, int.MaxValue);
			Regenerate(Seed);
		}

		private void ExecutePasteSeed(object parameter)
		{
			string clip = System.Windows.Clipboard.ContainsText() ? System.Windows.Clipboard.GetText().Trim() : "";
			if (int.TryParse(clip, out int seed) && seed > 0)
			{
				Seed = seed;
				Regenerate(seed);
				return;
			}
			System.Windows.MessageBox.Show(
				"В буфере обмена нет корректного кода набора. Скопируйте его из истории (кнопка «Скопировать код набора»).",
				"Информация", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
		}

		private void Regenerate(int seed)
		{
			var rand = new Random(seed);
			Edge = Math.Round(1.0 + rand.NextDouble() * 9.0, 2);
		}

		private void ExecuteShowSolution(object parameter)
		{
			if (string.IsNullOrEmpty(CalculationSteps))
			{
				System.Windows.MessageBox.Show("Сначала выполните вычисление.", "Информация", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
				return;
			}
			var window = new SolutionWindow(CalculationSteps);
			window.ShowDialog();
		}

		private void ExecuteShowHistory(object parameter)
		{
			var window = new HistoryWindow(this);
			window.ShowDialog();
		}

		// Метод для отображения 3D-куба
		private void ExecuteShow3DCube(object parameter)
		{
			if (Edge <= 0)
			{
				System.Windows.MessageBox.Show("Ребро должно быть положительным!", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
				return;
			}
			var result = CubeCalculator.Compute(Edge);
			var window = new Cube3DWindow(Edge, result.FaceArea, result.TotalSurface, result.Volume);
			window.ShowDialog();
		}
	}
}
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
		public override string HistoryKey => "Cube";

		public ICommand Show3DCubeCommand { get; }

		public CubeTaskViewModel()
		{
			Show3DCubeCommand = new RelayCommand(ExecuteShow3DCube);
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

		public override string Title => "Задача 1: Куб";

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
			string historyEntry = string.Format(
				"a={0:F2} → Sгр={1:F2}, Sп={2:F2}, V={3:F2}",
				Edge, FaceArea, TotalSurface, Volume);
			AddHistoryEntry(historyEntry);
		}

		// Метод для отображения 3D-куба
		private void ExecuteShow3DCube(object parameter)
		{
			if (Edge <= 0)
			{
				System.Windows.MessageBox.Show("Ребро должно быть положительным!", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
				return;
			}
			var window = new Cube3DWindow(Edge);
			window.ShowDialog();
		}
	}
}
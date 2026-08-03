using AlghoritmsAndDataStructures.Core.Calculators;
using AlghoritmsAndDataStructures.ViewModels.Base;
using System;

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

			// Формируем пошаговый вывод
			CalculationSteps =
				$"Формулы:\n" +
				$"S_грани = a² = {Edge:F2}² = {FaceArea:F2}\n" +
				$"S_полн = 6·a² = 6·{FaceArea:F2} = {TotalSurface:F2}\n" +
				$"V = a³ = {Edge:F2}³ = {Volume:F2}";

			// Добавляем в историю
			string historyEntry = $"a={Edge:F2} → Sгр={FaceArea:F2}, Sп={TotalSurface:F2}, V={Volume:F2}";
			AddHistoryEntry(historyEntry);
		}
	}
}
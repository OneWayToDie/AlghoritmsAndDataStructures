using AlghoritmsAndDataStructures.Core.Calculators;
using AlghoritmsAndDataStructures.ViewModels.Base;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class CubeTaskViewModel : BaseTaskViewModel
	{
		private double _edge = 0.0;   // теперь по умолчанию 0
		private double _faceArea;
		private double _totalSurface;
		private double _volume;

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

		public override string Title => "Задача 1: Куб";

		protected override void ExecuteCompute(object parameter)
		{
			if (Edge <= 0)
			{
				FaceArea = 0;
				TotalSurface = 0;
				Volume = 0;
				ResultText = "Ошибка: сторона должна быть положительной.";
				return;
			}

			var result = CubeCalculator.Compute(Edge);
			FaceArea = result.FaceArea;
			TotalSurface = result.TotalSurface;
			Volume = result.Volume;
			ResultText = "Вычислено успешно.";
		}
	}
}
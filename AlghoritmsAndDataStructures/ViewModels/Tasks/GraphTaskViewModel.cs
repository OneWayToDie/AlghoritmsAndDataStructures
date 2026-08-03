using AlghoritmsAndDataStructures.Core.Calculators;
using AlghoritmsAndDataStructures.ViewModels.Base;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class GraphTaskViewModel : BaseTaskViewModel
	{
		private double _x = 0.0;
		private double _r = 3.0;

		public double X
		{
			get => _x;
			set
			{
				_x = value;
				OnPropertyChanged(nameof(X));
			}
		}

		public double R
		{
			get => _r;
			set
			{
				_r = value;
				OnPropertyChanged(nameof(R));
			}
		}

		public override string Title => "Задача: вычисление функции по графику";

		protected override void ExecuteCompute(object parameter)
		{
			string error;
			var result = GraphCalculator.Compute(X, R, out error);

			if (result.HasValue)
			{
				ResultText = $"Y = {result.Value:F4}";
			}
			else
			{
				ResultText = $"Ошибка: {error}";
			}
		}
	}
}

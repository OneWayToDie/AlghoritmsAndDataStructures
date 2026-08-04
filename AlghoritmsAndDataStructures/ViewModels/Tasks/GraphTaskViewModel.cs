using System.Windows.Input;
using AlghoritmsAndDataStructures.Core.Calculators;
using AlghoritmsAndDataStructures.Helpers;
using AlghoritmsAndDataStructures.ViewModels.Base;
using AlghoritmsAndDataStructures.Views;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class GraphTaskViewModel : BaseTaskViewModel
	{
		private double _x = 0.0;
		private double _r = 3.0;

		public override string HistoryKey => "Graph";

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

		public ICommand ShowGraphCommand { get; }

		public override string Title => "Задача: вычисление функции по графику";

		public GraphTaskViewModel()
		{
			ShowGraphCommand = new RelayCommand(ExecuteShowGraph, CanShowGraph);
		}

		protected override void ExecuteCompute(object parameter)
		{
			string error;
			var result = GraphCalculator.Compute(X, R, out error);

			if (result.HasValue)
			{
				ResultText = string.Format("Y = {0:F4}", result.Value);
			}
			else
			{
				ResultText = string.Format("Ошибка: {0}", error);
			}
		}

		private void ExecuteShowGraph(object parameter)
		{
			bool isDark = App.IsDarkTheme;
			var graphWindow = new GraphWindow(X, R, isDark);
			graphWindow.ShowDialog();
		}

		private bool CanShowGraph(object parameter)
		{
			string err;
			var test = GraphCalculator.Compute(X, R, out err);
			return test.HasValue;
		}
	}
}
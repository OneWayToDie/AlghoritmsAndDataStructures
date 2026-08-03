using System.Windows.Input;
using AlghoritmsAndDataStructures.Core.Calculators;
using AlghoritmsAndDataStructures.Helpers;
using AlghoritmsAndDataStructures.ViewModels.Base;
using AlghoritmsAndDataStructures.Views; // Добавляем для создания окна

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
				ResultText = $"Y = {result.Value:F4}";
			}
			else
			{
				ResultText = $"Ошибка: {error}";
			}
		}

		private void ExecuteShowGraph(object parameter)
		{
			// Открываем окно с графиком, передаём текущие X, R и флаг темы
			bool isDark = App.IsDarkTheme;
			var graphWindow = new GraphWindow(X, R, isDark);
			graphWindow.ShowDialog(); // или Show() для немодального
		}

		private bool CanShowGraph(object parameter)
		{
			// Проверяем, что R корректен (не вызывает ошибок)
			string err;
			var test = GraphCalculator.Compute(X, R, out err);
			return test.HasValue;
		}
	}
}

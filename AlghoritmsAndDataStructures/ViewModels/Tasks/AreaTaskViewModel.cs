using System.Windows.Input;
using AlghoritmsAndDataStructures.Core.Calculators;
using AlghoritmsAndDataStructures.Helpers;
using AlghoritmsAndDataStructures.ViewModels.Base;
using AlghoritmsAndDataStructures.Views;

namespace AlghoritmsAndDataStructures.ViewModels.Tasks
{
	public class AreaTaskViewModel : BaseTaskViewModel
	{
		private double _x = 0;
		private double _y = 0;
		private double _a = 4;
		private double _b = 3;
		private double _r = 5;

		public double X { get => _x; set { _x = value; OnPropertyChanged(nameof(X)); } }
		public double Y { get => _y; set { _y = value; OnPropertyChanged(nameof(Y)); } }
		public double A { get => _a; set { _a = value; OnPropertyChanged(nameof(A)); } }
		public double B { get => _b; set { _b = value; OnPropertyChanged(nameof(B)); } }
		public double R { get => _r; set { _r = value; OnPropertyChanged(nameof(R)); } }

		public ICommand ShowVisualizationCommand { get; }

		public override string HistoryKey => "Area";
		public override string Title => "Задача: попадание точки в область";

		public AreaTaskViewModel()
		{
			ShowVisualizationCommand = new RelayCommand(ExecuteShowVisualization);
		}

		protected override void ExecuteCompute(object parameter)
		{
			string message;
			bool result = AreaChecker.Check(X, Y, A, B, R, out message);
			ResultText = message;
			string historyEntry = $"X={X:F2}, Y={Y:F2}, a={A:F2}, b={B:F2}, R={R:F2} → {message}";
			AddHistoryEntry(historyEntry);
		}

		private void ExecuteShowVisualization(object parameter)
		{
			var window = new AreaVisualizationWindow(X, Y, A, B, R);
			window.ShowDialog();
		}
	}
}
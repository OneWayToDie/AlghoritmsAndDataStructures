using System;
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
		private int _seed;

		public double X { get => _x; set { _x = value; OnPropertyChanged(nameof(X)); } }
		public double Y { get => _y; set { _y = value; OnPropertyChanged(nameof(Y)); } }
		public double A { get => _a; set { _a = value; OnPropertyChanged(nameof(A)); } }
		public double B { get => _b; set { _b = value; OnPropertyChanged(nameof(B)); } }
		public double R { get => _r; set { _r = value; OnPropertyChanged(nameof(R)); } }

		public int Seed
		{
			get => _seed;
			private set { _seed = value; OnPropertyChanged(nameof(Seed)); }
		}

		public ICommand ShowVisualizationCommand { get; }
		public ICommand ShowSolutionCommand { get; }
		public ICommand ShowHistoryCommand { get; }
		public ICommand GenerateCommand { get; }
		public ICommand PasteSeedCommand { get; }

		public override string HistoryKey => "Area";
		public override string Title => "ПР 3: попадание точки в область";

		public AreaTaskViewModel()
		{
			ShowVisualizationCommand = new RelayCommand(ExecuteShowVisualization);
			ShowSolutionCommand = new RelayCommand(ExecuteShowSolution);
			ShowHistoryCommand = new RelayCommand(ExecuteShowHistory);
			GenerateCommand = new RelayCommand(ExecuteGenerate);
			PasteSeedCommand = new RelayCommand(ExecutePasteSeed);
		}

		protected override void ExecuteCompute(object parameter)
		{
			string message;
			bool result = AreaChecker.Check(X, Y, A, B, R, out message);
			if (AreaChecker.IsOnCircle(X, Y, R) && !message.StartsWith("Ошибка", StringComparison.Ordinal))
				ResultText = message + " Точка лежит на окружности.";
			else
				ResultText = message;
			if (!message.StartsWith("Ошибка", StringComparison.Ordinal))
			{
				string historyEntry = WithCode($"X={X:F2}, Y={Y:F2}, a={A:F2}, b={B:F2}, R={R:F2} → {message}", Seed);
				AddHistoryEntry(historyEntry);
			}
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
			A = Math.Round(1.0 + rand.NextDouble() * 9.0, 2);
			B = Math.Round(1.0 + rand.NextDouble() * 9.0, 2);
			R = Math.Round(1.0 + rand.NextDouble() * 9.0, 2);
			X = Math.Round(rand.Next(-10, 11) + rand.NextDouble(), 2);
			Y = Math.Round(rand.Next(-10, 11) + rand.NextDouble(), 2);
		}

		private void ExecuteShowSolution(object parameter)
		{
			string message;
			bool result = AreaChecker.Check(X, Y, A, B, R, out message);
			if (message.StartsWith("Ошибка", StringComparison.Ordinal))
			{
				System.Windows.MessageBox.Show(message, "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
				return;
			}

			bool inRectLeft = (X >= -A) && (X <= 0) && (Y >= -B) && (Y <= 0);
			bool inRectRight = (X >= 0) && (X <= A) && (Y >= 0) && (Y <= B);
			bool inCircle = X * X + Y * Y <= R * R;
			bool onCircle = AreaChecker.IsOnCircle(X, Y, R);
			string circleStatus = onCircle ? "на окружности" : inCircle ? "внутри" : "снаружи";

			var sb = new System.Text.StringBuilder();
			sb.AppendLine("Проверка попадания точки в заштрихованную область");
			sb.AppendLine(string.Format("Точка: X = {0:F2}, Y = {1:F2}", X, Y));
			sb.AppendLine(string.Format("Область: a = {0:F2}, b = {1:F2}, R = {2:F2}", A, B, R));
			sb.AppendLine();
			sb.AppendLine("Заштриховано:");
			sb.AppendLine("  1) III квадрант: внутри прямоугольника [-a..0]x[-b..0] И внутри окружности x^2+y^2<=R^2");
			sb.AppendLine("  2) I квадрант: внутри прямоугольника [0..a]x[0..b] И снаружи окружности");
			sb.AppendLine();
			sb.AppendLine("Проверка по шагам:");
			sb.AppendLine(string.Format("  1. Точка в прямоугольнике [-a..0]x[-b..0] (III кв.): {0}", inRectLeft ? "да" : "нет"));
			sb.AppendLine(string.Format("  2. Точка в окружности: x^2+y^2 = {0:F2}, R^2 = {1:F2} → {2}", X * X + Y * Y, R * R, circleStatus));
			sb.AppendLine(string.Format("  3. Точка в прямоугольнике [0..a]x[0..b] (I кв.): {0}", inRectRight ? "да" : "нет"));
			sb.AppendLine();
			sb.AppendLine("Все условия вместе:");
			sb.AppendLine(string.Format("  (III кв. и внутри прямоугольника и внутри окружности) = {0}", inRectLeft && inCircle));
			sb.AppendLine(string.Format("  (I кв. и внутри прямоугольника и снаружи окружности) = {0}", inRectRight && !inCircle));
			sb.AppendLine();
			if (onCircle)
			{
				sb.AppendLine("Точка лежит на окружности (x^2 + y^2 = R^2).");
				sb.AppendLine();
			}
			sb.AppendLine("Итог: " + message);

			var window = new SolutionWindow(sb.ToString());
			window.ShowDialog();
		}

		private void ExecuteShowHistory(object parameter)
		{
			var window = new HistoryWindow(this);
			window.ShowDialog();
		}

		private void ExecuteShowVisualization(object parameter)
		{
			var window = new AreaVisualizationWindow(X, Y, A, B, R);
			window.ShowDialog();
		}
	}
}
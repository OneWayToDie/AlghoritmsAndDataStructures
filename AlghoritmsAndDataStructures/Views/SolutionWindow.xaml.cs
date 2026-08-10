using System.Windows;
using System.Windows.Input;

namespace AlghoritmsAndDataStructures.Views
{
	public partial class SolutionWindow : Window
	{
		public SolutionWindow(string solutionText)
		{
			InitializeComponent();
			SolutionText.Text = solutionText;
			Owner = Application.Current.MainWindow;
		}

		private void CaptionBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 1)
				this.DragMove();
		}

		private void MinimizeButton_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;
		private void MaximizeButton_Click(object sender, RoutedEventArgs e) => this.WindowState = (this.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
		private void CloseButton_Click(object sender, RoutedEventArgs e) => this.Close();

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && this.WindowState == WindowState.Normal)
				this.DragMove();
		}
	}
}

using System.Windows;
using System.Windows.Input;
using AlghoritmsAndDataStructures.ViewModels;

namespace AlghoritmsAndDataStructures.Views
{
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();
			Owner = Application.Current.MainWindow;

			var viewModel = new SettingsViewModel();
			viewModel.CloseRequested += () => this.Close();
			DataContext = viewModel;
		}

		private void CaptionBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 1)
				this.DragMove();
		}

		private void MinimizeButton_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;
		private void MaximizeButton_Click(object sender, RoutedEventArgs e) => this.WindowState = (this.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
		private void CloseButton_Click(object sender, RoutedEventArgs e) => this.Close();
	}
}
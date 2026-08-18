using System.Windows;
using System.Windows.Input;
using AlghoritmsAndDataStructures.ViewModels.Base;

namespace AlghoritmsAndDataStructures.Views
{
	public partial class HistoryWindow : Window
	{
		public HistoryWindow(BaseTaskViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
			HistoryListBox.ItemsSource = viewModel.History;
			Owner = Application.Current.MainWindow;
		}

		private void CaptionBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 1) this.DragMove();
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
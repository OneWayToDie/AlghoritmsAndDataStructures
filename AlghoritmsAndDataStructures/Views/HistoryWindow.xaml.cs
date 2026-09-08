using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
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

		private void HistoryListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			CopySeedButton.IsEnabled = TryGetSeed(HistoryListBox.SelectedItem as string, out _);
		}

		private void CopySeedButton_Click(object sender, RoutedEventArgs e)
		{
			if (HistoryListBox.SelectedItem is string entry && TryGetSeed(entry, out string seed))
			{
				Clipboard.SetText(seed);
			}
		}

		private static bool TryGetSeed(string entry, out string seed)
		{
			seed = null;
			if (string.IsNullOrEmpty(entry))
				return false;

			var match = Regex.Match(entry, @"(?:seed|код)=(\d+)");
			if (!match.Success)
				return false;

			seed = match.Groups[1].Value;
			return true;
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AlghoritmsAndDataStructures.ViewModels.Tasks;

namespace AlghoritmsAndDataStructures.Views
{
	public partial class TaskWindow : Window
	{
		public TaskWindow(object viewModel)
		{
			InitializeComponent();
			DataContext = viewModel;
			Owner = Application.Current.MainWindow;
		}

		private void CaptionBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 1)
				this.DragMove();
		}

		private void MinimizeButton_Click(object sender, RoutedEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			var tb = sender as TextBox;
			if (tb != null)
				tb.SelectAll();
		}
	}
}
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace AlghoritmsAndDataStructures.Helpers
{
	public static class TooltipService
	{
		private static Popup _popup;
		private static Border _border;
		private static TextBlock _textBlock;
		private static bool _isOpen = false;

		static TooltipService()
		{
			_popup = new Popup
			{
				AllowsTransparency = true,
				Placement = PlacementMode.Mouse,
				StaysOpen = true,  // Теперь не закрывается автоматически
				IsOpen = false
			};

			_border = new Border
			{
				Background = new SolidColorBrush(Color.FromRgb(255, 248, 220)),
				BorderBrush = new SolidColorBrush(Color.FromRgb(180, 180, 180)),
				BorderThickness = new Thickness(1),
				CornerRadius = new CornerRadius(4),
				Padding = new Thickness(8, 4, 8, 4),
				Effect = new System.Windows.Media.Effects.DropShadowEffect
				{
					Color = Colors.Black,
					Opacity = 0.15,
					BlurRadius = 4,
					ShadowDepth = 2
				}
			};

			_textBlock = new TextBlock
			{
				Foreground = System.Windows.Media.Brushes.Black,
				FontSize = 14,
				FontFamily = new FontFamily("Segoe UI, Arial, sans-serif"),
				TextWrapping = TextWrapping.Wrap,
				MaxWidth = 300
			};

			_border.Child = _textBlock;
			_popup.Child = _border;
		}

		public static void Show(string text, Point position)
		{
			if (string.IsNullOrEmpty(text)) return;

			// Если уже открыт с тем же текстом — обновляем позицию и выходим
			if (_isOpen && _textBlock.Text == text)
			{
				_popup.HorizontalOffset = 12;
				_popup.VerticalOffset = 8;
				_popup.Placement = PlacementMode.MousePoint;
				return;
			}

			// Иначе обновляем текст и открываем
			_textBlock.Text = text;
			_popup.HorizontalOffset = 12;
			_popup.VerticalOffset = 8;
			_popup.Placement = PlacementMode.MousePoint;
			_popup.IsOpen = true;
			_isOpen = true;
		}

		public static void Update(string text)
		{
			if (_isOpen && _textBlock != null)
			{
				_textBlock.Text = text;
			}
		}

		public static void Hide()
		{
			_popup.IsOpen = false;
			_isOpen = false;
		}
	}
}
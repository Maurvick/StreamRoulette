using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace StreamRoulette.Converters
{
	public class ColorToSolidColorBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Color color) return new SolidColorBrush(color);
			return Brushes.Transparent;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
	}
}
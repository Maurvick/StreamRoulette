using System.Globalization;
using System.Windows.Data;

namespace StreamRoulette.Converters
{
	public class TimeSpanToTimerValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "00:00";
			// Add a bit of milliseconds for correct rounding display
			return ((TimeSpan)value + new TimeSpan(0, 0, 0, 0, 999)).ToString(@"mm\:ss");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
	}
}
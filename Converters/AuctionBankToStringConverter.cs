using System.Globalization;
using System.Windows.Data;

namespace StreamRoulette.Converters
{
	// Returns only the integer part of value,
	// without hardcoded currency symbols.
	public class AuctionBankToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is int val)
			{
				// Format "N0" adds thousand separators (1,000,000)
				return val.ToString("N0", culture);
			}
			return "0";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
	}
}
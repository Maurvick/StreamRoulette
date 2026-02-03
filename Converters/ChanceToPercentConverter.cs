using System;
using System.Globalization;
using System.Windows.Data;

namespace StreamRoulette.Converters
{
	public class ChanceToPercentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return "0%";
			return Math.Round((double)value * 100, 1).ToString() + "%";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
	}
}
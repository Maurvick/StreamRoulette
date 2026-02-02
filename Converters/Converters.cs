using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace StreamRoulette.Converters
{
    // Конвертер кольору (Color -> Brush)
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color) return new SolidColorBrush(color);
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }

    // Конвертер шансів (0.5 -> "50%")
    public class ChanceToPercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "0%";
            return Math.Round((double)value * 100, 1).ToString() + "%";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }

    // Конвертер банку (100 -> "Банк: 100 грн.")
    public class AuctionBankToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = value is int i ? i : 0;
            return "Банк: " + val.ToString() + " грн.";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }

    // Конвертер часу (TimeSpan -> "10:00")
    public class TimeSpanToTimerValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "00:00";
            // Форматування часу, додаємо трохи мілісекунд для коректного округлення відображення
            return ((TimeSpan)value + new TimeSpan(0, 0, 0, 0, 999)).ToString(@"mm\:ss");
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }

    // Конвертер режимів (Enum <-> Boolean для RadioButton)
    public class AuctionModeToIsCheckedConverter : IValueConverter
    {
        // OneWay: Перетворює Enum (Mode) у bool (IsChecked)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return false;
            return value.ToString() == parameter.ToString();
        }

        // TwoWay: Перетворює bool (IsChecked) назад у Enum (Mode)
        // САМЕ ТУТ БУЛА ПОМИЛКА
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Якщо RadioButton обрано (true) і параметр передано
            if (value is bool isChecked && isChecked && parameter != null)
            {
                try
                {
                    // Намагаємося перетворити рядок (напр. "Normal") назад у Enum (AuctionMode)
                    // Використовуємо targetType, щоб не залежати від конкретного namespace
                    return Enum.Parse(targetType, parameter.ToString());
                }
                catch
                {
                    // Якщо щось пішло не так, нічого не робимо
                    return Binding.DoNothing;
                }
            }
            
            // Якщо RadioButton знято (false), ми нічого не змінюємо в моделі
            return Binding.DoNothing;
        }
    }
}
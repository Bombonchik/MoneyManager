using System.Globalization;

namespace MoneyManager.Converters
{
    internal class TextToCurrency : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal amount = (decimal)value;
            return $"$ {amount}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

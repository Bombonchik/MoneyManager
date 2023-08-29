using MoneyManager.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Converters
{
    public class TransactionTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var transactionType = (TransactionType)value;
            switch (transactionType)
            {
                case TransactionType.Income:
                    return Color.FromArgb("#02A816");
                case TransactionType.Expense:
                    return Color.FromArgb("#E6695B");
                case TransactionType.Transfer:
                    return Colors.White;
                default: return Colors.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

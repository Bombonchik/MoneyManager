using MoneyManager.DataTemplates;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Converters
{
    public class AccountNameExtractorsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return null;
            var transactionView = (TransactionView)value;
            if (transactionView.SourceAccountNameExtractor is null) return null;
            if (transactionView.DestinationAccountNameExtractor is null)
                return transactionView.SourceAccountNameExtractor.AccountName;
            else
                return $"{transactionView.SourceAccountNameExtractor.AccountName} ➝ {transactionView.DestinationAccountNameExtractor.AccountName}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

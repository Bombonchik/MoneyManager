
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Converters
{
    public class CodeToLottieConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (!float.TryParse(value.ToString(), out SKTileDefinitionRotati code))
            //    code = (string)value;
            return null;
           

            //switch ((string)value)
            //{
            //    case 0:
            //        lottienImageSource.File = "bars.json";
            //        return lottienImageSource;


            //    default: return "Unknown";
            //}
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

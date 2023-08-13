using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Converters
{
    public class CodeToWeatherConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!float.TryParse(value.ToString(), out float code))
                code = (int)value;

            switch (code)
            {
                case 0:
                    return "Clear Sky";
                case 1:
                    return "Mainly Clear";

                case 2:
                    return "Partly Cloudy";

                case 3:
                    return "Overcast";

                case 45:
                    return "Fog";

                case 48:
                    return "Rime Fog";

                case 51:
                    return "Drizzle: Light";

                case 53:
                    return "Drizzle: Moderalte";

                case 55:
                    return "Heavy Drizzle";

                case 56:
                    return "Chill Drizzle";

                case 57:
                    return "Freezing Drizzle";

                case 61:
                    return "Rain: Slight";

                case 63:
                    return "Rain: Moderate";

                case 65:
                    return "Strong rain";

                case 66:
                    return "Mild freeze rain";

                case 67:
                    return "Freezing Rain";

                case 71:
                    return "Light snow";

                case 73:
                    return "Mid snow";

                case 75:
                    return "Snow fall: Heavy";

                case 77:
                    return "Snow grains";

                case 80:
                    return "Showers: Slight";

                case 81:
                    return "Showers: Moderate";

                case 82:
                    return "Showers: Violent";

                case 85:
                    return "Light snow";

                case 86:
                    return "Snow showers";

                case 95:
                    return "Mild thunder";

                case 96:
                case 99:
                    return "Hailstorm";

                default: return "Unknown";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

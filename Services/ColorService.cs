using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Services
{
    public class ColorService
    {
        public Color GetColorFromGradient((Color, Color) colorRange)
        {
            Random rnd = new Random();

            // Define the start and end colors of your gradient
            Color startColor = colorRange.Item1;
            Color endColor = colorRange.Item2;

            // Generate a random position in the gradient
            double position = rnd.NextDouble();

            // Calculate the gradient color
            double r = startColor.Red + position * (endColor.Red - startColor.Red);
            double g = startColor.Green + position * (endColor.Green - startColor.Green);
            double b = startColor.Blue + position * (endColor.Blue - startColor.Blue);

            return Color.FromRgb(r, g, b);
        }
    }
}

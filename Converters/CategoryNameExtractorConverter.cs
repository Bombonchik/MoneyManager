﻿using MoneyManager.DataTemplates;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.Converters
{
    public class CategoryNameExtractorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return "Transfer";
            var categoryNameExtractor = (CategoryNameExtractor)value;
            return categoryNameExtractor.CategoryName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

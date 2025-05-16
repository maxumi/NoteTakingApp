using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiCrossplatformApp.Converters
{
    public class BoolToArrowConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Safely coerce to bool (false if null or not a bool)
            bool isExpanded = value is bool b && b;
            return isExpanded ? "⌄" : "›";
        }

        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            throw new NotSupportedException();
    }
}

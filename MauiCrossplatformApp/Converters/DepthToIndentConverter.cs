using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiCrossplatformApp.Converters
{

    public class DepthToIndentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
          => new Thickness((int)value * 16, 0, 0, 0);

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
          => throw new NotSupportedException();
    }
}

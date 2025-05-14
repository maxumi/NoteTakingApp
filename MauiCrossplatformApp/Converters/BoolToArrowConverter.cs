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
        public object Convert(object value, Type t, object p, CultureInfo c) =>
            (value as bool?) == true ? "⌄" : "›";   // down vs. right arrow

        public object ConvertBack(object v, Type t, object p, CultureInfo c) =>
            throw new NotSupportedException();
    }
}

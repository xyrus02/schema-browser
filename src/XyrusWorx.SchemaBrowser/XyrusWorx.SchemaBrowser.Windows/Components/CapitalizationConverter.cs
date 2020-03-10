using System;
using System.Globalization;
using System.Windows.Data;

namespace XyrusWorx.SchemaBrowser.Windows.Components
{
    class CapitalizationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string str ? $"{str.Substring(0, 1).ToUpper()}{str.Substring(1)}" : value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
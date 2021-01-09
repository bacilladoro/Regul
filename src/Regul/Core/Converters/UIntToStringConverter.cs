using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Regul.Core.Converters
{
    class UIntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "0x" + ((uint)value).ToString("X8");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value.ToString();
            return System.Convert.ToUInt32(str, str.StartsWith("0x") ? 16 : 10);
        }
    }
}

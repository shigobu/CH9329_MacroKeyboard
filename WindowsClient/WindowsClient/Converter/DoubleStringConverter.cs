using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WindowsClient.Converter
{
    /// <summary>
    /// 浮動小数点を文字列に変換するコンバーター。
    /// </summary>
    [ValueConversion(typeof(double), typeof(string))]
    class DoubleStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                return doubleValue.ToString(parameter.ToString());
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0.0;
        }
    }
}

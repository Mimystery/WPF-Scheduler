using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WPFTestCanvas.Model
{
    public class RowToTopConverter : IValueConverter
    {
        private const double RowHeight = 25; // Высота одной строки события

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int row)
            {
                return row * RowHeight;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

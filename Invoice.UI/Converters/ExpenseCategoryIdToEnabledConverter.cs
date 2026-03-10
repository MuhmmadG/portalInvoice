using System;
using System.Globalization;
using System.Windows.Data;

namespace Invoice.UI.Converters
{
    public class ExpenseCategoryIdToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // هنا id الخاص بالـ "م.خارجيه" فى جدولك مثلاً = 6
            if (value is int id && id == 6)
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

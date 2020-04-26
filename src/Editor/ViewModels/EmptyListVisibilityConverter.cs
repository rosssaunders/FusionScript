using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RxdSolutions.FusionScript.Wpf
{
    public class EmptyListVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            else
            {
                if (value.Equals(0))
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

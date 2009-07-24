using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace NuvoControl.Client.Viewer.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class PowerConverter: IValueConverter
    {
        #region IValueConverter Members

        public const string POWER_ON = "On";
        public const string POWER_OFF = "Off";

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
                return POWER_ON;
            else
                return POWER_OFF;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (((string)value).Equals(POWER_ON))
                return true;
            else
                return false;
        }

        #endregion
    }
}

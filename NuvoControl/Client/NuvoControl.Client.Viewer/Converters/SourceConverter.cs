using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Data;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Client.Viewer.Converters
{
    //[ValueConversion(typeof(Address), typeof(string))]
    public class SourceConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((values[0] is Address) && (values[1] is List<Source>))
            {
                Address address = (Address)values[0];
                List<Source> sources = (List<Source>)values[1];
                foreach (Source source in sources)
                {
                    if (address.Equals(source.Id))
                        return source.Name;
                }
            }
            return String.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Client.Viewer
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.07.2009
 *   File Name:      PowerConverter.cs
 * 
 ***************************************************************************************************
 * 
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace NuvoControl.Client.Viewer.Converters
{
    /// <summary>
    /// Converts the zone power state between boolean and string representation.
    /// </summary>
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

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
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
using NuvoControl.Common.Configuration;
using NuvoControl.Common;

namespace NuvoControl.Client.Viewer.Converters
{
    /// <summary>
    /// Converts zone quality to string representation and back.
    /// </summary>
    [ValueConversion(typeof(ZoneQuality), typeof(string))]
    public class QualityConverter : IValueConverter
    {
        #region IValueConverter Members

        public const string OFFLINE = "Offline";
        public const string ONLINE = "Online";


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((ZoneQuality)value == ZoneQuality.Offline)
                return OFFLINE;
            else
                return ONLINE;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (((string)value).Equals(OFFLINE))
                return ZoneQuality.Offline;
            else
                return ZoneQuality.Online;
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
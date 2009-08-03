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
 *   File Name:      AcknowledgedConverter.cs
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
    /// Converts the zone acknowledgment state between boolean and string representation.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(string))]
    public class AcknowledgedConverter: IValueConverter
    {
        #region IValueConverter Members

        public const string ACK = "Ack";
        public const string UNACK = "Unack";


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value)
                return UNACK;
            else
                return ACK;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (((string)value).Equals(UNACK))
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
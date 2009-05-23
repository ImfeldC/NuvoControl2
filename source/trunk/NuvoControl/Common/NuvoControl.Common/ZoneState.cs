/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common
 *   Author:         Bernhard Limacher
 *   Creation Date:  21.05.2009
 *   File Name:      ZoneState.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 21.05.2009, Bernhard Limacher: Initial implementation.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common.Logging;

using NuvoControl.Common.Configuration;


namespace NuvoControl.Common
{
    public enum ZoneQuality
    {
        Normal = 0,
        InError = 1,
        OutDated = 2
    }


    [Serializable]
    public class ZoneState
    {
        #region Fields

        private static ILog _log = LogManager.GetCurrentClassLogger();

        private int _volume;
        private bool _commanded = false;
        private Address _source;
        private DateTime _lastUpdate;
        private ZoneQuality _zoneQuality;

        #endregion

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/



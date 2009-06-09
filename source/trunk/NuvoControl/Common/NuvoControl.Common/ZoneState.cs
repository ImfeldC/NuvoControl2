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
        Online = 0,
        Offline = 1
    }


    [Serializable]
    public class ZoneState
    {
        #region Fields

        private static ILog _log = LogManager.GetCurrentClassLogger();

        private ZoneQuality _zoneQuality;

        // members only relevant in case of 'online'
        private int _volume;
        private bool _powerStatus = false;
        private Address _source;
        private DateTime _lastUpdate;   // is set in case a command is received from underlying transport layer

        #endregion

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/



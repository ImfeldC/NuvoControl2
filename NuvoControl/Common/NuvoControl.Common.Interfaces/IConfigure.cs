/**************************************************************************************************
 * 
 *   Copyright (C) Siemens AG 2006 All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common.Interfaces
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.05.2009
 *   File Name:      IConfigure.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 12.05.2009, Bernhard Limacher: Definition of the interface.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Common.Interfaces
{
    /// <summary>
    /// Defines functionality to read the actual configuration of the NuvoControl system.
    /// Defines functionality to modify the actual configuration of the NuvoControl system.
    /// Defines functionality to save the actual configuration of the NuvoControl system.
    /// </summary>
    public interface IConfigure
    {
        void GetGraphicConfiguration();
        void GetZoneKonfiguration(int zoneId);
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/

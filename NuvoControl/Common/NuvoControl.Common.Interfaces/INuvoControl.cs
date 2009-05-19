/**************************************************************************************************
 * 
 *   Copyright (C) Siemens AG 2006 All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common.Interfaces
 *   Author:         Bernhard Limacher
 *   Creation Date:  19.05.2009
 *   File Name:      INuvoControl.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 19.05.2009, Bernhard Limacher: Definition of the interface.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Common.Interfaces
{
    /// <summary>
    /// Defines the entry functions for the NuvoControl service
    /// </summary>
    public interface INuvoControl
    {
        void StartUp(string configurationFile);
        void ShutDown();
        void CreateSession();
        IConfigure IConfigure { get; }
        IMonitor IMonitor { get; }
        IControl IControl { get; }
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/

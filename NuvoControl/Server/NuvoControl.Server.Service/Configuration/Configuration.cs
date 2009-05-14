/**************************************************************************************************
 * 
 *   Copyright (C) Siemens AG 2006 All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.Service
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.05.2009
 *   File Name:      Configuration.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 12.05.2009, Bernhard Limacher: Implementation of the interface.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Common.Interfaces;

namespace NuvoControl.Server.Service.Configuration
{
    public class Configuration : IConfigure, IConfigureInternal
    {
        #region IConfigure Members

        public void GetGraphicConfiguration()
        {
            throw new NotImplementedException();
        }

        public void GetZoneKonfiguration(int zoneId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IConfigureInternal Members

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void GetSystemKonfiguration()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
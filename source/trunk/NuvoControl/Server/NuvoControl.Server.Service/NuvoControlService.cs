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
 *   File Name:      IControl.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 19.05.2009, Bernhard Limacher: Initial implementation for start up.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Common.Interfaces;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.Service.Configuration;
using NuvoControl.Server.ProtocolDriver.Interface;

namespace NuvoControl.Server.Service
{
    /// <summary>
    /// Implements the <paramref name="INuvoControl"/> interface.
    /// </summary>
    public class NuvoControlService: INuvoControl
    {
        #region Fields

        private NuvoControlConfiguration _configuration = null;
        private Dictionary<int, IProtocol> _protocoDrivers = new Dictionary<int, IProtocol>();

        #endregion

        #region INuvoControl Members

        public void StartUp(string configurationFile)
        {
            LoadNuvoControlConfiguration(configurationFile);
            InstantiateProcessModel();
            LoadProtocolDrivers();
        }

        public void ShutDown()
        {
            throw new NotImplementedException();
        }

        public void CreateSession()
        {
            throw new NotImplementedException();
        }

        public IConfigure IConfigure
        {
            get { return _configuration; }
        }

        public IMonitor IMonitor
        {
            get { throw new NotImplementedException(); }
        }

        public IControl IControl
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region Non-Public Interface

        private void LoadNuvoControlConfiguration(string configurationFile)
        {
            _configuration = new NuvoControlConfiguration(configurationFile);
        }

        private void InstantiateProcessModel()
        {
        }

        private void LoadProtocolDrivers()
        {
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/

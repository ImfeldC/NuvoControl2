/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
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

using NuvoControl.Common.Configuration;

using NuvoControl.Common.Interfaces;

namespace NuvoControl.Server.Service.Configuration
{
    public class NuvoControlConfiguration : IConfigure, IConfigureInternal
    {
        #region Fields

        private SystemConfiguration _systemConfiguration = null;
        private string _configurationFile = null;
        private ConfigurationLoader _configurationLoader = null;

        #endregion

        #region Constructors

        public NuvoControlConfiguration(string configurationFile)
        {
            this._configurationFile = configurationFile;
            Initialize();
        }

        #endregion

        #region IConfigure Members

        public Graphic GetGraphicConfiguration()
        {
            return _systemConfiguration.Graphic;
        }

        public Zone GetZoneKonfiguration(int zoneId)
        {
            throw new NotImplementedException();
        }

        public Function GetFunction(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<Function> GetFunctions(Address zoneId)
        {
            throw new NotImplementedException();
        }

        public bool AddFunction(Function newFunction)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region IConfigureInternal Members

        public SystemConfiguration SystemConfiguration
        {
            get { return _systemConfiguration; }
        }

        #endregion

        #region Non-Public Interface

        private void Initialize()
        {
            _configurationLoader = new ConfigurationLoader(_configurationFile);
            _configurationLoader.Validate();
            _systemConfiguration = _configurationLoader.GetConfiguration();
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
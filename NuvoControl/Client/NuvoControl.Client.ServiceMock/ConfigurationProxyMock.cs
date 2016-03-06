using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;

using NuvoControl.Client.ServiceAccess.ConfigurationService;


namespace NuvoControl.Client.ServiceMock
{
    public class ConfigurationProxyMock: IConfigure
    {
        private NuvoControl.Server.ConfigurationService.ConfigurationService _configurationService;

        public ConfigurationProxyMock(string configurationFile)
        {
            _configurationService = new NuvoControl.Server.ConfigurationService.ConfigurationService(configurationFile);
        }

        #region IConfigure Members

        public void RenewLease()
        {
        }

        public Graphic GetGraphicConfiguration()
        {
            return _configurationService.GetGraphicConfiguration();
        }

        public ZoneGraphic GetZoneGraphicConfiguration(Address zoneId)
        {
            throw new NotImplementedException();
        }

        public Function GetFunction(SimpleId id)
        {
            throw new NotImplementedException();
        }

        public Function[] GetFunctions(Address zoneId)
        {
            throw new NotImplementedException();
        }

        public bool AddFunction(Function newFunction)
        {
            throw new NotImplementedException();
        }

        public NuvoImage GetImage(string picturePath)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

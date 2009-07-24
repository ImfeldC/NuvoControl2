using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Client.ServiceAccess.MonitorAndControlService;
using NuvoControl.Client.ServiceAccess.ConfigurationService;

namespace NuvoControl.Client.ServiceAccess
{
    public static class ServiceProxy
    {
        public static MonitorAndControlProxy _monitorAndControlProxy = null;
        public static ConfigurationProxy _configurationProxy = null;

        public static MonitorAndControlProxy MonitorAndControlProxy
        {
            get
            {
                if (_monitorAndControlProxy == null)
                    _monitorAndControlProxy = new MonitorAndControlProxy();
                
                return _monitorAndControlProxy;
            }
        }

        public static ConfigurationProxy ConfigurationProxy
        {
            get
            {
                if (_configurationProxy == null)
                    _configurationProxy = new ConfigurationProxy();

                return _configurationProxy;
            }
        }

        public static void Inject(IConfigure configurationProxy)
        {
            _configurationProxy = new ConfigurationProxy(configurationProxy);
        }

        public static void Inject(IMonitorAndControl mAndCProxy)
        {
            _monitorAndControlProxy = new MonitorAndControlProxy(mAndCProxy);
        }

    }
}

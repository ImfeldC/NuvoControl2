using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Client.ServiceMock;
using NuvoControl.Client.ServiceAccess;

namespace NuvoControl.Client.Viewer.ServiceAccess
{
    class ServiceConfigurator
    {
        public static void Configure(bool test)
        {
            if (test == true)
            {
                ServiceProxy.Inject(new ConfigurationProxyMock(@"..\..\..\..\Config\NuvoControlKonfiguration.xml"));
                MonitorAndControlProxyMock mcMock = new MonitorAndControlProxyMock();
                ServiceProxy.Inject(mcMock);
                mcMock.SetCallback(ServiceProxy.MonitorAndControlProxy);
            }
        }
    }
}

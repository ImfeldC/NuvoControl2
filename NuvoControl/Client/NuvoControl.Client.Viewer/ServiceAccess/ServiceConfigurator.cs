using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Client.ServiceMock;

namespace NuvoControl.Client.Viewer.ServiceAccess
{
    class ServiceConfigurator
    {
        public static void Configure(bool test)
        {
            if (test == true)
            {
                NuvoControl.Client.ServiceAccess.ServiceProxy.Inject(
                    new ConfigurationProxyMock(@"..\..\..\..\Config\NuvoControlKonfiguration.xml"));
            }
        }
    }
}

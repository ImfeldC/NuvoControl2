using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using NuvoControl.Common.Configuration;
using NuvoControl.Common;

using NuvoControl.Server.WebServer.ConfigurationServiceReference;

namespace NuvoControl.Server.WebServer
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        protected void LoadConfiguration()
        {
            labelConfiguration.Text = "";

            ConfigureClient cfgIfc = null;
            cfgIfc = new ConfigureClient();
            // Connect to the discovered service endpoint
            cfgIfc.Endpoint.Address = Global.DiscoveredConfigurationClients.Endpoints[0].Address;

            for( int iZoneId=1; iZoneId<12; iZoneId++ )
            {
                Console.WriteLine("Read zone configuration for zone with id {0}.", iZoneId);
                Zone zone = cfgIfc.GetZoneKonfiguration(new Address(100, iZoneId));
                labelConfiguration.Text += zone.ToString();
                labelConfiguration.Text += "\n";
            }

            Graphic graphic = cfgIfc.GetGraphicConfiguration();
            Console.WriteLine("All graphic details: {0}", graphic.ToString());

            labelConfiguration.Text += graphic.ToString();
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            LoadConfiguration();
        }
    }
}
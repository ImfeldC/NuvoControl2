using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.ServiceModel;

using Common.Logging;

using NuvoControl.Common.Configuration;
using NuvoControl.Common;

using NuvoControl.Server.WebServer.MonitorAndControlServiceReference;

namespace NuvoControl.Server.WebServer
{


    public partial class StatusPage : System.Web.UI.Page
    {
        private static ILog _log = LogManager.GetCurrentClassLogger();

        private void Refresh()
        {
            ucZone1.Refresh();
            ucZone2.Refresh();
            ucZone3.Refresh();
            ucZoneX.Refresh();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Set Service Manager wthin all zone user controls
            ucZone1.SetServiceManager(Global.ServiceManager);
            ucZone2.SetServiceManager(Global.ServiceManager);
            ucZone3.SetServiceManager(Global.ServiceManager);
            ucZoneX.SetServiceManager(Global.ServiceManager);

            // Set zone id within all zone user controls
            ucZone1.SetZoneId(new Address(100, 2));
            ucZone2.SetZoneId(new Address(100, 1));
            ucZone3.SetZoneId(new Address(100, 4));

            if (!IsPostBack)
            {
                // Init list with available zones
                foreach (ZoneGraphic zone in Global.ServiceManager.Zones)
                {
                    listZones.Items.Add(zone.Name);
                }
            }
            // Set zone id of last zone user control, to the selected index
            ucZoneX.SetZoneId(Global.ServiceManager.GetZone(listZones.SelectedValue).Id);
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        protected void listZones_SelectedIndexChanged(object sender, EventArgs e)
        {
            _log.Trace(m => m("Selected Zone={0}", listZones.SelectedValue));
            // Set zone id of last zone user control, to the selected index
            ucZoneX.SetZoneId(Global.ServiceManager.GetZone(listZones.SelectedValue).Id);
        }
    }
}
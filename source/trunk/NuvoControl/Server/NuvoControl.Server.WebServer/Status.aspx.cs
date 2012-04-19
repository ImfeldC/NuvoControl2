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


    public class ServerCallback : IMonitorAndControlCallback
    {
        private static ILog _log = LogManager.GetCurrentClassLogger();

        public int _id = 1;
        #region IMonitorAndControlCallback Members

        public void OnZoneStateChanged(Address zoneId, ZoneState zoneState)
        {
            _log.Trace(m => m("Notification from server zone: {0}", zoneId.ToString()));
            //labelZoneState.Text += String.Format("Notification from server zone: {0}", zoneId.ToString());
        }

        #endregion
    }


    public partial class Status : System.Web.UI.Page
    {
        private static ILog _log = LogManager.GetCurrentClassLogger();

        private void Refresh()
        {
            ucZone1.Refresh();
            ucZone2.Refresh();
            ucZone3.Refresh();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ucZone1.SetServiceManager(Global.ServiceManager);
            ucZone2.SetServiceManager(Global.ServiceManager);
            ucZone3.SetServiceManager(Global.ServiceManager);

            ucZone1.SetZoneId(new Address(100, 2));
            ucZone2.SetZoneId(new Address(100, 1));
            ucZone3.SetZoneId(new Address(100, 4));
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            Refresh();
        }
    }
}
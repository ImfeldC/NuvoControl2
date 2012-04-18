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

        protected void LoadStatus(int iZoneId)
        {
            MonitorAndControlClient mcProxy = null;
            IMonitorAndControlCallback serverCallback = new ServerCallback();
            mcProxy = new MonitorAndControlClient(new InstanceContext(serverCallback));
            // Connect to the discovered service endpoint
            mcProxy.Endpoint.Address = Global.ServiceManager.DiscoveredMonitorControlClients.Endpoints[0].Address;
            mcProxy.Connect();

            _log.Trace(m => m("Read zone configuration for zone with id {0}.", iZoneId));
            ZoneState zoneState = mcProxy.GetZoneState(new Address(100, iZoneId));

            mcProxy.Disconnect();

            labelZoneState.Text = zoneState.ToString();
            Button1.Text = zoneState.PowerStatus.ToString();
        }


        protected void SwitchZone( int iZoneId )
        {
            MonitorAndControlClient mcProxy = null;
            IMonitorAndControlCallback serverCallback = new ServerCallback();
            mcProxy = new MonitorAndControlClient(new InstanceContext(serverCallback));
            // Connect to the discovered service endpoint
            mcProxy.Endpoint.Address = Global.ServiceManager.DiscoveredMonitorControlClients.Endpoints[0].Address;
            mcProxy.Connect();

            ZoneState zoneState = mcProxy.GetZoneState(new Address(100, iZoneId));
            zoneState.PowerStatus = !zoneState.PowerStatus;
            mcProxy.SetZoneState(new Address(100, iZoneId), zoneState);
            zoneState = mcProxy.GetZoneState(new Address(100, iZoneId));

            mcProxy.Disconnect();

            labelZoneState.Text = zoneState.ToString();
            Button1.Text = zoneState.PowerStatus.ToString();
        }


        protected void Button_Click(object sender, EventArgs e)
        {
            SwitchZone(2);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadStatus(2);
        }
    }
}
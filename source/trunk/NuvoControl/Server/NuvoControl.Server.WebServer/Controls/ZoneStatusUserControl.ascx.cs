using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Common.Logging;
using NuvoControl.Common;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Server.WebServer
{
    public partial class ZoneStatusUserControl : System.Web.UI.UserControl
    {
        private static ILog _log = LogManager.GetCurrentClassLogger();

        private ServiceManager _serviceManager = null;
        private Address _zoneId = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            _log.Trace(m => m("ZoneStatusUserControl loaded (Page_Load). isPostBack={0}",IsPostBack));
        }

        /// <summary>
        /// Sets the service manager and initializes the drop-down list of sources.
        /// </summary>
        /// <param name="serviceManager">Service Manager instance.</param>
        public void SetServiceManager(ServiceManager serviceManager)
        {
            _log.Trace(m => m("Set Service Manager, with {0} zones and {1} sources. isPostBack={2} (Control already initialized: {3})", serviceManager.Zones.Count, serviceManager.Sources.Count, IsPostBack, (_serviceManager == null) ? "No" : "Yes"));

            if (_serviceManager == null)
            {
                _serviceManager = serviceManager;

                if (!IsPostBack)
                {
                    // Set sources in drop-down list
                    listSources.Items.Clear();
                    foreach (Source source in serviceManager.Sources)
                    {
                        listSources.Items.Add(source.Name);
                    }
                }
            }
        }

        public void SetZoneId(Address zoneId)
        {
            _log.Trace(m => m("Set Zone Id={0} isPostBack={1} (Previous Zone Id={2})", zoneId.ToString(), IsPostBack, ((_zoneId == null) ? "null" : _zoneId.ToString())));
            _zoneId = zoneId;

            if (!IsPostBack)
            {
                labelZoneName.Text = _serviceManager.GetZone(_zoneId).Name;

                setZoneState(_serviceManager.GetZoneState(_zoneId));
            }
        }

        public void Refresh()
        {
            _log.Trace(m => m("Refresh Id={0} isPostBack={1}", _zoneId.ToString(), IsPostBack));
            setZoneState(_serviceManager.GetZoneState(_zoneId));
        }

        private void setZoneState(ZoneState zoneState)
        {
            _log.Trace(m => m("Update zone state. New Zone State = {0}", zoneState.ToString()));
            btnPower.Text = (zoneState.PowerStatus ? "Off" : "On");
            lblVolume.Text = zoneState.Volume.ToString();
            listSources.SelectedValue = _serviceManager.GetSource(zoneState.Source).Name;
        }

        protected void btnPower_Click(object sender, EventArgs e)
        {
            ZoneState zoneState = _serviceManager.SwitchZone(_zoneId);
            setZoneState(zoneState);
            _log.Trace(m => m("Power button clicked (btnPower_Click). New Zone State = {0}", zoneState.ToString()));
        }

        protected void btnVolDown_Click(object sender, EventArgs e)
        {
            ZoneState zoneState = _serviceManager.VolumeDown(_zoneId);
            setZoneState(zoneState);
            _log.Trace(m => m("Volume DOWN button clicked (btnVolDown_Click). New Zone State = {0}", zoneState.ToString()));
        }

        protected void btnVolUp_Click(object sender, EventArgs e)
        {
            ZoneState zoneState = _serviceManager.VolumeUp(_zoneId);
            setZoneState(zoneState);
            _log.Trace(m => m("Volume UP button clicked (btnVolUp_Click). New Zone State = {0}", zoneState.ToString()));
        }

        protected void listSources_SelectedIndexChanged(object sender, EventArgs e)
        {
            _log.Trace(m => m("Drop-down list of sources selected (listSources_SelectedIndexChanged). Selected Source = {0}", listSources.Text));
            ZoneState zoneState = _serviceManager.SwitchSource(_zoneId, _serviceManager.GetSource(listSources.Text).Id);
            setZoneState(zoneState);
            _log.Trace(m => m("New Zone State = {0}", zoneState.ToString()));
        }

    }
}
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
        private Address _zoneId = new Address();

        protected void Page_Load(object sender, EventArgs e)
        {
            _log.Trace(m => m("[{0}] ZoneStatusUserControl loaded (Page_Load), for zone {0}. isPostBack={1} (ServiceManager initialized: {2})", _zoneId.ToString(), IsPostBack, (_serviceManager == null) ? "No" : "Yes"));
        }

        /// <summary>
        /// Sets the service manager and initializes the drop-down list of sources.
        /// </summary>
        /// <param name="serviceManager">Service Manager instance.</param>
        public void SetServiceManager(ServiceManager serviceManager)
        {
            _log.Trace(m => m("[{0}] Set Service Manager, with {1} zones and {2} sources. isPostBack={3} (ServiceManager initialized: {4})", _zoneId.ToString(), serviceManager.Zones.Count, serviceManager.Sources.Count, IsPostBack, (_serviceManager == null) ? "No" : "Yes"));

            if (_serviceManager == null)
            {
                _serviceManager = serviceManager;

                if (!IsPostBack)
                {
                    // Set sources in drop-down list
                    listSources.Items.Clear();
                    listSources.Items.Add("-- select source --");
                    foreach (Source source in serviceManager.Sources)
                    {
                        listSources.Items.Add(source.Name);
                    }
                }
            }
        }

        public void SetZoneId(Address zoneId)
        {
            _log.Trace(m => m("[{0}] Set Zone Id={0} isPostBack={1} (Previous Zone Id={2})", zoneId.ToString(), IsPostBack, ((_zoneId == null) ? "null" : _zoneId.ToString())));
            _zoneId = zoneId;

            labelZoneName.Text = _serviceManager.GetZone(_zoneId).Name;
            Refresh();
        }

        public void Refresh()
        {
            _log.Trace(m => m("[{0}] Refresh Id={0} isPostBack={1}", _zoneId.ToString(), IsPostBack));
            setZoneState(_serviceManager.GetZoneState(_zoneId));
        }

        private void setZoneState(ZoneState zoneState)
        {
            _log.Trace(m => m("[{0}] Update zone state. New Zone State = {1}", _zoneId.ToString(), zoneState.ToString()));
            btnPower.Text = (zoneState.PowerStatus ? "Off" : "On");
            lblVolume.Text = zoneState.Volume.ToString();
            labelSource.Text = _serviceManager.GetSource(zoneState.Source).Name;
            // Set Tooltip with full zone state information.
            labelZoneName.ToolTip = zoneState.ToString();
        }

        protected void btnPower_Click(object sender, EventArgs e)
        {
            ZoneState zoneState = _serviceManager.SwitchZone(_zoneId);
            setZoneState(zoneState);
            _log.Trace(m => m("[{0}] Power button clicked (btnPower_Click). New Zone State = {1}", _zoneId.ToString(), zoneState.ToString()));
        }

        protected void btnVolDown_Click(object sender, EventArgs e)
        {
            ZoneState zoneState = _serviceManager.VolumeDown(_zoneId);
            setZoneState(zoneState);
            _log.Trace(m => m("[{0}] Volume DOWN button clicked (btnVolDown_Click). New Zone State = {1}", _zoneId.ToString(), zoneState.ToString()));
        }

        protected void btnVolUp_Click(object sender, EventArgs e)
        {
            ZoneState zoneState = _serviceManager.VolumeUp(_zoneId);
            setZoneState(zoneState);
            _log.Trace(m => m("[{0}] Volume UP button clicked (btnVolUp_Click). New Zone State = {1}", _zoneId.ToString(), zoneState.ToString()));
        }

        protected void listSources_SelectedIndexChanged(object sender, EventArgs e)
        {
            _log.Trace(m => m("[{0}] Drop-down list of sources selected (listSources_SelectedIndexChanged). Selected Source = {1}", _zoneId.ToString(), listSources.Text));
            ZoneState zoneState = _serviceManager.SwitchSource(_zoneId, _serviceManager.GetSource(listSources.Text).Id);
            setZoneState(zoneState);
            _log.Trace(m => m("[{0}] New Zone State = {1}", _zoneId.ToString(), zoneState.ToString()));
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ZoneServer;
using Common.Logging;
using NuvoControl.Common;

namespace NuvoControl.Server.FunctionServer
{
    public abstract class ConcreteFunction : IConcreteFunction
    {

        private static ILog _log = LogManager.GetCurrentClassLogger();

        protected IZoneServer _zoneServer = null;


        public ConcreteFunction(IZoneServer zoneServer)
        {
            _zoneServer = zoneServer;
            if (_zoneServer == null)
            {
                _log.Warn(m => m("Zone Server not available, cannot monitor any zone ..."));
            }
        }


        protected void subscribeZone(Address zoneId)
        {
            if (_zoneServer != null)
            {
                _log.Trace(m => m("ConcreteFunction: monitor the zone {0} ...", zoneId.ToString()));
                _zoneServer.Monitor(zoneId, OnZoneNotification);
            }
            else
            {
                _log.Error(m => m("Zone Server not available, cannot monitor the zone {0} ...", zoneId.ToString()));
            }
        }

        protected void unsubscribeZone(Address zoneId)
        {
            if (_zoneServer != null)
            {
                _log.Trace(m => m("ConcreteFunction: remove monitor for zone {0} ...", zoneId.ToString()));
                _zoneServer.RemoveMonitor(zoneId, OnZoneNotification);
            }
        }

        /// <summary>
        /// Calculates the last zone change to ON.
        /// </summary>
        /// <param name="lastZoneChangeToOn">Previous calculated date/time of the last status change to on.</param>
        /// <param name="oldState">Previous zone state.</param>
        /// <param name="newState">New zone state.</param>
        /// <returns></returns>
        protected DateTime calculateZoneChangeToON(DateTime lastZoneChangeToOn, ZoneState oldState, ZoneState newState)
        {
            if (oldState != null)
            {
                if (oldState.PowerStatus == false && newState.PowerStatus == true)
                {
                    // The state has changed from OFF to ON, store the update time
                    lastZoneChangeToOn = newState.LastUpdate;
                }
            }
            else
            {
                if (newState.PowerStatus == true)
                {
                    // we just started and got the first zone state. Store this time as
                    // start time (it's not correct, but better than doing nothing)
                    lastZoneChangeToOn = newState.LastUpdate;
                }
            }
            return lastZoneChangeToOn;
        }

        /// <summary>
        /// Notifcation handler, called from the zone server, which delivers zone state changes
        /// </summary>
        /// <param name="sender">The zone controller, for which the state change appened.</param>
        /// <param name="e">State change event arguments.</param>
        private void OnZoneNotification(object sender, ZoneStateEventArgs e)
        {
            //_log.Trace(m => m("ConcreteFunction: OnZoneNotification() EventArgs={0} ...", e.ToString()));
            notifyOnZoneUpdate(e);
        }


        protected abstract void notifyOnZoneUpdate(ZoneStateEventArgs e);


        #region IConcreteFunction Members

        public abstract Function Function { get; }

        public abstract bool Active { get; }

        public abstract void calculateFunction(DateTime aktTime);

        #endregion


    }
}

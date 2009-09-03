using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ZoneServer;
using Common.Logging;

namespace NuvoControl.Server.FunctionServer
{
    public abstract class ConcreteFunction : IConcreteFunction
    {

        private static ILog _log = LogManager.GetCurrentClassLogger();

        protected IZoneServer _zoneServer = null;


        public ConcreteFunction(IZoneServer zoneServer)
        {
            _zoneServer = zoneServer;
        }


        protected void subscribeZone(Address zoneId)
        {
            _log.Trace(m => m("ConcreteFunction: monitor the zone {0} ...", zoneId.ToString()));
            _zoneServer.Monitor(zoneId, OnZoneNotification);
        }

        protected void unsubscribeZone(Address zoneId)
        {
            _log.Trace(m => m("ConcreteFunction: remove monitor for zone {0} ...", zoneId.ToString()));
            _zoneServer.RemoveMonitor(zoneId, OnZoneNotification);
        }

        /// <summary>
        /// Notifcation handler, called from the zone server, which delivers zone state changes
        /// </summary>
        /// <param name="sender">The zone controller, for which the state change appened.</param>
        /// <param name="e">State change event arguments.</param>
        private void OnZoneNotification(object sender, ZoneStateEventArgs e)
        {
            ZoneController zoneController = sender as ZoneController;
            if (zoneController != null)
            {
                _log.Trace(m => m("ConcreteFunction: OnZoneNotification() EventArgs={0} ...", e.ToString()));
                notifyOnZoneUpdate(e);
            }
                
        }


        protected abstract void notifyOnZoneUpdate(ZoneStateEventArgs e);


        #region IConcreteFunction Members

        public abstract Function Function { get; }

        public abstract void calculateFunction(DateTime aktTime);

        #endregion


    }
}

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
    class ConcreteAlarmFunction : ConcreteFunction, IDisposable
    {

        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Private member to hold the configuration data for the alarm function
        /// </summary>
        AlarmFunction _function;

        /// <summary>
        /// The state of the zone.
        /// </summary>
        private ZoneState _zoneState;


        /// <summary>
        /// Constructor to instantiate a concrete alarm function.
        /// </summary>
        /// <param name="function">Configuration data for this alarm function.</param>
        /// <param name="zoneServer">Zone server, to get notification about zone changes.</param>
        public ConcreteAlarmFunction(AlarmFunction function, IZoneServer zoneServer)
            : base(zoneServer)
        {
            _function = function;
            subscribeZone(_function.ZoneId);
        }


        /// <summary>
        /// Method invoked to notify the function about zone state changes.
        /// The new zone state is stored in the member variable _zoneState
        /// </summary>
        /// <param name="e">Event argument, passed by the notification event.</param>
        protected override void notifyOnZoneUpdate(ZoneStateEventArgs e)
        {
            _log.Trace(m => m("ConcreteAlarmFunction: notifyOnZoneUpdate() EventArgs={0} ...", e.ToString()));
            _zoneState = e.ZoneState;
        }


        /// <summary>
        /// Returns the configuration data for this function.
        /// </summary>
        public override Function Function
        {
            get
            {
                return _function;
            }
        }

        
        public override void calculateFunction(DateTime aktTime)
        {
            _log.Trace(m => m("ConcreteAlarmFunction: calculateFunction at {0}: Active={1}", aktTime, isFunctionActiveToday(aktTime)));
        }


        public bool isFunctionActiveToday( DateTime aktTime )
        {
            return _function.ValidOnDays.Contains( aktTime.DayOfWeek);
        }


        #region IDisposable Members

        /// <summary>
        /// Method to dispose the object.
        /// </summary>
        public void Dispose()
        {
            unsubscribeZone(_function.ZoneId);
        }

        #endregion
    }
}

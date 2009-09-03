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
    /// <summary>
    /// This class implements the concrete sleep function, based on the 
    /// configuration read from XML configuration file.
    /// 
    /// The function is active, if the current time is within the ValidFrom/ValidTo range.
    /// Only within this time span the function is active and allowed to send commands to 
    /// change the zone state.
    /// 
    /// If active and the zone status is longer in status 'ON' then the specified 
    /// Sleep Duration, the zone is switched off.
    /// 
    /// </summary>
    class ConcreteSleepFunction : ConcreteFunction, IDisposable
    {

        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Private member to hold the configuration data for the sleep function
        /// </summary>
        SleepFunction _function;

        /// <summary>
        /// The state of the zone.
        /// </summary>
        private ZoneState _zoneState = null;

        /// <summary>
        /// Private member to store the last zone change to ON
        /// </summary>
        private DateTime _lastZoneChangeToON = new DateTime(2000, 1, 1);


        /// <summary>
        /// Constructor to instantiate a concrete sleep function.
        /// </summary>
        /// <param name="function">Configuration data for this sleep function.</param>
        /// <param name="zoneServer">Zone server, to get notification about zone changes.</param>
        public ConcreteSleepFunction(SleepFunction function, IZoneServer zoneServer)
            : base(zoneServer)
        {
            if (function == null)
            {
                throw new FunctionServerException("Function configuration is null. This is not allowed");
            }
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
            _log.Trace(m => m("ConcreteSleepFunction: notifyOnZoneUpdate() EventArgs={0} ...", e.ToString()));

            if (_zoneState != null)
            {
                if (_zoneState.PowerStatus == false && e.ZoneState.PowerStatus == true)
                {
                    // The state has changed from OFF to ON, store the update time
                    _lastZoneChangeToON = e.ZoneState.LastUpdate;
                }
            }
            else
            {
                if (e.ZoneState.PowerStatus == true)
                {
                    // we just started and got the first zone state. Store this time as
                    // start time (it's not correct, but better than doing nothing)
                    _lastZoneChangeToON = e.ZoneState.LastUpdate;
                }
            }
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

        /// <summary>
        /// Returns true if the function is active.
        /// </summary>
        public override bool Active
        {
            get
            {
                return isFunctionActiveRightNow(DateTime.Now);
            }
        }

        public override void calculateFunction(DateTime aktTime)
        {
            _log.Trace(m => m("ConcreteSleepFunction: calculateFunction at {0}: Active={1}, ZoneState={2}", aktTime, isFunctionActiveRightNow(aktTime), _zoneState));
            if (isFunctionActiveRightNow(aktTime))
            {
                if (aktTime < _lastZoneChangeToON)
                {
                    string strMessage = String.Format("The update time of last zone change {0} is in the future! Actual Time = {1}", _lastZoneChangeToON, aktTime );
                    _log.Fatal(strMessage);
                    throw new FunctionServerException(strMessage);
                }
                if ((_zoneState.PowerStatus == true) && (aktTime-_lastZoneChangeToON > _function.SleepDuration))
                {
                    // Zone power status is ON  and the sleep duration has been reached, switch off zone
                    _log.Trace(m=>m("Switch off zone! AkTime={0}, LastChangeToON={1}", aktTime, _lastZoneChangeToON));
                    if (_zoneServer != null)
                    {
                        ZoneState newState = new ZoneState( _zoneState );
                        newState.PowerStatus = false;
                        _zoneServer.SetZoneState(_function.ZoneId, newState);
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if the function is active right now.
        /// Active means, the current time is within the function ValidFrom/ValidTo configuration
        /// </summary>
        /// <param name="aktTime">Current time</param>
        /// <returns>true, if active</returns>
        public bool isFunctionActiveRightNow(DateTime aktTime)
        {
            if (_function.ValidFrom < _function.ValidTo)
            {
                return (aktTime.TimeOfDay >= _function.ValidFrom) &&
                       (aktTime.TimeOfDay <= _function.ValidTo);
            }
            else
            {
                // Handle them as a time range over midnight
                return (aktTime.TimeOfDay >= _function.ValidFrom) ||
                       (aktTime.TimeOfDay <= _function.ValidTo);
            }
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

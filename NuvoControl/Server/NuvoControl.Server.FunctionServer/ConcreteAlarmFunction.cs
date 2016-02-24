/**************************************************************************************************
 * 
 *   Copyright (C) 2016 by Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.FunctionServer
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ZoneServer;
using Common.Logging;
using NuvoControl.Common;
using NuvoControl.Server.ProtocolDriver.Interface;

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
        private ZoneState _zoneState = null;

        /// <summary>
        /// Private member to store the last zone change to ON
        /// </summary>
        private DateTime _lastZoneChangeToON = new DateTime(2000, 1, 1);

        /// <summary>
        /// True, if a alarm command has been sent.
        /// </summary>
        private bool _alarmCommandHasBeenSent = false;

        /// <summary>
        /// True, if an alarm is running right now.
        /// </summary>
        private bool _alarmRunning = false;

        /// <summary>
        /// True, if validty window is running.
        /// </summary>
        private bool _validityRunning = false;


        /// <summary>
        /// Constructor to instantiate a concrete alarm function.
        /// </summary>
        /// <param name="function">Configuration data for this alarm function.</param>
        /// <param name="zoneServer">Zone server, to get notification about zone changes.</param>
        /// <param name="audioDrivers">Audio Drivers, in case a sound needs to be played.</param>
        public ConcreteAlarmFunction(AlarmFunction function, IZoneServer zoneServer, Dictionary<int, IAudioDriver> audioDrivers)
            : base(zoneServer, function, audioDrivers)
        {
            if (function == null)
            {
                onFunctionError();
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
            _log.Trace(m => m("notifyOnZoneUpdate() EventArgs={0} ...", e.ToString()));
            _lastZoneChangeToON = calculateZoneChangeToON(_lastZoneChangeToON, _zoneState, e.ZoneState);
            _zoneState = new ZoneState(e.ZoneState);
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
        /// This method calculates (checks) based on the passed date/time, if
        /// the alarm function is active and if an action needs to be done.
        /// </summary>
        /// <param name="aktTime"></param>
        public override void calculateFunction(DateTime aktTime)
        {
            //_log.Trace(m => m("calculateFunction at {0}: Active={1}", aktTime, isFunctionActiveToday(aktTime)));
            if( isFunctionActiveToday( aktTime ) )
            {
                if (!_validityRunning)
                {
                    // start of validity reached ...
                    _validityRunning = true;
                    onValidityStart();
                }

                if ((aktTime.TimeOfDay >= _function.AlarmTime) &&
                    (aktTime.TimeOfDay < (_function.AlarmTime + _function.AlarmDuration)) )
                {
                    _log.Trace(m => m("calculateFunction at {0}: Function is in an active window. PowerStatus={1}, LastChangeToON={2}, Function={3}, Active={4}", 
                        aktTime, _zoneState.PowerStatus, _lastZoneChangeToON.TimeOfDay, Function, isFunctionActiveToday(aktTime)));

                    if (!_alarmRunning)
                    {
                        // Function starts ...
                        _alarmRunning = true;
                        onFunctionStart();
                    }

                    // alarm 'window' is running, check if we need to switch the zone ...
                    if ((_zoneState.PowerStatus == false) &&
                        (_lastZoneChangeToON < (aktTime.Date + _function.AlarmTime)))
                    {
                        // the zone is off, and no switch 'on' command was issued since
                        // the alarm window is active ...
                        LogHelper.Log(LogLevel.Trace, String.Format("ConcreteAlarmFunction: Switch zone '{2}' ON! AkTime={0}, LastChangeToON={1}", aktTime, _lastZoneChangeToON, _function.ZoneId));
                        if (_zoneServer != null)
                        {
                            ZoneState newState = new ZoneState(_zoneState);
                            newState.PowerStatus = true;
                            newState.Source = _function.SourceId;
                            newState.Volume = _function.Volume;
                            _zoneServer.SetZoneState(_function.ZoneId, newState);
                            _alarmCommandHasBeenSent = true;
                        }
                    }
                }

                if ((aktTime.TimeOfDay > (_function.AlarmTime + _function.AlarmDuration)) &&
                    (_alarmRunning == true))
                {
                    // alarm 'window' is past  ..

                    if (_zoneState.PowerStatus == true && _alarmCommandHasBeenSent)
                    {
                        // the zone is 'on' and an alarm has been send ...
                        LogHelper.Log(LogLevel.Trace, String.Format("ConcreteAlarmFunction: Switch zone '{2}' OFF! AkTime={0}, LastChangeToON={1}", aktTime, _lastZoneChangeToON, _function.ZoneId));
                        if (_zoneServer != null)
                        {
                            ZoneState newState = new ZoneState(_zoneState);
                            newState.PowerStatus = false;
                            _zoneServer.SetZoneState(_function.ZoneId, newState);
                        }
                    }

                    // Function ends ...
                    _alarmRunning = false;
                    onFunctionEnd();
                }
            }
            else
            {
                if (_validityRunning)
                {
                    // end of validity reached ...
                    _validityRunning = false;
                    onValidityEnd();
                }
            }
        }


        #region IDisposable Members

        /// <summary>
        /// Method to dispose the object.
        /// </summary>
        public new void Dispose()
        {
            unsubscribeZone(_function.ZoneId);
            base.Dispose();
        }

        #endregion
    }
}

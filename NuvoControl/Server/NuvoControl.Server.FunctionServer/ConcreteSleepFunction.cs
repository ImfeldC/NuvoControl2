﻿/**************************************************************************************************
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

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ZoneServer;
using NuvoControl.Server.ProtocolDriver.Interface;


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
        /// True, if a sleep function is running right now.
        /// </summary>
        private bool _sleepRunning = false;

        /// <summary>
        /// True, if validty window is running.
        /// </summary>
        private bool _validityRunning = false;


        /// <summary>
        /// Constructor to instantiate a concrete sleep function.
        /// </summary>
        /// <param name="function">Configuration data for this sleep function.</param>
        /// <param name="zoneServer">Zone server, to get notification about zone changes.</param>
        /// <param name="audioDrivers">Audio Drivers, in case a sound needs to be played.</param>
        public ConcreteSleepFunction(SleepFunction function, IZoneServer zoneServer, Dictionary<int, IAudioDriver> audioDrivers)
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
            //_log.Trace(m => m("ConcreteSleepFunction: notifyOnZoneUpdate() EventArgs={0} ...", e.ToString()));
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
        /// This method calculates (checks) if the function is active and if an action is required.
        /// The method is called periodically.
        /// </summary>
        /// <param name="aktTime">Time, used for the calculation.</param>
        public override void calculateFunction(DateTime aktTime)
        {
            //_log.Trace(m => m("calc sleep function at {0}: Active={1}, lastZoneChangeToON={2}, ZoneState={3}", aktTime, isFunctionActiveRightNow(aktTime), _lastZoneChangeToON, _zoneState));
            if (isFunctionActiveRightNow(aktTime))
            {
                if (aktTime < _lastZoneChangeToON)
                {
                    String strMessage = String.Format("The update time of last zone change {0} is in the future! Actual Time = {1}", _lastZoneChangeToON, aktTime);
                    LogHelper.Log(LogLevel.Fatal, strMessage);
                    onFunctionError();
                    throw new FunctionServerException(strMessage);
                }

                if (_zoneServer != null)
                {
                    if (!_validityRunning)
                    {
                        // start of validity reached ...
                        _validityRunning = true;
                        onValidityStart();
                    }

                    TimeSpan onTime = aktTime - _lastZoneChangeToON;
                    if (_zoneState.PowerStatus == true)
                    {
                        if (!_sleepRunning)
                        {
                            // Sleep function starts ...
                            _sleepRunning = true;
                            onFunctionStart();
                        }
                        if (onTime >= _function.SleepDuration)
                        {
                            // Zone power status is ON  and the sleep duration has been reached, switch off zone
                            LogHelper.Log(LogLevel.Trace, String.Format("ConcreteSleepFunction: Switch zone '{0}' OFF! AkTime={1}, LastChangeToON={2}", _function.ZoneId, aktTime, _lastZoneChangeToON));
                            if (_zoneServer != null)
                            {
                                ZoneState newState = new ZoneState(_zoneState);
                                newState.PowerStatus = false;
                                _zoneServer.SetZoneState(_function.ZoneId, newState);
                            }
                            // Sleep function ends ...
                            _sleepRunning = false;
                            onFunctionEnd();
                        }
                    }
                    else
                    {
                        LogHelper.Log(LogLevel.Trace, String.Format("Don't switch off zone! PowerStatus={0}, onTime={1}, AkTime={2}, LastChangeToON={3}", _zoneState.PowerStatus, onTime, aktTime, _lastZoneChangeToON));
                    }
                }
                else
                {
                    LogHelper.Log(LogLevel.Error, String.Format("ConcreteSleepFunction: Cannot switch zone '{0}', because zone status is missing! LastChangeToON={1}", _function.ZoneId, _lastZoneChangeToON));
                    onFunctionError();
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

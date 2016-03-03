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

using Common.Logging;

using NuvoControl.Common;  
using NuvoControl.Common.Configuration;             // ZoneChangeFunction
using NuvoControl.Server.ZoneServer;                // IZoneServer
using NuvoControl.Server.ProtocolDriver.Interface;  // IAudioDriver 



namespace NuvoControl.Server.FunctionServer
{
    public class ConcreteZoneChangeFunction : ConcreteFunction, IDisposable
    {
        /// <summary>
        /// Private member to hold the configuration data for the sleep function
        /// </summary>
        ZoneChangeFunction _function;

        /// <summary>
        /// The state of the zone.
        /// </summary>
        private ZoneState _zoneState = null;

        /// <summary>
        /// The new states of the zone.
        /// Received in notification method and processed in calculate method.
        /// </summary>
        private List<ZoneState> _newZoneState = new List<ZoneState>();

        /// <summary>
        /// Private member to store the last zone change to ON
        /// </summary>
        private DateTime _lastZoneChangeToON = new DateTime(2000, 1, 1);

        /// <summary>
        /// True, if validty window is running.
        /// </summary>
        private bool _validityRunning = false;


        /// <summary>
        /// Constructor to instantiate a concrete zone change function.
        /// </summary>
        /// <param name="function">Configuration data for this zone change function.</param>
        /// <param name="zoneServer">Zone server, to get notification about zone changes.</param>
        /// <param name="audioDrivers">Audio Drivers, in case a sound needs to be played.</param>
        public ConcreteZoneChangeFunction(ZoneChangeFunction function, IZoneServer zoneServer, Dictionary<int, IAudioDriver> audioDrivers)
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
        protected override void notifyOnZoneUpdate(ZoneServer.ZoneStateEventArgs e)
        {
            lock (this)
            {
                //_log.Trace(m => m("ConcreteSleepFunction: notifyOnZoneUpdate() EventArgs={0} ...", e.ToString()));
                _lastZoneChangeToON = calculateZoneChangeToON(_lastZoneChangeToON, _zoneState, e.ZoneState);

                // Store zone change, will be processed in calculate method
                _newZoneState.Add(e.ZoneState);
            }
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
            lock (this)
            {
                if (this.isActiveAt(aktTime))
                {
                    if (!_validityRunning)
                    {
                        // start of validity reached ...
                        _validityRunning = true;
                        onValidityStart();
                    }

                    foreach (ZoneState newZoneState in _newZoneState)
                    {
                        if (_zoneState != null)
                        {
                            // Function is active, calculate ...
                            if (_function.OnStatusChange && (_zoneState.PowerStatus != newZoneState.PowerStatus))
                            {
                                // Power status has changed, raise event ...
                                LogHelper.Log(LogLevel.Debug, String.Format("ConcreteZoneChangeFunction: Power status changed from {0} to {1}, raise event! LastChangeToON={2}, Function={3}", _zoneState.PowerStatus.ToString(), newZoneState.PowerStatus.ToString(), _lastZoneChangeToON, _function.ZoneId));
                                onFunctionStart();
                            }
                            if (_function.OnSourceChange && (_zoneState.Source != newZoneState.Source))
                            {
                                // Source has changed, raise event ...
                                LogHelper.Log(LogLevel.Debug, String.Format("ConcreteZoneChangeFunction: Source changed from {0} to {1}, raise event! LastChangeToON={2}, Function={3}", _zoneState.Source.ToString(), newZoneState.Source.ToString(), _lastZoneChangeToON, _function.ZoneId));
                                onFunctionStart();
                            }
                            if (_function.OnVolumeChange && (_zoneState.Volume != newZoneState.Volume))
                            {
                                // Volume has changed, raise event ...
                                LogHelper.Log(LogLevel.Debug, String.Format("ConcreteZoneChangeFunction: Volume changed from {0} to {1}, raise event! LastChangeToON={2}, Function={3}", _zoneState.Volume.ToString(), newZoneState.Volume.ToString(), _lastZoneChangeToON, _function.ZoneId));
                                onFunctionStart();
                            }
                            if (_function.OnQualityChange && (_zoneState.ZoneQuality != newZoneState.ZoneQuality))
                            {
                                // Zone quality has changed, raise event ...
                                LogHelper.Log(LogLevel.Debug, String.Format("ConcreteZoneChangeFunction: Zone quality changed from {0} to {1}, raise event! LastChangeToON={2}, Function={3}", _zoneState.ZoneQuality.ToString(), newZoneState.ZoneQuality.ToString(), _lastZoneChangeToON, _function.ZoneId));
                                onFunctionStart();
                            }
                        }
                        _zoneState = new ZoneState(newZoneState);
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
                    foreach (ZoneState newZoneState in _newZoneState)
                    {
                        _zoneState = new ZoneState(newZoneState);
                    }
                }
                // Clear list, independent of validity (means, remove status changes which arrive during invalid period)
                _newZoneState.Clear();
            }
        }


        /// <summary>
        /// Method to dispose the object.
        /// </summary>
        public new void Dispose()
        {
            unsubscribeZone(_function.ZoneId);
            base.Dispose();
        }

    }
}

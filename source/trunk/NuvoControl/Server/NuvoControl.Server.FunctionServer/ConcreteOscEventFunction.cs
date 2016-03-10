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
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ZoneServer;
using NuvoControl.Server.ProtocolDriver.Interface;




namespace NuvoControl.Server.FunctionServer
{
    class ConcreteOscEventFunction : ConcreteFunction, IDisposable
    {
        /// <summary>
        /// Private member to hold the configuration data for the osc event function
        /// </summary>
        OscEventFunction _function;

        Dictionary<int, IOscDriver> _oscDrivers;


        /// <summary>
        /// Constructor to instantiate a concrete zone change function.
        /// </summary>
        /// <param name="function">Configuration data for this zone change function.</param>
        /// <param name="zoneServer">Zone server, to get notification about zone changes.</param>
        /// <param name="audioDrivers">Audio Drivers, in case a sound needs to be played.</param>
        public ConcreteOscEventFunction(OscEventFunction function, IZoneServer zoneServer, Dictionary<int, IAudioDriver> audioDrivers, Dictionary<int, IOscDriver> oscDrivers)
            : base(zoneServer, function, audioDrivers)
        {
            _oscDrivers = oscDrivers;

            if (function == null)
            {
                onFunctionError();
                throw new FunctionServerException("Function configuration is null. This is not allowed");
            }
            _function = function;
            _oscDrivers[_function.OscDevice.ObjectId].onOscEventReceived += new OscEventReceivedEventHandler(ConcreteOscFunction_onOscEventReceived);
        }


        /// <summary>
        /// Method invoked to notify the function about zone state changes.
        /// </summary>
        /// <param name="e">Event argument, passed by the notification event.</param>
        protected override void notifyOnZoneUpdate(ZoneServer.ZoneStateEventArgs e)
        {
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
        }


        /// <summary>
        /// Method to dispose the object.
        /// </summary>
        public new void Dispose()
        {
            //TODO: Unsubscribe from osc events
            base.Dispose();
        }


        void ConcreteOscFunction_onOscEventReceived(object sender, OscEventReceivedEventArgs e)
        {
            if ((String.Compare(_function.OscLabel, e.OscEvent.OscLabel) == 0) && (String.Compare(_function.OscEvent,e.OscEvent.OscCommand.ToString())==0))
            {
                LogHelper.Log(LogLevel.Info, String.Format("---   [{0}]  Device={1} OscEvent:{2}/Value={3}", DateTime.Now.ToShortTimeString(), e.OscDevice, (e.OscEvent == null ? "<null>" : e.OscEvent.ToString()), e.OscValue));
                onFunctionStart();
            }
        }

    }
}

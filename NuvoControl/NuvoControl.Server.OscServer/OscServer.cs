/**************************************************************************************************
 * 
 *   Copyright (C) 2016 by Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server
 * 
 **************************************************************************************************/

using Common.Logging;
using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;


namespace NuvoControl.Server.OscServer
{
    /// <summary>
    /// Implements the interface of the osc server.
    /// 
    /// This server contains an image of state of the connected osc devices.
    /// It allows to command the devices and to retrieve state of osc devices.
    /// Typically, the zone server is instantiated once per Nuvo Control server.
    /// </summary>
    public class OscServer
    {

        #region Fields

        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Holds the osc device controller per Id
        /// </summary>
        private Dictionary<Address, OscDeviceController> _oscDeviceControllers = new Dictionary<Address, OscDeviceController>();

        /// <summary>
        /// Private member to store last update of the osc devices
        /// </summary>
        private Dictionary<IPAddress, DateTime> _oscDeviceLastUpdate = new Dictionary<IPAddress, DateTime>();

        /// <summary>
        /// Private member to hold the timer used to periodically update (blink) the server status LED
        /// </summary>
        private System.Timers.Timer _timerUpdateServerStatus = new System.Timers.Timer();

        private int adhocDeviceIdCounter = 999;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="oscDeviceControllers">All osc device controllers.</param>
        public OscServer(List<OscDeviceController> oscDeviceControllers)
        {
            foreach (OscDeviceController oscDeviceController in oscDeviceControllers)
            {
                _oscDeviceControllers[oscDeviceController.OscDeviceId] = oscDeviceController;
                _oscDeviceControllers[oscDeviceController.OscDeviceId].GetOscDriver().onOscEventReceived += OscServer_onOscEventReceived;
                _oscDeviceControllers[oscDeviceController.OscDeviceId].GetOscDriver().onOscNuvoEventReceived += OscServer_onOscNuvoEventReceived;
                _oscDeviceLastUpdate[oscDeviceController.OscDevice.IpAddress] = new DateTime(1970, 1, 1);    // set to a default time
            }
        }

        #endregion


        public void Start()
        {
            _log.Trace(m => m(String.Format("OscServer: Start ...")));
            foreach (OscDeviceController oscDeviceController in _oscDeviceControllers.Values )
            {
                oscDeviceController.GetOscDriver().SendMessage("/NuvoControl/message", String.Format("Start ... {0}", oscDeviceController.OscDeviceId));
                oscDeviceController.GetOscDriver().SendMessage("/NuvoControl/ServerStatus", 1.0);
                oscDeviceController.GetOscDriver().SendMessage("/NuvoControl/NuvoStatus", 0.25);
            }
            StartTime();
        }

        public void Stop()
        {
            _log.Trace(m => m(String.Format("OscServer: Stop ...")));
            foreach (OscDeviceController oscDeviceController in _oscDeviceControllers.Values)
            {
                oscDeviceController.GetOscDriver().SendMessage("/NuvoControl/message", String.Format("Stop ... {0}", oscDeviceController.OscDeviceId));
                oscDeviceController.GetOscDriver().SendMessage("/NuvoControl/ServerStatus", 0.0);
                oscDeviceController.GetOscDriver().SendMessage("/NuvoControl/NuvoStatus", 0.0);
            }
        }



        #region Timer: Update Server Status LED

        private void StartTime()
        {
            LogHelper.Log(LogLevel.Debug, String.Format("Renew play sound command, each {0}[min]", Properties.Settings.Default.UpdateServerStatusLEDIntervall));
            _timerUpdateServerStatus.Interval = (Properties.Settings.Default.UpdateServerStatusLEDIntervall < 2 ? 2 : Properties.Settings.Default.UpdateServerStatusLEDIntervall) * 1000;
            _timerUpdateServerStatus.Elapsed += new System.Timers.ElapsedEventHandler(_timerUpdateServerStatus_Elapsed);
            _timerUpdateServerStatus.Start();
        }

        /// <summary>
        /// Periodic timer routine to update server status (LED)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _timerUpdateServerStatus_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _log.Trace(m => m(String.Format("OscServer: Server Status ... ")));
            lock (_oscDeviceControllers)
            {
                foreach (OscDeviceController oscDeviceController in _oscDeviceControllers.Values)
                {
                    oscDeviceController.GetOscDriver().SendMessage("/NuvoControl/message", String.Format("Ping from {0} at {1}", EnvironmentHelper.getHostName(), DateTime.Now));
                    oscDeviceController.GetOscDriver().SendMessage("/NuvoControl/ServerStatus", 0.25);
                    Thread.Sleep(500);
                    oscDeviceController.GetOscDriver().SendMessage("/NuvoControl/ServerStatus", 1.0);
                }
            }
        }

        #endregion



        private static int sOscMessagesReceivedCount = 0;

        /// <summary>
        /// Event method, to receive osc messages from osc devices.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OscServer_onOscEventReceived(object sender, ProtocolDriver.Interface.OscEventReceivedEventArgs e)
        {
            sOscMessagesReceivedCount++;
            _log.Trace(m => m("OSCS: Device (id={0}) osc event ({1}) from {2}: {3}", e.OscDevice, sOscMessagesReceivedCount, e.SourceEndPoint, e.OscEvent.ToString()));
            _oscDeviceLastUpdate[e.SourceEndPoint.Address] = DateTime.Now;
        }


        private static int sNuvoMessagesReceivedCount = 0;

        /// <summary>
        /// Event method, to receive nuvo control messages from osc devices.
        /// This method sends updates to all connected clients.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OscServer_onOscNuvoEventReceived(object sender, ProtocolDriver.Interface.OscEventReceivedEventArgs e)
        {
            sNuvoMessagesReceivedCount++;
            _log.Trace(m => m("OSCS: Device (id={0}) nuvo control event ({1}) from {2}: {3}", e.OscDevice, sNuvoMessagesReceivedCount, e.SourceEndPoint, e.OscEvent.ToString()));
            _oscDeviceLastUpdate[e.SourceEndPoint.Address] = DateTime.Now;

            bool bKnown = false;
            foreach (OscDeviceController oscDeviceController in _oscDeviceControllers.Values)
            {
                // forward message to clients
                oscDeviceController.processOscNuvoEventForClients(new Address(e.OscDeviceId, e.OscEvent.getZoneId), e.OscEvent);
                // forward (debug) message to clients
                //oscDeviceController.GetOscDriver().SendMessage("/NuvoControl/message", String.Format("Notify {0}/{1}: {2}", sMessagesReceivedCount, oscDeviceController.OscDeviceId, e.OscEvent.ToString()));
                // Check if client is "known"
                if( String.Compare(oscDeviceController.OscDevice.IpAddress.ToString(), e.SourceEndPoint.Address.ToString()) == 0 )
                {
                    bKnown = true;
                }
            }

            // If client was/is not known ...
            if (!bKnown)
            {
                // ... create "ad-hoc" controller, to start updating this client, assuming the client listen to port 9000
                _log.Trace(m => m("OSCS.onOscNuvoEventReceived: Client {0} not known, add with id={1}!", e.SourceEndPoint, adhocDeviceIdCounter));
                lock (_oscDeviceControllers)
                {
                    Address address = new Address(e.OscDeviceId, adhocDeviceIdCounter);
                    OSCDevice oscDevice = new OSCDevice(address, eOSCDeviceType.OSCClient, address, String.Format("AdHocClient{0}", adhocDeviceIdCounter), e.SourceEndPoint.Address, 8000, 9000, null);
                    OscDeviceController adhocOscDeviceController = new OscDeviceController(address, oscDevice, _oscDeviceControllers[e.OscDevice]);
                    _oscDeviceControllers.Add(address, adhocOscDeviceController);
                    adhocDeviceIdCounter--;
                }
            }
        }


    }
}

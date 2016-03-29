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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common.Logging;

using NuvoControl.Common.Configuration;
using NuvoControl.Server.ProtocolDriver;


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
                _oscDeviceControllers[oscDeviceController.OscDeviceId].GetOscDriver().onOscNuvoEventReceived += OscServer_onOscNuvoEventReceived;
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
            }
        }

        public void Stop()
        {
            _log.Trace(m => m(String.Format("OscServer: Stop ...")));
            foreach (OscDeviceController oscDeviceController in _oscDeviceControllers.Values)
            {
                oscDeviceController.GetOscDriver().SendMessage("/NuvoControl/message", String.Format("Stop ... {0}", oscDeviceController.OscDeviceId));
                oscDeviceController.GetOscDriver().SendMessage("/NuvoControl/ServerStatus", 0.0);
            }
        }


        private static int sMessagesReceivedCount = 0;

        /// <summary>
        /// Event method, to receive nuvo control messages from osc devices.
        /// This method sends updates to all connected clients.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OscServer_onOscNuvoEventReceived(object sender, ProtocolDriver.Interface.OscEventReceivedEventArgs e)
        {
            _log.Trace(m => m("OSCS.onOscNuvoEventReceived: Osc Device (with id {0}) osc event received from {1}: {2}", e.OscDevice, e.SourceEndPoint, e.OscEvent.ToString()));
            sMessagesReceivedCount++;

            bool bKnown = false;
            foreach (OscDeviceController oscDeviceController in _oscDeviceControllers.Values)
            {
                // forward message to known clients
                oscDeviceController.GetOscDriver().SendMessage(e.OscEvent);
                // forward (debug) message to known clients
                oscDeviceController.GetOscDriver().SendMessage("/NuvoControl/message", String.Format("Notify {0}/{1}: {2}", sMessagesReceivedCount, oscDeviceController.OscDeviceId, e.OscEvent.ToString()));
                // Check if client is "known"
                if( String.Compare(oscDeviceController.OscDevice.IpAddress.ToString(), e.SourceEndPoint.Address.ToString()) == 0 )
                {
                    bKnown = true;
                }
            }

            // If not known ...
            if (!bKnown)
            {
                // ... create "ad-hoc" controller, to start updating this client, assuming the client listen to port 9000
                _log.Trace(m => m("OSCS.onOscNuvoEventReceived: Client {0} not known, add with id={1}!", e.SourceEndPoint, adhocDeviceIdCounter));
                Address address = new Address(e.OscDeviceId, adhocDeviceIdCounter);
                OSCDevice oscDevice = new OSCDevice(address, eOSCDeviceType.OSCClient, address, String.Format("AdHocClient{0}",adhocDeviceIdCounter), e.SourceEndPoint.Address, 8000, 9000, null);
                OscDeviceController adhocOscDeviceController = new OscDeviceController(address, 
                    oscDevice, 
                    _oscDeviceControllers[e.OscDevice].GetProtocolDriver(),
                    new TouchOscDriver(oscDevice) );
                _oscDeviceControllers.Add(address, adhocOscDeviceController);
                adhocDeviceIdCounter--;
            }
        }


    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2016 by Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.HostConsole
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Common.Logging;

using Bespoke.Common;
using Bespoke.Common.Osc;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ProtocolDriver.Interface;



namespace NuvoControl.Server.ProtocolDriver
{
    /// <summary>
    /// This class implements the driver to support messages from/to OSCTouch.
    /// 
    /// Full list of controls see:
    /// http://hexler.net/docs/touchosc-controls-reference
    ///
    /// Example of two default layouts:
    /// 
    /// Automat5:
    ///- Push Button: --
    ///- Toggle Button: /toggle
    ///- XY Pad: /xy
    ///- Fader: /fader
    ///- Rotary: /rotary
    ///- Encoder: /encoder
    ///- Multi-Toggle: /multitoggle
    ///- Multi-XY: --
    ///- Multi-Push: /multipush
    ///- Multi-Fader: /multifader 
    ///- 3 tabs
    ///
    ///BeatMachine:
    ///- Push Button: /push
    ///- Toggle Button: /toggle
    ///- XY Pad: /xy
    ///- Fader: /fader
    ///- Rotary: /rotary
    ///- Encoder: --
    ///- Multi-Toggle: /multitoggle
    ///- Multi-XY: --
    ///- Multi-Push: --
    ///- Multi-Fader: /multifader
    ///- 4 tabs
    ///
    /// </summary>
    public class TouchOscDriver : IOscDriver
    {
        public event OscEventReceivedEventHandler onOscEventReceived;

        private OSCDevice _oscDevice;
        private OscServer _oscServer;

        private int _sendMsgCounter = 0;

        public TouchOscDriver(OSCDevice oscDevice)
        {
            _oscDevice = oscDevice;
            if (_oscDevice.DeviceType == eOSCDeviceType.OSCServer)
            {
                _oscServer = new OscServer(TransportType.Udp, oscDevice.IpAddress, oscDevice.ListenPort, true, false);
            }
            else if( _oscDevice.DeviceType == eOSCDeviceType.OSCClient)
            {
                sendMessage("OSC Server started...", oscDevice.IpAddress, oscDevice.SendPort);
            }
        }

        public void Start()
        {
            if (_oscServer != null)
            {
                _oscServer.BundleReceived += new EventHandler<OscBundleReceivedEventArgs>(oscServer_BundleReceived);
                _oscServer.MessageReceived += new EventHandler<OscMessageReceivedEventArgs>(oscServer_MessageReceived);
                _oscServer.ReceiveErrored += new EventHandler<ExceptionEventArgs>(oscServer_ReceiveErrored);
                _oscServer.Start();
            }
        }

        public void Stop()
        {
            if (_oscDevice.DeviceType == eOSCDeviceType.OSCServer && _oscServer != null)
            {
                _oscServer.Stop();
                _oscDevice = null;
            }
            else if (_oscDevice.DeviceType == eOSCDeviceType.OSCClient)
            {
                sendMessage("OSC Server stopped...", _oscDevice.IpAddress, _oscDevice.SendPort);
            }
        }


        #region OSC Server Events

        private static int sBundlesReceivedCount = 0;
        private static int sMessagesReceivedCount = 0;

        private void oscServer_BundleReceived(object sender, OscBundleReceivedEventArgs e)
        {
            sBundlesReceivedCount++;
            OscBundle bundle = e.Bundle;
            LogHelper.Log(LogLevel.Info, string.Format("[OSC] Bundle Rcv [{0}:{1}]: Nested Bundles: {2} Nested Messages: {3} [{4}]", bundle.SourceEndPoint.Address, bundle.TimeStamp, bundle.Bundles.Count, bundle.Messages.Count, sBundlesReceivedCount));
        }

        private void oscServer_MessageReceived(object sender, OscMessageReceivedEventArgs e)
        {
            sMessagesReceivedCount++;
            LogHelper.Log(LogLevel.Info, string.Format("[OSC] Msg Rcv [{0}]: {1} / Message contains {2} objects. [{3}]", e.Message.SourceEndPoint.Address, e.Message.Address, e.Message.Data.Count, sMessagesReceivedCount));
            for (int i = 0; i < e.Message.Data.Count; i++)
            {
                LogHelper.Log(LogLevel.Debug, string.Format("[OSC] {0}: Value={1}", i, convertDataString(e.Message.Data[i])));
            }

            OscEvent oscEvent = processTouchOscMessageForNuvoControl(e.Message);
            if (oscEvent != null)
            {
                LogHelper.Log(LogLevel.Info, string.Format("[OSC] NuvoControl OscEvent={0}", oscEvent.ToString()));
                //raise the event, and pass data to next layer
                if (onOscEventReceived != null)
                {
                    onOscEventReceived(this, new OscEventReceivedEventArgs(_oscDevice.DeviceId, oscEvent));
                }
            }
            else
            { 
                oscEvent = processTouchOscMessageFromDefaultLayouts(e.Message);
                if (oscEvent != null)
                {
                    LogHelper.Log(LogLevel.Info, string.Format("[OSC] OscEvent={0}", oscEvent.ToString()));
                    //raise the event, and pass data to next layer
                    if (onOscEventReceived != null)
                    {
                        onOscEventReceived(this, new OscEventReceivedEventArgs(_oscDevice.DeviceId, oscEvent));
                    }
                }
                else
                {
                    LogHelper.Log(LogLevel.Warn, string.Format("[OSC] Unknown message: {0}", e.Message.Address));
                }
            }
        }

        private void oscServer_ReceiveErrored(object sender, ExceptionEventArgs e)
        {
            LogHelper.Log(LogLevel.Error, string.Format("[OSC] Error during reception of osc packet: {0}", e.Exception.Message));
        }

        /// <summary>
        /// Method to process messages send for specific NuvoControl layout
        /// </summary>
        /// <param name="message">Message from TouchOSC</param>
        /// <returns>OSC event class, or null if not known</returns>
        private OscEvent processTouchOscMessageForNuvoControl(OscMessage message)
        {

            if (String.Compare(message.Address, "/ping") == 0)
            {
                return new OscEvent(OscEvent.eOscEvent.Ping, message.Address);
            }
            if (message.Address.IndexOf("/NuvoControl") == 0)
            {
                sendMessage(String.Format("{0}", message.Address), message.SourceEndPoint.Address, _oscDevice.SendPort);
                return new OscEvent(OscEvent.eOscEvent.NuvoControl, message.Address, double.Parse(convertDataString(message.Data[0])));
            }

            return null;
        }

        /// <summary>
        /// Send message to osc device to address default "/NuvoControl.Control/message"
        /// </summary>
        /// <param name="strMessage">Message to send.</param>
        /// <param name="ipAddress">IP address of the osc device.</param>
        /// <param name="port">Port of the osc device.</param>
        private void sendMessage( string strMessage, IPAddress ipAddress, int port )
        {
            _sendMsgCounter++;
            IPEndPoint sourceEndPoint = new IPEndPoint(ipAddress, port);
            OscMessage labelMessage = new OscMessage(sourceEndPoint, "/NuvoControl.Control/message", String.Format("{0} [{1}]", strMessage, _sendMsgCounter));
            labelMessage.Send(sourceEndPoint);
        }

        /// <summary>
        /// Method to process messages send from default layouts (out-of-the-box)
        /// </summary>
        /// <param name="message">Message from TouchOSC</param>
        /// <returns>OSC event class, or null if not known</returns>
        private OscEvent processTouchOscMessageFromDefaultLayouts(OscMessage message)
        {
            if( String.Compare(message.Address,"/ping")==0)
            {
                return new OscEvent( OscEvent.eOscEvent.Ping, message.Address );
            }
            if (message.Address.IndexOf("toggle") > 0)
            {
                return new OscEvent((int.Parse(convertDataString(message.Data[0])) == 1 ? OscEvent.eOscEvent.SwitchOn : OscEvent.eOscEvent.SwitchOff), message.Address);
            }
            if (message.Address.IndexOf("push") > 0)
            {
                return new OscEvent((int.Parse(convertDataString(message.Data[0])) == 1 ? OscEvent.eOscEvent.SwitchOn : OscEvent.eOscEvent.SwitchOff), message.Address);
            }
            if (message.Address.IndexOf("fader") > 0)
            {
                return new OscEvent(OscEvent.eOscEvent.SetValue, message.Address, double.Parse(convertDataString(message.Data[0])));
            }
            if (message.Address.IndexOf("rotary") > 0)
            {
                return new OscEvent(OscEvent.eOscEvent.SetValue, message.Address, double.Parse(convertDataString(message.Data[0])));
            }
            if (message.Address.IndexOf("xy") > 0)
            {
                return new OscEvent(OscEvent.eOscEvent.SetValues, message.Address, double.Parse(convertDataString(message.Data[0])), double.Parse(convertDataString(message.Data[1])));
            }
            if (message.Address.IndexOf("encoder") > 0)
            {
                return new OscEvent((int.Parse(convertDataString(message.Data[0])) == 1 ? OscEvent.eOscEvent.ValueUp : (int.Parse(convertDataString(message.Data[0])) == 0 ? OscEvent.eOscEvent.ValueDown : OscEvent.eOscEvent.SetValue)), message.Address, int.Parse(convertDataString(message.Data[0])));
            }

            // check for tab changes
            for (int i = 0; i < 15; i++ )
            {
                if (String.Compare(message.Address, String.Format("/{0}",i)) == 0)
                {
                    return new OscEvent(OscEvent.eOscEvent.TabChange, message.Address, i);
                }
            }

            return null;
        }

        /// <summary>
        /// Converts message data to string.
        /// </summary>
        /// <param name="data">Data to convert.</param>
        /// <returns>Converted data as string.</returns>
        private string convertDataString(object data)
        {
            string dataString;

            if (data == null)
            {
                dataString = "Nil";
            }
            else
            {
                dataString = (data is byte[] ? BitConverter.ToString((byte[])data) : data.ToString());
            }

            return dataString;
        }

        #endregion



        public void RegisterMethod(OscEvent oscEvent)
        {
            throw new NotImplementedException();
        }

    }
}

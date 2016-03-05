/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.05.2009
 *   File Name:      Device.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 12.05.2009, Bernhard Limacher: First implementation.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NuvoControl.Common.Configuration
{
    /// <summary>
    /// This is a system configuration class. 
    /// 
    /// It is a data structurer.
    /// It defines attributes of a device. E.g. the NuvoEssentia.
    /// </summary>
    public class Device
    {
        #region Private Members

        /// <summary>
        /// The id of the device.
        /// </summary>
        private int _id = SystemConfiguration.ID_UNDEFINED;

        /// <summary>
        /// The communication parameters of the link to the device.
        /// </summary>
        Communication _communication = null;

        /// <summary>
        /// The protocol driver to be used to communicate with the device.
        /// </summary>
        private Protocol _protocolDriver = null;

        /// <summary>
        /// All zones of the device.
        /// </summary>
        private List<Zone> _zones = new List<Zone>();

        /// <summary>
        /// All sources of the device.
        /// </summary>
        private List<Source> _sources = new List<Source>();

        /// <summary>
        /// All audio devices of the device.
        /// </summary>
        private List<AudioDevice> _audioDevices = new List<AudioDevice>();

        /// <summary>
        /// All OSC devices of the device
        /// </summary>
        private List<OSCDevice> _oscDevices = new List<OSCDevice>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">The id of the device.</param>
        /// <param name="communication">The communication parameters of the link to the device.</param>
        /// <param name="protocolDriver">The protocol driver to be used to communicate with the device.</param>
        /// <param name="zones">All zones of the device.</param>
        /// <param name="sources">All sources of the device.</param>
        /// <param name="audioDevices">All audio devices of the device.</param>
        public Device(int id, Communication communication, Protocol protocolDriver, List<Zone> zones, List<Source> sources, List<AudioDevice> audioDevices, List<OSCDevice> oscDevices)
        {
            _id = id;
            _communication = communication;
            _protocolDriver = protocolDriver;
            _zones = zones;
            _sources = sources;
            _audioDevices = audioDevices;
            _oscDevices = oscDevices;
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// The id of the device.
        /// </summary>
        public int Id
        {
            get { return _id; }
        }

        /// <summary>
        /// The communication parameters of the link to the device.
        /// </summary>
        public Communication Communication
        {
            get { return _communication; }
        }

        /// <summary>
        /// The protocol driver to be used to communicate with the device.
        /// </summary>
        public Protocol ProtocolDriver
        {
            get { return _protocolDriver; }
        }

        /// <summary>
        /// All zones of the device.
        /// </summary>
        public List<Zone> Zones
        {
            get { return _zones; }
        }

        /// <summary>
        /// All sources of the device.
        /// </summary>
        public List<Source> Sources
        {
            get { return _sources; }
        }

        /// <summary>
        /// All audio devices of the device.
        /// </summary>
        public List<AudioDevice> AudioDevices
        {
            get { return _audioDevices; }
            set { _audioDevices = value; }
        }

        #endregion

        /// <summary>
        /// Returns string representative of this class.
        /// </summary>
        /// <returns>String representative of this class.</returns>
        public override string ToString()
        {
            string strDevice = "";

            strDevice += String.Format("Id={0} /", _id);
            strDevice += String.Format("Communication=[{0}] /", _communication.ToString());
            strDevice += String.Format("Protocol Driver=[{0}] /", _protocolDriver.ToString());

            // all Zones ...
            strDevice += String.Format("Zones=[");
            foreach (Zone zone in _zones)
            {
                strDevice += String.Format("Zone={0}, ", zone.ToString());
            }
            strDevice += String.Format("]");

            // all Sources ...
            strDevice += String.Format("Sources=[");
            foreach (Source source in _sources)
            {
                strDevice += String.Format("Source={0}, ", source.ToString());
            }
            strDevice += String.Format("]");

            // all Audio Devices ...
            if (_audioDevices != null)
            {
                strDevice += String.Format("AudioDevices=[");
                foreach (AudioDevice device in _audioDevices)
                {
                    strDevice += String.Format("Audio={0}, ", device.ToString());
                }
                strDevice += String.Format("]");
            }

            // all OSC Devices ...
            if (_oscDevices != null)
            { 
                strDevice += String.Format("OSCDevices=[");
                foreach (OSCDevice device in _oscDevices)
                {
                    strDevice += String.Format("OSC={0}, ", device.ToString());
                }
                strDevice += String.Format("]");
            }

            return strDevice;
        }

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/

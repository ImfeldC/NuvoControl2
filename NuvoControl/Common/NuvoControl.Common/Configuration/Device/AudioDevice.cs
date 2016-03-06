/**************************************************************************************************
 * 
 *   Copyright (C) 2016 by Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common
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
    /// It defines an audio device.
    /// </summary>
    [DataContract]
    public class AudioDevice
    {
        #region Private Members

        /// <summary>
        /// The address of the audio device.
        /// </summary>
        [DataMember]
        private Address _id = new Address(SystemConfiguration.ID_UNDEFINED, SystemConfiguration.ID_UNDEFINED);

        private string _name = "";

        private Address _sourceId = new Address();

        private string _player = "";

        private string _deviceType = "";

        private string _device = "";

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public AudioDevice()
        {
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="id">Id of this audio device.</param>
        /// <param name="sourceId">Source where to play the sound.</param>
        /// <param name="name">Name of the audio device.</param>
        /// <param name="player">Player (command, process, ...) to use for this audio device (e.g. "mpg321")</param>
        /// <param name="deviceType">Device type, where to play the sound.</param>
        /// <param name="device">Device, where to play the sound.</param>
        public AudioDevice(Address id, Address sourceId, string name, string player, string deviceType, string device)
        {
            _id = id;
            _sourceId = sourceId;
            _name = name;
            _player = player;
            _deviceType = deviceType;
            _device = device;
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// The address of the audio device.
        /// </summary>
        public Address Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Address SourceId
        {
            get { return _sourceId; }
            set { _sourceId = value; }
        }

        public string Player
        {
            get { return _player; }
            set { _player = value; }
        }

        public string DeviceType
        {
            get { return _deviceType; }
            set { _deviceType = value; }
        }

        public string Device
        {
            get { return _device; }
            set { _device = value; }
        }

        #endregion

        /// <summary>
        /// Returns string representative of this class.
        /// </summary>
        /// <returns>String representative of this class.</returns>
        public override string ToString()
        {
            return String.Format("Name={0} Source={1} Player={2}", _name, _sourceId.ToString(), _player);
        }

    }
}

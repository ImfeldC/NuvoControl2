/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
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
        private List<int> _zones = new List<int>();

        /// <summary>
        /// All sources of the device.
        /// </summary>
        private List<int> _sources = new List<int>();

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
        public Device(int id, Communication communication, Protocol protocolDriver, List<int> zones, List<int> sources)
        {
            this._id = id;
            this._communication = communication;
            this._protocolDriver = protocolDriver;
            this._zones = zones;
            this._sources = sources;
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
        public List<int> Zones
        {
            get { return _zones; }
        }

        /// <summary>
        /// All sources of the device.
        /// </summary>
        public List<int> Sources
        {
            get { return _sources; }
        }

        #endregion

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/

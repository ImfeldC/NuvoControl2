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
 *   File Name:      Hardware.cs
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
    /// It defines attibutes of the connected devices.
    /// </summary>
    public class Hardware
    {
        #region Private Members

        /// <summary>
        /// All connected devices.
        /// </summary>
        private List<Device> _devices = new List<Device>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="devices">The connected devices.</param>
        public Hardware(List<Device> devices)
        {
            this._devices = devices;
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// The connected devices.
        /// </summary>
        public List<Device> Devices
        {
            get { return _devices; }
        }

        /// <summary>
        /// Returns the device related to the address
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>Device related to the passed address</returns>
        public Device getDevice(Address address)
        {
            foreach (Device device in _devices)
            {
                if (device.Id == address.DeviceId)
                {
                    return device;
                }
            }
            return null;
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/

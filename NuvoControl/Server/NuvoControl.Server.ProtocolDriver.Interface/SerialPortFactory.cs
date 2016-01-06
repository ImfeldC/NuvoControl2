/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.ProtocolDriver.Interface
 *   Author:         Christian Imfeld
 *   Creation Date:  04.06.2009
 *   File Name:      SerialPortFactory.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 04.06.2009, Christian Imfeld: First implementation.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Common.Logging;

namespace NuvoControl.Server.ProtocolDriver.Interface
{
    /// <summary>
    /// This class provides a factory method to create the serial port drivers.
    /// </summary>
    public static class SerialPortFactory
    {
        #region Fields

        #region Common Logger
        /// <summary>
        /// Common logger object. Requires the using directive <c>Common.Logging</c>. See 
        /// <see cref="LogManager"/> for more information.
        /// </summary>
        private static ILog _log = LogManager.GetCurrentClassLogger();
        #endregion

        #endregion

        #region Public Interface

        /// <summary>
        /// Creates a serial port driver, based on the specified parameters.
        /// </summary>
        /// <param name="assemblyName">The assebly name containing the class specified by the second parameter.</param>
        /// <param name="className">The full specified class name, which implements *ISerialPort</param>
        /// <returns>The interface of the serial port driver or null, if the driver could not be loaded.</returns>
        public static ISerialPort LoadDriver(string assemblyName, string className)
        {
            try
            {
                return Activator.CreateInstance(assemblyName, className).Unwrap() as ISerialPort;
            }
            catch (Exception exc)
            {
                _log.Fatal("Serial Port driver could not be loaded.", exc);
                return null;
            }
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/

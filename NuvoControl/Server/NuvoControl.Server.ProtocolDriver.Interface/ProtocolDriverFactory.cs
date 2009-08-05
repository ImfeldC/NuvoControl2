/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.ProtocolDriver.Interface
 *   Author:         Bernhard Limacher
 *   Creation Date:  21.05.2009
 *   File Name:      ProtocolDriverFactory.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 21.05.2009, Bernhard Limacher: First implementation.
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
    /// This class provides a factory method to create the protocol drivers.
    /// </summary>
    public static class ProtocolDriverFactory
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
        /// Creates a protocol driver, based on the specified parameters.
        /// </summary>
        /// <param name="assemblyName">The assebly name containing the class specified by the second parameter.</param>
        /// <param name="className">The full specified class name, which implements IProtocol</param>
        /// <returns>The interface of the protocol driver or null, if the driver could not be loaded.</returns>
        public static IProtocol LoadDriver(string assemblyName, string className)
        {
            try
            {
                return Activator.CreateInstance(assemblyName, className).Unwrap() as IProtocol;
            }
            catch (Exception exc)
            {
                _log.Fatal("Protocol driver could not be loaded.", exc);
                return null;
            }
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/

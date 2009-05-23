/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common.Interfaces
 *   Author:         Bernhard Limacher
 *   Creation Date:  19.05.2009
 *   File Name:      INuvoControl.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 19.05.2009, Bernhard Limacher: Definition of the interface.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Common.Interfaces
{
    /// <summary>
    /// Defines the entry functions for the NuvoControl service
    /// </summary>
    public interface INuvoControl
    {
        /// <summary>
        /// Starts up the service. To be called once. Instantates all service objects.
        /// </summary>
        /// <param name="configurationFile">The Nuvo Control configuration.</param>
        void StartUp(string configurationFile);

        /// <summary>
        /// Shuts down the service.
        /// </summary>
        void ShutDown();

        /// <summary>
        /// Starts a session for a new client.
        /// </summary>
        /// <param name="clientId">The Id of the client.</param>
        /// <returns>True, in case of success. Otherwise false.</returns>
        bool StartSession(Guid clientId);

        /// <summary>
        /// Terminates a client session. All subscriptions of the client are deleted.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>True, in case of success. Otherwise false.</returns>
        bool EndSession(Guid clientId);

        /// <summary>
        /// Returns the <see cref="IConfigure"/> interface.
        /// This provides access to functionality related to the system configuration of the service.
        /// </summary>
        IConfigure IConfigure { get; }

        /// <summary>
        /// Returns the <see cref="IMonitorAndControl"/> interface.
        /// This provides access to functionality related to controlling and monitoring the state of the service with its zones
        /// </summary>
        IMonitorAndControl IMonitorAndControl { get; }
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/

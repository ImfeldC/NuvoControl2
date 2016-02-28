/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.ConfigurationService
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.05.2009
 *   File Name:      IConfigure.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 12.05.2009, Bernhard Limacher: Definition of the interface.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Common.Configuration;
using System.ServiceModel;

namespace NuvoControl.Server.ConfigurationService
{
    /// <summary>
    /// Defines functionality to read, modify and save the actual configuration of the NuvoControl system.
    /// </summary>
    [ServiceContract]
    public interface IConfigure
    {
        /// <summary>
        /// Renews the session lease time.
        /// </summary>
        [OperationContract]
        void RenewLease();

        /// <summary>
        /// Returns the graphical configuration of the Nuvo Control system configuration.
        /// Such data are floor plans, zone coordinates, source pictures, ....
        /// </summary>
        /// <returns>Graphical configuration.</returns>
        [OperationContract]
        Graphic GetGraphicConfiguration();

        /// <summary>
        /// Returns the underlying picture for the previous retrieved graphical configuration.
        /// </summary>
        /// <param name="picturePath">Path for the picture to retrieve.</param>
        /// <returns>Retreived picture.</returns>
        [OperationContract]
        NuvoImage GetImage(string picturePath);

        /// <summary>
        /// Returns the graphical configuration related to a zone.
        /// </summary>
        /// <param name="zoneId">The zone id.</param>
        /// <returns>Zone configuration.</returns>
        [OperationContract]
        ZoneGraphic GetZoneGraphicConfiguration(Address zoneId);

        /// <summary>
        /// Returns the specified function.
        /// </summary>
        /// <param name="id">The function id.</param>
        /// <returns>The specified function.</returns>
        [OperationContract]
        Function GetFunction(Guid id);

        /// <summary>
        /// Returns all functions which are applied to the specified zone.
        /// </summary>
        /// <param name="zoneId">The zone, for which functions shall be retrieved.</param>
        /// <returns>The related functions.</returns>
        [OperationContract]
        List<Function> GetFunctions(Address zoneId);

        /// <summary>
        /// Adds a new function to the system configuration.
        /// <b>The function is validated.</b>
        /// </summary>
        /// <param name="newFunction">The new function to add.</param>
        /// <returns>True, if the function is valid; otherwise false.</returns>
        [OperationContract]
        bool AddFunction(Function newFunction);
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/

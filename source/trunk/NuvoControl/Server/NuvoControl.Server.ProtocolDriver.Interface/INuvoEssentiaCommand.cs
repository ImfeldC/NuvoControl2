/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.ProtocolDriver.Interface
 *   Author:         Ch.Imfeld
 *   Creation Date:  6/12/2009 11:02:29 PM
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 6/12/2009 11:02:29 PM, Ch.Imfeld: Initial implementation.
 * 
 **************************************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Server.ProtocolDriver.Interface
{
    /// <summary>
    /// Specifies the interface for Nuvo Essentia commands.
    /// Either a single command or multiple commands combined in one group
    /// are handled by a class implementing this interface.
    /// </summary>
    public interface INuvoEssentiaCommand
    {
        /// <summary>
        /// Adds an additional new command in the container. 
        /// </summary>
        /// <param name="command">Command to add.</param>
        void addCommand(ENuvoEssentiaCommands command);

        /// <summary>
        /// Method to loop through the list of commands.
        /// The previous command is used as key to search for the next command.
        /// A null pointer is returned, if no command is available anymore.
        /// We don't return the whole list, because we may need to adapt the command
        /// because of values retruned by the previous command (e.g. in case of the 
        /// command SetVolume+2dB; this command needs first to read the current volume
        /// level, before the new volume level can be calculated).
        /// </summary>
        /// <param name="prevCommand">Command just before the next command.</param>
        /// <returns>Returns the command just after prevCommand.</returns>
        INuvoEssentiaSingleCommand getNextCommand(INuvoEssentiaSingleCommand prevCommand);
    }
}

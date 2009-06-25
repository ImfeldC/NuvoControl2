/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.ProtocolDriver
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

namespace NuvoControl.Server.ProtocolDriver
{
    /// <summary>
    /// Public exception class. Inherits from Exception.
    /// </summary>
    public class ProtocolDriverException : Exception
    {
        /// <summary>
        /// Public constructor for the protocol driver excpetion.
        /// A message is expected to describe the cause of the exception.
        /// </summary>
        /// <param name="message">Cause message of the exception.</param>
        public ProtocolDriverException(string message)
            : base(message)
        {
        }
    }
}

﻿/**************************************************************************************************
 * 
 *   Copyright (C) 2016 by Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.FunctionServer
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Common.Configuration;

namespace NuvoControl.Server.FunctionServer
{
    interface IConcreteCommand : IDisposable
    {
        /// <summary>
        /// Returns the underlying configuration data for this command object.
        /// </summary>
        Command Command { get; }

        bool onFunctionError { get; }

        bool onFunctionStart { get; }

        bool onFunctionEnd { get; }

        void execCommand(eCommandType cmdType, Function function);
    }
}

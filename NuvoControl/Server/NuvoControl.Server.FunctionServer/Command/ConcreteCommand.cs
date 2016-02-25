/**************************************************************************************************
 * 
 *   Copyright (C) 2016 by Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.Configuration
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;


namespace NuvoControl.Server.FunctionServer
{
    public abstract class ConcreteCommand : IConcreteCommand, IDisposable
    {

        private static ILog _log = LogManager.GetCurrentClassLogger();

        private Command _command = null;

        public ConcreteCommand(Command command)
        {
            _command = command;
        }



        public Command Command 
        {
            get
            {
                return _command;
            }
        }

        public bool onFunctionError 
        {
            get
            {
                return _command.OnFunctionError;
            }
        }


        public bool onFunctionStart 
        {
            get
            {
                return _command.OnFunctionStart;
            }
        }

        public bool onFunctionEnd 
        {
            get
            {
                return _command.OnFunctionEnd;
            }
        }


        public bool onValidityStart
        {
            get
            {
                return _command.OnValidityStart;
            }
        }

        public bool onValidityEnd
        {
            get
            {
                return _command.OnValidityEnd;
            }
        }


        /// <summary>
        /// Returns true, if this command type is set.
        /// </summary>
        /// <param name="cmdType">Command type to check (e.g. onFunctionStart)</param>
        /// <returns></returns>
        public bool checkCommandType(eCommandType cmdType)
        {
            if (cmdType==eCommandType.onFunctionError)
            {
                return onFunctionError;
            }
            else if (cmdType==eCommandType.onFunctionStart)
            {
                return onFunctionStart;
            }
            else if (cmdType==eCommandType.onFunctionEnd)
            {
                return onFunctionEnd;
            }
            else if (cmdType == eCommandType.onValidityStart)
            {
                return onValidityStart;
            }
            else if (cmdType == eCommandType.onValidityEnd)
            {
                return onValidityEnd;
            }
            return false;
        }

        public abstract void execCommand(eCommandType cmdType, Function function);

        /// <summary>
        /// Public override of ToString() method.
        /// </summary>
        /// <returns>Returns string representative.</returns>
        public override string ToString()
        {
            return String.Format("ConcreteCommand: {0}, OnError={2}, OnFuncStart={3}, OnFuncEnd={4}, OnValStart={5}, OnValEnd={6}, Id={1}",
                _command, _command.Id,
                (onFunctionError ? "Yes" : "No"), (onFunctionStart ? "Yes" : "No"), (onFunctionEnd ? "Yes" : "No"), (onValidityStart ? "Yes" : "No"), (onValidityEnd ? "Yes" : "No"));
        }


        public void Dispose()
        {
            // Dispose concrete command (base class)
        }
    }
}

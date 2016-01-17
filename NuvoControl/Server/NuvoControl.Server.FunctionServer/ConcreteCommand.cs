using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;


namespace NuvoControl.Server.FunctionServer
{
    public abstract class ConcreteCommand : IConcreteCommand
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
                return _command.OnFunctionEnd;
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
            return false;
        }

        public abstract void execCommand(eCommandType cmdType, Function function);

    }
}

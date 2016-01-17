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


        public abstract void execCommand(eCommandType cmdType);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;


namespace NuvoControl.Server.FunctionServer
{
    class ConcretePlaySoundCommand : ConcreteCommand
    {
        private PlaySoundCommand _playSoundCommand = null;

        public ConcretePlaySoundCommand(PlaySoundCommand command)
            :base( command )
        {
            _playSoundCommand = command;
        }

        public override void execCommand(eCommandType cmdType, Function function)
        {
            if (checkCommandType(cmdType))
            {
                LogHelper.Log(String.Format(">>> Execute PlaySound command on event {0}: PlayMailCommand={1} / Function={2}", cmdType, _playSoundCommand.ToString(), function.ToString()));
            }
        }

    }
}

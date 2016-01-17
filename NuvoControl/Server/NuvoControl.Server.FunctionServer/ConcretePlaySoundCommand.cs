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
        public ConcretePlaySoundCommand(PlaySoundCommand command)
            :base( command )
        {
        }

        public override void execCommand(eCommandType cmdType, Function function)
        {
            LogHelper.Log(String.Format("Execute PlaySound command on event {0}", cmdType));
        }

    }
}

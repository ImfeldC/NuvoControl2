using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;



namespace NuvoControl.Server.FunctionServer
{
    class ConcretePlaySoundCommand : ConcreteCommand
    {
        private PlaySoundCommand _playSoundCommand = null;
        private Process _process = null;

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

                if (EnvironmentHelper.isRunningOnLinux())
                {
                    playSoundOnUnix();
                }
                else
                {
                    playSound();
                }
            }

            if (cmdType == eCommandType.onFunctionEnd)
            {
                // Stop any sound ...
                // Close application
                if (_process != null)
                {
                    try
                    {
                        _process.Kill();
                    }
                    catch (System.InvalidOperationException exc)
                    {
                        //ignore any exception at shutdown
                    }
                }
            }
        }

        private void playSound()
        {
            // Example URI:
            // - http://www.tonycuffe.com/mp3/tail%20toddle.mp3
            // - http://www.tonycuffe.com/mp3/tailtoddle_lo.mp3
            // - http://www.tonycuffe.com/mp3/cairnomount.mp3
            // - http://drs3.radio.net/
            // - http://asx.skypro.ch/radio/internet-128/virus.asx

            Console.WriteLine("   Beep!");
        }

        private void playSoundOnUnix()
        {
            _process = EnvironmentHelper.run_cmd("/usr/bin/mpg321", _playSoundCommand.Url);
        }

    }
}

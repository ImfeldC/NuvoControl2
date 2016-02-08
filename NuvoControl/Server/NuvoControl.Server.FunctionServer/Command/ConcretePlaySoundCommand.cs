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
    class ConcretePlaySoundCommand : ConcreteCommand, IDisposable
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
            // onFunctionStart & onValidityStart configured to run ...
            if (checkCommandType(cmdType) && (cmdType == eCommandType.onFunctionStart | cmdType == eCommandType.onValidityStart) )
            {
                LogHelper.Log(LogLevel.Info, String.Format(">>> Execute PlaySound command on event {0}: PlaySoundCommand={1} / Function={2}", cmdType, _playSoundCommand.ToString(), function.ToString()));

                if (EnvironmentHelper.isRunningOnLinux())
                {
                    playSoundOnUnix();
                }
                else
                {
                    playSound();
                }
            }

            // onFunctionEnd & onValidityEnd configured to run ...
            if (checkCommandType(cmdType) && (cmdType == eCommandType.onFunctionEnd | cmdType == eCommandType.onValidityEnd) )
            {
                killProcess();
            }

            // onFunctionError configured to run ...
            // ... stop any running process in case of an error
            if (checkCommandType(cmdType) && cmdType == eCommandType.onFunctionError)
            {
                killProcess();
            }
        }

        /// <summary>
        /// Play sound on Windows platform
        /// </summary>
        private void playSound()
        {
            // Example URI:
            // - http://www.tonycuffe.com/mp3/tail%20toddle.mp3
            // - http://www.tonycuffe.com/mp3/tailtoddle_lo.mp3
            // - http://www.tonycuffe.com/mp3/cairnomount.mp3
            // - http://drs3.radio.net/
            // - http://asx.skypro.ch/radio/internet-128/virus.asx

            Console.WriteLine("   Beep!");
            LogHelper.Log(LogLevel.Info, String.Format("    ... play sound on Windows! Process={0}", (_process != null ? _process.ToString() : "(null)")));
        }

        /// <summary>
        /// Play sound on Unix platform
        /// </summary>
        private void playSoundOnUnix()
        {
            killProcess();
            _process = EnvironmentHelper.run_cmd("/usr/bin/mpg321", _playSoundCommand.Url);
            LogHelper.Log(LogLevel.Info, String.Format("    ... play sound on Unix! Process={0}", (_process != null ? _process.ToString() : "(null)") ));
        }

        /// <summary>
        /// Kill process
        /// </summary>
        private void killProcess()
        {
            // Stop any sound ...
            // Close application
            if (_process != null)
            {
                try
                {
                    _process.Kill();
                    _process = null;
                }
                catch (System.InvalidOperationException exc)
                {
                    //ignore any exception at shutdown
                    LogHelper.LogException("Cannot kill process in 'killProcess' method!", exc);
                }
            }
        }

        public void Dispose()
        {
            // kill any process, before shutdown
            killProcess();
            base.Dispose();
        }
    }
}

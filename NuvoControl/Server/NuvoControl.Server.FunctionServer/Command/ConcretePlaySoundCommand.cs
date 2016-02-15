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
        #region Private Fields

        private bool _isSoundPlaying = false;
        private PlaySoundCommand _playSoundCommand = null;
        private Process _process = null;

        /// <summary>
        /// Private member to hold the timer used to periodically renew the play sound command
        /// </summary>
        private System.Timers.Timer _timerRenewPlaySoundCommand = new System.Timers.Timer();

        #endregion

        public ConcretePlaySoundCommand(PlaySoundCommand command)
            :base( command )
        {
            _playSoundCommand = command;
            StartTime();
        }

        /// <summary>
        /// Method called for execCommand
        /// </summary>
        /// <param name="cmdType">Command type.</param>
        /// <param name="function">Related fucntion.</param>
        public override void execCommand(eCommandType cmdType, Function function)
        {
            // onFunctionStart & onValidityStart configured to run ...
            if (checkCommandType(cmdType) && (cmdType == eCommandType.onFunctionStart | cmdType == eCommandType.onValidityStart) )
            {
                LogHelper.Log(LogLevel.Info, String.Format(">>> Execute PlaySound@START command on event {0}", cmdType));
                LogHelper.Log(LogLevel.Trace, String.Format("      PlaySoundCommand={0} / Function={1}", _playSoundCommand.ToString(), function.ToString()));

                playSound();
            }

            // onFunctionEnd & onValidityEnd configured to run ...
            if (checkCommandType(cmdType) && (cmdType == eCommandType.onFunctionEnd | cmdType == eCommandType.onValidityEnd) )
            {
                LogHelper.Log(LogLevel.Info, String.Format(">>> Execute PlaySound@END command on event {0}", cmdType));
                LogHelper.Log(LogLevel.Trace, String.Format("      PlaySoundCommand={0} / Function={1}", _playSoundCommand.ToString(), function.ToString()));

                killProcess();
            }

            // onFunctionError configured to run ...
            // ... stop any running process in case of an error
            if (checkCommandType(cmdType) && cmdType == eCommandType.onFunctionError)
            {
                LogHelper.Log(LogLevel.Info, String.Format(">>> Execute PlaySound@ERROR command on event {0}", cmdType));
                LogHelper.Log(LogLevel.Trace, String.Format("      PlaySoundCommand={0} / Function={1}", _playSoundCommand.ToString(), function.ToString()));

                killProcess();
            }
        }


        #region Play Sound

        /// <summary>
        /// Play sound, calls OS related play sound method.
        /// </summary>
        private void playSound()
        {
            if (EnvironmentHelper.isRunningOnLinux())
            {
                playSoundOnUnix();
            }
            else
            {
                playSoundOnWindows();
            }
        }

        /// <summary>
        /// Play sound on Windows platform
        /// </summary>
        private void playSoundOnWindows()
        {
            // Example URI:
            // - http://www.tonycuffe.com/mp3/tail%20toddle.mp3
            // - http://www.tonycuffe.com/mp3/tailtoddle_lo.mp3
            // - http://www.tonycuffe.com/mp3/cairnomount.mp3
            // - http://drs3.radio.net/
            // - http://asx.skypro.ch/radio/internet-128/virus.asx

            LogHelper.Log(LogLevel.All, "   Beep!");
            _isSoundPlaying = true;
            LogHelper.Log(LogLevel.Info, String.Format("Play sound on Windows! Process={0}", (_process != null ? _process.ToString() : "(null)")));
        }

        /// <summary>
        /// Play sound on Unix platform. Stops (kills) any running play sound command.
        /// </summary>
        private void playSoundOnUnix()
        {
            killProcess();
            killAllProcessesOnUnix();
            // Call mpg321 to play sound on linux.
            // Use quiet mode (-q) if verbose level is Info (or higher).
            String args = String.Format("{0} {1}", (LogHelper.MinVerboseLogLevel<=LogLevel.Debug?"":"-q"), _playSoundCommand.Url);
            _process = EnvironmentHelper.run_cmd("/usr/bin/mpg321", args );
            _isSoundPlaying = true;
            LogHelper.Log(LogLevel.Info, String.Format("Play sound on Unix! args={0} ProcessName={1}", args, (_process != null ? _process.ProcessName : "(null)") ));
        }

        #endregion

        #region Kill Process

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
                _isSoundPlaying = false;
                LogHelper.Log(LogLevel.Info, String.Format("Play sound stopped! Process={0}", (_process != null ? _process.ToString() : "(null)")));
            }
        }


        /// <summary>
        /// Kill all processes on unix, which play sound.
        /// </summary>
        private void killAllProcessesOnUnix()
        {
            try
            {
                Process process = EnvironmentHelper.run_cmd("killall", "mpg321");
                LogHelper.Log(LogLevel.Info, String.Format("Kill all processes on Unix! ProcessName={0}", (process != null ? process.ProcessName : "(null)")));
                bool bRet = process.WaitForExit(1000);
                LogHelper.Log((bRet ? LogLevel.Debug : LogLevel.Warn), String.Format("Kill all processes on Unix exited! ProcessName={0}", (process != null ? process.ProcessName : "(null)")));
            }
            catch (Exception exc)
            {
                LogHelper.LogException("Exception in kill all processes on Unix!", exc);
            }
        }

        #endregion

        #region Renew Play Sound Command

        private void StartTime()
        {
            LogHelper.Log(LogLevel.Debug, String.Format("Renew play sound command, each {0}[min]", Properties.Settings.Default.RenewPlaySoundCommandIntervall*60));
            _timerRenewPlaySoundCommand.Interval = (Properties.Settings.Default.RenewPlaySoundCommandIntervall < 10 ? 10 : Properties.Settings.Default.RenewPlaySoundCommandIntervall) * 1000 * 60;
            _timerRenewPlaySoundCommand.Elapsed += new System.Timers.ElapsedEventHandler(_timerRenewPlaySoundCommand_Elapsed);
            _timerRenewPlaySoundCommand.Start();
        }

        /// <summary>
        /// Periodic timer routine to renew play sound command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _timerRenewPlaySoundCommand_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_isSoundPlaying)
            {
                LogHelper.Log(LogLevel.Info, String.Format(">>> Renew play sound command"));
                playSound();
            }
        }

        #endregion

        /// <summary>
        /// Dispose method, to esnure that play sound command (process) gets killed at shutdown
        /// </summary>
        public new void Dispose()
        {
            // kill any process, before shutdown
            killProcess();
            base.Dispose();
        }
    }
}

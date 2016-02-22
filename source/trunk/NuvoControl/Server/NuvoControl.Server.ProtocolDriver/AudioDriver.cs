using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ProtocolDriver.Interface;


namespace NuvoControl.Server.ProtocolDriver
{

    /// <summary>
    /// Public audio driver, see section <AudioDevice> of configuration settings.
    /// 
    /// For mpg321 settings, see http://www.include.gr/debian/mpg321/
    /// For aplay settings, see http://alsa.opensrc.org/Aplay
    /// 
    /// </summary>
    public class AudioDriver : IAudioDriver, IDisposable
    {
        /// <summary>
        /// Source Id, of the related source.
        /// </summary>
        private Address _sourceId;

        /// <summary>
        /// Audio device settings, as specified in the xml configuration file.
        /// </summary>
        private AudioDevice _audioDevice;

        /// <summary>
        /// URL to play.
        /// </summary>
        private string _url = "";

        /// <summary>
        /// True if sound is playing.
        /// </summary>
        private bool _isSoundPlaying = false;

        /// <summary>
        /// Process member, used to store process information of the player.
        /// </summary>
        private Process _process = null;

        /// <summary>
        /// Private member to hold the timer used to periodically renew the play sound command
        /// </summary>
        private System.Timers.Timer _timerRenewPlaySoundCommand = new System.Timers.Timer();

        /// <summary>
        /// Default constructor for audio driver.
        /// </summary>
        /// <param name="sourceId">Source Id, of related source.</param>
        /// <param name="audioDevice">Audio device settings.</param>
        public AudioDriver(Address sourceId, AudioDevice audioDevice)
        {
            _sourceId = sourceId;
            _audioDevice = audioDevice;
            StartTime();
        }

        /// <summary>
        /// Play sound.
        /// Stop playing if url is empty or <null>
        /// </summary>
        /// <param name="url">URL to play.</param>
        public void CommandPlaySound(string url)
        {
            _url = url;
            playSound();
        }

        /// <summary>
        /// Stop play sound, kill any running player.
        /// </summary>
        public void Close()
        {
            killProcess();
        }



        #region Play Sound

        /// <summary>
        /// Play sound, calls OS related play sound method.
        /// </summary>
        private void playSound()
        {
            switch (_audioDevice.Player)
            {
                case "mpg321":
                    playSoundWithMpg321();
                    break;
                default:
                    if (EnvironmentHelper.isRunningOnLinux())
                    {
                        playSoundWithMpg321();
                    }
                    else
                    {
                        playSoundOnWindows();
                    }
                    break;
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
            LogHelper.Log(LogLevel.Info, String.Format("Play sound on Windows! SourceId={0}, URL={1}, Process={2}", _sourceId, _url, (_process != null ? _process.ToString() : "(null)")));
        }

        /// <summary>
        /// Play sound on Unix platform, with player "mpg321"
        /// </summary>
        private void playSoundWithMpg321()
        {
            killProcess();
            lock (this)
            {
                // Call mpg321 to play sound on linux.
                // Use quiet mode (-q) if verbose level is Info (or higher). Use verbose mode (-v) if verbose level is Trace.
                String args = String.Format("{0} -o {1} -a {2}  {3}", 
                    (LogHelper.MinVerboseLogLevel <= LogLevel.Debug ? (LogHelper.MinVerboseLogLevel == LogLevel.Trace ? "-v" : "") : "-q"),
                    _audioDevice.DeviceType, _audioDevice.Device, _url);
                _process = EnvironmentHelper.run_cmd("/usr/bin/mpg321", args);
                _isSoundPlaying = true;
                LogHelper.Log(LogLevel.Info, String.Format("Play sound on Unix! SourceId={0}, URL={1}, args={2}, ProcessName={3}", _sourceId, _url, args, (_process != null ? _process.ProcessName : "(null)")));
            }
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
                lock (this)
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
                    LogHelper.Log(LogLevel.Debug, String.Format("Play sound stopped! SourceId={0}, Process={1}", _sourceId, (_process != null ? _process.ToString() : "(null)")));
                }
            }
        }


        /// <summary>
        /// Kill all processes on unix, which play sound.
        /// </summary>
        private void killAllProcessesOnUnix()
        {
            try
            {
                String args = String.Format("{0} {1}", (LogHelper.MinVerboseLogLevel <= LogLevel.Debug ? "" : "-q"), "mpg321");
                Process process = EnvironmentHelper.run_cmd("killall", args);
                LogHelper.Log(LogLevel.Debug, String.Format("Kill all processes on Unix! args={0} ProcessName={1}", args, (_process != null ? _process.ProcessName : "(null)")));
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
            LogHelper.Log(LogLevel.Debug, String.Format("Renew play sound command, each {0}[min]", Properties.Settings.Default.RenewPlaySoundCommandIntervall));
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
                LogHelper.Log(LogLevel.Info, String.Format(">>> Renew play sound command for SourceId={0}", _sourceId));
                playSound();
            }
        }

        #endregion

        /// <summary>
        /// Returns string representative for the functions.
        /// </summary>
        /// <returns>String of all functions.</returns>
        public override string ToString()
        {
            return String.Format("Audio Driver: SourceId={0} isSoundPlaying={1} URL={2} AudioDevice={3}", _sourceId.ToString(), _isSoundPlaying, _url, _audioDevice.ToString());
        }

        /// <summary>
        /// Dispose method, to close audio driver
        /// </summary>
        public void Dispose()
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>> Close audio driver for SourceId={0}", _sourceId));
            Close();
        }
    }
}

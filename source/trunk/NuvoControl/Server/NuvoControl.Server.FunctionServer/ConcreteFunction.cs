using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ZoneServer;
using NuvoControl.Server.ProtocolDriver.Interface;


namespace NuvoControl.Server.FunctionServer
{
    public abstract class ConcreteFunction : IConcreteFunction, IDisposable
    {
        /// <summary>
        /// Logger object.
        /// </summary>
        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Private member to store zone server.
        /// </summary>
        protected IZoneServer _zoneServer = null;

        protected Dictionary<int, IAudioDriver> _audioDrivers = null;

        /// <summary>
        /// Private member to hold the configuration data for the (base) function
        /// </summary>
        private Function _function;

        /// <summary>
        /// Private member to hold the concrete commands
        /// </summary>
        private List<IConcreteCommand> _commands = new List<IConcreteCommand>();



        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="zoneServer"></param>
        /// <param name="function"></param>
        public ConcreteFunction(IZoneServer zoneServer, Function function, Dictionary<int, IAudioDriver> audioDrivers)
        {
            if (function == null)
            {
                onFunctionError();
                throw new FunctionServerException("Function configuration is null. This is not allowed");
            }

            _zoneServer = zoneServer;
            if (_zoneServer == null)
            {
                _log.Warn(m => m("Zone Server not available, cannot monitor any zone ..."));
            }

            _audioDrivers = audioDrivers;
            _function = function;

            instantiateConcreteCommands();
        }


        /// <summary>
        /// Create concrete commands.
        /// </summary>
        private void instantiateConcreteCommands()
        {
            foreach (Command cmd in _function.Commands)
            {
                if( cmd != null )
                {
                    _commands.Add(ConcreteCommandFactory.instantiateConcreteCommand(cmd, _zoneServer, _audioDrivers));
                }
                else 
                {
                    LogHelper.Log( LogLevel.Fatal, String.Format("Invalid command found in command list! Function={0}", _function.ToString()));
                }
            }
        }

        /// <summary>
        /// Subscribe for zone events.
        /// </summary>
        /// <param name="zoneId"></param>
        protected void subscribeZone(Address zoneId)
        {
            if (_zoneServer != null)
            {
                _log.Trace(m => m("ConcreteFunction: monitor the zone {0} ...", zoneId.ToString()));
                _zoneServer.Monitor(zoneId, OnZoneNotification);
            }
            else
            {
                _log.Error(m => m("Zone Server not available, cannot monitor the zone {0} ...", zoneId.ToString()));
            }
        }

        /// <summary>
        /// Unsubscribe for zone events.
        /// </summary>
        /// <param name="zoneId"></param>
        protected void unsubscribeZone(Address zoneId)
        {
            if (_zoneServer != null)
            {
                _log.Trace(m => m("ConcreteFunction: remove monitor for zone {0} ...", zoneId.ToString()));
                _zoneServer.RemoveMonitor(zoneId, OnZoneNotification);
            }
        }

        /// <summary>
        /// Calculates the last zone change to ON.
        /// </summary>
        /// <param name="lastZoneChangeToOn">Previous calculated date/time of the last status change to on.</param>
        /// <param name="oldState">Previous zone state.</param>
        /// <param name="newState">New zone state.</param>
        /// <returns></returns>
        protected DateTime calculateZoneChangeToON(DateTime lastZoneChangeToOn, ZoneState oldState, ZoneState newState)
        {
            if (oldState != null)
            {
                if (oldState.PowerStatus == false && newState.PowerStatus == true)
                {
                    // The state has changed from OFF to ON, store the update time
                    lastZoneChangeToOn = newState.LastUpdate;
                }
            }
            else
            {
                if (newState.PowerStatus == true)
                {
                    // we just started and got the first zone state. Store this time as
                    // start time (it's not correct, but better than doing nothing)
                    lastZoneChangeToOn = newState.LastUpdate;
                }
            }
            return lastZoneChangeToOn;
        }

        /// <summary>
        /// Notifcation handler, called from the zone server, which delivers zone state changes
        /// </summary>
        /// <param name="sender">The zone controller, for which the state change appened.</param>
        /// <param name="e">State change event argument.</param>
        private void OnZoneNotification(object sender, ZoneStateEventArgs e)
        {
            //_log.Trace(m => m("ConcreteFunction: OnZoneNotification() EventArgs={0} ...", e.ToString()));
            notifyOnZoneUpdate(e);
        }

        /// <summary>
        /// Abstract method called by base class ConcreteFunction, to notify super class in case of a zone update.
        /// </summary>
        /// <param name="e">State change event argument.</param>
        protected abstract void notifyOnZoneUpdate(ZoneStateEventArgs e);

        /// <summary>
        /// Protected method to be called by super class, in case of an error in function.
        /// </summary>
        protected void onFunctionError()
        {
            LogHelper.Log(LogLevel.Debug, String.Format(">>> onFunctionError: {0} [Function={1}]", this.ToString(), (_function!=null?_function.ToString():"<null>")));
            onFunctionEvent(eCommandType.onFunctionError);
        }

        /// <summary>
        /// Protected method to be called by super class, in case the function starts.
        /// </summary>
        protected void onFunctionStart()
        {
            LogHelper.Log(LogLevel.Debug, String.Format(">>> onFunctionStart: {0}", _function.ToString()));
            onFunctionEvent(eCommandType.onFunctionStart);
        }

        /// <summary>
        /// Protected method to be called by super class, in case function ends.
        /// </summary>
        protected void onFunctionEnd()
        {
            LogHelper.Log(LogLevel.Debug, String.Format(">>> onFunctionEnd: {0}", _function.ToString()));
            onFunctionEvent(eCommandType.onFunctionEnd);
        }

        /// <summary>
        /// Protected method to be called by super class, in case the validity starts.
        /// </summary>
        protected void onValidityStart()
        {
            LogHelper.Log(LogLevel.Debug, String.Format(">>> onValidityStart: {0}", _function.ToString()));
            onFunctionEvent(eCommandType.onValidityStart);
        }

        /// <summary>
        /// Protected method to be called by super class, in case validity ends.
        /// </summary>
        protected void onValidityEnd()
        {
            LogHelper.Log(LogLevel.Debug, String.Format(">>> onValidityEnd: {0}", _function.ToString()));
            onFunctionEvent(eCommandType.onValidityEnd);
        }

        /// <summary>
        /// Private method which calls the event method of super class.
        /// </summary>
        /// <param name="commandType">Type of command (e.g. onFunctionError)</param>
        private void onFunctionEvent(eCommandType commandType)
        {
            foreach (IConcreteCommand cmd in _commands)
            {
                if (cmd != null)
                {
                    cmd.execCommand(commandType, _function);
                }
                else
                {
                    LogHelper.Log(LogLevel.Error, String.Format("Ignore <null> concrete command, in list [{0}]", _commands.ToString()));
                }
            }
        }


        #region IConcreteFunction Members

        public abstract Function Function { get; }

        public abstract bool Active { get; }

        public abstract void calculateFunction(DateTime aktTime);

        #endregion



        public void Dispose()
        {
            // Dispose concrete function (base class)
            foreach (IConcreteCommand cmd in _commands)
            {
                if( cmd != null )
                    cmd.Dispose();
            }
        }
    }
}

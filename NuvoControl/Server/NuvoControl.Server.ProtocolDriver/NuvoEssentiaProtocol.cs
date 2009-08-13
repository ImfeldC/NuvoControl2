/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.ProtocolDriver
 *   Author:         Ch.Imfeld
 *   Creation Date:  6/12/2009 11:02:29 PM
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 6/12/2009 11:02:29 PM, Ch.Imfeld: Initial implementation.
 * 
 **************************************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Server.ProtocolDriver.Interface;
using Common.Logging;
using System.Collections;
using AMS.Profile;
using System.Threading;

namespace NuvoControl.Server.ProtocolDriver
{
    /// <summary>
    /// This class implements the Nuvo Essentia protocol.
    /// 
    /// Based on the interface \ref INuvoEssentiaProtocol this class implements
    /// the concrete protocol used to communicate with the Nuvo Essentia system.
    /// 
    /// As example it ensures also the required delay between two commands.
    /// According to the specification, it is required to wait at least 50[ms] before 
    /// sending the next command.
    /// </summary>
    public class NuvoEssentiaProtocol : INuvoEssentiaProtocol
    {
        #region Common Logger
        /// <summary>
        /// Common logger object. Requires the using directive <c>Common.Logging</c>. See 
        /// <see cref="LogManager"/> for more information.
        /// </summary>
        private ILog _log = LogManager.GetCurrentClassLogger();
        #endregion

        private int _deviceId;
        private INuvoTelegram _serialPort;
        private DateTime _lastTimeACommandHasBeenSent = DateTime.Now;

        private Queue<INuvoEssentiaSingleCommand> _runningCommands = new Queue<INuvoEssentiaSingleCommand>();
        private NuvoEssentiaSingleCommand _errorNuvoEssentiaCommand = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ErrorInCommand);

        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <param name="deviceId">Device Id.</param>
        /// <param name="nuvoTelegram">Optional. Used in case of unit test to pass a mock telegram object.</param>
        public NuvoEssentiaProtocol(int deviceId, INuvoTelegram nuvoTelegram)
        {
            _deviceId = deviceId;

            _serialPort = ((nuvoTelegram == null)?new NuvoTelegram(null):nuvoTelegram);
            _serialPort.onTelegramReceived += new NuvoTelegramEventHandler(_serialPort_onTelegramReceived);
        }

        /// <summary>
        /// Event method called in case a telegram has been received.
        /// </summary>
        /// <param name="sender">This point to the sender of this event.</param>
        /// <param name="e">Event parameters, passed by the sender.</param>
        void _serialPort_onTelegramReceived(object sender, NuvoTelegramEventArgs e)
        {
            INuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(e.Message);
            command.IncomingCommand = e.Message;
            command = compareIncomingCommandWithRunningCommand(command);

            if (command.Valid)
            {
                //raise the event, and pass data to next layer
                if (onCommandReceived != null)
                {
                    onCommandReceived(this,
                      new NuvoEssentiaProtocolEventArgs(_deviceId,command));
                }
            }
        }

        /// <summary>
        /// Method to check, if the incoming command is related to a previous send
        /// outgoing command.
        /// </summary>
        /// <param name="incomingCommand">Incoming command.</param>
        /// <returns>Return the running command.</returns>
        private INuvoEssentiaSingleCommand compareIncomingCommandWithRunningCommand(INuvoEssentiaSingleCommand incomingCommand)
        {
            INuvoEssentiaSingleCommand command = null;

            if (_runningCommands.Count > 0)
            {
                command = _runningCommands.Peek();
                //if (command != null && compareCommands(command, incomingCommand))
                if (command != null && NuvoEssentiaSingleCommand.compareCommandString(command.IncomingCommandTemplate, incomingCommand.IncomingCommand)
                                    && compareZoneIds(command.ZoneId, incomingCommand.ZoneId) )
                {
                    // incoming command matches a previous outgoing command
                    command = _runningCommands.Dequeue();
                    command.IncomingCommand = incomingCommand.IncomingCommand;
                }
                else if (NuvoEssentiaSingleCommand.compareCommandString(_errorNuvoEssentiaCommand.IncomingCommandTemplate, incomingCommand.IncomingCommand))
                {
                    // incoming command indicates an error, assign them to the first command in queue
                    command = _runningCommands.Dequeue();
                    command.IncomingCommand = incomingCommand.IncomingCommand;
                    _log.Error(m => m("An error returned by Nuvo Essentia to the command '{0}'", command));
                }
                else
                {
                    // no outgoing command found. This command has been issued unsolicied
                    command = incomingCommand;
                }
            }
            else
            {
                // no outgoing command available. This command has been issued unsolicied
                command = incomingCommand;
            }

            return command;
        }

        #region INuvoEssentiaProtocol Members

        /// <summary>
        /// Public event, used in case a command answer is received from Nuvo Essentia.
        /// </summary>
        public event NuvoEssentiaProtocolEventHandler onCommandReceived;

        /// <summary>
        /// Open method, to open a connection to the serial port.
        /// </summary>
        /// <param name="serialPortConnectInformation">Connection information, to open the serial port.</param>
        public void Open(SerialPortConnectInformation serialPortConnectInformation)
        {
            _serialPort.Open(serialPortConnectInformation);
        }

        /// <summary>
        /// Close method, to close the serial port connection.
        /// </summary>
        public void Close()
        {
            _serialPort.Close();
        }

        /// <summary>
        /// Send method to send a command string.
        /// </summary>
        /// <param name="commandString">Command sting to send.</param>
        public void SendCommand(string commandString)
        {
            NuvoEssentiaSingleCommand command = convertString2NuvoEssentiaCommand(commandString);
            Send(command);
        }

        /// <summary>
        /// Send method to send a single command.
        /// </summary>
        /// <param name="command">Combined command to send.</param>
        public void SendCommand(INuvoEssentiaSingleCommand command)
        {
            Send(command);
        }

        /// <summary>
        /// Send method to send a combined command.
        /// </summary>
        /// <param name="command">Combined command to send.</param>
        public void SendCommand(INuvoEssentiaCommand command)
        {
            for (INuvoEssentiaSingleCommand singleCommand = command.getNextCommand(null);
                singleCommand != null;
                singleCommand = command.getNextCommand(singleCommand)  )
            {
                Send(singleCommand);
            }
        }

        #endregion

        /// <summary>
        /// Send method for a single command.
        /// </summary>
        /// <param name="command">Command to send.</param>
        private void Send(INuvoEssentiaSingleCommand command)
        {
            int delaySinceLastCommand = 0;
            while ((delaySinceLastCommand=(DateTime.Now - _lastTimeACommandHasBeenSent).Milliseconds) < 50)
            {
                _log.Debug(m => m("Wait with command '{0}' execution, because the delay of {1}[ms] is too small.", command, delaySinceLastCommand));
                Thread.Sleep(50 - delaySinceLastCommand);
            }
            if (command.Command != ENuvoEssentiaCommands.NoCommand)
            {
                command.SendDateTime = DateTime.Now;
                _runningCommands.Enqueue(command);
                _serialPort.SendTelegram(command.OutgoingCommand);
                _lastTimeACommandHasBeenSent = DateTime.Now;
            }
            else
            {
                _log.Warn(m => m("Invalid command (NoCommand) received, not sent to the serial port!"));
            }
        }


        /// <summary>
        /// Converts the command string to a Nuvo Essentia command.
        /// </summary>
        /// <param name="command">Nuvo Essentia command string.</param>
        /// <returns></returns>
        private NuvoEssentiaSingleCommand convertString2NuvoEssentiaCommand(string command)
        {
            return new NuvoEssentiaSingleCommand(NuvoEssentiaSingleCommand.searchNuvoEssentiaCommandWithOutgoingCommand(command));
        }

        /// <summary>
        /// Comperes the zon eids.
        /// </summary>
        /// <param name="zone1">First zone to compare the id.</param>
        /// <param name="zone2">Second zone to compare the id.</param>
        /// <returns>Returns true, if the zone ids are the same.</returns>
        public static bool compareZoneIds(ENuvoEssentiaZones zone1, ENuvoEssentiaZones zone2)
        {
            if ((zone1 != ENuvoEssentiaZones.NoZone) && (zone2 != ENuvoEssentiaZones.NoZone))
            {
                if (zone1 != zone2)
                {
                    // Zone ids are not the same
                    return false;
                }
            }

            // Either the zone ids are the same, or ...
            // At least one zone is not defined, so we can't compare them
            // return in this case 'true'
            return true;
        }

    }
}

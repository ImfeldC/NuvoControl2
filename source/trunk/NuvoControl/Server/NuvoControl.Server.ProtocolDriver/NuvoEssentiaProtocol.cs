using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Server.ProtocolDriver.Interface;
using Common.Logging;
using System.Collections;
using AMS.Profile;

namespace NuvoControl.Server.ProtocolDriver
{
    public class NuvoEssentiaProtocol : INuvoEssentiaProtocol
    {
        private ILog _log = LogManager.GetCurrentClassLogger();

        private int _deviceId;
        private INuvoTelegram _serialPort;

        private Queue<INuvoEssentiaSingleCommand> _runningCommands = new Queue<INuvoEssentiaSingleCommand>();
        private NuvoEssentiaSingleCommand _errorNuvoEssentiaCommand = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ErrorInCommand);

        public NuvoEssentiaProtocol(int deviceId, INuvoTelegram nuvoTelegram)
        {
            _deviceId = deviceId;

            _serialPort = ((nuvoTelegram == null)?new NuvoTelegram(null):nuvoTelegram);
            _serialPort.onTelegramReceived += new NuvoTelegramEventHandler(_serialPort_onTelegramReceived);
        }

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

        public event NuvoEssentiaProtocolEventHandler onCommandReceived;

        public void Open(SerialPortConnectInformation serialPortConnectInformation)
        {
            _serialPort.Open(serialPortConnectInformation);
        }

        public void Close()
        {
            _serialPort.Close();
        }

        public void SendCommand(string commandString)
        {
            NuvoEssentiaSingleCommand command = convertString2NuvoEssentiaCommand(commandString);
            Send(command);
        }

        public void SendCommand(INuvoEssentiaSingleCommand command)
        {
            Send(command);
        }

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


        private void Send(INuvoEssentiaSingleCommand command)
        {
            if (command.Command != ENuvoEssentiaCommands.NoCommand)
            {
                command.SendDateTime = DateTime.Now;
                _runningCommands.Enqueue(command);
                _serialPort.SendTelegram(command.OutgoingCommand);
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

            // Either trrrhe zone ids are the same, or ...
            // At least one zone is not defined, so we can't compare them
            // return in this case 'true'
            return true;
        }

    }
}

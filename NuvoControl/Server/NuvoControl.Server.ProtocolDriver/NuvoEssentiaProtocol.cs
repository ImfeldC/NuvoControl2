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
    class NuvoEssentiaProtocol : INuvoEssentiaProtocol
    {
        private ILog _log = LogManager.GetCurrentClassLogger();
        private Profile _profile;

        private INuvoTelegram _serialPort;

        private Queue<INuvoEssentiaCommand> _runningCommands = new Queue<INuvoEssentiaCommand>();
        private NuvoEssentiaSingleCommand _errorNuvoEssentiaCommand = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ErrorInCommand);

        public NuvoEssentiaProtocol(INuvoTelegram nuvoTelegram)
        {
            _profile = new Xml("NuvoEssentiaProfile.xml");
            _log.Debug(m => m("Open profile file: {0}", _profile.Name));

            if (nuvoTelegram == null)
                _serialPort = new NuvoTelegram(null);
            else
                _serialPort = nuvoTelegram;
            _serialPort.onTelegramReceived += new NuvoTelegramEventHandler(_serialPort_onTelegramReceived);
        }

        void _serialPort_onTelegramReceived(object sender, NuvoTelegramEventArgs e)
        {
            INuvoEssentiaCommand command = new NuvoEssentiaSingleCommand(e.Message);
            command.IncomingCommand = e.Message;
            command = compareIncomingCommandWithRunningCommand(command);

            if (command.Valid)
            {
                //raise the event, and pass data to next layer
                if (onCommandReceived != null)
                {
                    onCommandReceived(this,
                      new NuvoEssentiaProtocolEventArgs(command));
                }
            }
        }

        private INuvoEssentiaCommand compareIncomingCommandWithRunningCommand(INuvoEssentiaCommand incomingCommand)
        {
            INuvoEssentiaCommand command = null;

            if (_runningCommands.Count > 0)
            {
                command = _runningCommands.Peek();
                //if (command != null && compareCommands(command, incomingCommand))
                if (command != null && compareCommandString(command.IncomingCommandTemplate, incomingCommand.IncomingCommand)
                                    && compareZoneIds(command.ZoneId, incomingCommand.ZoneId) )
                {
                    // incoming command matches a previous outgoing command
                    command = _runningCommands.Dequeue();
                    command.IncomingCommand = incomingCommand.IncomingCommand;
                }
                else if (compareCommandString(_errorNuvoEssentiaCommand.IncomingCommandTemplate, incomingCommand.IncomingCommand))
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

        public void SendCommand(INuvoEssentiaCommand command)
        {
            Send(command);
        }

        #endregion


        private void Send(INuvoEssentiaCommand command)
        {
            command.SendDateTime = DateTime.Now;
            _serialPort.SendTelegram(command.OutgoingCommandTemplate);
            _runningCommands.Enqueue(command);
        }


        /// <summary>
        /// Converts the command string to a Nuvo Essentia command.
        /// </summary>
        /// <param name="command">Nuvo Essentia command string.</param>
        /// <returns></returns>
        private NuvoEssentiaSingleCommand convertString2NuvoEssentiaCommand(string command)
        {
            return new NuvoEssentiaSingleCommand(searchNuvoEssentiaCommand(command));
        }

        /// <summary>
        /// Searches in the profile the command passed as string.
        /// This can either be an incomming- or outgoing-command.
        /// </summary>
        /// <param name="command">Command (passed as string)</param>
        /// <returns>Enumeration of the found command. Returns NoCommand if command string isn't available.</returns>
        private ENuvoEssentiaCommands searchNuvoEssentiaCommand(string command)
        {
            string[] sectionNames = _profile.GetSectionNames();
            foreach (string section in sectionNames)
            {
                string[] sectionEntries = _profile.GetEntryNames(section);
                foreach (string entry in sectionEntries)
                {
                    if ( compareCommandString((string)_profile.GetValue(section, entry),command) )
                    {
                        _log.Debug(m => m("Entry found: Entry={0}, Section={1}", entry, section));
                        return (ENuvoEssentiaCommands)Enum.Parse(typeof(ENuvoEssentiaCommands), section, true);
                    }
                }
            }
            // command not found
            return ENuvoEssentiaCommands.NoCommand;
        }

        /// <summary>
        /// Compares the received command string with a configured command string.
        /// The comparision takes into account, that ...
        /// - Placeholders in the configured command string are set with the specific value in the received command string
        /// - Depending on the zone status the received command string has another length then expected
        /// </summary>
        /// <param name="configuredCommand">Configured command string.</param>
        /// <param name="receivedCommand">Received command string.</param>
        /// <returns>True of the received command string is equal to the configured command string.</returns>
        public static bool compareCommandString(string configuredCommand, string receivedCommand)
        {
            // The strings are equal, when ...
            // a) The configured string is equal to the received string OR
            // b) The received string contains PWRON (=Power ON) and 
            //    the length of the received string is 1 smaller than the received string
            //    (This is because the configured string contains 3 placeholders, for either OFF or ON)
            // c) The format of the received string is equal to the received string
            //    (don't compare lower characters, because they are placeholders)

            // Check Length first ...
            if ((configuredCommand.Length == receivedCommand.Length) ||
                (receivedCommand.Contains("PWRON") && (configuredCommand.Length - 1 == receivedCommand.Length))
              )
            {
                // replace PWRON with PWR_ON, this allows future comparison on character level (both strings have the same size)
                receivedCommand = receivedCommand.Replace("PWRON", "PWRXON");

                // Compare each character
                for (int i = 0; i < receivedCommand.Length; i++)
                {
                    if ((receivedCommand[i] != configuredCommand[i]) &&
                        (!Char.IsLetter(configuredCommand[i]) || Char.IsUpper(configuredCommand[i])) )
                    {
                        // Format missmatch
                        return false;
                    }
                }
            }
            else
            {
                // Size missmatch
                return false;
            }

            // Comparison ok
            return true;
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

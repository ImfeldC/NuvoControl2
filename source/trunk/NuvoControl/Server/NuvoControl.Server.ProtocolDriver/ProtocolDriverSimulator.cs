using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Server.ProtocolDriver.Interface;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Server.ProtocolDriver.Simulator
{
    public class ProtocolDriverSimulator : ISerialPort
    {

        /// <summary>
        /// The enumeration EProtocolDriverSimulationMode contains a list of
        /// all available simulation modes supported by the simulation driver
        /// and the simulator.
        /// The following modes are known:
        /// - NoSimulation: There is no simulation active. No answer is returned, and no 
        ///                 spontaneous events are generated.
        /// - AllOk: Simulation is active. Each command is simulated with the correct 
        ///          expected answer. The requested command is parsed and the state is 
        ///          stored in the simulator. An answer is generated to indicate NuvoControl
        ///          that the command has been executed without any problem.
        /// - AllFail: Simulation is active. Return to each command an error message.
        /// - WrongAnswer: Simulation is active. On each command is a 'wrong' (but valid)
        ///                answer returned.
        /// </summary>
        public enum EProtocolDriverSimulationMode
        {
            NoSimulation = 0,   // no (automatic) answer
            AllOk = 1,          // answer with correct command
            AllFail = 2,        // no answer, return error
            WrongAnswer = 3     // Answer with different answer
        }

        private int _deviceId = 1;
        private bool _isOpen = false;

        private EProtocolDriverSimulationMode _mode = EProtocolDriverSimulationMode.AllOk;

        public ProtocolDriverSimulator()
        {
        }

        #region ISerialPort Members

        public event SerialPortEventHandler onDataReceived;

        public void Open(SerialPortConnectInformation serialPortConnectInformation)
        {
            _isOpen = true;
        }

        public void Close()
        {
            _isOpen = false;
        }

        public bool IsOpen
        {
            get { return _isOpen; }
        }

        public void Write(string text)
        {
            switch (_mode)
            {
                case EProtocolDriverSimulationMode.NoSimulation:
                    // Do nothing :-)
                    break;

                case EProtocolDriverSimulationMode.AllOk:
                    simulateAllOk(text);
                    break;

                default:
                    throw new ProtocolDriverException(string.Format("The specified simulation mode '{0}' is not supported by the internal simulator!", _mode.ToString()));
            }

        }

        #endregion

        /// <summary>
        /// Creates a Nuvo Essentia command object, using the command string to retrieve the
        /// correct command type. The comand string is checked against the incoming- and outgoing
        /// commands defined in the XML profiles.
        /// If the correct command type is retrieved, all non-initialized values are set to a useful
        /// value.
        /// </summary>
        /// <param name="commandString">Command string</param>
        /// <returns>Nuvo Essentia command</returns>
        public static NuvoEssentiaSingleCommand createNuvoEssentiaSingleCommand(string commandString)
        {
            char[] _charToRemove = { '*', '\r' };
            EIRCarrierFrequency[] ircf = { EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz };
            NuvoEssentiaSingleCommand rawCommand = new NuvoEssentiaSingleCommand(commandString.Trim(_charToRemove));
            NuvoEssentiaSingleCommand fullCommand = new NuvoEssentiaSingleCommand(
                (rawCommand.Command == ENuvoEssentiaCommands.NoCommand ? ENuvoEssentiaCommands.ReadStatusCONNECT : rawCommand.Command),
                (rawCommand.ZoneId == ENuvoEssentiaZones.NoZone ? ENuvoEssentiaZones.Zone1 : rawCommand.ZoneId),
                (rawCommand.SourceId == ENuvoEssentiaSources.NoSource ? ENuvoEssentiaSources.Source1 : rawCommand.SourceId),
                (rawCommand.VolumeLevel == -999 ? 50 : rawCommand.VolumeLevel),
                (rawCommand.BassLevel == -999 ? 10 : rawCommand.BassLevel),
                (rawCommand.TrebleLevel == -999 ? 10 : rawCommand.TrebleLevel),
                EZonePowerStatus.ZoneStatusON, ircf, EDIPSwitchOverrideStatus.DIPSwitchOverrideOFF,EVolumeResetStatus.VolumeResetOFF,ESourceGroupStatus.SourceGroupOFF,"1.1");

            return fullCommand;
        }

        private void simulateAllOk(string text)
        {
            passDataBackToUpperLayer(new SerialPortEventArgs(createIncomingCommand(text)));
        }

        private void passDataBackToUpperLayer( SerialPortEventArgs arg )
        {
            if (onDataReceived != null)
            {
                onDataReceived(this, arg);
            }
        }

        /// <summary>
        /// Creates for the passed command the expected incoming command string.
        /// It considers values like zone id, volume, etc. to build the expected string.
        /// It adds also the expected start and stop character.
        /// </summary>
        /// <param name="command">Outgoing command</param>
        /// <returns>Returns the expected incoming command string.</returns>
        public static string createIncomingCommand(NuvoEssentiaSingleCommand command)
        {
            EIRCarrierFrequency[] ircf = { command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source1), command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source2), command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source3), command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source4), command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source5), command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source6) };
            string msg = NuvoEssentiaSingleCommand.replacePlaceholders(command.IncomingCommandTemplate, command.ZoneId, command.SourceId, command.VolumeLevel, command.BassLevel, command.TrebleLevel, command.PowerStatus, ircf, command.SourceGrupStatus, command.VolumeResetStatus, command.DIPSwitchOverrideStatus, command.FirmwareVersion);
            return string.Format("#{0}\r", msg);
        }


        /// <summary>
        /// Creates for the passed command string the expected incoming command string.
        /// It considers values like zone id, volume, etc. to build the expected string.
        /// It adds also the expected start and stop character.
        /// </summary>
        /// <param name="command">Outgoing command string, where all placeholders have been replaced.</param>
        /// <returns>Returns the expected incoming command string.</returns>
        public static string createIncomingCommand(string text)
        {
            char[] _charToRemove = { '*', '\r' };
            NuvoEssentiaSingleCommand command = createNuvoEssentiaSingleCommand(text.Trim(_charToRemove));
            return createIncomingCommand(command);
        }
    }
}

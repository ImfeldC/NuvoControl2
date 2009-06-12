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
        public enum EProtocolDriverSimulationMode
        {
            AllOk = 1
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
            switch (_mode)
            {
                case EProtocolDriverSimulationMode.AllOk:
                    _isOpen = true;
                    break;

                default:
                    throw new NotImplementedException();
                    break;
            }
        }

        public void Close()
        {
            switch (_mode)
            {
                case EProtocolDriverSimulationMode.AllOk:
                    _isOpen = false;
                    break;

                default:
                    throw new NotImplementedException();
                    break;
            }
        }

        public bool IsOpen
        {
            get { return _isOpen; }
        }

        public void Write(string text)
        {
            char[] charToRemove = { '*', '\r' };
            NuvoEssentiaSingleCommand command = createNuvoEssentiaSingleCommand(text.Trim(charToRemove));

            switch (_mode)
            {
                case EProtocolDriverSimulationMode.AllOk:
                    simulateAllOk(command);
                    break;

                default:
                    throw new NotImplementedException();
                    break;
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
        private NuvoEssentiaSingleCommand createNuvoEssentiaSingleCommand(string commandString)
        {
            EIRCarrierFrequency[] ircf = { EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz };
            NuvoEssentiaSingleCommand rawCommand = new NuvoEssentiaSingleCommand(commandString);
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

        private void simulateAllOk(NuvoEssentiaSingleCommand command)
        {
            passDataBackToUpperLayer(new SerialPortEventArgs(createIncomingCommand(command)));
        }

        private void passDataBackToUpperLayer( SerialPortEventArgs arg )
        {
            if (onDataReceived != null)
            {
                onDataReceived(this, arg);
            }
        }

        /// <summary>
        /// Creates for the passed in command string the expected incoming command string.
        /// It considers values like zone id, volume, etc. to build the expected string.
        /// It adds also the expected start and stop character.
        /// </summary>
        /// <param name="command">Outgoing command string, where all placeholders have been replaced by real values.</param>
        /// <returns>Returns the expected incoming command string.</returns>
        public static string createIncomingCommand(NuvoEssentiaSingleCommand command)
        {
            EIRCarrierFrequency[] ircf = { command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source1), command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source2), command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source3), command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source4), command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source5), command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source6) };
            string msg = NuvoEssentiaSingleCommand.replacePlaceholders(command.IncomingCommandTemplate, command.ZoneId, command.SourceId, command.VolumeLevel, command.BassLevel, command.TrebleLevel, command.PowerStatus, ircf, command.SourceGrupStatus, command.VolumeResetStatus, command.DIPSwitchOverrideStatus, command.FirmwareVersion);
            return string.Format("#{0}\r", msg);
        }
    }
}

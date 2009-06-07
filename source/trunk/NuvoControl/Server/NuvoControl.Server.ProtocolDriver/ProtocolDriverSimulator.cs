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
        //private Communication _commConfig = new Communication("COM1", 9600, 8, 1, "None");
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
            string commandString = text.Trim(charToRemove);
            //ENuvoEssentiaCommands command = NuvoEssentiaProtocol.searchNuvoEssentiaCommand(commandString);
            NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(commandString);

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


        private void simulateAllOk(NuvoEssentiaSingleCommand command)
        {
            //TODO implement simulation mode
            string commandString = command.IncomingCommandTemplate;
            EIRCarrierFrequency[] ircf = { command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source1), command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source2), command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source3), command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source4), command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source5), command.IrCarrierFrequencySource(ENuvoEssentiaSources.Source6) };
            commandString = NuvoEssentiaSingleCommand.replacePlaceholders(commandString, command.ZoneId, command.SourceId, command.VolumeLevel, command.BassLevel, command.TrebleLevel, command.PowerStatus, ircf);

            passDataBackToUpperLayer(new SerialPortEventArgs("#Z02PWRON,SRC5,GRP0,VOL-33\r"));
        }

        private void passDataBackToUpperLayer( SerialPortEventArgs arg )
        {
            if (onDataReceived != null)
            {
                onDataReceived(this, arg);
            }
        }

    }
}

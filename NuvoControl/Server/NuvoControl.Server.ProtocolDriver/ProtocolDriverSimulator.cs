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
            NuvoEssentiaProtocol _prot = new NuvoEssentiaProtocol(_deviceId,null);
            ENuvoEssentiaCommands command = _prot.searchNuvoEssentiaCommand(text);

            switch (_mode)
            {
                case EProtocolDriverSimulationMode.AllOk:
                    simulateAllOk(new NuvoEssentiaSingleCommand(command));
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

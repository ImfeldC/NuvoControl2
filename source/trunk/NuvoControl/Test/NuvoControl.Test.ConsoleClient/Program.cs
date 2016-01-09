using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;     // used for Application
using System.Text;
using System.IO.Ports;

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;

using NuvoControl.Server.ProtocolDriver;
using NuvoControl.Server.ProtocolDriver.Interface;



namespace NuvoControl.Test.ConsoleClient
{
    class SerialPortTest
    {
        static void Main(string[] args)
        {
            ILog log = LogManager.GetCurrentClassLogger();
            log.Debug(m => m("Starting Test Console Client! (Version={0})", Application.ProductVersion));
            LogHelper.Log("**** Test Console Client started. *******", log);
            
            Console.WriteLine(">>> Starting Test Console Client  --- Assembly Version={0} / Deployment Version={1} / Product Version={2} (using .NET 4.0) ... ",
                "n/a", "n/a", Application.ProductVersion);
            //Console.WriteLine(">>> Starting Console Client  --- Assembly Version={0} / Deployment Version={1} / Product Version={2} (using .NET 4.0) ... ",
            //    AppInfoHelper.getAssemblyVersion(), AppInfoHelper.getDeploymentVersion(), Application.ProductVersion);
            Console.WriteLine("    Linux={0} / Detected environment: {1}", EnvironmentHelper.isRunningOnLinux(), EnvironmentHelper.getOperatingSystem() );
            Console.WriteLine();

            // Load command line argumnets
            var options = new Options();
            CommandLine.Parser.Default.ParseArguments(args, options);
            if (options.Help)
            {
                Console.WriteLine(options.GetUsage());
            }

            ////////////////////////////////
            // Test Serial Port
            ////////////////////////////////

            Console.WriteLine("Start serial port tests....");
            // Connect and Test (direct serial port)
            SerialPortTest myTest = new SerialPortTest(options);
            myTest.Test();

            // Send command, passed with -s option (direct to serial port)
            if( options.sendData != null )
                myTest.SendReceiveData(options.sendData + "\r\n");

            myTest.Close();
            Console.WriteLine("End serial port tests....");
            Console.WriteLine();


            ///////////////////////////////
            // Test Protocol driver
            ///////////////////////////////

            Console.WriteLine("Start protocol driver tests....");
            // Connect and Test (via protocol driver)
            myTest.TestCommand();

            myTest.sendCommand(
                options.strCommand == null ? "ReadStatusCONNECT" : options.strCommand,
                options.strZone == null ? "Zone1" : options.strZone, 
                options.strSource == null ? "Source1" : options.strSource, 
                options.strSource == null ? "ZoneStatusOFF" : options.strPowerStatus, 
                options.volume, 20, 20);

            //Console.WriteLine("Read version ...");
            //NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadVersion);
            //Console.WriteLine("Ootgoing command: " + command.OutgoingCommand);
            //_nuvoServer.SendCommand(_address, command);
            Console.WriteLine("End protocol driver tests....");

            Console.WriteLine(">>> ");
            Console.WriteLine(">>> ");
            Console.WriteLine(">>> Press <Enter> to stop the console application.");
            Console.ReadLine();
        }


        private Options _options = null;

        private System.IO.Ports.SerialPort mySerial;
        private string _portName = "/dev/ttyAMA0";  // default port name within Raspberry Pi
        private int _baudRate = 9600;               // default: 9600 baud
        private int _readTimeout = 4000;            // default: 4000 [ms]
 

        // Constructor
        public SerialPortTest(string portName, int baudRate, int readTimeout)
        {
            _portName = portName;
            if (baudRate > 0 )
                _baudRate = baudRate;
            if (readTimeout > 0)
                _readTimeout = readTimeout;
        }

        public SerialPortTest(Options options)
        {
            _options = options;
            _portName = options.portName;
            if (options.baudRate > 0)
                _baudRate = options.baudRate;
            if (options.readTimeout > 0)
                _readTimeout = options.readTimeout;
        }

        ////////////////////////////////
        // Test Serial Port
        ////////////////////////////////

        public void Test()
        {
            // Test connection, with firmware query command
            SendReceiveData("*VER\r\n");
        }

        public void SendReceiveData(string sendData)
        {
            if (mySerial != null)
                if (mySerial.IsOpen)
                    mySerial.Close();

            try
            {
                Console.WriteLine("Open connection to Port '{0}' (Baud Rate={1})", _portName, _baudRate);
                mySerial = new System.IO.Ports.SerialPort(_portName, _baudRate);
                mySerial.Open();
                mySerial.ReadTimeout = _readTimeout;
                SendData(sendData);
            }
            catch (System.ArgumentException exc)
            {
                Console.WriteLine("Exception! {0}", exc.ToString());
                mySerial = null;
            }

            if (mySerial != null)
            {
                string inputData = ReadData();
                //Console.WriteLine("Message received:" + inputData);
            }
        }

        public string ReadData()
        {
            byte tmpByte;
            string rxString = "";
 
            try
            {
                tmpByte = (byte)mySerial.ReadByte();
                //Console.WriteLine("Start ...." + tmpByte);

                while (tmpByte != 255)
                {
                    rxString += ((char)tmpByte);
                    tmpByte = (byte)mySerial.ReadByte();
                    //Console.WriteLine("Get data ...." + tmpByte);
                }
            }
            catch (System.TimeoutException exc)
            {
                // ignore timeout, finish read-out
            }

            Console.WriteLine("Message received:" + rxString);
            return rxString;
        }
 
        public bool SendData(string Data)
        {
            mySerial.Write(Data);
            Console.WriteLine("Message send:" + Data);
            return true;
        }

        public void Close()
        {
            mySerial.Close();
        }


        ///////////////////////////////
        // Test Protocol driver
        ///////////////////////////////

        // Private members
        Address _address = new Address(1, 1);
        INuvoProtocol _nuvoServer;
        private NuvoControl.Server.ProtocolDriver.SerialPort _serialPort = new NuvoControl.Server.ProtocolDriver.SerialPort();

        public void TestCommand()
        {
            // Open a protocol stack (using a class implementing IProtocol)
            Console.WriteLine("Open connection to Port '{0}'", _options.portName);
            _nuvoServer = new NuvoEssentiaProtocolDriver();
            _nuvoServer.onCommandReceived += new ProtocolCommandReceivedEventHandler(nuvoServer_onCommandReceived);
            _nuvoServer.onZoneStatusUpdate += new ProtocolZoneUpdatedEventHandler(_nuvoServer_onZoneStatusUpdate);

            Console.WriteLine("Create telegram class for serial port ...");
            NuvoCommandTelegram nuvoTelegram = new NuvoCommandTelegram(_serialPort);
            _nuvoServer.Open(ENuvoSystem.NuVoEssentia, 1, new Communication(_portName, _baudRate, 8, 1, "None"), new NuvoEssentiaProtocol(1, nuvoTelegram));
            Console.WriteLine("Serail port created and opened ... '{0}'", _serialPort.ToString());

            Console.WriteLine("Send command '{0}'", ENuvoEssentiaCommands.ReadVersion.ToString());
            _nuvoServer.SendCommand(_address, new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadVersion));
        }

        public void sendCommand(string strCommand, string strZone, string strSource, string strZoneStatus, int volume, int basslevel, int treblelevel)
        {
            //NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand( NuvoEssentiaProtocol.convertString2NuvoEssentiaCommand(strCommand).Command, ENuvoEssentiaSources.Source1);

            NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                (ENuvoEssentiaCommands)Enum.Parse(typeof(ENuvoEssentiaCommands), strCommand, true),
                (ENuvoEssentiaZones)Enum.Parse(typeof(ENuvoEssentiaZones), strZone, true),
                (ENuvoEssentiaSources)Enum.Parse(typeof(ENuvoEssentiaSources), strSource, true),
                (int)volume, (int)basslevel, (int)treblelevel,
                (EZonePowerStatus)Enum.Parse(typeof(EZonePowerStatus), strZoneStatus, true),
                new EIRCarrierFrequency[6],
                EDIPSwitchOverrideStatus.DIPSwitchOverrideOFF,
                EVolumeResetStatus.VolumeResetOFF,
                ESourceGroupStatus.SourceGroupOFF, "V1.0");
            Console.WriteLine("Send commend: " + command.OutgoingCommand);
            if (_nuvoServer != null)
            {
                _nuvoServer.SendCommand(_address, command);
            }            
        }

        void nuvoServer_onCommandReceived(object sender, ProtocolCommandReceivedEventArgs e)
        {
            Console.WriteLine("Command Received:" + e.Command.IncomingCommand);
        }

        void _nuvoServer_onZoneStatusUpdate(object sender, ProtocolZoneUpdatedEventArgs e)
        {
            Console.WriteLine("Zone Update: Zone='{0}' State='{1}'", e.ZoneAddress, e.ZoneState);
        }

    }
}

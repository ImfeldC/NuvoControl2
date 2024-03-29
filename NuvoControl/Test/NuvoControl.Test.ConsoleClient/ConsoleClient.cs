﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;     // needed for Application
//using System.Windows.Controls;  // needed for MediaElement (not supported on Raspberry Pi!)
using System.Text;
using System.IO;                // FileStream
using System.IO.Ports;
using System.Media;             // SoundPlayer
using System.Net;
using System.Diagnostics;       // ProcessStartInfo
using System.Threading;         // Sleep

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;

using NuvoControl.Server.ProtocolDriver;
using NuvoControl.Server.ProtocolDriver.Interface;

using NuvoControl.ThingSpeak;



namespace NuvoControl.Test.ConsoleClient
{
    class SerialPortTest
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Load command line argumnets
            var options = new Options();
            CommandLine.Parser.Default.ParseArguments(args, options);
            // Set global verbose mode
            LogHelper.SetOptions( options );
            const string strAppName = "Test Console Client";
            LogHelper.LogAppStart(strAppName);
            LogHelper.LogArgs(args);
            if (options.Help)
            {
                Console.WriteLine(options.GetUsage());
            }

            ////////////////////////////////
            // ThingSpeak Test
            ////////////////////////////////
/*
            LogHelper.Log(LogLevel.All, "Start ThingSpeak tests.... ");

            short TSResponse3 = 0;
            Boolean bRet3 = NuvoControl.ThingSpeak.ThingSpeak.SendDataToThingSpeak("10BLEA0XICFWMWW1", "1", "25", out TSResponse3);
            Thread.Sleep(20000);    // wait approx. 20s before sending new data
            short TSResponse1 = 0;
            Boolean bRet1 = NuvoControl.ThingSpeak.ThingSpeak.SendDataToThingSpeak("10BLEA0XICFWMWW1", "1", "2", "3", "4", "5", "6", "7", "8", out TSResponse1);
            Thread.Sleep(20000);    // wait approx. 20s before sending new data
            short TSResponse2 = 0;
            Boolean bRet2 = NuvoControl.ThingSpeak.ThingSpeak.UpdateThingkSpeakStatus("10BLEA0XICFWMWW1", ("Test Status: " + strAppName), out TSResponse2);

            LogHelper.Log(LogLevel.All, "End ThingSpeak tests....");
            LogHelper.Log(LogLevel.All, "");
*/

            ////////////////////////////////
            // Play Sound Test
            ////////////////////////////////

            LogHelper.Log(LogLevel.All, "Start play sound tests.... ");

            #region Play File
            if (options.soundFile != null)
            {
                LogHelper.Log(LogLevel.Info, String.Format("   Play File {0} ...", options.soundFile));
                try
                {
                    // Example, see http://raspberrypi.stackexchange.com/questions/3368/is-there-a-way-to-get-soundplayer-to-work-or-is-there-an-alternative
                    // To test on Raspberry Pi: aplay tada.wav
                    // How to configure: https://www.raspberrypi.org/documentation/configuration/audio-config.md
                    {
                        // Plays *.wav only
                        SoundPlayer player = new SoundPlayer(options.soundFile);
                        player.PlaySync();
                        player.Dispose();
                    }
                }
                catch (System.IO.FileNotFoundException exc)
                {
                    LogHelper.Log(LogLevel.Error, String.Format("   File {0} not found, skip sound test.", options.soundFile));
                }
                catch (System.ArgumentException exc)
                {
                    LogHelper.Log(LogLevel.Fatal, String.Format("   File {0} not supported, skip sound test. [Exception={1}]", options.soundFile, exc.ToString()));
                }
            }
            #endregion

            #region Play Stream
            if (options.soundStream != null)
            {
                LogHelper.Log(LogLevel.Info, String.Format("   Play Stream {0} ...", options.soundStream));
                try
                {
                    // Example URI:
                    // - http://www.tonycuffe.com/mp3/tail%20toddle.mp3
                    // - http://www.tonycuffe.com/mp3/tailtoddle_lo.mp3
                    // - http://www.tonycuffe.com/mp3/cairnomount.mp3
                    // - http://drs3.radio.net/
                    // - http://asx.skypro.ch/radio/internet-128/virus.asx

                    /* => MediaElement (not supported on Raspberry Pi!)
                    MediaElement mediaElement = new MediaElement();

                    mediaElement.Source = new Uri(options.soundStream);
                    // See http://stackoverflow.com/questions/19852537/setting-the-loadedbehaviour-and-unloadedbehaviour-of-mediaelement-wpf-applicatio
                    mediaElement.LoadedBehavior = MediaState.Manual;
                    mediaElement.UnloadedBehavior = MediaState.Stop;
                    // Play media
                    mediaElement.Play();

                    Console.WriteLine(">>> Press <Enter> to continue.");
                    Console.ReadLine();
                    */

                    /*
                    // Creates an HttpWebRequest with the specified URL. 
                    HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(options.soundStream);
                    // Sends the HttpWebRequest and waits for the response.			
                    HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                    // Gets the stream associated with the response.
                    Stream receiveStream = myHttpWebResponse.GetResponseStream();

                    // Plays *.wav only
                    SoundPlayer player = new SoundPlayer(receiveStream);
                    player.PlaySync();
                    player.Dispose();
                    */

                }
                catch (System.IO.FileNotFoundException exc)
                {
                    LogHelper.Log(LogLevel.Error, String.Format("   Stream {0} not found, skip sound test.", options.soundStream));
                }
                catch (System.ArgumentException exc)
                {
                    LogHelper.Log(LogLevel.Error, String.Format("   Stream {0} not supported, skip sound test. [Exception={1}]", options.soundStream, exc.ToString()));
                }
                catch (System.InvalidOperationException exc)
                {
                    LogHelper.Log(LogLevel.Fatal, String.Format("   Stream {0} invalid. [Exception={1}]", options.soundStream, exc.ToString()));
                }
                catch (System.NotSupportedException exc)
                {
                    LogHelper.Log(LogLevel.Fatal, String.Format("   Not supported excpetion for {0}. [Exception={1}]", options.soundStream, exc.ToString()));
                }
            }
            #endregion

            LogHelper.Log(LogLevel.All, "End play sound tests....");
            LogHelper.Log(LogLevel.All, "");


            ///////////////////////////////
            // Test Spawn Process
            ///////////////////////////////

            LogHelper.Log(LogLevel.All, "Start Process tests.... ");
            Process process = null;

            if (options.processCmd != null)
            {
                process = run_cmd(options.processCmd, options.processArg);
            }

            LogHelper.Log(LogLevel.All, "End Process tests....");
            LogHelper.Log(LogLevel.All, "");

            ///////////////////////////////
            // Test Mail
            ///////////////////////////////

            LogHelper.Log(LogLevel.All, "Start Mail tests.... ");

            if (options.mailRecepient != null)
            {
                bool bSend = MailHelper.SendMail(options.mailRecepient,
                    (options.mailSubject == null ? "Mail from Nuvo Control" : options.mailSubject),
                    (options.mailBody == null ? "<Empty Body>" : options.mailBody));
                LogHelper.Log(LogLevel.Info, String.Format("    Mail {1} send to {0}", options.mailRecepient, (bSend==true?"":"NOT")));
            }

            LogHelper.Log(LogLevel.All, "End Mail tests....");

            ////////////////////////////////
            // Test Serial Port
            ////////////////////////////////

            LogHelper.Log(LogLevel.All, "Start serial port tests....");
            // Connect and Test (direct serial port)
            SerialPortTest myTest = new SerialPortTest(options);
            myTest.Test();

            // Send command, passed with -s option (direct to serial port)
            if (options.sendData != null)
                myTest.SendReceiveData(options.sendData + "\r\n");

            myTest.Close();
            LogHelper.Log(LogLevel.All, "End serial port tests....");
            LogHelper.Log(LogLevel.All, "");


            ///////////////////////////////
            // Test Protocol driver
            ///////////////////////////////

            LogHelper.Log(LogLevel.All, "Start protocol driver tests....");
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
            LogHelper.Log(LogLevel.All, "End protocol driver tests....");

            String configurationFile = "./Config/NuvoControlKonfiguration.xml";
            String remoteConfigurationFile = "";
            LogHelper.Log(LogLevel.Info, ">>> Loading configuration...");
            LogHelper.Log(LogLevel.Info, String.Format(">>>   from {0}", configurationFile));
            LogHelper.Log(LogLevel.Info, String.Format(">>>   and append {0}", remoteConfigurationFile));

            NuvoControl.Server.ConfigurationService.ConfigurationService _configurationService = null;
            _configurationService = new NuvoControl.Server.ConfigurationService.ConfigurationService(configurationFile, remoteConfigurationFile);


            LogHelper.Log(LogLevel.All, ">>> ");
            LogHelper.Log(LogLevel.All, ">>> ");
            LogHelper.Log(LogLevel.All, ">>> Press <Enter> to stop the console application.");
            Console.ReadLine();

            // Close application
            if (process != null)
            {
                try
                {
                    process.Kill();
                }
                catch( System.InvalidOperationException exc )
                {
                    //ignore any exception at shutdown
                }
            }
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

        #region Test Serial Port

        public void Test()
        {
            // Test connection, with firmware query command
            SendReceiveData("*VER\r\n");
        }

        public void SendReceiveData(string sendData)
        {
            if (mySerial != null)
            {
                if (mySerial.IsOpen)
                    mySerial.Close();

                try
                {
                    LogHelper.Log(LogLevel.Info, String.Format("Open connection to Port '{0}' (Baud Rate={1})", _portName, _baudRate));
                    mySerial = new System.IO.Ports.SerialPort(_portName, _baudRate);
                    mySerial.Open();
                    mySerial.ReadTimeout = _readTimeout;
                    SendData(sendData);
                }
                catch (System.ArgumentException exc)
                {
                    LogHelper.Log(LogLevel.Fatal, String.Format("Exception! {0}", exc.ToString()));
                    mySerial = null;
                }

                if (mySerial != null)
                {
                    string inputData = ReadData();
                    //Console.WriteLine("Message received:" + inputData);
                }
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

            LogHelper.Log(LogLevel.Info, String.Format("Message received:" + rxString.Trim().Replace('\r', '-')));
            return rxString;
        }
 
        public bool SendData(string Data)
        {
            if (mySerial != null)
            {
                mySerial.Write(Data);
                LogHelper.Log(LogLevel.Info, String.Format("Message send:" + Data));
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Close()
        {
            if( mySerial != null )
                mySerial.Close();
        }

        #endregion


        ///////////////////////////////
        // Test Protocol driver
        ///////////////////////////////

        #region Test Protocl driver

        // Private members
        Address _address = new Address(1, 1);
        INuvoProtocol _nuvoServer;
        private NuvoControl.Server.ProtocolDriver.SerialPort _serialPort = new NuvoControl.Server.ProtocolDriver.SerialPort();

        public void TestCommand()
        {
            // Open a protocol stack (using a class implementing IProtocol)
            LogHelper.Log(LogLevel.Info, String.Format("Open connection to Port '{0}'", _options.portName));
            _nuvoServer = new NuvoEssentiaProtocolDriver();
            _nuvoServer.onCommandReceived += new ProtocolCommandReceivedEventHandler(nuvoServer_onCommandReceived);
            _nuvoServer.onZoneStatusUpdate += new ProtocolZoneUpdatedEventHandler(_nuvoServer_onZoneStatusUpdate);

            LogHelper.Log(LogLevel.Info, String.Format("Create telegram class for serial port ..."));
            NuvoCommandTelegram nuvoTelegram = new NuvoCommandTelegram(_serialPort);
            _nuvoServer.Open(ENuvoSystem.NuVoEssentia, 1, new Communication(_portName, _baudRate, 8, 1, "None"), new NuvoEssentiaProtocol(1, nuvoTelegram));
            LogHelper.Log(LogLevel.Info, String.Format("Serail port created and opened ... '{0}'", _serialPort.ToString()));

            LogHelper.Log(LogLevel.Info, String.Format("Send command '{0}'", ENuvoEssentiaCommands.ReadVersion.ToString()));
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
            LogHelper.Log(LogLevel.Info, String.Format("Send command: " + command.OutgoingCommand));
            if (_nuvoServer != null)
            {
                _nuvoServer.SendCommand(_address, command);
            }            
        }

        void nuvoServer_onCommandReceived(object sender, ProtocolCommandReceivedEventArgs e)
        {
            LogHelper.Log(LogLevel.Info, String.Format("Command Received:" + e.Command.IncomingCommand));
        }

        void _nuvoServer_onZoneStatusUpdate(object sender, ProtocolZoneUpdatedEventArgs e)
        {
            LogHelper.Log(LogLevel.Info, String.Format("Zone Update: Zone='{0}' State='{1}'", e.ZoneAddress, e.ZoneState));
        }

        #endregion


        ///////////////////////////////
        // Test Start Process
        ///////////////////////////////

        // See http://stackoverflow.com/questions/11779143/run-a-python-script-from-c-sharp
        public static Process run_cmd(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = cmd;       // e.g. cmd is full path to python.exe
            start.Arguments = args;     // e.g. args is path to .py file and any cmd line args
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            Process process = Process.Start(start);
            LogHelper.Log(LogLevel.Info, String.Format("   Process {0} {1} started .... id={2} [{3}]", cmd, args, process.Id, process.ToString()));

            /*
            using (StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                Console.Write(result);
            }
            */

            return process;
        }



    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.IO.Ports;

namespace NuvoControl.Test.ConsoleClient
{
    class SerialPortTest
    {
        static void Main(string[] args)
        {
            Console.WriteLine(">>> Starting Console Client  --- Assembly Version={0} / Deployment Version={1} / Product Version={2} (using .NET 4.0) ... ",
                "n/a", "n/a", Application.ProductVersion);
            //Console.WriteLine(">>> Starting Console Client  --- Assembly Version={0} / Deployment Version={1} / Product Version={2} (using .NET 4.0) ... ",
            //    AppInfoHelper.getAssemblyVersion(), AppInfoHelper.getDeploymentVersion(), Application.ProductVersion);
            Console.WriteLine();

            var options = new Options();
            CommandLine.Parser.Default.ParseArguments(args, options);
            if (options.Help)
            {
                Console.WriteLine(options.GetUsage());
            }

            // Connect and Test
            SerialPortTest myTest = new SerialPortTest(options.portName,options.baudRate,options.readTimeout);
            myTest.Test();

            if( options.sendData != null )
                myTest.SendReceiveData(options.sendData + "\r\n");

            Console.WriteLine(">>> Press <Enter> to stop the console application.");
            Console.ReadLine();
        }

        private SerialPort mySerial;
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
                mySerial = new SerialPort(_portName, _baudRate);
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
            Console.WriteLine("End ....");
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

    }
}

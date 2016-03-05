using System;
using System.Threading;
using System.Net;
using System.Collections.Generic;
using System.Drawing;
using Bespoke.Common.Osc;

namespace Transmitter
{
    public enum DemoType
    {
        Udp,
        Tcp,
        Multicast
    }

    public class Program
    {
        //public static IPAddress ipAddress = IPAddress.Loopback;
        //public static IPAddress ipAddress = IPAddress.Parse("192.168.1.118");
        public static IPAddress ipAddress = IPAddress.Parse(Properties.Settings.Default.IPAddress);

        public static void Main(string[] args)
        {
            Dictionary<DemoType, Type> transmitters = new Dictionary<DemoType, Type>()
            {
                { DemoType.Udp, typeof(UdpTransmitter) },
                { DemoType.Tcp, typeof(TcpTransmitter) },
                { DemoType.Multicast, typeof(MulticastTransmitter) }
            };

            DemoType demoType = GetDemoType();
            ITransmitter transmitter = Activator.CreateInstance(transmitters[demoType]) as ITransmitter;

            OscBundle bundle = CreateTestBundle();
            transmitter.Start(bundle);

            // Stop the transmitter, and exit, when a key is pressed.
            Console.ReadKey();
            transmitter.Stop();
        }

        private static DemoType GetDemoType()
        {
            Dictionary<ConsoleKey, DemoType> keyMappings = new Dictionary<ConsoleKey, DemoType>()
            {
                { ConsoleKey.D1, DemoType.Udp },
                { ConsoleKey.D2, DemoType.Tcp },
                { ConsoleKey.D3, DemoType.Multicast }
            };

            Console.WriteLine("\nWelcome to the Bespoke Osc Transmitter Demo.\nPlease select the type of transmitter you would like to use:");
            Console.WriteLine("  1. Udp\n  2. Tcp\n  3. Udp Multi-cast");

            ConsoleKeyInfo key = Console.ReadKey();
            while (keyMappings.ContainsKey(key.Key) == false)
            {
                Console.WriteLine("\nInvalid selection\n");
                Console.WriteLine("  1. Udp\n  2. Tcp\n  3. Udp Multi-cast");
                key = Console.ReadKey();
            }

            return keyMappings[key.Key];
        }

        private static OscBundle CreateTestBundle()
        {
            //Program.ipAddress = IPAddress.Parse("192.168.1.126");
            //Program.ipAddress = IPAddress.Loopback;
            IPEndPoint sourceEndPoint = new IPEndPoint(Program.ipAddress, Properties.Settings.Default.Port);
            OscBundle bundle = new OscBundle(sourceEndPoint);

            OscBundle nestedBundle = new OscBundle(sourceEndPoint);
            OscMessage nestedMessage = new OscMessage(sourceEndPoint, TestMethod);
            nestedMessage.AppendNil();
            nestedMessage.Append("Some String");
            nestedMessage.Append(10);
            nestedMessage.Append(100000L);
            nestedMessage.Append(1234.567f);
            nestedMessage.Append(10.0012345);
            nestedMessage.Append(new byte[] { 1, 2, 3, 4 });
            nestedMessage.Append(new OscTimeTag());
            nestedMessage.Append('c');
            nestedMessage.Append(Color.DarkGoldenrod);
            nestedMessage.Append(true);
            nestedMessage.Append(false);
            nestedMessage.Append(float.PositiveInfinity);
            nestedBundle.Append(nestedMessage);
            bundle.Append(nestedBundle);

            OscMessage message = new OscMessage(sourceEndPoint, AliveMethod);
            message.Append(9876.543f);
            bundle.Append(message);

            OscMessage pingMessage = new OscMessage(sourceEndPoint, PingMethod);
            bundle.Append(pingMessage);

            return bundle;
        }

        private static readonly string AliveMethod = "/osctest/alive";
        private static readonly string TestMethod = "/osctest/test";
        private static readonly string PingMethod = "/ping";
    }
}

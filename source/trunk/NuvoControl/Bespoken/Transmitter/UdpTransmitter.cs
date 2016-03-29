using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Bespoke.Common;
using Bespoke.Common.Osc;

namespace Transmitter
{
    public class UdpTransmitter : ITransmitter
    {
        public void Start(OscPacket packet)
        {
            Assert.ParamIsNotNull(packet);
            OscPacket.UdpClient = new UdpClient(SourcePort);

            mCancellationTokenSource = new CancellationTokenSource();
            mSendPacketsTask = Task.Run(() => SendPacketsAsync(packet, mCancellationTokenSource.Token));
        }

        public void Stop()
        {
            mCancellationTokenSource.Cancel();
            mSendPacketsTask.Wait();
        }

        public void Dispose()
        {
            mCancellationTokenSource.Dispose();
        }

        private async Task SendPacketsAsync(OscPacket packet, CancellationToken cancellationToken)
        {
            try
            {
                int transmissionCount = 0;

                while (cancellationToken.IsCancellationRequested == false)
                {
                    IPEndPoint sourceEndPoint = new IPEndPoint(Program.ipAddress, Properties.Settings.Default.Port);
                    OscMessage labelMessage = new OscMessage(sourceEndPoint, "/NuvoControl/ZoneName", String.Format("Hello {0}",transmissionCount));
                    labelMessage.Send(sourceEndPoint);

                    Console.Clear();
                    Console.WriteLine("Osc Transmitter: Udp");
                    Console.WriteLine("with IP Address={0} on Port={1}", sourceEndPoint.Address, sourceEndPoint.Port);
                    Console.WriteLine("IsBundle={0}", labelMessage.IsBundle);
                    if (!packet.IsBundle)
                    {
                        Console.WriteLine("Packet={0}", labelMessage.ToString());
                    }
                    Console.WriteLine("Transmission Count: {0}\n", ++transmissionCount);
                    Console.WriteLine("Press any key to exit.");

                    await Task.Delay(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static readonly IPEndPoint Destination = new IPEndPoint(Program.ipAddress, Properties.Settings.Default.Port);
        private static readonly int SourcePort = 10024;

        private CancellationTokenSource mCancellationTokenSource;
        private Task mSendPacketsTask;
    }
}

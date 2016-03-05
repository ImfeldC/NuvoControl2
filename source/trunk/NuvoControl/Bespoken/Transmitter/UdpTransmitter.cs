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
                    packet.Send(Destination);

                    Console.Clear();
                    Console.WriteLine("Osc Transmitter: Udp");
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

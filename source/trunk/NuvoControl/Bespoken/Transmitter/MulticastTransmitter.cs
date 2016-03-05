using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Bespoke.Common;
using Bespoke.Common.Osc;

namespace Transmitter
{
    public class MulticastTransmitter : ITransmitter
    {
        public void Start(OscPacket packet)
        {
            Assert.ParamIsNotNull(packet);

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
                    Console.WriteLine("Osc Transmitter: Multicast");
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

        private static readonly IPEndPoint Destination = new IPEndPoint(IPAddress.Parse("224.25.26.27"), Properties.Settings.Default.Port);

        private CancellationTokenSource mCancellationTokenSource;
        private Task mSendPacketsTask;
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Bespoke.Common;
using Bespoke.Common.Osc;

namespace Transmitter
{
    public class TcpTransmitter : ITransmitter
    {
        public void Start(OscPacket packet)
        {
            Assert.ParamIsNotNull(packet);

            mOscClient = new OscClient(Destination);
            mOscClient.Connect();
            packet.Client = mOscClient;

            mCancellationTokenSource = new CancellationTokenSource();
            mSendPacketsTask = Task.Run(() => SendPacketsAsync(packet, mCancellationTokenSource.Token));
        }

        public void Stop()
        {
            mCancellationTokenSource.Cancel();
            mSendPacketsTask.Wait();
            mOscClient.Close();
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
                    packet.Send();

                    Console.Clear();
                    Console.WriteLine("Osc Transmitter: Tcp");
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

        private CancellationTokenSource mCancellationTokenSource;
        private Task mSendPacketsTask;
        private OscClient mOscClient;    
    }
}

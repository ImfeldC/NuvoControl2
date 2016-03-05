using System;
using Bespoke.Common.Osc;

namespace Transmitter
{
    /// <summary>
    /// Demo Osc packet transmitter interface.
    /// </summary>
    public interface ITransmitter : IDisposable
    {
        /// <summary>
        /// Start the transmitter.
        /// </summary>
        /// <param name="packet">The packet to transmit.</param>
        void Start(OscPacket packet);

        /// <summary>
        /// Stop the transmitter.
        /// </summary>
        void Stop();
    }
}

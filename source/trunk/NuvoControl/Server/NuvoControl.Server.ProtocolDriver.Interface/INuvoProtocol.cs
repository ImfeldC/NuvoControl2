using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Server.ProtocolDriver.Interface
{
    /// <summary>
    /// This interface extends the main interface <c>IProtocol</c>, with specific methods for 
    /// Nuvo devices. This interface extends the possible range of commands, but it is 
    /// no longer general and the same for all devices.
    /// </summary>
    public interface INuvoProtocol : IProtocol
    {
        /// <summary>
        /// Open method to establish a connection to the device.
        /// This method is used to pass-in an already instantied underlying protocol stack.
        /// It is mainly used in case of unit tests, to pass-in a mock test object.
        /// Is possible use the method of the <c>IProtocol</c> interface. 
        /// See <see cref="IProtocol.Open"/> for more information.
        /// Use - if possible - the <c>Open</c> method of the base interface <c>IProtocol</c>.
        /// </summary>
        /// <param name="system">System type.</param>
        /// <param name="deviceId">Device Id, for this device.</param>
        /// <param name="communicationConfiguration">Comminucation Configuration required to establish a connection.</param>
        /// <param name="essentiaProtocol">Protocol stack, mainly used for mock test objects.</param>
        void Open(ENuvoSystem system, int deviceId, Communication communicationConfiguration, INuvoEssentiaProtocol essentiaProtocol);

        /// <summary>
        /// Sends a single command to the underlying device.
        /// </summary>
        /// <param name="zoneAddress">Zone address, where to send the command.</param>
        /// <param name="command">Nuvo Command, which will be sent to the specified zone.</param>
        void SendCommand(Address zoneAddress, INuvoEssentiaSingleCommand command);

        /// <summary>
        /// Sends a singel or combined command to the underlying device.
        /// </summary>
        /// <param name="zoneAddress">Zone address, where to send the command.</param>
        /// <param name="command">Nuvo Command, which will be sent to the specified zone.</param>
        void SendCommand(Address zoneAddress, INuvoEssentiaCommand command);
    }
}

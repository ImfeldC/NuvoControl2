
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Common.Configuration;

namespace NuvoControl.Server.ProtocolDriver.Interface
{
    public interface IAudioDriver
    {
        /// <summary>
        /// Play the speified URL on the specified source.
        /// </summary>
        /// <param name="URL">URL to play.</param>
        void CommandPlaySound(string URL);

        void Close();
    }
}

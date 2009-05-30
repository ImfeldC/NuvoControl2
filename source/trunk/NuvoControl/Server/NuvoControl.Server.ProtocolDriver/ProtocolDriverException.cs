using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Server.ProtocolDriver
{
    class ProtocolDriverException : Exception
    {
        public ProtocolDriverException(string message)
            : base(message)
        {
        }
    }
}

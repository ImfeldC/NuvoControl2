using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Server.FunctionServer
{
    interface IConcreteFunction
    {
        Function Function { get; }
    }
}

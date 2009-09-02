using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Server.FunctionServer
{
    public abstract class ConcreteFunction : IConcreteFunction
    {
        #region IConcreteFunction Members

        public abstract Function Function { get; }

        #endregion

    }
}

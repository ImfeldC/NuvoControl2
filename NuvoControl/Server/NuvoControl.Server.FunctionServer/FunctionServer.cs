using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Server.ZoneServer;
using NuvoControl.Common.Configuration;
using Common.Logging;

namespace NuvoControl.Server.FunctionServer
{
    public class FunctionServer
    {
        #region Fields

        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The zone server.
        /// </summary>
        private IZoneServer _zoneServer = null;

        private List<Function> _functions = null;

        #endregion

        public FunctionServer(IZoneServer zoneServer, List<Function> functions)
        {
            _zoneServer = zoneServer;
            _functions = functions;

            foreach (Function func in _functions)
            {
                _log.Trace(m=>m("Function: {0} ... loaded", func.ToString() ));
            }
        }
    }
}

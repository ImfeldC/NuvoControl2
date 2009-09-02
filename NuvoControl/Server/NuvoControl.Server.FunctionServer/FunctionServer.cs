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

        /// <summary>
        /// Private list holding the concrete functions
        /// </summary>
        private List<IConcreteFunction> _concreteFunctions = new List<IConcreteFunction>();

        #endregion

        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <param name="zoneServer">Zone server, to get/set zone state.</param>
        /// <param name="functions">Functions</param>
        public FunctionServer(IZoneServer zoneServer, List<Function> functions)
        {
            _zoneServer = zoneServer;

            instantiateFunctions(functions);

            traceFunctions();
        }

        private void instantiateFunctions( List<Function> functions )
        {
            foreach( Function func in functions )
            {
                _concreteFunctions.Add( ConcreteFunctionFactory.instantiateConcreteFuntion(func) );
            }
        }

        /// <summary>
        /// Trace the content of the functions. Use ToString() method for that.
        /// </summary>
        public void traceFunctions()
        {
            _log.Trace(m => m("Functions: {0} ... loaded", this.ToString()));
        }

        /// <summary>
        /// Returns string representative for the functions.
        /// </summary>
        /// <returns>String of all functions.</returns>
        public override string ToString()
        {
            int i = 0;
            string strFunctions = "";

            foreach (IConcreteFunction func in _concreteFunctions)
            {
                strFunctions += String.Format("f({0})=[{1}] ", i, func.Function.ToString());
                i++;
            }
            strFunctions += "\n";

            return strFunctions;
        }
    }
}

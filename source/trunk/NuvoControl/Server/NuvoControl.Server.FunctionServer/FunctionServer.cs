using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common.Logging;

using NuvoControl.Server.ZoneServer;
using NuvoControl.Common.Configuration;
using NuvoControl.Common;
using NuvoControl.Server.ProtocolDriver.Interface;

namespace NuvoControl.Server.FunctionServer
{
    public class FunctionServer : IDisposable
    {
        #region Fields

        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The zone server.
        /// </summary>
        private IZoneServer _zoneServer = null;

        private Dictionary<int, IAudioDriver> _audioDrivers = null;

        /// <summary>
        /// Private list holding the concrete functions
        /// </summary>
        private List<IConcreteFunction> _concreteFunctions = new List<IConcreteFunction>();

        /// <summary>
        /// Private member to hold the timer used to periodically re-calculate the functions
        /// </summary>
        private System.Timers.Timer _timerFunction = new System.Timers.Timer();


        #endregion

        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <param name="zoneServer">Zone server, to get/set zone state.</param>
        /// <param name="functions">Functions</param>
        public FunctionServer(IZoneServer zoneServer, List<Function> functions, Dictionary<int,IAudioDriver> audioDrivers)
        {
            _zoneServer = zoneServer;
            _audioDrivers = audioDrivers;

            instantiateFunctions(functions);

            traceFunctions();

            _log.Trace(m => m("Function timer started, each {0}[s]", Properties.FunctionServer.Default.FunctionIntervall));
            _timerFunction.Interval = (Properties.FunctionServer.Default.FunctionIntervall < 10 ? 10 : Properties.FunctionServer.Default.FunctionIntervall) * 1000;
            _timerFunction.Elapsed += new System.Timers.ElapsedEventHandler(_timerFunction_Elapsed);
            _timerFunction.Start();
        }

        void _timerFunction_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            calculateFunctions();
        }

        public void calculateFunctions()
        {
            DateTime aktTime = DateTime.Now;
            foreach (IConcreteFunction func in _concreteFunctions)
            {
                if (func != null)
                {
                    func.calculateFunction(aktTime);
                }
                else
                {
                    LogHelper.Log(LogLevel.Error, "Skip <null> pointer function. Do you miss a conrete function implementation?");
                }
            }
        }


        private void instantiateFunctions( List<Function> functions )
        {
            foreach( Function func in functions )
            {
                _concreteFunctions.Add(ConcreteFunctionFactory.instantiateConcreteFuntion(func, _zoneServer, _audioDrivers));
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
            string strFunctions = "";

            int i = 0;
            foreach (IConcreteFunction func in _concreteFunctions)
            {
                strFunctions += String.Format("f({0})=[{1}] ", i, (func != null ? func.Function.ToString() : "<null>"));
                i++;
            }
            strFunctions += "\n";

            i = 0;
            foreach (int key in _audioDrivers.Keys)
            {
                strFunctions += String.Format("d({0})=[{1} ", i, _audioDrivers[key].ToString());
                i++;
            }
            strFunctions += "\n";

            return strFunctions;
        }

        #region IDisposable Members

        public void Dispose()
        {
            _log.Trace(m => m("Function server disposed!"));
            _timerFunction.Stop();

            // Dispose all functions ...
            foreach (IConcreteFunction func in _concreteFunctions)
            {
                if( func != null )
                    func.Dispose();
            }
        }

        #endregion
    }
}

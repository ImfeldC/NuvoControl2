using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

using Common.Logging;



namespace NuvoControl.Server.WebServer
{
    public class Global : System.Web.HttpApplication
    {

        private static ILog _log = LogManager.GetCurrentClassLogger();

        private static ServiceManager serviceManager = null;

        public static ServiceManager ServiceManager
        {
            get { return serviceManager; }
        }

        /// <summary>
        /// Method called at application start.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            _log.Trace(m => m("Application_Start ..."));

            serviceManager = new ServiceManager();

            // Discover Services
            //serviceManager.DiscoverConfigurationServices();
            //serviceManager.DiscoverMonitorControlServices();

            // Use dedicated service hosts
            serviceManager.ConfigurationServiceHostAdress = new System.ServiceModel.EndpointAddress("http://imfi-laptopdell:8080/ConfigurationService");
            serviceManager.MonitorControlServiceHostAdress = new System.ServiceModel.EndpointAddress("http://imfi-laptopdell:8080/MonitorAndControlService");

            // Load Configuration
            serviceManager.LoadConfiguration();

            // Load Zone State
            serviceManager.GetAllZoneStates();
        }

        /// <summary>
        /// Method called at application shutdown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
            _log.Trace(m => m("Application_End ..."));
        }

        /// <summary>
        /// Method called when an unhandled error occurs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            _log.Trace(m => m("Application_Error ..."));
        }

        /// <summary>
        /// Method called when a new session is started.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
            _log.Trace(m => m("Session_Start ..."));
        }

        /// <summary>
        /// Method called when session ends.
        /// Note: The Session_End event is raised only when the sessionstate mode
        /// is set to InProc in the Web.config file. If session mode is set to StateServer 
        /// or SQLServer, the event is not raised.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.
            _log.Trace(m => m("Session_End ..."));
        }

    }
}

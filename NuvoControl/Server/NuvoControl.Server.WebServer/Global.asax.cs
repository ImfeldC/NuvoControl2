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


        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup

            serviceManager = new ServiceManager();

            // Discover Services
            //serviceManager.DiscoverConfigurationServices();
            //serviceManager.DiscoverMonitorControlServices();

            // Use dedicated service hosts
            serviceManager.ConfigurationServiceHostAdress = new System.ServiceModel.EndpointAddress("http://imfi-laptopdell:8080/ConfigurationService");
            serviceManager.MonitorControlServiceHostAdress = new System.ServiceModel.EndpointAddress("http://imfi-laptopdell:8080/MonitorAndControlService");

            // Load Configuration
            serviceManager.LoadConfiguration();
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

    }
}

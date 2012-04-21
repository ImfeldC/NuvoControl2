using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Common
{
    /// <summary>
    /// This class contains helper methods th retrieve infromatin about the application context.
    /// </summary>
    public class AppInfoHelper
    {
        /// <summary>
        /// Returns the assembly version.
        /// </summary>
        /// <returns></returns>
        static public string getAssemblyVersion()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// Returns the deployment (publish) version, if available; otherwise "(n/a)" is returned.
        /// </summary>
        /// <returns></returns>
        static public string getDeploymentVersion()
        {
            string deploymentVersion="";
            try
            {
                deploymentVersion = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            catch( Exception )
            {
                deploymentVersion = "(n/a)";
            }
            return deploymentVersion;
        }
    }
}

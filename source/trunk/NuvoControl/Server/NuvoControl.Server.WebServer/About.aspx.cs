using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NuvoControl.Common;

namespace NuvoControl.Server.WebServer
{
    public partial class About : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Add product information
            labelAppInfo.Text = "<b>Application Information:</b> <br>";
            labelAppInfo.Text += String.Format("Assembly Version = {0}<br>", AppInfoHelper.getAssemblyVersion());
            labelAppInfo.Text += String.Format("Deployment Version = {0}<br>", AppInfoHelper.getDeploymentVersion());    
        
            // Add assembly information
            labelAppInfo.Text += "<b>Assembly Information:</b> <br>";
            labelAppInfo.Text += String.Format("Full Name = {0}<br>", System.Reflection.Assembly.GetExecutingAssembly().FullName);
            labelAppInfo.Text += String.Format("Image Runtime Version = {0}<br>", System.Reflection.Assembly.GetExecutingAssembly().ImageRuntimeVersion);
            labelAppInfo.Text += String.Format("Location = {0}<br>", System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
    }
}

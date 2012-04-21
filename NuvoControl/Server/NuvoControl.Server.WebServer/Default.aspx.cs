using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NuvoControl.Server.WebServer
{
    public partial class DefaultPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lableHostNum.Text = Global.ServiceManager.NumOfDiscoveredConfigurationServiceHosts.ToString();
            labelHostAdress.Text = Global.ServiceManager.ConfigurationServiceHostAdress.ToString();
            linkHostAdress.Text = Global.ServiceManager.ConfigurationServiceHostAdress.ToString();
        }
    }
}

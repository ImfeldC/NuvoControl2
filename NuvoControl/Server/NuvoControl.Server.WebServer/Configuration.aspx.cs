using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Common.Logging;
using Common.Logging.Simple;

using NuvoControl.Common.Configuration;
using NuvoControl.Common;

using NuvoControl.Server.WebServer.ConfigurationServiceReference;

namespace NuvoControl.Server.WebServer
{
    public partial class ConfigurationPage : System.Web.UI.Page
    {

        private static ILog _log = LogManager.GetCurrentClassLogger();


        /// <summary>
        /// Initializes the configuration tree view, based on the graphic configuration read from configuration service.
        /// </summary>
        protected void PopulateTree()
        {
            // Initialize Tree View
            treeConfiguration.Nodes.Clear();

            TreeNode nodeBuilding = new TreeNode(Global.ServiceManager.Graphic.Building.Name);
            foreach (Floor floor in Global.ServiceManager.Graphic.Building.Floors)
            {
                //_log.Trace(m => m("Read FLOOR with id={0}.", floor.Id));
                TreeNode nodeFloor = new TreeNode(floor.Name);
                foreach (Zone zone in floor.Zones)
                {
                    //_log.Trace(m => m("Zone found with id {0} with name {1}.", zone.Id.ToString(), zone.Name));
                    TreeNode nodeZone = new TreeNode(zone.Name);
                    nodeFloor.ChildNodes.Add(nodeZone);
                }
                nodeBuilding.ChildNodes.Add(nodeFloor);
            }
            treeConfiguration.Nodes.Add(nodeBuilding);
        }


        protected void GetConfiguration()
        {
            _log.Trace(m => m("LoadConfiguration started."));

            labelConfiguration.Text = "";
            foreach (Zone zone in Global.ServiceManager.Zones)
            {
                _log.Trace(m => m("Read zone configuration for zone with id {0}.", zone.Id));
                labelConfiguration.Text += zone.ToString();
                labelConfiguration.Text += "\n--------------------------\n";
            }

            _log.Trace(m => m("All graphic details: {0}", Global.ServiceManager.Graphic));
            labelConfiguration.Text += Global.ServiceManager.Graphic.ToString();
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            GetConfiguration();
            PopulateTree();
        }

        protected void treeConfiguration_SelectedNodeChanged(object sender, EventArgs e)
        {
            _log.Trace(m => m("Node selected (treeConfiguration_SelectedNodeChanged) Selected Node={0}, Path={1}.", treeConfiguration.SelectedNode.Text, treeConfiguration.SelectedNode.ValuePath));

        }

        protected void buttonReadFunctions_Click(object sender, EventArgs e)
        {
            List<Function> functions = Global.ServiceManager.LoadFunctions();
            labelFunctions.Text = functions.ToString<Function>(" / ");
        }
    }
}
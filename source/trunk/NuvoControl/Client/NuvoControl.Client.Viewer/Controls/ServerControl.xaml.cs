using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NuvoControl.Client.Viewer.Controls
{
    /// <summary>
    /// Interaction logic for ServerControl.xaml
    /// </summary>
    public partial class ServerControl : UserControl
    {
        /// <summary>
        /// Private member to store the selected server. Empty if no server has been selected.
        /// </summary>
        private string _selectedServer = "";

        /// <summary>
        /// Constructor.
        /// </summary>
        public ServerControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Retruns the selected server.
        /// </summary>
        public string SelectedServer
        {
            get { return _selectedServer; }
        }

        /// <summary>
        /// Set the list of servers.
        /// </summary>
        /// <param name="discoveredServers"></param>
        public void setServerList(List<string> discoveredServers)
        {
            foreach (string server in discoveredServers)
            {
                _cboxServerList.Items.Add(server);
            }
        }

        /// <summary>
        /// Stores the value of the selected server of the combobox in the member for the selected server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnServerConnect_Click(object sender, RoutedEventArgs e)
        {
            _selectedServer = _cboxServerList.Text;
        }
    }
}

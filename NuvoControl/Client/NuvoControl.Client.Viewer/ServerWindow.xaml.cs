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
using System.Windows.Shapes;

using Common.Logging;

using NuvoControl.Client.ServiceAccess;


namespace NuvoControl.Client.Viewer
{
    /// <summary>
    /// Interaction logic for ServerWindow.xaml
    /// </summary>
    public partial class ServerWindow : Window
    {
        /// <summary>
        /// Trace logger
        /// </summary>
        private static ILog _log = LogManager.GetCurrentClassLogger();

        public ServerWindow()
        {
            InitializeComponent();

            _ctrlServerConnect.setServerList( ServiceProxy.ServiceDiscovery.DiscoveredServers );

            _ctrlServerConnect._btnServerConnect.Click += new RoutedEventHandler(_btnServerConnect_Click);
        }

        void _btnServerConnect_Click(object sender, RoutedEventArgs e)
        {
            _log.Trace(m => m("Connect to server {0}", _ctrlServerConnect.SelectedServer));
            this.Close();
        }

    }
}

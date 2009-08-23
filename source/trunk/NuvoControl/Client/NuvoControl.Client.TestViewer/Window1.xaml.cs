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
using System.ServiceModel;
using System.Threading;

using NuvoControl.Client.TestViewer.ConfigurationServiceReference;
using NuvoControl.Client.TestViewer.MonitorAndControlServiceReference;
using NuvoControl.Client.TestViewer.Controls;
using NuvoControl.Common.Configuration;
using NuvoControl.Common;

namespace NuvoControl.Client.TestViewer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    [CallbackBehavior(UseSynchronizationContext = false)]
    public partial class Window1 : Window, IMonitorAndControlCallback
    {
        private ConfigureClient _configurationProxy = null;
        private MonitorAndControlClient _monitorAndControlProxy = null;
        private Graphic _graphicConfiguration = null;
        private SynchronizationContext _synchronizationContext = null;
        private Dictionary<Address, ZoneControl> _zoneControlByAddress = new Dictionary<Address, ZoneControl>();

        private ZoneState _zone1State = null;

        public Window1()
        {
            InitializeComponent();
            _synchronizationContext = SynchronizationContext.Current;
            Loaded += new RoutedEventHandler(Window1_Loaded);
            Closed += new EventHandler(Window1_Closed);
        }

        void Window1_Closed(object sender, EventArgs e)
        {
            _configurationProxy.Close();
            _monitorAndControlProxy.Disconnect();
            _monitorAndControlProxy.Close();
        }

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeMonitorAndControlProxy();
            InitializeConfigurationProxy();
            ReadConfiguration();
            InstantiateZoneControls();
            InitializeZones();
            SubscribeAllZones();
        }


        private void InitializeMonitorAndControlProxy()
        {
            try
            {
                IMonitorAndControlCallback serverCallback = this;
                _monitorAndControlProxy = new MonitorAndControlClient(new InstanceContext(serverCallback));
                _monitorAndControlProxy.SetClientBaseAddress();
                _monitorAndControlProxy.Connect();
            }
            catch (Exception)
            {
                _monitorAndControlProxy.Abort();
            }
        }


        private void InitializeConfigurationProxy()
        {
            try
            {
                _configurationProxy = new ConfigureClient();
            }
            catch (Exception)
            {
                _configurationProxy.Abort();
            }
        }


        private void ReadConfiguration()
        {
            try
            {
                _graphicConfiguration = _configurationProxy.GetGraphicConfiguration();
            }
            catch (Exception)
            {
                _configurationProxy.Abort();
            }
        }

        private void InstantiateZoneControls()
        {
            List<Zone> allZones = GetAllZones(_graphicConfiguration);
            int columCount;
            int rowCount;
            ConfigureMainGrid(allZones.Count, out columCount, out rowCount);

            int columnIndex = 0;
            int rowIndex = 0;
            foreach (Zone zone in allZones)
            {
                ZoneControl zoneCtrl = new ZoneControl();
                zoneCtrl.Zone = zone;
                zoneCtrl.Sources = _graphicConfiguration.Sources;
                zoneCtrl._zoneStateChange += new RoutedEventHandler(zoneCtrl__zoneStateChange);
                _gridMain.Children.Add(zoneCtrl);
                _zoneControlByAddress.Add(zone.Id, zoneCtrl);
                Grid.SetColumn(zoneCtrl, columnIndex++);
                Grid.SetRow(zoneCtrl, rowIndex);

                if (columnIndex == columCount)
                {
                    columnIndex = 0;
                    rowIndex++;
                }
            }
        }


        void zoneCtrl__zoneStateChange(object sender, RoutedEventArgs e)
        {
            try
            {
                ZoneControl zoneCtrl = sender as ZoneControl;
                if (zoneCtrl != null)
                {
                    _monitorAndControlProxy.SetZoneState(zoneCtrl.Zone.Id, zoneCtrl.ZoneStateCmd);
                }
            }
            catch (Exception)
            {
                _monitorAndControlProxy.Abort();
            }
        }



        private void InitializeZones()
        {
            try
            {
                foreach (ZoneControl zoneCtrl in _zoneControlByAddress.Values)
                {
                    ZoneState zoneState = _monitorAndControlProxy.GetZoneState(zoneCtrl.Zone.Id);
                    zoneCtrl.ZoneState = zoneState;
                }
            }
            catch (Exception)
            {
                _monitorAndControlProxy.Abort();
            }
        }


        private void SubscribeAllZones()
        {
            try
            {
                foreach (ZoneControl zoneCtrl in _zoneControlByAddress.Values)
                {
                    _monitorAndControlProxy.Monitor(zoneCtrl.Zone.Id);
                }
            }
            catch (Exception)
            {
                _monitorAndControlProxy.Abort();
            }
        }


        private void ConfigureMainGrid(int zoneCount, out int columCount, out int rowCount)
        {
            int noOfGridColumns = 2;
            int noOfGridRows = zoneCount / noOfGridColumns;
            if ((zoneCount % noOfGridColumns) != 0)
                noOfGridRows++;

            for (int i = 0; i < noOfGridColumns; i++)
            {
                _gridMain.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int j = 0; j < noOfGridRows; j++)
            {
                _gridMain.RowDefinitions.Add(new RowDefinition());
            }

            columCount = noOfGridColumns;
            rowCount = noOfGridRows;
        }


        private List<Zone> GetAllZones(Graphic graphic)
        {
            List<Zone> allZones = new List<Zone>();
            foreach (Floor floor in graphic.Building.Floors)
            {
                allZones.AddRange(floor.Zones);
            }
            return allZones;
        }


        #region IMonitorAndControlCallback Members

        public void OnZoneStateChanged(Address zoneId, ZoneState zoneState)
        {
            if (_zoneControlByAddress.ContainsKey(zoneId))
            {
                SendOrPostCallback callback = delegate
                {
                    _zoneControlByAddress[zoneId].ZoneState = zoneState;
                };
                _synchronizationContext.Post(callback, null);
            }

        }

        #endregion
    }
}

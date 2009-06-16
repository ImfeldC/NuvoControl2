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
using NuvoControl.Common.Configuration;
using NuvoControl.Common;

namespace NuvoControl.Client.TestViewer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window, IMonitorAndControlCallback
    {
        private ConfigureClient configurationProxy;
        private MonitorAndControlClient monitorAndControlProxy;
        private ZoneState _zone1State = null;
        private SynchronizationContext _synchronizationContext = null;

        public Window1()
        {
            InitializeComponent();
            _synchronizationContext = SynchronizationContext.Current;
            Loaded += new RoutedEventHandler(Window1_Loaded);
            Unloaded += new RoutedEventHandler(Window1_Unloaded);
        }

        void Window1_Unloaded(object sender, RoutedEventArgs e)
        {
            configurationProxy.Close();
            monitorAndControlProxy.Disconnect();
            monitorAndControlProxy.Close();
        }

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeMonitorAndControlProxy();
            InitializeConfigurationProxy();
            InitializeZone();
       }

        private void InitializeMonitorAndControlProxy()
        {
            try
            {
                IMonitorAndControlCallback serverCallback = this;
                monitorAndControlProxy = new MonitorAndControlClient(new InstanceContext(serverCallback));
                monitorAndControlProxy.SetClientBaseAddress();
                monitorAndControlProxy.Connect();
            }
            catch (Exception exc)
            {
                monitorAndControlProxy.Abort();
            }
        }


        private void InitializeConfigurationProxy()
        {
            try
            {
                configurationProxy = new ConfigureClient();
                Zone zone = configurationProxy.GetZoneKonfiguration(new Address(100, 4));

                Console.WriteLine("Zone name: {0}", zone.Name);
                Console.WriteLine("Picture type: {0}", zone.PictureType);

                Graphic graphic = configurationProxy.GetGraphicConfiguration();
            }
            catch (Exception exc)
            {
                configurationProxy.Abort();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Button subscribeButton = sender as Button;
            if ((string)subscribeButton.Content == "Subscribe")
            {
                monitorAndControlProxy.Monitor(new Address(100, 1));
                subscribeButton.Content = "Unsubscribe";
            }
            else
            {
                monitorAndControlProxy.RemoveMonitor(new Address(100, 1));
                subscribeButton.Content = "Subscribe";
            }
        }

        private void InitializeZone()
        {
            _zone1State = monitorAndControlProxy.GetZoneState(new Address(100, 1));

            lblVolumeState.Content = _zone1State.Volume.ToString();
        }

        private void radioButton1_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void radioButton2_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _zone1State.Volume = (int)e.NewValue;
            monitorAndControlProxy.SetZoneState(new Address(100, 1), _zone1State);
        }

        #region IMonitorAndControlCallback Members

        public void OnZoneStateChanged(Address zoneId, ZoneState zoneState)
        {
            _zone1State = zoneState;
            Console.Beep(100, 100);

            SendOrPostCallback callback = delegate
            {
                lblVolumeState.Content = _zone1State.Volume;
            };
            _synchronizationContext.Post(callback, null);
        }

        #endregion
    }
}

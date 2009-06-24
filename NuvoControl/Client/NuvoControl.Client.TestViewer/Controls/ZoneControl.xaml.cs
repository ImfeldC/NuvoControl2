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

using NuvoControl.Common;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Client.TestViewer.Controls
{
    /// <summary>
    /// Interaction logic for ZoneControl.xaml
    /// </summary>
    public partial class ZoneControl : UserControl
    {
        public event RoutedEventHandler _zoneStateChange;

        private Zone _zone;
        private ZoneState _zoneStateCmd;
        private bool disableNotify = false;
        private List<Source> _sources = new List<Source>();

        public ZoneControl()
        {
            InitializeComponent();
        }

        public Zone Zone
        {
            set
            {
                lock(this)
                {
                    _zone = value;
                    lblZone.Content = _zone.Name;
                }
            }
            get { return _zone; }
        }

       
        public ZoneState ZoneState
        {
            set
            {
                lock (this)
                {
                    disableNotify = true;
                    lblQuality.Content = value.ZoneQuality.ToString();
                    lblAck.Content = value.CommandUnacknowledged ? "Unacknowledged" : "Acknowledged";
                    prgVolume.Value = value.Volume;
                    btnPower.IsChecked = value.PowerStatus;
                    int index = _sources.BinarySearch(new Source(value.Source));
                    if (index >= 0)
                        cbxSource.SelectedItem = _sources[index];

                    _zoneStateCmd = new ZoneState(value);
                    disableNotify = false;
                }
            }
        }


        public List<Source> Sources
        {
            set
            {
                lock (this)
                {
                    _sources = value;
                    cbxSource.ItemsSource = _sources;
                }
            }
        }

        public ZoneState ZoneStateCmd
        {
            get
            {
                return _zoneStateCmd;
            }
        }

        private void btnPower_Checked(object sender, RoutedEventArgs e)
        {
            if (_zoneStateChange != null && disableNotify == false)
            {
                _zoneStateCmd.PowerStatus = true;
                _zoneStateChange(this, e);
            }
        }

        private void btnPower_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_zoneStateChange != null && disableNotify == false)
            {
                _zoneStateCmd.PowerStatus = false;
                _zoneStateChange(this, e);
            }
        }

        private void cbxSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_zoneStateChange != null && disableNotify == false)
            {
                _zoneStateCmd.Source = (e.AddedItems[0] as Source).Id;
                _zoneStateChange(this, e);
            }
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            if (_zoneStateChange != null)
            {
                _zoneStateCmd.Volume-=5;
                if (_zoneStateCmd.Volume < ZoneState.VOLUME_MINVALUE)
                    _zoneStateCmd.Volume = ZoneState.VOLUME_MINVALUE;
                _zoneStateChange(this, e);
            }
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            if (_zoneStateChange != null)
            {
                _zoneStateCmd.Volume+=5;
                if (_zoneStateCmd.Volume > ZoneState.VOLUME_MAXVALUE)
                    _zoneStateCmd.Volume = ZoneState.VOLUME_MAXVALUE;
                _zoneStateChange(this, e);
            }
        }
    }
}

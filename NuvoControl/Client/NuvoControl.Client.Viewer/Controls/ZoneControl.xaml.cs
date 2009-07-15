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
    /// Interaction logic for ZoneControl.xaml
    /// </summary>
    public partial class ZoneControl : UserControl
    {
        public ZoneControl()
        {
            InitializeComponent();
        }

        private void _btnOpenCommander_Click(object sender, RoutedEventArgs e)
        {
            _popupCommander.IsOpen = true;
        }

        private void _btnCloseCommander_Click(object sender, RoutedEventArgs e)
        {
            if ((string)((FrameworkElement)e.OriginalSource).Tag == "ClosePopupButton")
                _popupCommander.IsOpen = false;
        }

        public void SetArea(Point[] coordinates)
        {
            _polygonArea.Points = new PointCollection(coordinates);
        }
    }
}

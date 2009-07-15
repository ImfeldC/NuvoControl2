using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for FloorView.xaml
    /// </summary>
    public partial class FloorView : UserControl
    {
        //Popup popup;
        public FloorView()
        {
            InitializeComponent();

            //popup = new Popup();
            //popup.StaysOpen = false;
            //popup.Child = new ZoneCommander();
            //Canvas.SetTop(popup, 0);
            //Canvas.SetLeft(popup, 0);
            //_canvasFloor.Children.Add(popup);
        }

        public ImageSource Source
        {
            set { this._imageBuilding.Source = value; }
        }

        public void CreateZone()
        {
            //ZoneInfo zoneInfo = new ZoneInfo();
            //Canvas.SetTop(zoneInfo, 250);
            //Canvas.SetLeft(zoneInfo, 70);
            //_canvasFloor.Children.Add(zoneInfo);

            //ZoneCommander zoneCommander = new ZoneCommander();
            //Canvas.SetTop(zoneCommander, 250);
            //Canvas.SetLeft(zoneCommander, 200);
            //_canvasFloor.Children.Add(zoneCommander);

            //Button btnCommand = new Button();
            //btnCommand.Content = "Edit";
            //btnCommand.Click += new RoutedEventHandler(btnCommand_Click);
            //Canvas.SetTop(btnCommand, 200);
            //Canvas.SetLeft(btnCommand, 200);
            //_canvasFloor.Children.Add(btnCommand);

            ZoneControl zoneControl = new ZoneControl();
            Point[] points = new Point[8];
            points[0] = new Point(0,0);
            points[1] = new Point(100,0);
            points[2] = new Point(200,70);
            points[3] = new Point(130,70);
            points[4] = new Point(130,130);
            points[5] = new Point(200,130);
            points[6] = new Point(200,300);
            points[7] = new Point(0,300);
            zoneControl.SetArea(points);

            Canvas.SetTop(zoneControl, 200);
            Canvas.SetLeft(zoneControl, 300);
            _canvasFloor.Children.Add(zoneControl);
        }

        void btnCommand_Click(object sender, RoutedEventArgs e)
        {
           //_popupCommander.IsOpen = true;
        }

    }
}

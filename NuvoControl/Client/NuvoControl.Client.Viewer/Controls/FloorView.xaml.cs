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

using NuvoControl.Client.Viewer.ViewModel;
using NuvoControl.Common;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Client.Viewer.Controls
{
    /// <summary>
    /// Interaction logic for FloorView.xaml
    /// </summary>
    public partial class FloorView : UserControl, IFloorViewNotification
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

        private void CalculateOffset(List<Point> coordinates, out int xOffset, out int yOffset)
        {
            xOffset = int.MaxValue;
            yOffset = int.MaxValue;
            foreach (Point point in coordinates)
            {
                if (point.X < xOffset)
                    xOffset = (int)point.X;
                if (point.Y < yOffset)
                    yOffset = (int)point.Y;
            }
        }

        private List<Point> CalculateRelativeCoordinates(List<Point> coordinates, int xOffset, int yOffset)
        {
            List<Point> newCoordinates = new List<Point>();
            foreach (Point point in coordinates)
            {
                newCoordinates.Add(new Point(point.X - xOffset, point.Y - yOffset));
            }
            return newCoordinates;
        }


        public void CreateFloorZone(Zone zone, List<Source> sources)
        {
            int xOffset;
            int yOffset;
            CalculateOffset(zone.FloorPlanCoordinates, out xOffset, out yOffset);
            List<Point> relativeCoordinates = CalculateRelativeCoordinates(zone.FloorPlanCoordinates, xOffset, yOffset);
            Zone zoneMod = new Zone(zone.Id, zone.Name, zone.PicturePath, zone.PictureType, relativeCoordinates);
            ZoneControl zoneControl = new ZoneControl();
            ZoneState state = new ZoneState();
            ZoneContext zoneContext = new ZoneContext(zoneMod, sources);
            zoneControl.DataContext = zoneContext;
            Canvas.SetLeft(zoneControl, xOffset);
            Canvas.SetTop(zoneControl, yOffset);
            _canvasFloor.Children.Add(zoneControl);
        }

        public void UnloadFloorZones()
        {
            List<ZoneControl> zoneControls = new List<ZoneControl>();
            foreach (UIElement uiElement in _canvasFloor.Children)
            {
                if (uiElement is ZoneControl)
                    zoneControls.Add(uiElement as ZoneControl);
            }
            foreach (ZoneControl zoneControl in zoneControls)
            {
                _canvasFloor.Children.Remove(zoneControl);
            }
        }

        void btnCommand_Click(object sender, RoutedEventArgs e)
        {
           //_popupCommander.IsOpen = true;
        }


        #region IFloorViewNotification Members

        public void UpdateFloorZones(Floor activeFloor, List<Source> sources)
        {
            UnloadFloorZones();

            foreach (Zone zone in activeFloor.Zones)
            {
                CreateFloorZone(zone, sources);
            }
        }

        #endregion

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
        }


        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
        }
    }
}

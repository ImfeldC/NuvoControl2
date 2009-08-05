/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Client.Viewer
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.07.2009
 *   File Name:      FloorView.cs
 * 
 ***************************************************************************************************
 * 
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using NuvoControl.Client.Viewer.ViewModel;
using NuvoControl.Common;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Client.Viewer.Controls
{
    /// <summary>
    /// Interaction logic for FloorView.xaml
    /// 
    /// Contains the logic to unload and load its appropriate zones.
    /// </summary>
    public partial class FloorView : UserControl, IFloorViewNotification
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public FloorView()
        {
            InitializeComponent();
        }

        #endregion

        #region IFloorViewNotification Members

        /// <summary>
        /// Loads (instantiates) the zones of this floor view.
        /// </summary>
        /// <param name="activeFloor">The foor for which to instantiate the zones.</param>
        /// <param name="sources">All sources</param>
        public void LoadFloorZones(Floor activeFloor, List<Source> sources)
        {
            foreach (Zone zone in activeFloor.Zones)
            {
                CreateFloorZone(zone, sources);
            }
        }


        /// <summary>
        /// Unloads all zones of this floor view.
        /// </summary>
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
                (zoneControl.DataContext as ZoneContext).ZoneUnloaded();
                _canvasFloor.Children.Remove(zoneControl);
            }
        }

        #endregion

        #region Non-Public Interface


        /// <summary>
        /// Creates the specified zone of a floor view.
        /// </summary>
        /// <param name="zone">The zone to create.</param>
        /// <param name="sources">All available sources</param>
        private void CreateFloorZone(Zone zone, List<Source> sources)
        {
            int xOffset;
            int yOffset;
            CalculateOffset(zone.FloorPlanCoordinates, out xOffset, out yOffset);
            List<Point> relativeCoordinates = ShiftCoordinates(zone.FloorPlanCoordinates, xOffset, yOffset);
            Zone zoneMod = new Zone(zone.Id, zone.Name, zone.PicturePath, zone.PictureType, relativeCoordinates);
            ZoneControl zoneControl = new ZoneControl();
            ZoneState state = new ZoneState();
            List<Zone> zones = new List<Zone>();
            zones.Add(zoneMod);
            ZoneContext zoneContext = new ZoneContext(zones, sources);
            zoneContext.ZoneLoaded();
            zoneControl.DataContext = zoneContext;
            Canvas.SetLeft(zoneControl, xOffset);
            Canvas.SetTop(zoneControl, yOffset);
            _canvasFloor.Children.Add(zoneControl);
        }


        /// <summary>
        /// Calculates the offset, which a zone is placed within the floor view.
        /// </summary>
        /// <param name="coordinates">Zone coordinates.</param>
        /// <param name="xOffset">The calculated x-offset.</param>
        /// <param name="yOffset">The calculated y-offset.</param>
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


        /// <summary>
        /// Shifts the ccordinates by the offset parameters.
        /// </summary>
        /// <param name="coordinates">The coordinates to shift.</param>
        /// <param name="xOffset">The x-offest</param>
        /// <param name="yOffset">The y-offset.</param>
        /// <returns>The shifted coordinates.</returns>
        private List<Point> ShiftCoordinates(List<Point> coordinates, int xOffset, int yOffset)
        {
            List<Point> newCoordinates = new List<Point>();
            foreach (Point point in coordinates)
            {
                newCoordinates.Add(new Point(point.X - xOffset, point.Y - yOffset));
            }
            return newCoordinates;
        }





        #endregion

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/

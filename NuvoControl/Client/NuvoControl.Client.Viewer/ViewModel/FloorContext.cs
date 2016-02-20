/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Client.Viewer
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.07.2009
 *   File Name:      FloorContext.cs
 * 
 ***************************************************************************************************
 * 
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows;
using System.IO;

using NuvoControl.Common.Configuration;
using NuvoControl.Client.Viewer.Commands;

namespace NuvoControl.Client.Viewer.ViewModel
{
    /// <summary>
    /// This is the logical context for the floor view of the viewer.
    /// It contains the logical context of the floors of a building.
    /// </summary>
    public class FloorContext : INotifyPropertyChanged, IHierarchyContext
    {
        #region Fields

        /// <summary>
        /// All floors of a building.
        /// </summary>
        private List<Floor> _floors;

        /// <summary>
        /// All available sources of the audio system.
        /// </summary>
        private List<SourceGraphic> _sources;

        /// <summary>
        /// The currently shown floor in the floor view.
        /// </summary>
        private Floor _activeFloor;

        /// <summary>
        /// The notification handle to the floor view.
        /// </summary>
        private IFloorViewNotification _floorView;

        /// <summary>
        /// The parent context. This is the main context (building)
        /// </summary>
        private IHierarchyContext _parent = null;

        /// <summary>
        /// The child context. This is the zone context.
        /// </summary>
        private IHierarchyContext _child = null;

        /// <summary>
        /// Visibility of the floor view.
        /// </summary>
        private Visibility _visibility = Visibility.Collapsed;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="floors">All floors of a building.</param>
        /// <param name="sources">All available sources of the audio system.</param>
        /// <param name="floorView">The notification handle to the floor view.</param>
        public FloorContext(List<Floor> floors, List<SourceGraphic> sources, IFloorViewNotification floorView)
        {
            _floors = floors;
            _sources = sources;
            _activeFloor = _floors[0];
            _floorView = floorView;
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// The name of the current shown floor.
        /// </summary>
        public string FloorName
        {
            get { return _activeFloor.Name; }
        }

        /// <summary>
        /// The plan of the current shown floor.
        /// </summary>
        public BitmapImage FloorPlan
        {
            get
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), _activeFloor.FloorPlanPath);
                return new BitmapImage(new Uri(path));
            }
        }

        /// <summary>
        /// Tooltip for the floor view.
        /// </summary>
        public string ToolTip
        {
            get { return ("Click to browse down"); }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        #endregion

        #region IHierarchyContext Members


        /// <summary>
        /// The name of the object, shown in the current hierarchy.
        /// Here this is the name of the active floor.
        /// </summary>
        public string ObjectName
        {
            get { return _activeFloor.Name; }
        }


        /// <summary>
        /// The address of the object, shown in the current hierarchy.
        /// Here it is the address of the active floor.
        /// </summary>
        public Address ObjectId
        {
            get { return _activeFloor.Id; }
        }


        /// <summary>
        /// The parent hierarchy context.
        /// Here this is the building context.
        /// </summary>
        public IHierarchyContext Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }


        /// <summary>
        /// The child hierarchy context.
        /// Here, this is the zone context.
        /// </summary>
        public IHierarchyContext Child
        {
            get { return _child; }
            set { _child = value; }
        }


        /// <summary>
        /// The visibility of the hierarchy context. 
        /// Only one can be visible at a time.
        /// </summary>
        public Visibility ContextVisibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                NotifyPropertyChanged(new PropertyChangedEventArgs("ContextVisibility"));
            }
        }


        /// <summary>
        /// True, if the context has a next object to show.
        /// Here, this is the next floor to show.
        /// </summary>
        public bool HasNextObject
        {
            get { return true; }
        }


        /// <summary>
        /// The name of the next object.
        /// Here, this is the name of the next floor.
        /// </summary>
        public string NextObjectName
        {
            get { return GetNextFloor().Name; }
        }


        /// <summary>
        /// Go to next object of this context.
        /// Here, this is the next floor.
        /// </summary>
        public void NextObject()
        {
            Navigate(GetNextFloor().Id);
        }


        /// <summary>
        /// True, if the context has a previous object to show.
        /// Here, this is the previous floor to show.
        /// </summary>
        public bool HasPreviousObject
        {
            get { return true; }
        }


        /// <summary>
        /// The name of the previous object.
        /// Here, this is the name of the previous floor.
        /// </summary>
        public string PreviousObjectName
        {
            get { return GetPreviousFloor().Name; }
        }


        /// <summary>
        /// Go to previous object of this context.
        /// Here, this is the previous floor.
        /// </summary>
        public void PreviousObject()
        {
            Navigate(GetPreviousFloor().Id);
        }


        /// <summary>
        /// Navigate to a specified object of this context.
        /// </summary>
        /// <param name="id">The id of the object to navigate to. Here, this is the id of a floor.</param>
        public void Navigate(Address id)
        {
            Navigate(id, true);
        }


        /// <summary>
        /// Get all navigations objects of this context.
        /// Here, these are all floors.
        /// </summary>
        public List<NavigationItem> NavigationObjects
        {
            get
            {
                List<NavigationItem> items = new List<NavigationItem>();
                foreach (Floor floor in _floors)
                {
                    items.Add(new NavigationItem(this, floor.Id, floor.Name));
                }
                return items;
            }
        }


        /// <summary>
        /// Called, when this context got activated.
        /// i.e. the corresponding floor view was set to visible.
        /// </summary>
        /// <param name="id">The id of the object to navigate to.</param>
        public void OnHierarchyActivated(Address id)
        {
            Navigate(id, false);
        }


        /// <summary>
        /// Called, when this context is deactivated.
        /// i.e. the floor view was set to collapsed.
        /// </summary>
        public void OnHierarchyDeactivated()
        {
            _floorView.UnloadFloorZones();
        }


        /// <summary>
        /// Updates the context with the specified parameter.
        /// </summary>
        /// <param name="context"></param>
        public void UpdateContext(IHierarchyContext context)
        {
            // not used
        }


        #endregion

        #region Non-Public Interface

        /// <summary>
        /// Navigate to a specified object of this context.
        /// </summary>
        /// <param name="id">The id of the object to navigate to. Here, this is the id of a floor.</param>
        /// <param name="unloadPreviousFloor">Set to true, if previous floor must be unloaded first.</param>
        private void Navigate(Address id, bool unloadPreviousFloor)
        {
            if (id != null)
            {
                Floor tempFloor = FindFloor(id);
                if (tempFloor == null)
                    _activeFloor = _floors[0];
                else
                    _activeFloor = tempFloor;
            }
            else
                _activeFloor = _floors[0];

            _child.UpdateContext(new ZoneContext(_activeFloor.Zones, _sources));
            NotifyPropertyChanged(new PropertyChangedEventArgs(""));

            if (unloadPreviousFloor)
                _floorView.UnloadFloorZones();
            _floorView.LoadFloorZones(_activeFloor, _sources);

        }


        /// <summary>
        /// Gets the next floor object of the floor context.
        /// </summary>
        /// <returns></returns>
        private Floor GetNextFloor()
        {
            int index = _floors.IndexOf(_activeFloor);
            if (index >= _floors.Count - 1)
                return _floors[0];
            else
                return _floors[index + 1];
        }


        /// <summary>
        /// Gets the previous floor object of the floor context.
        /// </summary>
        /// <returns></returns>
        private Floor GetPreviousFloor()
        {
            int index = _floors.IndexOf(_activeFloor);
            if (index <= 0)
                return _floors[_floors.Count - 1];
            else
                return _floors[index - 1];
        }


        /// <summary>
        /// Finds the specified floor object.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The found object; null if none found.</returns>
        private Floor FindFloor(Address id)
        {
            foreach (Floor floor in _floors)
            {
                if (floor.Id.Equals(id))
                    return floor;
            }
            return null;
        }

        #endregion
    }



    /// <summary>
    /// Interface to notify the floor view to update its zones.
    /// </summary>
    public interface IFloorViewNotification
    {
        void LoadFloorZones(Floor activeFloor, List<SourceGraphic> sources);
        void UnloadFloorZones();
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/
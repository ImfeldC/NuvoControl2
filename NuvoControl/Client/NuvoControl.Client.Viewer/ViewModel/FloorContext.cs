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
    public class FloorContext : INotifyPropertyChanged, IHierarchyContext
    {
        private List<Floor> _floors;
        private List<Source> _sources;
        private Floor _activeFloor;
        private IFloorViewNotification _floorView;
        private IHierarchyContext _parent = null;
        private IHierarchyContext _child = null;
        private Visibility _visibility = Visibility.Collapsed;


        public FloorContext(List<Floor> floors, List<Source> sources, IFloorViewNotification floorView)
        {
            _floors = floors;
            _sources = sources;
            _activeFloor = _floors[0];
            _floorView = floorView;
        }

 
        public string FloorName
        {
            get { return _activeFloor.Name; }
        }

        public BitmapImage FloorPlan
        {
            get
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), _activeFloor.FloorPlanPath);
                return new BitmapImage(new Uri(path));
            }
        }

        public string ToolTip
        {
            get { return ("Click to browse down"); }
        }

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


        public void Navigate(Address id)
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

            _floorView.UnloadFloorZones();
            _floorView.LoadFloorZones(_activeFloor, _sources);
        }


        private Floor GetNextFloor()
        {
            int index = _floors.IndexOf(_activeFloor);
            if (index >= _floors.Count - 1)
                return _floors[0];
            else
                return _floors[index + 1];
        }

        private Floor GetPreviousFloor()
        {
            int index = _floors.IndexOf(_activeFloor);
            if (index <= 0)
                return _floors[_floors.Count - 1];
            else
                return _floors[index - 1];
        }

        private Floor FindFloor(Address id)
        {
            foreach (Floor floor in _floors)
            {
                if (floor.Id.Equals(id))
                    return floor;
            }
            return null;
        }


        public void OnHierarchyActivated()
        {
            //_floorView.LoadFloorZones(_activeFloor, _sources);
            //_child.UpdateContext(new ZoneContext(_activeFloor.Zones, _sources));
        }

        public void OnHierarchyDeactivated()
        {
            _floorView.UnloadFloorZones();
        }


        public void UpdateContext(IHierarchyContext context)
        {
            // not used
        }



        #endregion

    }

    public interface IFloorViewNotification
    {
        void LoadFloorZones(Floor activeFloor, List<Source> sources);
        void UnloadFloorZones();
    }
}

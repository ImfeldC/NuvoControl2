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

        public List<NavigationItem> NavigationItems
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


        public string Name
        {
            get { return _activeFloor.Name; }
        }

        public Address Id
        {
            get { return _activeFloor.Id; }
        }

        public IHierarchyContext Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public IHierarchyContext Child
        {
            get { return _child; }
            set { _child = value; }
        }

        public bool HasNext
        {
            get { return true; }
        }

        public string NextName
        {
            get { return GetNextFloor().Name; }
        }

        public void Next()
        {
            _activeFloor = GetNextFloor();
            _child.UpdateContext(new ZoneContext(_activeFloor.Zones, _sources));

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));

            // TODO unsubscribe
            _floorView.UpdateFloorZones(_activeFloor, _sources);
        }

        private Floor GetNextFloor()
        {
            int index = _floors.IndexOf(_activeFloor);
            if (index >= _floors.Count - 1)
                return _floors[0];
            else
                return _floors[index + 1];
        }

        public bool HasPrevious
        {
            get { return true; }
        }

        public string PreviousName
        {
            get { return GetPreviousFloor().Name; }
        }

        public void Previous()
        {
            _activeFloor = GetPreviousFloor();
            _child.UpdateContext(new ZoneContext(_activeFloor.Zones, _sources));

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
            // TODO unsubscribe
            _floorView.UpdateFloorZones(_activeFloor, _sources);
        }

        public void Navigate(Address id)
        {
            Floor tempFloor = FindFloor(id);
            if (tempFloor == null)
                return;

            _activeFloor = tempFloor;
            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
            // TODO unsubscribe
            _floorView.UpdateFloorZones(_activeFloor, _sources);
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

        public Visibility Visibility1
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                NotifyPropertyChanged(new PropertyChangedEventArgs("Visibility1"));
            }
        }

        public void OnHierarchyChanged()
        {
            // noting to do
            _floorView.UpdateFloorZones(_activeFloor, _sources);
        }


        public void UpdateContext(IHierarchyContext context)
        {
            // not used
        }

        #endregion

    }

    public interface IFloorViewNotification
    {
        void UpdateFloorZones(Floor activeFloor, List<Source> sources);
    }
}

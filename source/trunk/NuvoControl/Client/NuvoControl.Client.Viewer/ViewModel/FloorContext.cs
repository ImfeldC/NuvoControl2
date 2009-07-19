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

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        #endregion

        #region IHierarchyContext Members


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
            get { return false; }
        }

        public void Next()
        {
            _activeFloor = _floors[1];
            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
            // TODO unsubscribe
            _floorView.UpdateZones(_activeFloor, _sources);
        }

        public bool HasPrevious
        {
            get { return false; }
        }

        public void Previous()
        {
            _activeFloor = _floors[0];
            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
            // TODO unsubscribe
            _floorView.UpdateZones(_activeFloor, _sources);
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

        #endregion

    }

    public interface IFloorViewNotification
    {
        void UpdateZones(Floor activeFloor, List<Source> sources);
    }
}

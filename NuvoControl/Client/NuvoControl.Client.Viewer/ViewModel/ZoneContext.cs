using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows;
using System.IO;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Client.Viewer.Commands;

namespace NuvoControl.Client.Viewer.ViewModel
{
    public class ZoneContext : INotifyPropertyChanged, IHierarchyContext
    {
        private List<Zone> _zones;
        private Zone _activeZone;
        private ZoneState _zoneState;
        private List<Source> _sources = new List<Source>();
        private ListCollectionView _viewSources;
        private IHierarchyContext _parent = null;
        private IHierarchyContext _child = null;
        private Visibility _visibility = Visibility.Collapsed;

        CommandBinding _binding;

        public ZoneContext(Zone zone, List<Source> sources)
        {
            this._zones = null;
            this._activeZone = zone;
            this._zoneState = new ZoneState();
            this._sources = sources;
            _viewSources = (ListCollectionView)CollectionViewSource.GetDefaultView(this._sources);
            _viewSources.CurrentChanged += new EventHandler(_viewSources_CurrentChanged);

            _binding = new CommandBinding(CustomCommands.VolumeUp, VolumeUpCommand_Executed, VolumeUpCommand_CanExecute);
            //_bindings.Add(binding);
        }

        public ZoneContext(List<Zone> zones, List<Source> sources)
        {
            this._zones = zones;
            this._activeZone = zones[0];
            this._zoneState = new ZoneState();
            this._sources = sources;
            _viewSources = (ListCollectionView)CollectionViewSource.GetDefaultView(this._sources);
            _viewSources.CurrentChanged += new EventHandler(_viewSources_CurrentChanged);

            _binding = new CommandBinding(CustomCommands.VolumeUp, VolumeUpCommand_Executed, VolumeUpCommand_CanExecute);
            //_bindings.Add(binding);
        }


        private void VolumeUpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }


        private void VolumeUpCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        void _viewSources_CurrentChanged(object sender, EventArgs e)
        {
            ListCollectionView view = sender as ListCollectionView;
            if (view != null)
            {
                // Command the service
                // set the property !!!
                this._zoneState.Source = (view.CurrentItem as Source).Id;
                NotifyPropertyChanged(new PropertyChangedEventArgs("ZoneStateSource"));
            }
        }

        public CommandBinding CommandBinding
        {
            get { return _binding; }
        }

        public string ZoneName
        {
            get { return _activeZone.Name; }
        }

        public ZoneState ZoneState
        {
            get { return _zoneState; }
        }

        public string ZoneStateSource
        {
            get { return _zoneState.Source.ToString(); }

        }

        public BitmapImage ZoneImage
        {
            get
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), _activeZone.PicturePath);
                return new BitmapImage(new Uri(path));
            }
        }

        public List<Source> Sources
        {
            get { return _sources; }
        }

        public ListCollectionView ViewSources
        {
            get { return _viewSources; }
        }

        public PointCollection FloorPlanCoordinates
        {
            get { return new PointCollection(_activeZone.FloorPlanCoordinates); }
        }

        private void NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IHierarchyContext Members

        public string Name
        {
            get { return _activeZone.Name; }
        }

        public Address Id
        {
            get { return _activeZone.Id; }
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
            get { return GetNextZone().Name; }
        }

        public void Next()
        {
            _activeZone = GetNextZone();

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
        }

        private Zone GetNextZone()
        {
            int index = _zones.IndexOf(_activeZone);
            if (index >= _zones.Count - 1)
                return _zones[0];
            else
                return _zones[index + 1];
        }

        public bool HasPrevious
        {
            get { return true; }
        }

        public string PreviousName
        {
            get { return GetPreviousZone().Name; }
        }

        public void Previous()
        {
            _activeZone = GetPreviousZone();

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
        }

        private Zone GetPreviousZone()
        {
            int index = _zones.IndexOf(_activeZone);
            if (index <= 0)
                return _zones[_zones.Count - 1];
            else
                return _zones[index - 1];

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
        }

        #endregion

    }
}

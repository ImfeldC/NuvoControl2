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
using System.Threading;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Client.Viewer.Commands;
using NuvoControl.Client.ServiceAccess;

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

        private CommandBindingCollection _bindings = new CommandBindingCollection();
        private SynchronizationContext _synchronizationContext = null;

        //public ZoneContext(Zone zone, List<Source> sources)
        //{
        //    this._zones = null;
        //    this._activeZone = zone;
        //    this._zoneState = new ZoneState();
        //    this._sources = sources;
        //    _viewSources = (ListCollectionView)CollectionViewSource.GetDefaultView(this._sources);
        //    _viewSources.CurrentChanged += new EventHandler(_viewSources_CurrentChanged);

        //    _binding = new CommandBinding(CustomCommands.VolumeUp, VolumeUpCommand_Executed, VolumeUpCommand_CanExecute);
        //    //_bindings.Add(binding);
        //}

        public ZoneContext(List<Zone> zones, List<Source> sources)
        {
            _synchronizationContext = SynchronizationContext.Current;

            this._zones = zones;
            this._activeZone = zones[0];
            this._zoneState = new ZoneState();
            this._sources = sources;
            _viewSources = (ListCollectionView)CollectionViewSource.GetDefaultView(this._sources);
            _viewSources.CurrentChanged += new EventHandler(_viewSources_CurrentChanged);

            CommandBinding binding = new CommandBinding(CustomCommands.VolumeUp, VolumeUpCommand_Executed, VolumeUpCommand_CanExecute);
            _bindings.Add(binding);
            binding = new CommandBinding(CustomCommands.VolumeDown, VolumeDownCommand_Executed, VolumeDownCommand_CanExecute);
            _bindings.Add(binding);
            binding = new CommandBinding(CustomCommands.Power, PowerCommand_Executed, PowerCommand_CanExecute);
            _bindings.Add(binding);

        }


        private void VolumeUpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            lock (this)
            {
                _zoneState.CommandUnacknowledged = true;
                ZoneState cmdState = new ZoneState(_zoneState);
                cmdState.Volume += 5;
                if (cmdState.Volume > ZoneState.VOLUME_MAXVALUE)
                    cmdState.Volume = ZoneState.VOLUME_MAXVALUE;

                ServiceProxy.MonitorAndControlProxy.SetZoneState(_activeZone.Id, _zoneState);
            }
        }

        private void VolumeUpCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            lock (this)
            {
                if (_zoneState.Volume < ZoneState.VOLUME_MAXVALUE)
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
        }

        private void VolumeDownCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            lock (this)
            {
                _zoneState.CommandUnacknowledged = true;
                ZoneState cmdState = new ZoneState(_zoneState);
                cmdState.Volume -= 5;
                if (cmdState.Volume < ZoneState.VOLUME_MINVALUE)
                    cmdState.Volume = ZoneState.VOLUME_MINVALUE;

                ServiceProxy.MonitorAndControlProxy.SetZoneState(_activeZone.Id, _zoneState);
            }
        }

        private void VolumeDownCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            lock (this)
            {
                if (_zoneState.Volume > ZoneState.VOLUME_MINVALUE)
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
        }

        private void PowerCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            lock (this)
            {
                _zoneState.CommandUnacknowledged = true;
                ZoneState cmdState = new ZoneState(_zoneState);
                cmdState.PowerStatus = !cmdState.PowerStatus;

                ServiceProxy.MonitorAndControlProxy.SetZoneState(_activeZone.Id, _zoneState);
            }
        }

        private void PowerCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        void _viewSources_CurrentChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                ListCollectionView view = sender as ListCollectionView;
                if (view != null)
                {
                    _zoneState.CommandUnacknowledged = true;
                    ZoneState cmdState = new ZoneState(_zoneState);
                    cmdState.Source = (view.CurrentItem as Source).Id;

                    ServiceProxy.MonitorAndControlProxy.SetZoneState(_activeZone.Id, _zoneState);

                    //this._zoneState.Source = (view.CurrentItem as Source).Id;
                    //NotifyPropertyChanged(new PropertyChangedEventArgs("ZoneStateSource"));
                }
            }
        }

        public CommandBindingCollection CommandBindingsDynamic
        {
            get { return _bindings; }
        }

        public string ZoneName
        {
            get { return _activeZone.Name; }
        }

        public ZoneState ZoneState
        {
            get { return _zoneState; }
        }

        public Address SourceZoneState
        {
            get { return _zoneState.Source; }

        }

        public bool PowerZoneState
        {
            get { return _zoneState.PowerStatus; }
            set 
            {
            }        
        }

        public ZoneQuality QualityZoneState
        {
            get { return _zoneState.ZoneQuality; }

        }

        public bool UnackZoneState
        {
            get { return _zoneState.CommandUnacknowledged; }
        }

        public string VolumeZoneState
        {
            get { return _zoneState.Volume.ToString(); }
        }

        public int Volume
        {
            get { return _zoneState.Volume; }
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

        public List<NavigationItem> NavigationItems
        {
            get
            {
                List<NavigationItem> items = new List<NavigationItem>();
                foreach (Zone zone in _zones)
                {
                    items.Add(new NavigationItem(this, zone.Id, zone.Name));
                }
                return items;
            }
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
            // TODO unsubscribe
        }

        public void Navigate(Address id)
        {
            Zone tempZone = FindZone(id);
            if (tempZone == null)
                return;

            Unsubscribe(_activeZone.Id);
            _activeZone = tempZone;
            Subscribe(_activeZone.Id);
            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
            // TODO unsubscribe
        }

        private Zone GetPreviousZone()
        {
            int index = _zones.IndexOf(_activeZone);
            if (index <= 0)
                return _zones[_zones.Count - 1];
            else
                return _zones[index - 1];

        }

        private Zone FindZone(Address id)
        {
            foreach (Zone zone in _zones)
            {
                if (zone.Id.Equals(id))
                    return zone;
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

        public void OnHierarchyActivated()
        {
            Subscribe(_activeZone.Id);
        }


        public void OnHierarchyDeactivated()
        {
            Unsubscribe(_activeZone.Id);
        }


        public void UpdateContext(IHierarchyContext context)
        {
            if ((context is ZoneContext) == false)
                return;

            ZoneContext newContext = context as ZoneContext;
            _zones = newContext._zones;
            _activeZone = _zones[0];
            _sources = newContext._sources;

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
        }

        #endregion


        /// <summary>
        /// Subscribe for notifications for all zones of this context
        /// </summary>
        public void Subscribe(Address id)
        {
            ServiceProxy.MonitorAndControlProxy.Monitor(id, ZoneUpdateNotification);
        }


        /// <summary>
        /// Unsubscribes for notifications for all zones of this context
        /// </summary>
        public void Unsubscribe(Address id)
        {
            ServiceProxy.MonitorAndControlProxy.RemoveMonitor(id, ZoneUpdateNotification);
        }

        public void UnsubscribeAll()
        {
            foreach (Zone zone in _zones)
            {
                Unsubscribe(zone.Id);
            }
        }

        public void ZoneUpdateNotification(object sender, ZoneStateEventArgs e)
        {
            SendOrPostCallback callback = delegate
            {
                _zoneState = new ZoneState(e.ZoneState);
                NotifyPropertyChanged(new PropertyChangedEventArgs(""));
            };
            _synchronizationContext.Post(callback, null);


        }

    }
}

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
 *   File Name:      ZoneContext.cs
 * 
 ***************************************************************************************************
 * 
 * 
 **************************************************************************************************/

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
    /// <summary>
    /// This is the logical context for the zone view of the viewer.
    /// It contains the logical context of the zones of a floor.
    /// </summary>
    public class ZoneContext : INotifyPropertyChanged, IHierarchyContext
    {
        #region Fields

        /// <summary>
        /// All zones of a floor.
        /// </summary>
        private List<Zone> _zones;

        /// <summary>
        /// The currently shown zone in the zone view.
        /// </summary>
        private Zone _activeZone;

        /// <summary>
        /// Thew zone state of the zone shown in the zone view.
        /// </summary>
        private ZoneState _zoneState;

        /// <summary>
        /// All available sources of the audio system.
        /// </summary>
        private List<Source> _sources = new List<Source>();

        /// <summary>
        /// Collection view to the sources
        /// </summary>
        private ListCollectionView _viewSources;

        /// <summary>
        /// The parent context. This is the floor  context.
        /// </summary>
        private IHierarchyContext _parent = null;

        /// <summary>
        /// The child context. No child context exists for the zone context.
        /// </summary>
        private IHierarchyContext _child = null;

        /// <summary>
        /// Visibility of the zone view.
        /// </summary>
        private Visibility _visibility = Visibility.Collapsed;

        /// <summary>
        /// Flag, used to suppress source selection changes.
        /// </summary>
        private bool _ignoreViewSelectionChange = false;

        /// <summary>
        /// Comamnd bindings for volume up and down command and power on/off command
        /// </summary>
        private CommandBindingCollection _bindings = new CommandBindingCollection();

        /// <summary>
        /// Used to dispatch server notifications to the GUI thread.
        /// </summary>
        private SynchronizationContext _synchronizationContext = null;

        private IDisposable _deferHandle = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="zones"></param>
        /// <param name="sources"></param>
        public ZoneContext(List<Zone> zones, List<Source> sources)
        {
            _synchronizationContext = SynchronizationContext.Current;

            this._zones = zones;
            this._activeZone = zones[0];
            this._zoneState = new ZoneState();
            this._sources = sources.ToArray().ToList();
            _viewSources = (ListCollectionView)CollectionViewSource.GetDefaultView(this._sources);
            _viewSources.CurrentChanged += new EventHandler(_viewSources_CurrentChanged);

            // defer update on creation in order not to get notifications form the source list
            //_deferHandle = _viewSources.DeferRefresh();

            CommandBinding binding = new CommandBinding(CustomCommands.VolumeUp, VolumeUpCommand_Executed, VolumeUpCommand_CanExecute);
            _bindings.Add(binding);
            binding = new CommandBinding(CustomCommands.VolumeDown, VolumeDownCommand_Executed, VolumeDownCommand_CanExecute);
            _bindings.Add(binding);
            binding = new CommandBinding(CustomCommands.Power, PowerCommand_Executed, PowerCommand_CanExecute);
            _bindings.Add(binding);

        }

        #endregion

        #region Command Handlers


        /// <summary>
        /// Command handler for volume up command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VolumeUpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            lock (this)
            {
                _zoneState.CommandUnacknowledged = true;
                ZoneState cmdState = new ZoneState(_zoneState);
                cmdState.Volume += 5;
                if (cmdState.Volume > ZoneState.VOLUME_MAXVALUE)
                    cmdState.Volume = ZoneState.VOLUME_MAXVALUE;
                NotifyPropertyChanged(new PropertyChangedEventArgs(""));

                ServiceProxy.MonitorAndControlProxy.SetZoneState(_activeZone.Id, cmdState);
            }
        }


        /// <summary>
        /// CanExecute handler for volume up command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        /// <summary>
        /// Command handler for volume down command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VolumeDownCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            lock (this)
            {
                _zoneState.CommandUnacknowledged = true;
                ZoneState cmdState = new ZoneState(_zoneState);
                cmdState.Volume -= 5;
                if (cmdState.Volume < ZoneState.VOLUME_MINVALUE)
                    cmdState.Volume = ZoneState.VOLUME_MINVALUE;
                NotifyPropertyChanged(new PropertyChangedEventArgs(""));

                ServiceProxy.MonitorAndControlProxy.SetZoneState(_activeZone.Id, cmdState);
            }
        }


        /// <summary>
        /// CanExecute handler for volume down command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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


        /// <summary>
        /// Command handler for power on/off command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PowerCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            lock (this)
            {
                _zoneState.CommandUnacknowledged = true;
                ZoneState cmdState = new ZoneState(_zoneState);
                cmdState.PowerStatus = !cmdState.PowerStatus;
                NotifyPropertyChanged(new PropertyChangedEventArgs(""));

                ServiceProxy.MonitorAndControlProxy.SetZoneState(_activeZone.Id, cmdState);
            }
        }


        /// <summary>
        /// CanExecute handler for power on/off command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PowerCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        #endregion



        #region Public Interface


        /// <summary>
        /// The command bindings of the zone control commands
        /// </summary>
        public CommandBindingCollection CommandBindingsDynamic
        {
            get { return _bindings; }
        }


        /// <summary>
        /// The name of the active zone.
        /// </summary>
        public string ZoneName
        {
            get { return _activeZone.Name; }
        }


        /// <summary>
        /// The state of the active zone.
        /// </summary>
        public ZoneState ZoneState
        {
            get { return _zoneState; }
        }


        /// <summary>
        /// The source selected for the active zone.
        /// </summary>
        public Address SourceZoneState
        {
            get { return _zoneState.Source; }

        }


        /// <summary>
        /// The power state of the active zone
        /// </summary>
        public bool PowerZoneState
        {
            get { return _zoneState.PowerStatus; }
            set 
            {
            }        
        }


        /// <summary>
        /// Returns the image, dependent on the power state.
        /// </summary>
        public BitmapImage PowerImage
        {
            get
            {
                if (_zoneState.PowerStatus)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), @"./Images/Loudspeaker.PNG");
                    return new BitmapImage(new Uri(path));
                }
                else
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), @"./Images/LoudspeakerOff.PNG");
                    return new BitmapImage(new Uri(path));
                }
            }
        }


        /// <summary>
        /// Retruns the image, dependent on zone quality and if the command is acklowleded or not.
        /// </summary>
        public BitmapImage NetworkState
        {
            get
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), @"./Images/OnlineAcknowledged.PNG");
                if (_zoneState.ZoneQuality == ZoneQuality.Offline)
                    path = Path.Combine(Directory.GetCurrentDirectory(), @"./Images/Offline.PNG");
                else if (_zoneState.CommandUnacknowledged)
                    path = Path.Combine(Directory.GetCurrentDirectory(), @"./Images/OnlineUnacknowledged.PNG");
                return new BitmapImage(new Uri(path));
            }
        }


        /// <summary>
        /// The quality of the active zone
        /// </summary>
        public ZoneQuality QualityZoneState
        {
            get { return _zoneState.ZoneQuality; }

        }


        /// <summary>
        /// The unacknowledged flag of the active zone.
        /// </summary>
        public bool UnackZoneState
        {
            get { return _zoneState.CommandUnacknowledged; }
        }


        /// <summary>
        /// Returns the zone state (Offline/Online and Ack/Unack) as string.
        /// </summary>
        public string ZoneStateMixed
        {
            get { return String.Format("{0}; {1}", _zoneState.ZoneQuality.ToString(), (_zoneState.CommandUnacknowledged ? "Unack" : "Ack")); }
        }


        /// <summary>
        /// The volume of the active zone.
        /// </summary>
        public string VolumeZoneState
        {
            get { return _zoneState.Volume.ToString(); }
        }


        /// <summary>
        /// The volume of the acative zone.
        /// </summary>
        public int Volume
        {
            get { return _zoneState.Volume; }
        }


        /// <summary>
        /// The image of the active zone.
        /// </summary>
        public BitmapImage ZoneImage
        {
            get
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), _activeZone.PicturePath);
                return new BitmapImage(new Uri(path));
            }
        }


        /// <summary>
        /// All audio sources
        /// </summary>
        public List<Source> Sources
        {
            get { return _sources; }
        }


        /// <summary>
        /// The list vollection view of the sources
        /// </summary>
        public ListCollectionView ViewSources
        {
            get { return _viewSources; }
        }


        /// <summary>
        /// The currently selected source.
        /// </summary>
        public Source SelectedSource
        {
            get
            {
                Source sourceItem = _sources.Find(delegate(Source source)
                {
                    return (source.Id.Equals(_zoneState.Source)) ? true : false;
                });
                return sourceItem;
            }
            set
            {
                if (_ignoreViewSelectionChange || _zoneState.Source.Equals(value.Id))
                    return;

                _zoneState.CommandUnacknowledged = true;
                ZoneState cmdState = new ZoneState(_zoneState);
                cmdState.Source = value.Id;
                ServiceProxy.MonitorAndControlProxy.SetZoneState(_activeZone.Id, cmdState);
            }
        }


        /// <summary>
        /// The floor plan coordinates of the active zone.
        /// </summary>
        public PointCollection FloorPlanCoordinates
        {
            get { return new PointCollection(_activeZone.FloorPlanCoordinates); }
        }


        /// <summary>
        /// The left position of the zone control
        /// </summary>
        public int ZoneControlCoordinateX
        {
            get { return (int)_activeZone.ZoneControlCoordinate.X; }
        }


        /// <summary>
        /// The top position of the zone control
        /// </summary>
        public int ZoneControlCoordinateY
        {
            get { return (int)_activeZone.ZoneControlCoordinate.Y; }
        }


        /// <summary>
        /// Called from parent context when the zone is instantiated.
        /// </summary>
        public void ZoneLoaded()
        {
            _ignoreViewSelectionChange = true;

            Subscribe(_activeZone.Id);
            NotifyPropertyChanged(new PropertyChangedEventArgs(""));

            _ignoreViewSelectionChange = false;
        }


        /// <summary>
        /// Called from parent context when the zone is deleted.
        /// </summary>
        public void ZoneUnloaded()
        {
            Unsubscribe(_activeZone.Id);
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            if (_deferHandle != null)
            {
                _deferHandle.Dispose();
                _deferHandle = null; 
            } 
            
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }        
        
        #endregion

        #region IHierarchyContext Members

        /// <summary>
        /// The name of the object, shown in the current hierarchy.
        /// Here, this is the name of the active zone.
        /// </summary>
        public string ObjectName
        {
            get { return _activeZone.Name; }
        }

        /// <summary>
        /// The address of the object, shown in the current hierarchy.
        /// Here this is the address of the current zone.
        /// </summary>
        public Address ObjectId
        {
            get { return _activeZone.Id; }
        }


        /// <summary>
        /// The parent hierarchy context.
        /// Here this is the floor context.
        /// </summary>
        public IHierarchyContext Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }


        /// <summary>
        /// The child hierarchy context.
        /// Here, there are no childs
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
        /// Here, this is the next zone to show.
        /// </summary>
        public bool HasNextObject
        {
            get { return true; }
        }


        /// <summary>
        /// The name of the next object.
        /// Here, this is the name of the next zone.
        /// </summary>
        public string NextObjectName
        {
            get { return GetNextZone().Name; }
        }


        /// <summary>
        /// Go to next object of this context.
        /// Here, this is the next zone.
        /// </summary>
        public void NextObject()
        {
            Navigate(GetNextZone().Id);
        }


        /// <summary>
        /// True, if the context has a previous object to show.
        /// Here, this is the previous zone to show.
        /// </summary>
        public bool HasPreviousObject
        {
            get { return true; }
        }


        /// <summary>
        /// The name of the previous object.
        /// Here, this is the name of the previous zone.
        /// </summary>
        public string PreviousObjectName
        {
            get { return GetPreviousZone().Name; }
        }


        /// <summary>
        /// Go to previous object of this context.
        /// Here, this is the previous zone.
        /// </summary>
        public void PreviousObject()
        {
            Navigate(GetPreviousZone().Id);
        }


        /// <summary>
        /// Navigate to a specified object of this context.
        /// </summary>
        /// <param name="id">The id of the object to navigate to. Here, this is the id of a zone.</param>
        public void Navigate(Address id)
        {
            Navigate(id, true);
        }


        /// <summary>
        /// Get all navigations objects of this context.
        /// Here, these are all zones of a floor.
        /// </summary>
        public List<NavigationItem> NavigationObjects
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


        /// <summary>
        /// Called, when this context got activated.
        /// i.e. the corresponding zone view was set to visible.
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
            Unsubscribe(_activeZone.Id);
        }


        /// <summary>
        /// Updates the context with the specified new zone context.
        /// </summary>
        /// <param name="context">The new zone context.</param>
        public void UpdateContext(IHierarchyContext context)
        {
            if ((context is ZoneContext) == false)
                return;

            _ignoreViewSelectionChange = true;

            ZoneContext newContext = context as ZoneContext;
            _zones = newContext._zones;
            _activeZone = _zones[0];
            _sources = newContext._sources;

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));

            _ignoreViewSelectionChange = false;

        }

        #endregion

        #region Non-Public Interface

        /// <summary>
        /// Navigate to a specified object of this context.
        /// </summary>
        /// <param name="id">The id of the object to navigate to. Here, this is the id of a zone.</param>
        /// <param name="unsubscribePreviousZone">Set to true, if previous zone must be unsubscribed first.</param>
        private void Navigate(Address id, bool unsubscribePreviousZone)
        {
            _ignoreViewSelectionChange = true;

            Zone tempZone = FindZone(id);
            if (tempZone == null)
                tempZone = _zones[0];

            if (unsubscribePreviousZone)
                Unsubscribe(_activeZone.Id);
            _activeZone = tempZone;
            Subscribe(_activeZone.Id);
            NotifyPropertyChanged(new PropertyChangedEventArgs(""));

            _ignoreViewSelectionChange = false;
        }


        /// <summary>
        /// Gets the next zone object of the zone context.
        /// </summary>
        /// <returns></returns>
        private Zone GetNextZone()
        {
            int index = _zones.IndexOf(_activeZone);
            if (index >= _zones.Count - 1)
                return _zones[0];
            else
                return _zones[index + 1];
        }


        /// <summary>
        /// Gets the previous zone object of the zone context.
        /// </summary>
        /// <returns></returns>
        private Zone GetPreviousZone()
        {
            int index = _zones.IndexOf(_activeZone);
            if (index <= 0)
                return _zones[_zones.Count - 1];
            else
                return _zones[index - 1];

        }


        /// <summary>
        /// Finds the specified zone object.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The found object; null if none found.</returns>
        private Zone FindZone(Address id)
        {
            foreach (Zone zone in _zones)
            {
                if (zone.Id.Equals(id))
                    return zone;
            }
            return null;
        }


        /// <summary>
        /// Subscribe for notifications for all zones of this context. Reads the zones state immediate.
        /// </summary>
        private void Subscribe(Address id)
        {
            _zoneState = ServiceProxy.MonitorAndControlProxy.GetZoneState(id);
            ServiceProxy.MonitorAndControlProxy.Monitor(id, ZoneUpdateNotification);
        }


        /// <summary>
        /// Unsubscribes for notifications for all zones of this context
        /// </summary>
        private void Unsubscribe(Address id)
        {
            ServiceProxy.MonitorAndControlProxy.RemoveMonitor(id, ZoneUpdateNotification);
        }


        /// <summary>
        /// Selection change handler for the source selection list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _viewSources_CurrentChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                ListCollectionView view = sender as ListCollectionView;
                if (view != null && view.CurrentItem != null && _ignoreViewSelectionChange == false)
                {
                    if (_zoneState.Source.Equals((view.CurrentItem as Source).Id))
                        return;

                    //_zoneState.CommandUnacknowledged = true;
                    //ZoneState cmdState = new ZoneState(_zoneState);
                    //cmdState.Source = (view.CurrentItem as Source).Id;
                    //ServiceProxy.MonitorAndControlProxy.SetZoneState(_activeZone.Id, cmdState);
                }
            }
        }

        #endregion

        #region Server Notification

        public void ZoneUpdateNotification(object sender, ZoneStateEventArgs e)
        {
            SendOrPostCallback callback = delegate
            {
                _ignoreViewSelectionChange = true;
                _zoneState = new ZoneState(e.ZoneState);
                NotifyPropertyChanged(new PropertyChangedEventArgs(""));
                _ignoreViewSelectionChange = false;
            };
            _synchronizationContext.Post(callback, null);
        }

        #endregion

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
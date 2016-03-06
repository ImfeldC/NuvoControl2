/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.ConfigurationService
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.05.2009
 *   File Name:      Configuration.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 12.05.2009, Bernhard Limacher: Implementation of the interface.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Common.Logging;

using NuvoControl.Common;       // required for ToString<T> list extension
using NuvoControl.Common.Configuration;
using NuvoControl.Server.Dal;

using System.ServiceModel;


namespace NuvoControl.Server.ConfigurationService
{
    /// <summary>
    /// This class implements the interface of the configuration service <see cref="IConfigure"/> and <see cref="IConfigureInternal"/>
    /// 
    /// The configuration service is a WCF-service, hosted as singleton.
    /// It defines functionality to read the actual configuration of the NuvoControl system.
    /// It defines functionality to modify the actual configuration of the NuvoControl system.
    /// It defines functionality to save the actual configuration of the NuvoControl system.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class ConfigurationService : IConfigureInternal, IDisposable
    {
        #region Fields

        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The active system configuration of the Nuvo Control system.
        /// </summary>
        private SystemConfiguration _systemConfiguration = null;

        /// <summary>
        /// The file name, containing the persistent system configuration.
        /// </summary>
        private string _configurationFile = null;
        private string _remoteConfigurationFile = null;

        /// <summary>
        /// Helper, to read the configuration.
        /// </summary>
        private ConfigurationLoader _configurationLoader = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configurationFile">The file name, containing the persistent system configuration.</param>
        public ConfigurationService(string configurationFile)
        {
            this._configurationFile = configurationFile;
            Initialize();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configurationFile">The file name, containing the persistent system configuration.</param>
        public ConfigurationService(string configurationFile, string remoteConfigurationFile)
        {
            this._configurationFile = configurationFile;
            this._remoteConfigurationFile = remoteConfigurationFile;
            Initialize();
        }

        #endregion

        #region IConfigure Members

        /// <summary>
        /// <see cref="IConfigure"/>
        /// </summary>
        public void RenewLease()
        {
            //_log.Trace(m=>m("Configuration Service; RenewLease()."));
        }


        /// <summary>
        /// <see cref="IConfigure"/>
        /// </summary>
        /// <returns></returns>
        public Graphic GetGraphicConfiguration()
        {
            _log.Trace(m=>m("Configuration Service; GetGraphicConfiguration()."));

            return _systemConfiguration.Graphic;
        }

        /// <summary>
        /// <see cref="IConfigure"/>
        /// </summary>
        /// <param name="picturePath"></param>
        /// <returns></returns>
        public NuvoImage GetImage(string picturePath)
        {
            _log.Trace(m => m("Configuration Service; GetImage( {0} ).", picturePath));
            NuvoImage img = null;
            try
            {
                img = new NuvoImage(picturePath);
                _log.Trace(m => m("Configuration Service; return image {0}.", img.ToString()));
            }
            catch (ArgumentException exc)
            {
                _log.Fatal(m => m("Cannot load image {0}! Exception={1}", picturePath, exc));
            }
            return img;
        }


        public Zone GetZoneHWConfiguration(Address zoneId)
        {
            Device device = _systemConfiguration.Hardware.getDevice(zoneId);
            foreach (Zone zone in device.Zones)
            {
                if (zone.Id == zoneId)
                {
                    return zone;
                }
            }
            return null;
        }

        /// <summary>
        /// <see cref="IConfigure"/>
        /// </summary>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        public ZoneGraphic GetZoneGraphicConfiguration(Address zoneId)
        {
            _log.Trace(m => m(String.Format("Configuration Service; GetZoneGrpahicConfiguration(ZoneId={0}).", zoneId.ToString())));

            List<ZoneGraphic> zones = new List<ZoneGraphic>();
            foreach (Floor floor in _systemConfiguration.Graphic.Building.Floors)
            {
                zones.AddRange((from zone in floor.Zones where zone.Id == zoneId select zone));
            }

            if (zones.Count == 0)
            {
                _log.ErrorFormat("No zone found for the specified address: {0}.", zoneId);
                throw new ArgumentException(String.Format("No zone found for the specified address: {0}.", zoneId));
            }
            if (zones.Count > 1)
            {
                _log.ErrorFormat("More than one zone found for the specified address: {0}.", zoneId);
                throw new ArgumentException(String.Format("More than one zone found for the specified address: {0}.", zoneId));
            }

            return zones[0];
        }


        public Source GetSourceHWConfiguration(Address sourceId)
        {
            Device device = _systemConfiguration.Hardware.getDevice(sourceId);
            foreach (Source source in device.Sources)
            {
                if (source.Id == sourceId)
                {
                    return source;
                }
            }
            return null;
        }

        public SourceGraphic GetSourceGraphicConfiguration(Address sourceId)
        {
            foreach (SourceGraphic source in _systemConfiguration.Graphic.Sources)
            {
                if (source.Id == sourceId)
                {
                    return source;
                }
            }
            return null;
        }


        /// <summary>
        /// <see cref="IConfigure"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Function GetFunction(SimpleId id)
        {
            _log.Trace(m => m(String.Format("Configuration Service; GetFunction(Id={0}).", id.ToString())));

            // Search function with the specified Guid
            foreach (Function function in _systemConfiguration.Functions)
            {
                if (function.Id == id)
                {
                    _log.Trace(m => m(String.Format("Function with Id={0} found: {1}.", id.ToString(), function.ToString())));
                    return function;
                }
            }

            _log.Warn(m => m(String.Format("Function with Id={0} NOT found: {1}.", id.ToString(), _systemConfiguration.Functions.ToString<Function>(" / "))));
            return null;
        }

        /// <summary>
        /// <see cref="IConfigure"/>
        /// </summary>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        public List<Function> GetFunctions(Address zoneId)
        {
            _log.Trace(m => m(String.Format("Configuration Service; GetFunctions(ZoneId={0}).", zoneId.ToString())));

            // Search functions of the specified zone
            List<Function> zoneFunctions = new List<Function>();
            foreach( Function function in _systemConfiguration.Functions )
            {
                if( function.ZoneId == zoneId )
                {
                    zoneFunctions.Add( function );
                }
            }

            _log.Trace(m => m(String.Format("Functions of ZoneId={0}: {1}.", zoneId.ToString(), zoneFunctions.ToString<Function>(" / "))));
            return zoneFunctions;
        }

        /// <summary>
        /// <see cref="IConfigure"/>
        /// </summary>
        /// <param name="newFunction"></param>
        /// <returns></returns>
        public bool AddFunction(Function newFunction)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region IConfigureInternal Members

        /// <summary>
        /// <see cref="IConfigureInternal"/>
        /// </summary>
        public SystemConfiguration SystemConfiguration
        {
            get { return _systemConfiguration; }
        }

        /// <summary>
        /// Validates the loaded system configuration. <see cref="IConfigureInternal"/>
        /// </summary>
        /// <returns>True, if the system configuration is valid. Otherwise false.</returns>
        public bool Validate()
        {
            lock (this)
            {
                try
                {
                    if (_systemConfiguration == null)
                        Initialize();

                    if (ValidateDevices() == false)
                        return false;
                    if (ValidateGraphicFloorZones() == false)
                        return false;
                    if (ValidateGraphicSources() == false)
                        return false;
                    if (ValidateFunctionZone() == false)
                        return false;
                    if (ValidateAlarmFunctionSource() == false)
                        return false;

                    return true;
                }
                catch (Exception exc)
                {
                    _log.Error("Validation of the xml configuration file failed.", exc);
                    return false;
                }
            }
        }

        /// <summary>
        /// Check if configuration changed. If yes, reload them.
        /// </summary>
        /// <returns>true if configuration changed.</returns>
        public bool CheckConfiguration()
        {
            return RefreshConfiguration();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Called, before the GC removes the object.
        /// </summary>
        public void Dispose()
        {
            Cleanup();
        }

        #endregion

        #region Non-Public Interface

        /// <summary>
        /// Initialization, reads the configuration from the DAL
        /// </summary>
        private void Initialize()
        {
            _configurationLoader = new ConfigurationLoader(_configurationFile);
            if (_remoteConfigurationFile != null && _remoteConfigurationFile != "" )
            {
                _configurationLoader.AppendConfiguration(_remoteConfigurationFile);
            }
            _systemConfiguration = _configurationLoader.GetConfiguration();
            Validate();
        }

        /// <summary>
        /// Check if configuration file changed. If yes, reload them.
        /// </summary>
        /// <returns>true if configuration file changed and reloaded.</returns>
        private bool RefreshConfiguration()
        {
            bool bRefresh = _configurationLoader.RefreshConfiguration();
            if (bRefresh)
            {
                _systemConfiguration = _configurationLoader.GetConfiguration();
                Validate();
            }
            return bRefresh;
        }

        /// <summary>
        /// Validates the id of the devices.
        /// </summary>
        /// <returns>True, if the id's are valid and distinct.</returns>
        private bool ValidateDevices()
        {
            List<int> deviceIds = (from device in _systemConfiguration.Hardware.Devices select device.Id).ToList<int>();
            return IsDistinct(deviceIds);
        }

        /// <summary>
        /// Validates the zones of the floors. Their Id must match one defined by the hardware and they must be distinct.
        /// </summary>
        /// <returns>True, if the id's are valid. Otherwise false.</returns>
        private bool ValidateGraphicFloorZones()
        {
            List<Address> graphicZoneIds = new List<Address>();
            foreach (Floor floor in _systemConfiguration.Graphic.Building.Floors)
            {
                graphicZoneIds.AddRange(from zone in floor.Zones select zone.Id);
            }

            if (IsDistinct(graphicZoneIds) == false)
                return false;

            if (DoAllZonesExist(graphicZoneIds) == false)
                return false;

            return true;
        }

        /// <summary>
        /// Validates the sources of the graphic configuration. Their Id must match one defined by the hardware and they must be distinct.
        /// </summary>
        /// <returns>True, if the id's are valid. Otherwise false.</returns>
        private bool ValidateGraphicSources()
        {
            List<Address> graphicSourceIds = new List<Address>();
            graphicSourceIds.AddRange((from source in _systemConfiguration.Graphic.Sources select source.Id).ToList<Address>());

            if (IsDistinct(graphicSourceIds) == false)
                return false;

            if (DoAllSourcesExist(graphicSourceIds) == false)
                return false;

            return true;
        }

        /// <summary>
        /// Validates the zones of the functions. Their Id must match one defined by the hardware.
        /// </summary>
        /// <returns>True, if the id's are valid. Otherwise false.</returns>
        private bool ValidateFunctionZone()
        {
            List<Address> functionZoneIds = (from function in _systemConfiguration.Functions select function.ZoneId).ToList<Address>();

            if (DoAllZonesExist(functionZoneIds) == false)
                return false;

            return true;
        }

        /// <summary>
        /// Validates the sources of the alarm functions. Their Id must match one defined by the hardware.
        /// </summary>
        /// <returns>True, if the id's are valid. Otherwise false.</returns>
        private bool ValidateAlarmFunctionSource()
        {
            List<Address> alarmFunctionSourceIds = new List<Address>();
            foreach (Function func in _systemConfiguration.Functions)
            {
                if (func is AlarmFunction)
                    alarmFunctionSourceIds.Add(((AlarmFunction)func).SourceId);
            }

            if (DoAllSourcesExist(alarmFunctionSourceIds) == false)
                return false;

            return true;
        }

        /// <summary>
        /// Checks, if the list contains distinct objects.
        /// </summary>
        /// <param name="ids">The list to check.</param>
        /// <returns>True, if all elements are distinct. Otherwise false.</returns>
        private static bool IsDistinct(List<int> ids)
        {
            Dictionary<int, int> distinctMap = new Dictionary<int, int>();
            foreach (int id in ids)
            {
                if (distinctMap.ContainsKey(id))
                    return false;

                distinctMap.Add(id, id);
            }
            return true;
        }

        /// <summary>
        /// Checks, if the address list contains distinct address objects.
        /// </summary>
        /// <param name="ids">The list to check</param>
        /// <returns>True, if all elements are distinct. Otherwise false.</returns>
        private static bool IsDistinct(List<Address> ids)
        {
            Dictionary<Address, Address> distinctMap = new Dictionary<Address, Address>();
            foreach (Address address in ids)
            {
                if (distinctMap.ContainsKey(address))
                    return false;

                distinctMap.Add(address, address);
            }
            return true;
        }

        /// <summary>
        /// Checks, if the zone addresses all exist within the hardware configuration.
        /// </summary>
        /// <param name="ids">The addresses to check.</param>
        /// <returns>True, if all addresses exist. Otherwise false.</returns>
        private bool DoAllZonesExist(List<Address> ids)
        {
            foreach (Address address in ids)
            {
                Device device = DoesDeviceExist(address);
                if (device == null)
                    return false;

                List<Zone> zoneIds = (from zoneId in device.Zones where zoneId.Id.ObjectId == address.ObjectId select zoneId).ToList<Zone>();
                if (zoneIds.Count == 0)
                    return false;

                if (zoneIds.Count > 1)
                {
                    _log.ErrorFormat("There is more than one zone with the same Id = {0} in the same device = {1}.", address.ObjectId, address.DeviceId);
                    throw new Exception(String.Format("There is more than one zone with the same Id = {0} in the same device {1}.", address.DeviceId, address.DeviceId));
                }
            }
            return true;
        }

        /// <summary>
        /// Checks, if the source addresses all exist within the hardware configuration.
        /// </summary>
        /// <param name="ids">The addresses to check.</param>
        /// <returns>True, if all addresses exist. Otherwise false.</returns>
        private bool DoAllSourcesExist(List<Address> ids)
        {
            foreach (Address address in ids)
            {
                Device device = DoesDeviceExist(address);
                if (device == null)
                    return false;

                List<Source> sourceIds = (from sourceId in device.Sources where sourceId.Id.ObjectId == address.ObjectId select sourceId).ToList<Source>();
                if (sourceIds.Count == 0)
                    return false;

                if (sourceIds.Count > 1)
                {
                    _log.ErrorFormat("There is more than one source with the same Id = {0} in the same device = {1}.", address.ObjectId, address.DeviceId);
                    throw new Exception(String.Format("There is more than one source with the same Id = {0} in the same device {1}.", address.DeviceId, address.DeviceId));
                }
            }
            return true;
        }

        /// <summary>
        /// Checks, if the device exists within the hardware configuration.
        /// </summary>
        /// <param name="address">The addresses to check.</param>
        /// <returns>True, if the device id exisits. Otherwise false.</returns>
        private Device DoesDeviceExist(Address address)
        {
            List<Device> devices =
                (from device in _systemConfiguration.Hardware.Devices where device.Id.Equals(address.DeviceId) select device).ToList<Device>();
            if (devices.Count == 0)
                return null;

            if (devices.Count > 1)
            {
                _log.ErrorFormat("There is more than one device with the same Id = {0} in the configuration.", address.DeviceId);
                throw new Exception(String.Format("There is more than one device with the same Id = {0} in the configuration.", address.DeviceId));
            }
            return devices[0];
        }


        /// <summary>
        /// Removes all state.
        /// </summary>
        private void Cleanup()
        {
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/
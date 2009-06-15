/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.Service
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

using Common.Logging;

using NuvoControl.Common.Interfaces;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.Dal;

using System.ServiceModel;


namespace NuvoControl.Server.Service.Configuration
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ConfigurationService : IConfigureInternal, IDisposable
    {
        #region Fields

        private static ILog _log = LogManager.GetCurrentClassLogger();

        private SystemConfiguration _systemConfiguration = null;
        private string _configurationFile = null;
        private ConfigurationLoader _configurationLoader = null;

        #endregion

        #region Constructors

        public ConfigurationService(string configurationFile)
        {
            this._configurationFile = configurationFile;
            Initialize();
        }

        #endregion

        #region IConfigure Members

        public Graphic GetGraphicConfiguration()
        {
            return _systemConfiguration.Graphic;
        }

        public Zone GetZoneKonfiguration(Address zoneId)
        {
            List<Zone> zones = new List<Zone>();
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

        public Function GetFunction(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<Function> GetFunctions(Address zoneId)
        {
            throw new NotImplementedException();
        }

        public bool AddFunction(Function newFunction)
        {
            throw new NotImplementedException();
        }


        #endregion

        #region IConfigureInternal Members

        public SystemConfiguration SystemConfiguration
        {
            get { return _systemConfiguration; }
        }

        /// <summary>
        /// Validates the loaded system configuration.
        /// </summary>
        /// <returns>True, if the system configuration is valid. Otherwise false.</returns>
        public bool Validate()
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

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Cleanup();
        }

        #endregion

        #region Non-Public Interface

        private void Initialize()
        {
            _configurationLoader = new ConfigurationLoader(_configurationFile);
            _systemConfiguration = _configurationLoader.GetConfiguration();
            Validate();
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
                NuvoEssentia device = DoesDeviceExist(address);
                if (device == null)
                    return false;

                List<int> zoneIds = (from zoneId in device.Zones where zoneId == address.ObjectId select zoneId).ToList<int>();
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
                NuvoEssentia device = DoesDeviceExist(address);
                if (device == null)
                    return false;

                List<int> sourceIds = (from sourceId in device.Sources where sourceId == address.ObjectId select sourceId).ToList<int>();
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
        private NuvoEssentia DoesDeviceExist(Address address)
        {
            List<NuvoEssentia> devices =
                (from device in _systemConfiguration.Hardware.Devices where device.Id.Equals(address.DeviceId) select device).ToList<NuvoEssentia>();
            if (devices.Count == 0)
                return null;

            if (devices.Count > 1)
            {
                _log.ErrorFormat("There is more than one device with the same Id = {0} in the configuration.", address.DeviceId);
                throw new Exception(String.Format("There is more than one device with the same Id = {0} in the configuration.", address.DeviceId));
            }
            return devices[0];
        }


        private void Cleanup()
        {
        }

        #endregion


    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
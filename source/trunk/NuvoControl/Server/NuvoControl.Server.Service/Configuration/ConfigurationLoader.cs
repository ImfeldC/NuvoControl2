/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.Service
 *   Author:         Bernhard Limacher
 *   Creation Date:  14.05.2009
 *   File Name:      ConfigurationLoader.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 14.05.2009, Bernhard Limacher: First implmentation.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Drawing;

using Common.Logging;

using NuvoControl.Common.Configuration;


namespace NuvoControl.Server.Service.Configuration
{
    /// <summary>
    /// This class contains the functionality to load the Nuvo Control XML configuration file into a XDocument object.
    /// In addition, it contains validation functions for the XDocument object.
    /// And it contains the functionality to convert the XDocument into serialzable objects (Data Transfer Objects)
    /// to be used all over the application.
    /// </summary>
    public class ConfigurationLoader
    {
        #region Fields

        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Represents the XML file
        /// </summary>
        private XDocument _configuration = null;

        /// <summary>
        /// The converted Nuvo Control system configuration. These is a hierarchy of data transfer objects.
        /// </summary>
        private SystemConfiguration _systemConfiguration = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">XML file containing the configuration data.</param>
        public ConfigurationLoader(string file)
        {   
            _configuration = XDocument.Load(file);
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// Validates the loaded system configuration.
        /// </summary>
        /// <returns>True, if the system configuration is valid. Otherwise false.</returns>
        public bool Validate()
        {
            try
            {
                if (_systemConfiguration == null)
                    ReadSystemConfiguration();

                if (ValidateVersion() == false)
                    return false;
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

        /// <summary>
        /// Returns the system configuration.
        /// If it is not yet converted from the XML file, it will be done immediate.
        /// </summary>
        /// <returns></returns>
        public SystemConfiguration GetConfiguration()
        {
            if (_systemConfiguration == null)
                ReadSystemConfiguration();

            return _systemConfiguration;
        }

        #endregion

        #region Non-Public Interface

        /// <summary>
        /// Creates the system configuration based on the XML configuration file.
        /// </summary>
        private void ReadSystemConfiguration()
        {
            List<Function> functions = new List<Function>();
            functions.AddRange(ReadSleepFunctions().Cast<Function>());
            functions.AddRange(ReadAlarmFunctions().Cast<Function>());

            _systemConfiguration = new SystemConfiguration(
                new Hardware(ReadDevices()),
                new Graphic(new Building(ReadFloorsOfGraphic()), ReadSourcesOfGraphic()),
                functions);
        }

        /// <summary>
        /// Reads and creates the devices based on the XML configuration file.
        /// </summary>
        /// <returns>The created devices.</returns>
        private List<NuvoEssentia> ReadDevices()
        {
            IEnumerable<NuvoEssentia> nuvoDevices =
                from device in _configuration.Root.Element("Configuration").Element("Hardware").Elements("NuvoEssentia")
                select new NuvoEssentia(
                    (int)device.Attribute("Id"),
                    new Communication((string)device.Element("Communication").Attribute("Port"),
                        (int)device.Element("Communication").Attribute("BaudRate"),
                        (int)device.Element("Communication").Attribute("DataBits"),
                        (int)device.Element("Communication").Attribute("ParityBit"),
                        (string)device.Element("Communication").Attribute("ParityMode")),    
                    new Protocol((string)device.Element("ProtocolDriver").Attribute("Name"),
                        (string)device.Element("ProtocolDriver").Attribute("AssemblyName"),
                        (string)device.Element("ProtocolDriver").Attribute("ClassName")),
                    (from zone in device.Element("Zones").Elements("Zone") select (int)zone.Attribute("Id")).ToList<int>(),
                    (from source in device.Element("Sources").Elements("Source") select (int)source.Attribute("Id")).ToList<int>()
                    );

            return nuvoDevices.ToList<NuvoEssentia>();
        }

        /// <summary>
        /// Reads and creates the floor objects based on the XML configuration file.
        /// </summary>
        /// <returns>The created floor configuration objects.</returns>
        private List<Floor> ReadFloorsOfGraphic()
        {
            IEnumerable<Floor> floors =
                from floor in _configuration.Root.Element("Configuration").Element("Graphic").Element("Building").Elements("Floor")
                select new Floor(
                    (string)floor.Attribute("Name"),
                    (string)floor.Element("FloorPlan").Attribute("RelativePath"),
                    (string)floor.Element("FloorPlan").Attribute("PictureType"),
                    (from zone in floor.Element("Zones").Elements("Zone")
                     select new Zone(
                        new Address(int.Parse(((string)zone.Attribute("Id")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[0]),
                            int.Parse(((string)zone.Attribute("Id")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[1])),
                        (string)zone.Attribute("Name"),
                        (string)zone.Element("Picture").Attribute("RelativePath"),
                        (string)zone.Element("Picture").Attribute("PictureType"),
                        (from pt in zone.Element("FloorPlanCoordinates").Elements("Point")
                         select new Point((int)pt.Attribute("x"), (int)pt.Attribute("y"))).ToList<Point>())).ToList<Zone>()
                     );

            return floors.ToList<Floor>();
        }

        /// <summary>
        /// Reads and creates the source objects based on the XML configuration file.
        /// </summary>
        /// <returns>The created source configuration objects.</returns>
        private List<Source> ReadSourcesOfGraphic()
        {
            IEnumerable<Source> sources =
                from source in _configuration.Root.Element("Configuration").Element("Graphic").Element("Sources").Elements("Source")
                select new Source(
                    new Address(int.Parse(((string)source.Attribute("Id")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[0]),
                        int.Parse(((string)source.Attribute("Id")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[1])),
                    (string)source.Attribute("Name"),
                    (string)source.Element("Picture").Attribute("RelativePath"),
                    (string)source.Element("Picture").Attribute("PictureType")
                     );

            return sources.ToList<Source>();
        }

        /// <summary>
        /// Reads and creates the sleep function objects based on the XML configuration file.
        /// </summary>
        /// <returns>The created sleep function configuration objects.</returns>
        private List<SleepFunction> ReadSleepFunctions()
        {
            IEnumerable<SleepFunction> functions =
                from function in _configuration.Root.Element("Configuration").Element("Functions").Elements("SleepFunction")
                select new SleepFunction(
                    (Guid)function.Attribute("Id"),
                    new Address(int.Parse(((string)function.Attribute("ZoneId")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[0]),
                        int.Parse(((string)function.Attribute("ZoneId")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[1])),
                    new TimeSpan(0, (int)function.Attribute("SleepDuration"), 0),
                    TimeSpan.Parse((string)function.Element("Validity").Attribute("ActiveFrom")),
                    TimeSpan.Parse((string)function.Element("Validity").Attribute("ActiveTo"))
                     );

            return functions.ToList<SleepFunction>();
        }

        /// <summary>
        /// Reads and creates the alarm function objects based on the XML configuration file.
        /// </summary>
        /// <returns>The created alarm function configuration objects.</returns>
        private List<AlarmFunction> ReadAlarmFunctions()
        {
            IEnumerable<AlarmFunction> functions =
                from function in _configuration.Root.Element("Configuration").Element("Functions").Elements("AlarmFunction")
                select new AlarmFunction(
                    (Guid)function.Attribute("Id"),
                    new Address(int.Parse(((string)function.Attribute("ZoneId")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[0]),
                        int.Parse(((string)function.Attribute("ZoneId")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[1])),
                    new Address(int.Parse(((string)function.Attribute("SourceId")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[0]),
                        int.Parse(((string)function.Attribute("SourceId")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[1])),
                    TimeSpan.Parse((string)function.Attribute("AlarmTime")),
                    new TimeSpan(0, (int)function.Attribute("AlarmDuration"), 0),
                    (from day in function.Element("Validity").Element("Days").Elements("Day")
                     select (DayOfWeek)Enum.Parse(typeof(DayOfWeek), (string)day.Attribute("Name"))).ToList<DayOfWeek>()
                    );

            return functions.ToList<AlarmFunction>();
        }

        /// <summary>
        /// Validates the version of the XML file. It msut match the current code base.
        /// </summary>
        /// <returns>True, if version is valid. Otherwise false.</returns>
        private bool ValidateVersion()
        {
            if (((string)_configuration.Root.Element("Configuration").Attribute("Version")).Equals(SystemConfiguration.VERSION))
                return true;
            else
                return false;
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

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
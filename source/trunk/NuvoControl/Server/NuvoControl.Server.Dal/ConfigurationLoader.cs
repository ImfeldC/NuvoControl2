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
using System.Windows;

using Common.Logging;

using NuvoControl.Common.Configuration;


namespace NuvoControl.Server.Dal
{
    /// <summary>
    /// This class contains the functionality to load the Nuvo Control XML configuration file into a XDocument object.
    /// It checks the version of the XML file and
    /// it contains the functionality to convert the XDocument into the configuration data objects, which describe the system.
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
        /// The converted Nuvo Control system configuration. These is a hierarchy of data structurer objects.
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
        /// Validates the version of the XML file.
        /// </summary>
        /// <returns>True, if the version is valid. Otherwise false.</returns>
        public bool Validate()
        {
            try
            {
                if (_systemConfiguration == null)
                    ReadSystemConfiguration();

                if (ValidateVersion() == false)
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
                new Graphic(
                    new Building(
                        new Address(int.Parse(((string)_configuration.Root.Element("Configuration").Element("Graphic").Element("Building").Attribute("Id")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[0]),
                          int.Parse(((string)_configuration.Root.Element("Configuration").Element("Graphic").Element("Building").Attribute("Id")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[1])),
                         (string)_configuration.Root.Element("Configuration").Element("Graphic").Element("Building").Attribute("Name"),
                          ReadFloorsOfGraphic(),
                         (string)_configuration.Root.Element("Configuration").Element("Graphic").Element("Building").Element("Picture").Attribute("RelativePath"),
                         (string)_configuration.Root.Element("Configuration").Element("Graphic").Element("Building").Element("Picture").Attribute("PictureType")),
                    ReadSourcesOfGraphic()),
                functions);
        }


        /// <summary>
        /// Reads and creates the devices based on the XML configuration file.
        /// </summary>
        /// <returns>The created devices.</returns>
        private List<Device> ReadDevices()
        {
            IEnumerable<Device> nuvoDevices =
                from device in _configuration.Root.Element("Configuration").Element("Hardware").Elements("NuvoEssentia")
                select new Device(
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

            return nuvoDevices.ToList<Device>();
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
                    new Address(int.Parse(((string)floor.Attribute("Id")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[0]),
                      int.Parse(((string)floor.Attribute("Id")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[1])),
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
        /// Validates the version of the XML file. It must match the current code base.
        /// </summary>
        /// <returns>True, if version is valid. Otherwise false.</returns>
        private bool ValidateVersion()
        {
            if (((string)_configuration.Root.Element("Configuration").Attribute("Version")).Equals(SystemConfiguration.VERSION))
                return true;
            else
                return false;
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
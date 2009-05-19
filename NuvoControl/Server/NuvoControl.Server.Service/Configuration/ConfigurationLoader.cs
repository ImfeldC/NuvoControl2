/**************************************************************************************************
 * 
 *   Copyright (C) Siemens AG 2006 All Rights Reserved. Confidential
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

using NuvoControl.Common.Configuration;


namespace NuvoControl.Server.Service.Configuration
{
    public class ConfigurationLoader
    {
        #region Fields

        private XDocument _configuration = null;
        private SystemConfiguration _systemConfiguration = null;

        #endregion

        #region Constructors

        public ConfigurationLoader(string file)
        {   
            _configuration = XDocument.Load(file);
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="IOException"
        public bool Validate()
        {
            if (_systemConfiguration == null)
                ReadSystemConfiguration();
            return true;

            //ValidateVersion();
            //ValidateGraphicFloorZones();
            //ValidateGraphicSources();
            //ValidateSleepFunctionZone();
            //ValidateAlarmFunctionZone();
            //ValidateAlarmFunctionSource();
        }


        public SystemConfiguration GetConfiguration()
        {
            if (_systemConfiguration == null)
                ReadSystemConfiguration();

            return _systemConfiguration;
        }

        #endregion

        #region Non-Public Interface

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


        private List<NuvoEssentia> ReadDevices()
        {
            IEnumerable<NuvoEssentia> nuvoDevices =
                from device in _configuration.Root.Element("Configuration").Element("Hardware").Elements("NuvoEssentia")
                select new NuvoEssentia(
                    (int)device.Attribute("Id"),
                    (string)device.Element("Communication").Attribute("Port"),
                    (int)device.Element("Communication").Attribute("BaudRate"),
                    (int)device.Element("Communication").Attribute("DataBits"),
                    (int)device.Element("Communication").Attribute("ParityBit"),
                    (string)device.Element("Communication").Attribute("ParityMode"),
                    new ProtocolDriver(device.Element("ProtocolDriver").Attribute("Name"), device.Element("ProtocolDriver").Attribute("AssemblyName"), device.Element("ProtocolDriver").Attribute("ClassName")),
                    (from zone in device.Element("Zones").Elements("Zone") select (int)zone.Attribute("Id")).ToList<int>(),
                    (from source in device.Element("Sources").Elements("Source") select (int)source.Attribute("Id")).ToList<int>()
                    );

            return nuvoDevices.ToList<NuvoEssentia>();
        }


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
                        new UniqueZoneId(int.Parse(((string)zone.Attribute("Id")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[0]),
                            int.Parse(((string)zone.Attribute("Id")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[1])),
                        (string)zone.Attribute("Name"),
                        (string)zone.Element("Picture").Attribute("RelativePath"),
                        (string)zone.Element("Picture").Attribute("PictureType"),
                        (from pt in zone.Element("FloorPlanCoordinates").Elements("Point")
                         select new Point((int)pt.Attribute("x"), (int)pt.Attribute("y"))).ToList<Point>())).ToList<Zone>()
                     );

            return floors.ToList<Floor>();
        }


        private List<Source> ReadSourcesOfGraphic()
        {
            IEnumerable<Source> sources =
                from source in _configuration.Root.Element("Configuration").Element("Graphic").Element("Sources").Elements("Source")
                select new Source(
                    new UniqueSourceId(int.Parse(((string)source.Attribute("Id")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[0]),
                        int.Parse(((string)source.Attribute("Id")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[1])),
                    (string)source.Attribute("Name"),
                    (string)source.Element("Picture").Attribute("RelativePath"),
                    (string)source.Element("Picture").Attribute("PictureType")
                     );

            return sources.ToList<Source>();
        }


        private List<SleepFunction> ReadSleepFunctions()
        {
            IEnumerable<SleepFunction> functions =
                from function in _configuration.Root.Element("Configuration").Element("Functions").Elements("SleepFunction")
                select new SleepFunction(
                    (Guid)function.Attribute("Id"),
                    new UniqueZoneId(int.Parse(((string)function.Attribute("ZoneId")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[0]),
                        int.Parse(((string)function.Attribute("ZoneId")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[1])),
                    new TimeSpan(0, (int)function.Attribute("SleepDuration"), 0),
                    TimeSpan.Parse((string)function.Element("Validity").Attribute("ActiveFrom")),
                    TimeSpan.Parse((string)function.Element("Validity").Attribute("ActiveTo"))
                     );

            return functions.ToList<SleepFunction>();
        }


        private List<AlarmFunction> ReadAlarmFunctions()
        {
            IEnumerable<AlarmFunction> functions =
                from function in _configuration.Root.Element("Configuration").Element("Functions").Elements("AlarmFunction")
                select new AlarmFunction(
                    (Guid)function.Attribute("Id"),
                    new UniqueZoneId(int.Parse(((string)function.Attribute("ZoneId")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[0]),
                        int.Parse(((string)function.Attribute("ZoneId")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[1])),
                    new UniqueSourceId(int.Parse(((string)function.Attribute("SourceId")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[0]),
                        int.Parse(((string)function.Attribute("SourceId")).Split(new char[] { SystemConfiguration.ID_SEPARATOR })[1])),
                    TimeSpan.Parse((string)function.Attribute("AlarmTime")),
                    new TimeSpan(0, (int)function.Attribute("AlarmDuration"), 0),
                    (from day in function.Element("Validity").Element("Days").Elements("Day")
                     select (DayOfWeek)Enum.Parse(typeof(DayOfWeek), (string)day.Attribute("Name"))).ToList<DayOfWeek>()
                    );

            return functions.ToList<AlarmFunction>();
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
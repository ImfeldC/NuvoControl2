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
using System.Net;
using System.Security.Cryptography;


namespace NuvoControl.Server.Dal
{
    /// <summary>
    /// This class contains the functionality to load the Nuvo Control XML configuration file into a XDocument object.
    /// 
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
        /// Represents the XML filename
        /// </summary>
        private string _configurationFilename = null;
        /// <summary>
        /// Represents the modification date and time of the XML file
        /// </summary>
        private DateTime _configurationFileWriteDateTime = new DateTime(1900, 1, 1);
        /// <summary>
        /// Contains the MD5 hash of the configuration file
        /// </summary>
        private byte[] _configurationFileHash = null;

        /// <summary>
        /// Represents the XML filename to append
        /// </summary>
        private string _appendConfigurationFilename = null;
        /// <summary>
        /// Represents the modification date and time of the XML file to append
        /// </summary>
        private DateTime _appendConfigurationFileWriteDateTime = new DateTime(1900, 1, 1);
        /// <summary>
        /// Contains the MD5 hash of the configuration file to append
        /// </summary>
        private byte[] _appendConfigurationFileHash =  null;

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
            _configurationFilename = file;
            _systemConfiguration = null;    // throw away existing converted configuration
            ReadXMLFiles();
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

        /// <summary>
        /// Adds the specified configuration to the existing configuration.
        /// </summary>
        /// <param name="file">XML file containing the configuration data to append.</param>
        public void AppendConfiguration(string file)
        {
            _appendConfigurationFilename = file;  
            _systemConfiguration = null;    // throw away existing converted configuration
            ReadXMLFiles();
        }

        /// <summary>
        /// Refresh (reload) configuration if at least one configuration file has changed.
        /// The method compares the modification date/time to determine if a refresh (reload) is required.
        /// </summary>
        /// <returns>true if one or more file modification date/time has changed.</returns>
        public bool RefreshConfiguration()
        {
            bool bChanged = CheckXMLFiles();

            if (bChanged)
            {
                _systemConfiguration = null;    // throw away existing converted configuration
                ReadXMLFiles();
            }

            return bChanged;
        }

        #endregion

        #region Non-Public Interface

        /// <summary>
        /// Reads the XML document(s).
        /// </summary>
        private void ReadXMLFiles()
        {
            _configuration = XDocument.Load(_configurationFilename);
            _configurationFileWriteDateTime = File.GetLastWriteTime(_configurationFilename);
            _configurationFileHash = calculateHash(_configurationFilename);
            _log.Trace(m => m("\nXML Configuration {0} loaded. GetLastWriteTime={1} calculateHash={2}", _configurationFilename, _configurationFileWriteDateTime.ToString(), ByteArrayToString(_configurationFileHash)));
            //Console.WriteLine("\nXML Configuration {0} loaded. GetLastWriteTime={1} calculateHash={2}", _configurationFilename, _configurationFileWriteDateTime.ToString(), ByteArrayToString(_configurationFileHash)));

            if (_appendConfigurationFilename != null)
            {
                XDocument appendConfiguration = XDocument.Load(_appendConfigurationFilename);

                try
                {
                    // Load configuration from remote server (URI)
                    Uri myUri = new Uri(_appendConfigurationFilename);
                    // Creates an HttpWebRequest for the specified URL. 
                    HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(myUri);
                    HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                    _appendConfigurationFileWriteDateTime = myHttpWebResponse.LastModified;
                    _appendConfigurationFileHash = null;
                    myHttpWebResponse.Close();
                    _log.Trace(m => m("\nXML Configuration {0} from remote added. GetLastWriteTime={1} calculateHash={2}", _appendConfigurationFilename, _appendConfigurationFileWriteDateTime.ToString(), ByteArrayToString(_appendConfigurationFileHash) ));
                    //Console.WriteLine("\nXML Configuration {0} from remote added. GetLastWriteTime={1} calculateHash={2}", _appendConfigurationFilename, _appendConfigurationFileWriteDateTime.ToString(), ByteArrayToString(_appendConfigurationFileHash) ));
                }
                catch (UriFormatException ex)
                {
                    // Load configuration from local server
                    _appendConfigurationFileWriteDateTime = File.GetLastWriteTime(_appendConfigurationFilename);
                    _appendConfigurationFileHash = calculateHash(_appendConfigurationFilename);
                    Console.WriteLine("\nXML Configuration {0} added. GetLastWriteTime={1} calculateHash={2}", _appendConfigurationFilename, _appendConfigurationFileWriteDateTime.ToString(), ByteArrayToString(_appendConfigurationFileHash));
                }

                // Add Functions and Devices
                if (appendConfiguration.Root.Element("Configuration").Element("Hardware") != null)
                {
                    _configuration.Root.Element("Configuration").Element("Hardware").Add(appendConfiguration.Root.Element("Configuration").Element("Hardware").Elements());
                }
                _configuration.Root.Element("Configuration").Element("Functions").Add(appendConfiguration.Root.Element("Configuration").Element("Functions").Elements());
            }
        }

        /// <summary>
        /// Compares (check) the modification date of the configuration files.
        /// </summary>
        /// <returns>true in case one or more configuration file changed.</returns>
        private Boolean CheckXMLFiles()
        {
            Boolean fileChanged = false;

            fileChanged = checkLocalFile();

            if ( (_appendConfigurationFilename != null) && (fileChanged == false) )
            {
                fileChanged = checkRemoteFile();
            }

            return fileChanged;
        }


        /// <summary>
        /// Check for changes on a local file.
        /// TODO: There is problem to check if a local file has changed.
        /// See also: http://stackoverflow.com/questions/1448716/net-fileinfo-lastwritetime-fileinfo-lastaccesstime-are-wrong
        /// See also: http://stackoverflow.com/questions/9992223/file-getlastwritetime-seems-to-be-returning-out-of-date-value
        /// </summary>
        /// <returns>True if file has changed.</returns>
        private bool checkLocalFile()
        {
            bool fileChanged = false;

            if (DateTime.Compare(_configurationFileWriteDateTime, File.GetLastWriteTime(_configurationFilename)) != 0)
            {
                Console.WriteLine("\n\nThe configuration file was modified (GetLastWriteTime). {0} vs. {1}", _configurationFileWriteDateTime.ToString(), File.GetLastWriteTime(_configurationFilename).ToString());
                fileChanged = true;
            }
            else if (!compareHash(_configurationFileHash, calculateHash(_configurationFilename)))
            {
                Console.WriteLine("\n\nThe configuration file was modified (calculateHash). {0} vs. {1}", _configurationFileHash.ToString(), ByteArrayToString(calculateHash(_configurationFilename)));
                fileChanged = true;
            }
            else
            {
                _log.Trace(m => m("\nThe configuration file {2} was NOT modified. GetLastWriteTime={0} calculateHash={1}", File.GetLastWriteTime(_configurationFilename).ToString(), ByteArrayToString(calculateHash(_configurationFilename)), _configurationFilename));
                //Console.WriteLine("\n\nThe configuration file {2} was NOT modified. GetLastWriteTime={0} calculateHash={1}", File.GetLastWriteTime(_configurationFilename).ToString(), ByteArrayToString(calculateHash(_configurationFilename)), _configurationFilename);
            }
            return fileChanged;
        }

        /// <summary>
        /// Check remote file (http) for changes
        /// </summary>
        /// <returns>True if file has changed.</returns>
        private bool checkRemoteFile()
        {
            bool fileChanged = false;
            Uri myUri = new Uri(_appendConfigurationFilename);
            // Creates an HttpWebRequest for the specified URL. 
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(myUri);
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
            {
                _log.Trace(m => m("Request succeeded and the requested information is in the response , Description : {0}", myHttpWebResponse.StatusDescription));
                //Console.WriteLine("\r\nRequest succeeded and the requested information is in the response , Description : {0}", myHttpWebResponse.StatusDescription);
            }
            // Uses the LastModified property to compare with stored date/time 
            if (DateTime.Compare(_appendConfigurationFileWriteDateTime, myHttpWebResponse.LastModified) != 0)
            {
                Console.WriteLine("\n\nThe configuration file to append was modified. {0} vs. {1}", _appendConfigurationFileWriteDateTime.ToString(), myHttpWebResponse.LastModified.ToString());
                fileChanged = true;
            }
            // Releases the resources of the response.
            myHttpWebResponse.Close();
            return fileChanged;
        }

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
                from device in _configuration.Root.Element("Configuration").Element("Hardware").Elements("Device")
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
                         select new Point((int)pt.Attribute("x"), (int)pt.Attribute("y"))).ToList<Point>(),
                         new Point((double)zone.Element("ZoneControlCoordinate").Attribute("x"), (double)zone.Element("ZoneControlCoordinate").Attribute("y")))).ToList<Zone>()
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
                        int.Parse((string)function.Attribute("Volume")),
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

        /// <summary>
        /// Calculates MD5 hash of a local file.
        /// Max. file size is 2GB
        /// </summary>
        /// <param name="filePath">file name</param>
        /// <returns>MD5 hash of specifid file.</returns>
        private byte[] calculateHash(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                //return BitConverter.ToInt64(md5.ComputeHash(File.ReadAllBytes(filePath)),0);
                //return BitConverter.ToString(md5.ComputeHash(File.ReadAllBytes(filePath))).Replace("-", "").ToLower();
                return md5.ComputeHash(File.ReadAllBytes(filePath));
            }
        }

        /// <summary>
        /// Converts byte array (hash) into string.
        /// </summary>
        /// <param name="arrInput">Byte array to convert.</param>
        /// <returns>String for the passed byte array.</returns>
        private static string ByteArrayToString(byte[] arrInput)
        {
            if (arrInput != null)
            {
                int i;
                StringBuilder sOutput = new StringBuilder(arrInput.Length);
                for (i = 0; i < arrInput.Length - 1; i++)
                {
                    sOutput.Append(arrInput[i].ToString("X2"));
                }
                return sOutput.ToString();
            }
            else
            {
                return "null";
            }
        }

        /// <summary>
        /// Compares two hashes. Returns true if they are equal.
        /// </summary>
        /// <param name="tmpHash">First hash value to compare.</param>
        /// <param name="tmpNewHash">Second hash value to compare.</param>
        /// <returns>True if the hash values are equal.</returns>
        private bool compareHash(Byte[] tmpHash, Byte[] tmpNewHash)
        {
            bool bEqual = false;
            if (tmpNewHash.Length == tmpHash.Length)
            {
                int i = 0;
                while ((i < tmpNewHash.Length) && (tmpNewHash[i] == tmpHash[i]))
                {
                    i += 1;
                }
                if (i == tmpNewHash.Length)
                {
                    bEqual = true;
                }
            }

            //if (bEqual)
            //    Console.WriteLine("The two hash values are the same");
            //else
            //    Console.WriteLine("The two hash values are not the same");

            return bEqual;
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
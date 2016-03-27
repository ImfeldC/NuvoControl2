/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.Dal.UnitTest
 *   Author:         Bernhard Limacher
 *   Creation Date:  14.05.2009
 *   File Name:      ConfigurationLoaderTest.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 14.05.2009, Bernhard Limacher: Implementation of the interface.
 * 
 **************************************************************************************************/


using NuvoControl.Common.Configuration;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.IO;
using System.Xml;
using System.Windows;

using NuvoControl.Server.Dal;
using NuvoControl.Common;

namespace NuvoControl.Server.Dal.UnitTest
{
    /// <summary>
    /// Test class for ConfigurationLoaderTest
    /// 
    /// It is intended to contain all ConfigurationLoaderTest Unit Tests
    /// </summary>
    [TestClass()]
    public class ConfigurationLoaderTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for ConfigurationLoader Constructor with wellformed XML file.
        /// </summary>
        [TestMethod()]
        public void ConfigurationLoaderConstructorTestWellFormedXml()
        {
            string file = @"NuvoControlKonfigurationUnitTest.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
            Assert.IsTrue(true, "Testing the successful instantiation of the XML Document.");
        }


        /// <summary>
        ///A test for ConfigurationLoader Constructor with wrong XML file path.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(DirectoryNotFoundException), "The directory exists inappropriately.")]
        public void ConfigurationLoaderConstructorTestWrongXmlFilePath()
        {
            string file = @"..\xx\NuvoControlKonfiguration.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
        }


        /// <summary>
        ///A test for ConfigurationLoader Constructor with wrong XML file name.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(FileNotFoundException), "The XML Configuration file exists inappropriately.")]
        public void ConfigurationLoaderConstructorTestWrongXmlFileName()
        {
            string file = @"NuvoControlKonfigur.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
        }


        /// <summary>
        ///A test for ConfigurationLoader Constructor with not well formed XML file.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(XmlException), "The XML Configuration file is well formed inappropriately.")]
        public void ConfigurationLoaderConstructorTestNotWellFormedXml()
        {
            string file = @"NuvoControlKonfigurationNotWellFormed.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
        }


        /// <summary>
        ///A test for Validate
        /// </summary>
        [TestMethod()]
        public void ValidateTest()
        {
            // If the system configuration version changes, you need to adapt/review this test case
            Assert.AreEqual("3.0", SystemConfiguration.VERSION);

            string file = @"NuvoControlKonfigurationUnitTest.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
            Assert.AreEqual(true, target.Validate(), "Testing the successful validation of the XML configuration version.");
        }


        /// <summary>
        ///A test for GetConfiguration
        /// </summary>
        [TestMethod()]
        public void GetConfigurationTest()
        {
            // If the system configuration version changes, you need to adapt/review this test case
            Assert.AreEqual("3.0", SystemConfiguration.VERSION);

            string file = @"NuvoControlKonfigurationUnitTest2.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
            SystemConfiguration systemConfiguration = target.GetConfiguration();
            Assert.AreEqual("3.0", systemConfiguration.ConfigurationVersion);

            TestContext.WriteLine("Testing device communication parameters...");
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Id, 100);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Communication.BaudRate, 9600);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Communication.DataBits, 8);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Communication.ParityBit, 1);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Communication.ParityMode, "None");

            TestContext.WriteLine("Testing device protocol driver parameters...");
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].ProtocolDriver.Name, "Nuvo Essentia Protokoll Driver");
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].ProtocolDriver.AssemblyName, "NuvoControl.Server.ProtocolDriver");
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].ProtocolDriver.ClassName, "NuvoControl.Server.ProtocolDriver.NuvoEssentiaProtocolDriver");

            TestContext.WriteLine("Testing audio driver parameters...");
            Assert.AreEqual(3, systemConfiguration.Hardware.Devices[0].AudioDevices.Count);
            Assert.AreEqual(new Address(100,1), systemConfiguration.Hardware.Devices[0].AudioDevices[0].Id);
            Assert.AreEqual(1, systemConfiguration.Hardware.Devices[0].AudioDevices[0].Id.ObjectId);
            Assert.AreEqual(new Address(100, 2), systemConfiguration.Hardware.Devices[0].AudioDevices[1].Id);
            Assert.AreEqual(2, systemConfiguration.Hardware.Devices[0].AudioDevices[1].Id.ObjectId);
            Assert.AreEqual(3, systemConfiguration.Hardware.Devices[0].AudioDevices[2].Id.ObjectId);
            Assert.AreEqual("hw:0,0", systemConfiguration.Hardware.Devices[0].AudioDevices[0].Device);

            TestContext.WriteLine("Testing osc driver parameters...");
            Assert.AreEqual(3, systemConfiguration.Hardware.Devices[0].OscDevices.Count);
            // OSC Client 1
            Assert.AreEqual(eOSCDeviceType.OSCClient, systemConfiguration.Hardware.Devices[0].OscDevices[0].DeviceType);
            Assert.AreEqual(2, systemConfiguration.Hardware.Devices[0].OscDevices[0].Id.ObjectId);
            Assert.AreEqual(9000, systemConfiguration.Hardware.Devices[0].OscDevices[0].SendPort);
            Assert.AreEqual(-1, systemConfiguration.Hardware.Devices[0].OscDevices[0].ListenPort);
            // OSC Server
            Assert.AreEqual(eOSCDeviceType.OSCServer, systemConfiguration.Hardware.Devices[0].OscDevices[2].DeviceType);
            Assert.AreEqual(1, systemConfiguration.Hardware.Devices[0].OscDevices[2].Id.ObjectId);
            Assert.AreEqual(9000, systemConfiguration.Hardware.Devices[0].OscDevices[2].SendPort);
            Assert.AreEqual(8000, systemConfiguration.Hardware.Devices[0].OscDevices[2].ListenPort);
            // OSC Client 2
            Assert.AreEqual(eOSCDeviceType.OSCClient, systemConfiguration.Hardware.Devices[0].OscDevices[1].DeviceType);
            Assert.AreEqual(3, systemConfiguration.Hardware.Devices[0].OscDevices[1].Id.ObjectId);
            Assert.AreEqual(9002, systemConfiguration.Hardware.Devices[0].OscDevices[1].SendPort);
            Assert.AreEqual(-1, systemConfiguration.Hardware.Devices[0].OscDevices[1].ListenPort);
            Assert.AreEqual(18, systemConfiguration.Hardware.Devices[0].OscDevices[1].OscDeviceLayouts.Count);

            TestContext.WriteLine("Testing device zone parameters...");
            Assert.AreEqual(12, systemConfiguration.Hardware.Devices[0].Zones.Count);
            Assert.AreEqual( 1, systemConfiguration.Hardware.Devices[0].Zones[0].Id.ObjectId);
            Assert.AreEqual( 2, systemConfiguration.Hardware.Devices[0].Zones[1].Id.ObjectId);
            Assert.AreEqual( 3, systemConfiguration.Hardware.Devices[0].Zones[2].Id.ObjectId);
            Assert.AreEqual( 4, systemConfiguration.Hardware.Devices[0].Zones[3].Id.ObjectId);
            Assert.AreEqual( 5, systemConfiguration.Hardware.Devices[0].Zones[4].Id.ObjectId);
            Assert.AreEqual( 6, systemConfiguration.Hardware.Devices[0].Zones[5].Id.ObjectId);
            Assert.AreEqual( 7, systemConfiguration.Hardware.Devices[0].Zones[6].Id.ObjectId);
            Assert.AreEqual( 8, systemConfiguration.Hardware.Devices[0].Zones[7].Id.ObjectId);
            Assert.AreEqual( 9, systemConfiguration.Hardware.Devices[0].Zones[8].Id.ObjectId);
            Assert.AreEqual(10, systemConfiguration.Hardware.Devices[0].Zones[9].Id.ObjectId);
            Assert.AreEqual(11, systemConfiguration.Hardware.Devices[0].Zones[10].Id.ObjectId);
            Assert.AreEqual(12, systemConfiguration.Hardware.Devices[0].Zones[11].Id.ObjectId);

            TestContext.WriteLine("Testing device source parameters...");
            Assert.AreEqual(6, systemConfiguration.Hardware.Devices[0].Sources.Count);
            Assert.AreEqual(1, systemConfiguration.Hardware.Devices[0].Sources[0].Id.ObjectId);
            Assert.AreEqual(2, systemConfiguration.Hardware.Devices[0].Sources[1].Id.ObjectId);
            Assert.AreEqual(3, systemConfiguration.Hardware.Devices[0].Sources[2].Id.ObjectId);
            Assert.AreEqual(4, systemConfiguration.Hardware.Devices[0].Sources[3].Id.ObjectId);
            Assert.AreEqual(5, systemConfiguration.Hardware.Devices[0].Sources[4].Id.ObjectId);
            Assert.AreEqual(6, systemConfiguration.Hardware.Devices[0].Sources[5].Id.ObjectId);

            TestContext.WriteLine("Testing some graphic parameters...");
            Assert.AreEqual(2, systemConfiguration.Graphic.Building.Floors.Count);
            Assert.AreEqual("Wohnbereich", systemConfiguration.Graphic.Building.Floors[0].Name);
            Assert.AreEqual(@".\Images\Wohnbereich.bmp", systemConfiguration.Graphic.Building.Floors[0].FloorPlanPath);
            Assert.AreEqual("bmp", systemConfiguration.Graphic.Building.Floors[0].FloorPlanType);
            Assert.AreEqual(9, systemConfiguration.Graphic.Building.Floors[0].Zones.Count);
            Assert.AreEqual(new Address(100, 1), systemConfiguration.Graphic.Building.Floors[0].Zones[0].Id);
            Assert.AreEqual("Esszimmer", systemConfiguration.Graphic.Building.Floors[0].Zones[0].Name);
            Assert.AreEqual(@".\Images\Esszimmer.jpg", systemConfiguration.Graphic.Building.Floors[0].Zones[0].PicturePath);
            Assert.AreEqual("jpg", systemConfiguration.Graphic.Building.Floors[0].Zones[0].PictureType);
            Assert.AreEqual(4, systemConfiguration.Graphic.Building.Floors[0].Zones[0].FloorPlanCoordinates.Count);
            Assert.AreEqual(485, systemConfiguration.Graphic.Building.Floors[0].Zones[0].FloorPlanCoordinates[1].X);
            Assert.AreEqual(210, systemConfiguration.Graphic.Building.Floors[0].Zones[0].FloorPlanCoordinates[1].Y);
            Assert.AreEqual(6, systemConfiguration.Graphic.Sources.Count);
            Assert.AreEqual(new Address(100, 1), systemConfiguration.Graphic.Sources[0].Id);
            Assert.AreEqual("DAB Hama DIR3100", systemConfiguration.Graphic.Sources[0].Name);
            Assert.AreEqual(systemConfiguration.Graphic.Sources[0].PicturePath, @".\Images\Tuner.jpg");
            Assert.AreEqual(systemConfiguration.Graphic.Sources[0].PictureType, "jpg");

            
            TestContext.WriteLine("Testing some function parameters...");
            Assert.AreEqual(12, systemConfiguration.Functions.Count);
            SleepFunction sleepFct = systemConfiguration.Functions[0] as SleepFunction;
            Assert.AreEqual(new SimpleId("2445f69e-a5a7-465e-95be-9179913d3780"), sleepFct.Id);
            Assert.AreEqual(new Address(100, 3), sleepFct.ZoneId);
            Assert.AreEqual(new TimeSpan(0, 60, 0), sleepFct.SleepDuration);
            Assert.AreEqual(new TimeSpan(20, 00, 0), sleepFct.ValidFrom);
            Assert.AreEqual(new TimeSpan(2, 0, 0), sleepFct.ValidTo);
            Assert.AreEqual(1, sleepFct.Commands.Count);

            AlarmFunction alarmFct = systemConfiguration.Functions[3] as AlarmFunction;
            Assert.AreEqual(new SimpleId("11111111-0001-1111-1111-111111111111"), alarmFct.Id);
            Assert.AreEqual(new Address(100, 2), alarmFct.ZoneId);
            Assert.AreEqual(new TimeSpan(6, 45, 0), alarmFct.AlarmTime);
            Assert.AreEqual(new TimeSpan(2, 0, 0), alarmFct.AlarmDuration);
            Assert.AreEqual(new Address(100, 1), alarmFct.SourceId);
            Assert.AreEqual(1, alarmFct.ValidOnDays.Count);
            Assert.AreEqual(DayOfWeek.Monday, alarmFct.ValidOnDays[0]);
            Assert.AreEqual(1, alarmFct.Commands.Count);

            ZoneChangeFunction zonechangeFct = systemConfiguration.Functions[6] as ZoneChangeFunction;
            Assert.AreEqual(new SimpleId("1234"), zonechangeFct.Id);
            Assert.AreEqual(1, zonechangeFct.ValidOnDays.Count);
            Assert.AreEqual(DayOfWeek.Tuesday, zonechangeFct.ValidOnDays[0]);
            Assert.AreEqual(0, zonechangeFct.Commands.Count);

            OscEventFunction oscFunc = systemConfiguration.Functions[7] as OscEventFunction;
            Assert.AreEqual(new SimpleId("504"), oscFunc.Id);
            Assert.AreEqual(1, oscFunc.ValidOnDays.Count);
            Assert.AreEqual(DayOfWeek.Friday, oscFunc.ValidOnDays[0]);
            Assert.AreEqual(2, oscFunc.Commands.Count);

            OscEventFunction oscFunc2 = systemConfiguration.Functions[8] as OscEventFunction;
            Assert.AreEqual(new SimpleId("505"), oscFunc2.Id);
            Assert.AreEqual("/4/toggle3", oscFunc2.OscLabel);
            Assert.AreEqual(null, oscFunc2.ValidOnDays);
            Assert.AreEqual(1, oscFunc2.Commands.Count);

            OscEventFunction oscFunc3 = systemConfiguration.Functions[9] as OscEventFunction;
            Assert.AreEqual(new SimpleId("506"), oscFunc3.Id);
            Assert.AreEqual("/4/toggle5", oscFunc3.OscLabel);
            Assert.AreEqual("100.1", oscFunc3.OscDevice.ToString());
            Assert.AreEqual("SwitchOff", oscFunc3.OscEvent);
            Assert.AreEqual(null, oscFunc3.ValidOnDays);
            Assert.AreEqual(1, oscFunc3.Commands.Count);

        }

    
        /// <summary>
        /// A test for AppendConfiguration
        /// </summary>
        [TestMethod()]
        public void AppendConfigurationTest()
        {
            // If the system configuration version changes, you need to adapt/review this test case
            Assert.AreEqual("3.0", SystemConfiguration.VERSION);

            string file = @"NuvoControlKonfigurationUnitTest.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
            SystemConfiguration systemConfiguration = target.GetConfiguration();
            Assert.AreEqual("3.0", systemConfiguration.ConfigurationVersion);
            Assert.AreEqual(8, systemConfiguration.Functions.Count);

            string appendfile = @"NuvoControlKonfigurationRemote.xml";
            target.AppendConfiguration(appendfile);
            systemConfiguration = target.GetConfiguration();
            Assert.AreEqual("3.0", systemConfiguration.ConfigurationVersion);
            Assert.AreEqual(10, systemConfiguration.Functions.Count);
        
        }

        /// <summary>
        /// A test for AppendConfiguration (with no HW section)
        /// </summary>
        [TestMethod()]
        public void AppendConfigurationTestNoHW()
        {
            // If the system configuration version changes, you need to adapt/review this test case
            Assert.AreEqual("3.0", SystemConfiguration.VERSION);

            string file = @"NuvoControlKonfigurationUnitTest.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
            SystemConfiguration systemConfiguration = target.GetConfiguration();
            Assert.AreEqual("3.0", systemConfiguration.ConfigurationVersion);
            Assert.AreEqual(8, systemConfiguration.Functions.Count);

            string appendfile = @"NuvoControlKonfigurationRemoteNoHW.xml";
            target.AppendConfiguration(appendfile);
            systemConfiguration = target.GetConfiguration();
            Assert.AreEqual("3.0", systemConfiguration.ConfigurationVersion);
            Assert.AreEqual(11, systemConfiguration.Functions.Count);

        }

        /// <summary>
        /// A test to load configuration from remote lcoation
        /// </summary>
        [TestMethod()]
        public void LoadRemoteConfigurationTest()
        {
            // If the system configuration version changes, you need to adapt/review this test case
            Assert.AreEqual("3.0", SystemConfiguration.VERSION);

            string file = @"NuvoControlKonfigurationUnitTest.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
            SystemConfiguration systemConfiguration = target.GetConfiguration();
            Assert.AreEqual("3.0", systemConfiguration.ConfigurationVersion);
            Assert.AreEqual(8, systemConfiguration.Functions.Count);

            string appendfile = @"http://www.imfeld.net/publish/configuration/NuvoControlKonfigurationRemote.xml";
            target.AppendConfiguration(appendfile);
            systemConfiguration = target.GetConfiguration();
            Assert.AreEqual("3.0", systemConfiguration.ConfigurationVersion);
            Assert.AreEqual(10, systemConfiguration.Functions.Count);

        }

        /* Remote configuration file, is not supported!
        /// <summary>
        /// A test to load configuration from remote lcoation
        /// </summary>
        [TestMethod()]
        public void LoadRemoteConfigurationOnlyTest()
        {
            string file = @"http://www.imfeld.net/publish/configuration/NuvoControlKonfigurationRemote.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
            SystemConfiguration systemConfiguration = target.GetConfiguration();
            Assert.AreEqual("2.0", SystemConfiguration.VERSION);
            Assert.AreEqual(7, systemConfiguration.Functions.Count);
        }
        */


        /// <summary>
        /// A test to check if date/time of configuration file changed
        /// </summary>
        [TestMethod()]
        public void CheckConfigurationDateTimeTest()
        {
            string file = @"NuvoControlKonfigurationUnitTest.xml";
            string appendfile = @"http://www.imfeld.net/publish/configuration/NuvoControlKonfigurationRemote.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
            target.AppendConfiguration(appendfile);
            SystemConfiguration systemConfiguration = target.GetConfiguration();

            bool bChanged = target.RefreshConfiguration();
            Assert.IsFalse(bChanged);

        }

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/
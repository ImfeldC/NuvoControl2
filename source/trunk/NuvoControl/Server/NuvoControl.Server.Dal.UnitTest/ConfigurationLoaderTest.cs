/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
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
using System.Drawing;

using NuvoControl.Server.Dal;

namespace NuvoControl.Server.Dal.UnitTest
{
    /// <summary>
    ///This is a test class for ConfigurationLoaderTest and is intended
    ///to contain all ConfigurationLoaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConfigurationLoaderTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
        ///</summary>
        [TestMethod()]
        public void ConfigurationLoaderConstructorTestWellFormedXml()
        {
            string file = @"NuvoControlKonfiguration.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
            Assert.IsTrue(true, "Testing the successful instantiation of the XML Document.");
        }


        /// <summary>
        ///A test for ConfigurationLoader Constructor with wrong XML file path.
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(DirectoryNotFoundException), "The directory exists inappropriately.")]
        public void ConfigurationLoaderConstructorTestWrongXmlFilePath()
        {
            string file = @"..\xx\NuvoControlKonfiguration.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
        }


        /// <summary>
        ///A test for ConfigurationLoader Constructor with wrong XML file name.
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(FileNotFoundException), "The XML Configuration file exists inappropriately.")]
        public void ConfigurationLoaderConstructorTestWrongXmlFileName()
        {
            string file = @"NuvoControlKonfigur.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
        }


        /// <summary>
        ///A test for ConfigurationLoader Constructor with not well formed XML file.
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(XmlException), "The XML Configuration file is well formed inappropriately.")]
        public void ConfigurationLoaderConstructorTestNotWellFormedXml()
        {
            string file = @"NuvoControlKonfigurationNotWellFormed.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
        }


        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void ValidateTest()
        {
            string file = @"NuvoControlKonfiguration.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
            Assert.AreEqual(true, target.Validate(), "Testing the successful validation of the XML configuration version.");
        }


        /// <summary>
        ///A test for GetConfiguration
        ///</summary>
        [TestMethod()]
        public void GetConfigurationTest()
        {
            string file = @"NuvoControlKonfiguration.xml";
            ConfigurationLoader target = new ConfigurationLoader(file);
            SystemConfiguration systemConfiguration = target.GetConfiguration();
            Assert.AreEqual(SystemConfiguration.VERSION, "1.0");

            TestContext.WriteLine("Testing device communication parameters...");
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Id, 100);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Communication.BaudRate, 9600);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Communication.DataBits, 8);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Communication.ParityBit, 1);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Communication.ParityMode, "None");

            TestContext.WriteLine("Testing device protocol driver parameters...");
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].ProtocolDriver.Name, "Nuvo Essentia Protkoll Driver");
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].ProtocolDriver.AssemblyName, "NuvoControl.Server.ProtocolDriver");
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].ProtocolDriver.ClassName, "NuvoControl.Server.ProtocolDriver.NuvoEssentiaProtocolDriver");

            TestContext.WriteLine("Testing device zone parameters...");
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Zones.Count, 12);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Zones[0], 1);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Zones[1], 2);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Zones[2], 3);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Zones[3], 4);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Zones[4], 5);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Zones[5], 6);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Zones[6], 7);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Zones[7], 8);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Zones[8], 9);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Zones[9], 10);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Zones[10], 11);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Zones[11], 12);

            TestContext.WriteLine("Testing device source parameters...");
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Sources.Count, 6);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Sources[0], 1);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Sources[1], 2);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Sources[2], 3);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Sources[3], 4);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Sources[4], 5);
            Assert.AreEqual(systemConfiguration.Hardware.Devices[0].Sources[5], 6);

            TestContext.WriteLine("Testing some graphic parameters...");
            Assert.AreEqual(systemConfiguration.Graphic.Building.Floors.Count, 2);
            Assert.AreEqual(systemConfiguration.Graphic.Building.Floors[0].Name, "Wohnbereich");
            Assert.AreEqual(systemConfiguration.Graphic.Building.Floors[0].FloorPlanPath, @".\Images\Wohnbereich.jpg");
            Assert.AreEqual(systemConfiguration.Graphic.Building.Floors[0].FloorPlanType, "jpg");
            Assert.AreEqual(systemConfiguration.Graphic.Building.Floors[0].Zones.Count, 9);
            Assert.AreEqual(systemConfiguration.Graphic.Building.Floors[0].Zones[0].Id, new Address(100, 1));
            Assert.AreEqual(systemConfiguration.Graphic.Building.Floors[0].Zones[0].Name, "Esszimmer");
            Assert.AreEqual(systemConfiguration.Graphic.Building.Floors[0].Zones[0].PicturePath, @".\Images\Esszimmer.jpg");
            Assert.AreEqual(systemConfiguration.Graphic.Building.Floors[0].Zones[0].PictureType, "jpg");
            Assert.AreEqual(systemConfiguration.Graphic.Building.Floors[0].Zones[0].FloorPlanCoordinates.Count, 4);
            Assert.AreEqual(systemConfiguration.Graphic.Building.Floors[0].Zones[0].FloorPlanCoordinates[1].X, 10);
            Assert.AreEqual(systemConfiguration.Graphic.Building.Floors[0].Zones[0].FloorPlanCoordinates[1].Y, 15);
            Assert.AreEqual(systemConfiguration.Graphic.Sources.Count, 5);
            Assert.AreEqual(systemConfiguration.Graphic.Sources[0].Id, new Address(100, 1));
            Assert.AreEqual(systemConfiguration.Graphic.Sources[0].Name, "Tuner A");
            Assert.AreEqual(systemConfiguration.Graphic.Sources[0].PicturePath, @".\Images\Tuner.jpg");
            Assert.AreEqual(systemConfiguration.Graphic.Sources[0].PictureType, "jpg");

            TestContext.WriteLine("Testing some function parameters...");
            Assert.AreEqual(systemConfiguration.Functions.Count, 3);
            SleepFunction sleepFct = systemConfiguration.Functions[0] as SleepFunction;
            Assert.AreEqual(sleepFct.Id, new Guid("2445f69e-a5a7-465e-95be-9179913d3786"));
            Assert.AreEqual(sleepFct.ZoneId, new Address(100, 1));
            Assert.AreEqual(sleepFct.SleepDuration, new TimeSpan(0, 60, 0));
            Assert.AreEqual(sleepFct.ValidFrom, new TimeSpan(23, 0, 0));
            Assert.AreEqual(sleepFct.ValidTo, new TimeSpan(2, 0, 0));
            AlarmFunction alarmFct = systemConfiguration.Functions[1] as AlarmFunction;
            Assert.AreEqual(alarmFct.Id, new Guid("14bdca34-ea36-4419-8cd4-788a73a81c93"));
            Assert.AreEqual(alarmFct.ZoneId, new Address(100, 2));
            Assert.AreEqual(alarmFct.AlarmTime, new TimeSpan(7, 0, 0));
            Assert.AreEqual(alarmFct.AlarmDuration, new TimeSpan(0, 10, 0));
            Assert.AreEqual(alarmFct.SourceId, new Address(100, 2));
            Assert.AreEqual(alarmFct.ValidOnDays.Count, 2);
            Assert.AreEqual(alarmFct.ValidOnDays[0], DayOfWeek.Monday);
            Assert.AreEqual(alarmFct.ValidOnDays[1], DayOfWeek.Tuesday);


        }
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
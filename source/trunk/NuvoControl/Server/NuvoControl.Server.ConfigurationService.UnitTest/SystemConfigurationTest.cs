/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.ConfigurationService.UnitTest
 *   Author:         Bernhard Limacher
 *   Creation Date:  14.05.2009
 *   File Name:      SystemConfigurationTest.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 14.05.2009, Bernhard Limacher: Implementation of the interface.
 * 
 **************************************************************************************************/


using NuvoControl.Server;
using NuvoControl.Common.Configuration;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.IO;
using System.Xml;
using System.Drawing;

namespace NuvoControl.Server.ConfigurationService.UnitTest
{ 
    /// <summary>
    /// Test class for SystemConfigurationTest 
    /// 
    /// It is intended to contain all SystemConfigurationTest Unit Tests
    /// </summary>
    [TestClass()]
    public class SystemConfigurationTest
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
        ///A test for Validate
        ///It tests the successful validation if the system configuration.
        /// </summary>
        [TestMethod()]
        public void ValidateTest()
        {
            string file = @"NuvoControlKonfigurationUnitTest.xml";
            ConfigurationService target = new ConfigurationService(file);
            Assert.AreEqual(true, target.Validate(), "Testing the successful validation of the XML configuration.");
        }


        /// <summary>
        ///A test for Validate
        ///It tests the validation in case of not distinct zone definitions
        /// </summary>
        [TestMethod()]
        public void ValidateTestNotDistinctZones()
        {
            string file = @"NuvoControlKonfigurationNotDistinctZones.xml";
            ConfigurationService target = new ConfigurationService(file);
            Assert.AreEqual(false, target.Validate(), "Testing not distinct zone definitions.");
        }


        /// <summary>
        ///A test for Validate
        ///It tests the validation in case of not distinct source definitions
        /// </summary>
        [TestMethod()]
        public void ValidateTestNotDistinctSources()
        {
            string file = @"NuvoControlKonfigurationNotDistinctSources.xml";
            ConfigurationService target = new ConfigurationService(file);
            Assert.AreEqual(false, target.Validate(), "Testing not distinct source definitions.");
        }


        /// <summary>
        ///A test for Validate
        ///It tests invalid zone id definition in the graphic configuration.
        /// </summary>
        [TestMethod()]
        public void ValidateTestInvalidZoneIdInGraphic()
        {
            string file = @"NuvoControlKonfigurationInvalidZoneIdInGraphic.xml";
            ConfigurationService target = new ConfigurationService(file);
            Assert.AreEqual(false, target.Validate(), "Testing invalid zone id in graphic configuration.");
        }


        /// <summary>
        ///A test for Validate
        ///It tests invalid source id definition in the graphic configuration.
        /// </summary>
        [TestMethod()]
        public void ValidateTestInvalidSourceIdInGraphic()
        {
            string file = @"NuvoControlKonfigurationInvalidSourceIdInGraphic.xml";
            ConfigurationService target = new ConfigurationService(file);
            Assert.AreEqual(false, target.Validate(), "Testing invalid source id in graphic configuration.");
        }


        /// <summary>
        ///A test for Validate
        ///It tests invalid zone id definition in sleep function.
        /// </summary>
        [TestMethod()]
        public void ValidateTestInvalidZoneIdInSleepFunction()
        {
            string file = @"NuvoControlKonfigurationInvalidZoneIdInSleepFunction.xml";
            ConfigurationService target = new ConfigurationService(file);
            Assert.AreEqual(false, target.Validate(), "Testing invalid zone id in sleep function.");
        }


        /// <summary>
        ///A test for Validate
        ///It tests invalid source id definition in alarm function.
        /// </summary>
        [TestMethod()]
        public void ValidateTestInvalidSourceIdInAlarmFunction()
        {
            string file = @"NuvoControlKonfigurationInvalidSourceIdInAlarmFunction.xml";
            ConfigurationService target = new ConfigurationService(file);
            Assert.AreEqual(false, target.Validate(), "Testing invalid source id in alarm function.");
        }

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/
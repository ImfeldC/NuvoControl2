/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.ProtocolDriver.UnitTest
 *   Author:         Ch.Imfeld
 *   Creation Date:  6/12/2009 11:02:29 PM
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 6/12/2009 11:02:29 PM, Ch.Imfeld: Initial implementation.
 * 
 **************************************************************************************************/


using NuvoControl.Server.ProtocolDriver.Simulator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuvoControl.Server.ProtocolDriver;
using NuvoControl.Server.ProtocolDriver.Interface;

namespace NuvoControl.Server.ProtocolDriver.Test
{
    
    
    /// <summary>
    ///This is a test class for ProtocolDriverSimulatorTest and is intended
    ///to contain all ProtocolDriverSimulatorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProtocolDriverSimulatorTest
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
        /// A test for createIncomingCommand.
        /// This method tests the crateion of the expected incoming command.
        /// </summary>
        [TestMethod()]
        public void createIncomingCommandTest()
        {
            {   // Test 1
                EIRCarrierFrequency[] ircf = { EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz };
                NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                    ENuvoEssentiaCommands.ReadStatusCONNECT,
                    ENuvoEssentiaZones.Zone2, ENuvoEssentiaSources.Source3, -50, 5, -3,
                    EZonePowerStatus.ZoneStatusON, ircf,
                    EDIPSwitchOverrideStatus.DIPSwitchOverrideOFF,
                    EVolumeResetStatus.VolumeResetOFF,
                    ESourceGroupStatus.SourceGroupOFF, "V1.0");
                string actual;
                actual = ProtocolDriverSimulator.createIncomingCommand(command);
                Assert.AreEqual("#Z02PWRON,SRC3,GRP0,VOL-50\r", actual);
            }

            {   // Test 2
                EIRCarrierFrequency[] ircf = { EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz };
                NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                    ENuvoEssentiaCommands.ReadStatusCONNECT,
                    ENuvoEssentiaZones.Zone4, ENuvoEssentiaSources.Source3, -20, 5, -3,
                    EZonePowerStatus.ZoneStatusOFF, ircf,
                    EDIPSwitchOverrideStatus.DIPSwitchOverrideOFF,
                    EVolumeResetStatus.VolumeResetOFF,
                    ESourceGroupStatus.SourceGroupON, "V1.0");
                string actual;
                actual = ProtocolDriverSimulator.createIncomingCommand(command);
                Assert.AreEqual("#Z04PWROFF,SRC3,GRP1,VOL-20\r", actual);
            }

            {   // Test 3
                EIRCarrierFrequency[] ircf = { EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz };
                NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                    ENuvoEssentiaCommands.ReadStatusZONE,
                    ENuvoEssentiaZones.Zone4, ENuvoEssentiaSources.Source3, -20, 5, -3,
                    EZonePowerStatus.ZoneStatusOFF, ircf,
                    EDIPSwitchOverrideStatus.DIPSwitchOverrideOFF,
                    EVolumeResetStatus.VolumeResetOFF,
                    ESourceGroupStatus.SourceGroupON, "V1.0");
                string actual;
                actual = ProtocolDriverSimulator.createIncomingCommand(command);
                Assert.AreEqual("#Z04OR0,BASS+05,TREB-03,GRP1,VRST0\r", actual);
            }

            {   // Test 4
                EIRCarrierFrequency[] ircf = { EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz };
                NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                    ENuvoEssentiaCommands.ReadStatusZONE,
                    ENuvoEssentiaZones.Zone4, ENuvoEssentiaSources.Source3, -20, -4, 2,
                    EZonePowerStatus.ZoneStatusOFF, ircf,
                    EDIPSwitchOverrideStatus.DIPSwitchOverrideON,
                    EVolumeResetStatus.VolumeResetON,
                    ESourceGroupStatus.SourceGroupOFF, "V1.0");
                string actual;
                actual = ProtocolDriverSimulator.createIncomingCommand(command);
                Assert.AreEqual("#Z04OR1,BASS-04,TREB+02,GRP0,VRST1\r", actual);
            }

            {   // Test 5
                EIRCarrierFrequency[] ircf = { EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR56kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR56kHz, EIRCarrierFrequency.IR38kHz };
                NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                    ENuvoEssentiaCommands.ReadStatusSOURCEIR,
                    ENuvoEssentiaZones.Zone4, ENuvoEssentiaSources.Source3, -20, -4, 2,
                    EZonePowerStatus.ZoneStatusOFF, ircf,
                    EDIPSwitchOverrideStatus.DIPSwitchOverrideON,
                    EVolumeResetStatus.VolumeResetON,
                    ESourceGroupStatus.SourceGroupOFF, "V1.0");
                string actual;
                actual = ProtocolDriverSimulator.createIncomingCommand(command);
                Assert.AreEqual("#IRSET:38,56,38,38,56,38\r", actual);
            }

            {   // Test 6
                EIRCarrierFrequency[] ircf = { EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR56kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR38kHz, EIRCarrierFrequency.IR56kHz, EIRCarrierFrequency.IR38kHz };
                NuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                    ENuvoEssentiaCommands.ReadVersion,
                    ENuvoEssentiaZones.Zone4, ENuvoEssentiaSources.Source3, -20, -4, 2,
                    EZonePowerStatus.ZoneStatusOFF, ircf,
                    EDIPSwitchOverrideStatus.DIPSwitchOverrideON,
                    EVolumeResetStatus.VolumeResetON,
                    ESourceGroupStatus.SourceGroupOFF, "v1.23");
                string actual;
                actual = ProtocolDriverSimulator.createIncomingCommand(command);
                Assert.AreEqual("#NUVO_E6D_v1.23\r", actual);
            }
        }
    }
}

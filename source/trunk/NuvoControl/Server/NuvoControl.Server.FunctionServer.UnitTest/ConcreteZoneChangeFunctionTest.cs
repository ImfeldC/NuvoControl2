using NuvoControl.Server.FunctionServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ZoneServer;
using NuvoControl.Server.ProtocolDriver.Interface;

namespace NuvoControl.Server.FunctionServer.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ConcreteZoneChangeFunctionTest and is intended
    ///to contain all ConcreteZoneChangeFunctionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConcreteZoneChangeFunctionTest
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
        ///A test for Function
        ///</summary>
        [TestMethod()]
        public void FunctionTest()
        {
            ZoneChangeFunction function = new ZoneChangeFunction();
            IZoneServer zoneServer = null; 
            Dictionary<int, IAudioDriver> audioDrivers = null; 
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers); 
            Function actual;
            actual = target.Function;
            Assert.IsNotNull(actual);
        }

        ///<summary>
        /// A test for Active, per default it is expected that a function is valid (if nothing is configured)
        ///</summary>
        [TestMethod()]
        public void ActiveTest1()
        {
            ZoneChangeFunction function = new ZoneChangeFunction();
            IZoneServer zoneServer = null;
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);
            bool actual;
            actual = target.Active;
            Assert.AreEqual(true, actual);
        }

        ///<summary>
        /// A test for Active
        /// Expected: True, as current day is added to valid day of week
        ///</summary>
        [TestMethod()]
        public void ActiveTest2()
        {
            List<DayOfWeek> dayOfWeek = new List<DayOfWeek>();
            dayOfWeek.Add(DateTime.Now.DayOfWeek);
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address(), new Address(), 0, dayOfWeek, null);
            IZoneServer zoneServer = null;
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);
            bool actual;
            actual = target.Active;
            Assert.AreEqual(true, actual);
        }

        ///<summary>
        /// A test for Active
        /// Expected result: False, because current day is not part of the list
        ///</summary>
        [TestMethod()]
        public void ActiveTest3()
        {
            List<DayOfWeek> dayOfWeek = new List<DayOfWeek>();
            dayOfWeek.Add(DayOfWeek.Monday);
            dayOfWeek.Add(DayOfWeek.Tuesday);
            dayOfWeek.Add(DayOfWeek.Wednesday);
            dayOfWeek.Add(DayOfWeek.Thursday);
            dayOfWeek.Add(DayOfWeek.Friday);
            dayOfWeek.Add(DayOfWeek.Saturday);
            dayOfWeek.Add(DayOfWeek.Sunday);
            dayOfWeek.Remove(DateTime.Now.DayOfWeek);
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address(), new Address(), 0, dayOfWeek, null);
            IZoneServer zoneServer = null;
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);
            bool actual;
            actual = target.Active;
            Assert.AreEqual(false, actual);
        }

        ///<summary>
        /// A test for Active
        /// Expected result: True, because all days are in the list.
        ///</summary>
        [TestMethod()]
        public void ActiveTest4()
        {
            List<DayOfWeek> dayOfWeek = new List<DayOfWeek>();
            dayOfWeek.Add(DayOfWeek.Monday);
            dayOfWeek.Add(DayOfWeek.Tuesday);
            dayOfWeek.Add(DayOfWeek.Wednesday);
            dayOfWeek.Add(DayOfWeek.Thursday);
            dayOfWeek.Add(DayOfWeek.Friday);
            dayOfWeek.Add(DayOfWeek.Saturday);
            dayOfWeek.Add(DayOfWeek.Sunday);
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address(), new Address(), 0, dayOfWeek, null);
            IZoneServer zoneServer = null;
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);
            bool actual;
            actual = target.Active;
            Assert.AreEqual(true, actual);
        }

        ///<summary>
        /// A test for Active
        /// Expected result: False, because valid from starts in 1h
        ///</summary>
        [TestMethod()]
        public void ActiveTest5()
        {
            TimeSpan validFrom = new TimeSpan(DateTime.Now.Hour + 1, 0, 0);
            TimeSpan validTo = new TimeSpan(23, 59, 59);
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address(), new Address(), 0, null, validFrom, validTo, null);
            IZoneServer zoneServer = null;
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);
            bool actual;
            actual = target.Active;
            Assert.AreEqual(false, actual);
        }

        ///<summary>
        /// A test for Active
        /// Expected result: True, because valid from started before 1h
        ///</summary>
        [TestMethod()]
        public void ActiveTest6()
        {
            TimeSpan validFrom = new TimeSpan(DateTime.Now.Hour - 1, 0, 0);
            TimeSpan validTo = new TimeSpan(23, 59, 59);
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address(), new Address(), 0, null, validFrom, validTo, null);
            IZoneServer zoneServer = null;
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);
            bool actual;
            actual = target.Active;
            Assert.AreEqual(true, actual);
        }

        ///<summary>
        /// A test for Active
        /// Expected result: True, because valid from started 1h ago
        ///</summary>
        [TestMethod()]
        public void ActiveTest7()
        {
            List<DayOfWeek> dayOfWeek = new List<DayOfWeek>();
            dayOfWeek.Add(DayOfWeek.Monday);
            dayOfWeek.Add(DayOfWeek.Tuesday);
            dayOfWeek.Add(DayOfWeek.Wednesday);
            dayOfWeek.Add(DayOfWeek.Thursday);
            dayOfWeek.Add(DayOfWeek.Friday);
            dayOfWeek.Add(DayOfWeek.Saturday);
            dayOfWeek.Add(DayOfWeek.Sunday);
            TimeSpan validFrom = new TimeSpan(DateTime.Now.Hour - 1, 0, 0);
            TimeSpan validTo = new TimeSpan(23, 59, 59);
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address(), new Address(), 0, dayOfWeek, validFrom, validTo, null);
            IZoneServer zoneServer = null;
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);
            bool actual;
            actual = target.Active;
            Assert.AreEqual(true, actual);
        }

        ///<summary>
        /// A test for Active
        /// Expected result: False, because valid starts in 1h, even is all days are in list.
        ///</summary>
        [TestMethod()]
        public void ActiveTest8()
        {
            List<DayOfWeek> dayOfWeek = new List<DayOfWeek>();
            dayOfWeek.Add(DayOfWeek.Monday);
            dayOfWeek.Add(DayOfWeek.Tuesday);
            dayOfWeek.Add(DayOfWeek.Wednesday);
            dayOfWeek.Add(DayOfWeek.Thursday);
            dayOfWeek.Add(DayOfWeek.Friday);
            dayOfWeek.Add(DayOfWeek.Saturday);
            dayOfWeek.Add(DayOfWeek.Sunday);
            TimeSpan validFrom = new TimeSpan(DateTime.Now.Hour + 1, 0, 0);
            TimeSpan validTo = new TimeSpan(23, 59, 59);
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address(), new Address(), 0, dayOfWeek, validFrom, validTo, null);
            IZoneServer zoneServer = null;
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);
            bool actual;
            actual = target.Active;
            Assert.AreEqual(false, actual);
        }

        ///<summary>
        /// A test for Active
        /// Expected result: False, because valid ended 1h ago, even if all days are in list.
        ///</summary>
        [TestMethod()]
        public void ActiveTest9()
        {
            List<DayOfWeek> dayOfWeek = new List<DayOfWeek>();
            dayOfWeek.Add(DayOfWeek.Monday);
            dayOfWeek.Add(DayOfWeek.Tuesday);
            dayOfWeek.Add(DayOfWeek.Wednesday);
            dayOfWeek.Add(DayOfWeek.Thursday);
            dayOfWeek.Add(DayOfWeek.Friday);
            dayOfWeek.Add(DayOfWeek.Saturday);
            dayOfWeek.Add(DayOfWeek.Sunday);
            TimeSpan validFrom = new TimeSpan(0, 0, 1);     // NOTE: (0,0,0) is considered as "not set" (not used)
            TimeSpan validTo = new TimeSpan(DateTime.Now.Hour - 1, 59, 59);
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address(), new Address(), 0, dayOfWeek, validFrom, validTo, null);
            IZoneServer zoneServer = null;
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);
            bool actual;
            actual = target.Active;
            Assert.AreEqual(false, actual);
        }

        ///<summary>
        /// A test for notifyOnZoneUpdate
        /// Expected result: No exception
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.FunctionServer.dll")]
        public void notifyOnZoneUpdateTest1()
        {
            ZoneChangeFunction function = new ZoneChangeFunction();
            IZoneServer zoneServer = null;
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction_Accessor target = new ConcreteZoneChangeFunction_Accessor(function,zoneServer,audioDrivers);
            ZoneStateEventArgs e = new ZoneStateEventArgs(new ZoneState());
            target.notifyOnZoneUpdate(e);
            //ok, pass without exception
        }

        ///<summary>
        /// A test for notifyOnZoneUpdate
        /// Expected result: No exception
        ///</summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.FunctionServer.dll")]
        public void notifyOnZoneUpdateTest2()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new SimpleId(), true, true, true, true, true, true, true, new Address("100.1"), "OFF", "100.6", 10));
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address(), new Address(), 0, true, true, true, true, null, new TimeSpan(), new TimeSpan(), commands);
            IZoneServer zoneServer = null;
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction_Accessor target = new ConcreteZoneChangeFunction_Accessor(function, zoneServer, audioDrivers);
            ZoneStateEventArgs e = new ZoneStateEventArgs(new ZoneState());
            target.notifyOnZoneUpdate(e); // init zone state
            target.notifyOnZoneUpdate(e);
        }


        ///<summary>
        /// A test for ConcreteZoneChangeFunction Constructor
        /// Expected Result: No exeception
        ///</summary>
        [TestMethod()]
        public void ConcreteZoneChangeFunctionConstructorTest()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new SimpleId(), true, true, true, true, true, true, true, new Address("100.1"), "OFF", "100.6", 10));
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address(), new Address(), 0, true, true, true, true, null, new TimeSpan(), new TimeSpan(), commands);
            IZoneServer zoneServer = null;
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);
            // No exception
        }

        ///<summary>
        /// A test for ConcreteZoneChangeFunction command
        /// Expected Result: No exeception, one commands in zone state list
        ///</summary>
        [TestMethod()]
        public void ConcreteZoneChangeFunctionCommandTest1()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new SimpleId(), true, true, true, true, true, true, true, new Address("100.1"), "OFF", "100.6", 10));
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address(), new Address(), 0, true, true, true, true, null, new TimeSpan(), new TimeSpan(), commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));

            target.calculateFunction(DateTime.Now);

            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);     // OnValidityStart
        }

        ///<summary>
        /// A test for ConcreteZoneChangeFunction command
        /// Expected Result: No exeception, two commands in zone state list
        ///</summary>
        [TestMethod()]
        public void ConcreteZoneChangeFunctionCommandTest2()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new SimpleId(), true, true, true, true, true, true, true, new Address("100.1"), "OFF", "100.6", 10));
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address(), new Address(), 0, true, true, true, true, null, new TimeSpan(), new TimeSpan(), commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), false, 10, ZoneQuality.Online));

            target.calculateFunction(DateTime.Now);

            Assert.AreEqual(2, zoneServer.ZoneStateList.Count);     // OnValidityStart & OnFunctionStart with power status change
        }

        ///<summary>
        /// A test for ConcreteZoneChangeFunction command
        /// Expected Result: No exeception, four commands in zone state list
        ///</summary>
        [TestMethod()]
        public void ConcreteZoneChangeFunctionCommandTest3()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new SimpleId(), true, true, true, true, true, true, true, new Address("100.1"), "OFF", "100.6", 10));
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address(), new Address(), 0, true, true, true, true, null, new TimeSpan(), new TimeSpan(), commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.5"), false, 20, ZoneQuality.Online));

            target.calculateFunction(DateTime.Now);

            Assert.AreEqual(4, zoneServer.ZoneStateList.Count);     // OnValidityStart & OnFunctionStart with power status, source and volume change
        }

        ///<summary>
        /// A test for ConcreteZoneChangeFunction command
        /// Expected Result: No exeception, three commands in zone state list
        ///</summary>
        [TestMethod()]
        public void ConcreteZoneChangeFunctionCommandTest4()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new SimpleId(), true, true, true, true, true, true, true, new Address("100.1"), "OFF", "100.6", 10));
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address(), new Address(), 0, false, true, true, true, null, new TimeSpan(), new TimeSpan(), commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.5"), false, 20, ZoneQuality.Online));

            target.calculateFunction(DateTime.Now);

            Assert.AreEqual(3, zoneServer.ZoneStateList.Count);     // OnValidityStart & OnFunctionStart with source and volume change
        }

        ///<summary>
        /// A test for ConcreteZoneChangeFunction command
        /// Expected Result: No exeception, three commands in zone state list
        ///</summary>
        [TestMethod()]
        public void ConcreteZoneChangeFunctionCommandTest5()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new SimpleId(), true, true, true, true, true, true, true, new Address("100.1"), "OFF", "100.6", 10));
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address(), new Address(), 0, true, false, true, true, null, new TimeSpan(), new TimeSpan(), commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.5"), false, 20, ZoneQuality.Online));

            target.calculateFunction(DateTime.Now);

            Assert.AreEqual(3, zoneServer.ZoneStateList.Count);     // OnValidityStart & OnFunctionStart with power status and volume change
        }

        ///<summary>
        /// A test for ConcreteZoneChangeFunction command
        /// Expected Result: No exeception, three commands in zone state list
        ///</summary>
        [TestMethod()]
        public void ConcreteZoneChangeFunctionCommandTest6()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new SimpleId(), true, true, true, true, true, true, true, new Address("100.1"), "OFF", "100.6", 10));
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address(), new Address(), 0, true, true, false, true, null, new TimeSpan(), new TimeSpan(), commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.5"), false, 20, ZoneQuality.Online));

            target.calculateFunction(DateTime.Now);

            Assert.AreEqual(3, zoneServer.ZoneStateList.Count);     // OnValidityStart & OnFunctionStart with power status and source change
        }

        ///<summary>
        /// A test for ConcreteZoneChangeFunction command
        /// Expected Result: No exeception, one commands in zone state list
        ///</summary>
        [TestMethod()]
        public void ConcreteZoneChangeFunctionCommandTest7()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new SimpleId(), true, true, true, true, true, true, true, new Address("100.1"), "OFF", "100.6", 10));
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address("100.1"), new Address("100.6"), 10, null, commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.5"), false, 20, ZoneQuality.Online));

            target.calculateFunction(DateTime.Now);

            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);     // OnValidityStart 
        }

        ///<summary>
        /// A test for ConcreteZoneChangeFunction command
        /// Expected Result: No exeception, one commands in zone state list
        ///</summary>
        [TestMethod()]
        public void ConcreteZoneChangeFunctionCommandTest8()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new SimpleId(), true, true, true, true, true, true, true, new Address("100.1"), "OFF", "100.6", 10));
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address("100.1"), new Address("100.6"), 10, null, commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.5"), false, 20, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.4"), true, 20, ZoneQuality.Online));

            target.calculateFunction(DateTime.Now);

            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);     // OnValidityStart 
        }

        ///<summary>
        /// A test for ConcreteZoneChangeFunction command
        /// Expected Result: No exeception, two commands in zone state list
        ///</summary>
        [TestMethod()]
        public void ConcreteZoneChangeFunctionCommandTest9()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new SimpleId(), true, true, true, true, true, true, true, new Address("100.1"), "OFF", "100.6", 10));
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address("100.1"), new Address(), 0, false, false, false, true, null, new TimeSpan(), new TimeSpan(), commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.5"), false, 20, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.4"), true, 20, ZoneQuality.Offline));

            target.calculateFunction(DateTime.Now);

            Assert.AreEqual(2, zoneServer.ZoneStateList.Count);     // OnValidityStart & OnFunctionStart + Quality change
        }

        ///<summary>
        /// A test for ConcreteZoneChangeFunction command
        /// Expected Result: No exeception, no commands in zone state list
        ///</summary>
        [TestMethod()]
        public void ConcreteZoneChangeFunctionCommandTest10()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new SimpleId(), false, false, false, false, false, true, true, new Address("100.1"), "OFF", "100.6", 10));
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address("100.1"), new Address(), 0, true, true, true, true, null, new TimeSpan(), new TimeSpan(), commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.5"), false, 20, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.4"), true, 20, ZoneQuality.Offline));

            target.calculateFunction(DateTime.Now);

            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);     // ---
        }

        ///<summary>
        /// A test for ConcreteZoneChangeFunction command
        /// Expected Result: No exeception, no commands in zone state list and no sound played
        ///</summary>
        [TestMethod()]
        public void ConcreteZoneChangeFunctionCommandTest11()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new SimpleId(), false, false, false, false, false, true, true, new Address("100.1"), "OFF", "100.6", 10));
            commands.Add(new PlaySoundCommand(new SimpleId(), false, false, false, false, false, true, true, new Address("100.2"), "http://www.imfeld.net/mp3_stream"));
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address("100.1"), new Address(), 0, true, true, true, true, null, new TimeSpan(), new TimeSpan(), commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.5"), false, 20, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.4"), true, 20, ZoneQuality.Offline));

            target.calculateFunction(DateTime.Now);

            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);                         // ---
            Assert.AreEqual("", ((AudioDriverMock)audioDrivers[2]).Url);                // no URL was played
            Assert.AreEqual(false, ((AudioDriverMock)audioDrivers[2]).IsPlaying);       // no URL was played
        }

        ///<summary>
        /// A test for ConcreteZoneChangeFunction command
        /// Expected Result: No exeception, no commands in zone state list but sound playing
        ///</summary>
        [TestMethod()]
        public void ConcreteZoneChangeFunctionCommandTest12()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new SimpleId(), false, false, false, false, false, true, true, new Address("100.1"), "OFF", "100.6", 10));
            commands.Add(new PlaySoundCommand(new SimpleId(), false, false, false, true, true, true, true, new Address("100.2"), "http://www.imfeld.net/mp3_stream"));
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address("100.1"), new Address(), 0, true, true, true, true, null, new TimeSpan(), new TimeSpan(), commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);

            target.calculateFunction(DateTime.Now);

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.5"), false, 20, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.4"), true, 20, ZoneQuality.Offline));

            target.calculateFunction(DateTime.Now);

            Assert.AreEqual(true, target.Active);                                                               // function is "active"
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);                                                 // ---
            Assert.AreEqual("http://www.imfeld.net/mp3_stream", ((AudioDriverMock)audioDrivers[2]).Url);        // URL is playing
            Assert.AreEqual(true, ((AudioDriverMock)audioDrivers[2]).IsPlaying);                                // URL is still playing
        }

        ///<summary>
        /// A test for ConcreteZoneChangeFunction command
        ///</summary>
        [TestMethod()]
        public void ConcreteZoneChangeFunctionCommandTest13()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new SimpleId(), false, false, false, true, true, true, true, new Address("100.1"), "OFF", "100.6", 10));
            commands.Add(new PlaySoundCommand(new SimpleId(), false, false, false, true, true, true, true, new Address("100.2"), "http://www.imfeld.net/mp3_stream"));
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address("100.1"), new Address(), 0, true, true, true, true, null, new TimeSpan(), new TimeSpan(), commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);

            target.calculateFunction(DateTime.Now);

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.5"), false, 20, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.4"), true, 20, ZoneQuality.Offline));

            target.calculateFunction(DateTime.Now);

            Assert.AreEqual(true, target.Active);                                                               // function is "active"
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);                                                 // OnValidityStart
            Assert.AreEqual("http://www.imfeld.net/mp3_stream", ((AudioDriverMock)audioDrivers[2]).Url);        // URL is playing
            Assert.AreEqual(true, ((AudioDriverMock)audioDrivers[2]).IsPlaying);                                // URL is still playing
        }

        ///<summary>
        /// A test for ConcreteZoneChangeFunction command
        ///</summary>
        [TestMethod()]
        public void ConcreteZoneChangeFunctionCommandTest14()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new SimpleId(), false, false, false, true, true, true, true, new Address("100.1"), "OFF", "100.6", 10));
            commands.Add(new PlaySoundCommand(new SimpleId(), false, false, false, true, true, true, true, new Address("100.2"), "http://www.imfeld.net/mp3_stream"));
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address("100.1"), new Address(), 0, true, true, true, true, null, new TimeSpan(14, 0, 0), new TimeSpan(15, 0, 0), commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);

            target.calculateFunction(new DateTime(2016,2,24,10,0,0) );
            //Assert.AreEqual(false, target.Active);                                                              // function is not active
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);                                                 // no command in queue
            Assert.AreEqual("", ((AudioDriverMock)audioDrivers[2]).Url);                                        // no URL is playing
            Assert.AreEqual(false, ((AudioDriverMock)audioDrivers[2]).IsPlaying);                               // not playing

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.5"), false, 20, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.4"), true, 20, ZoneQuality.Offline));

            target.calculateFunction(new DateTime(2016, 2, 24, 12, 0, 0));
            Assert.AreEqual(false, target.isActiveAt(new DateTime(2016, 2, 24, 12, 0, 0)));                     // function is not active
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);                                                 // no command in queue
            Assert.AreEqual("", ((AudioDriverMock)audioDrivers[2]).Url);                                        // no URL is playing
            Assert.AreEqual(false, ((AudioDriverMock)audioDrivers[2]).IsPlaying);                               // not playing

            target.calculateFunction(new DateTime(2016, 2, 24, 13, 59, 59));
            Assert.AreEqual(false, target.isActiveAt(new DateTime(2016, 2, 24, 13, 59, 59)));                   // function is not active
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);                                                 // no command in queue
            Assert.AreEqual("", ((AudioDriverMock)audioDrivers[2]).Url);                                        // no URL is playing
            Assert.AreEqual(false, ((AudioDriverMock)audioDrivers[2]).IsPlaying);                               // not playing

            target.calculateFunction(new DateTime(2016, 2, 24, 14, 0, 0));
            Assert.AreEqual(true, target.isActiveAt(new DateTime(2016, 2, 24, 14, 0, 0)));                      // function is active
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);                                                 // one command in queue
            Assert.AreEqual("http://www.imfeld.net/mp3_stream", ((AudioDriverMock)audioDrivers[2]).Url);        // URL is playing
            Assert.AreEqual(true, ((AudioDriverMock)audioDrivers[2]).IsPlaying);                                // is playing

            target.calculateFunction(new DateTime(2016, 2, 24, 14, 0, 1));
            Assert.AreEqual(true, target.isActiveAt(new DateTime(2016, 2, 24, 14, 0, 1)));                      // function not active
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);                                                 // no new command in queue
            Assert.AreEqual("http://www.imfeld.net/mp3_stream", ((AudioDriverMock)audioDrivers[2]).Url);        // URL is still playing
            Assert.AreEqual(true, ((AudioDriverMock)audioDrivers[2]).IsPlaying);                                // is playing

            target.calculateFunction(new DateTime(2016, 2, 24, 15, 0, 0));
            Assert.AreEqual(true, target.isActiveAt(new DateTime(2016, 2, 24, 15, 0, 0)));                      // function is active
            Assert.AreEqual(1, zoneServer.ZoneStateList.Count);                                                 // one command in queue
            Assert.AreEqual("http://www.imfeld.net/mp3_stream", ((AudioDriverMock)audioDrivers[2]).Url);        // URL is playing
            Assert.AreEqual(true, ((AudioDriverMock)audioDrivers[2]).IsPlaying);                                // is playing

            target.calculateFunction(new DateTime(2016, 2, 24, 15, 0, 1));
            Assert.AreEqual(false, target.isActiveAt(new DateTime(2016, 2, 24, 15, 0, 1)));                     // function not active anymore
            Assert.AreEqual(2, zoneServer.ZoneStateList.Count);                                                 // second command in queue
            Assert.AreEqual("", ((AudioDriverMock)audioDrivers[2]).Url);                                        // no URL is playing
            Assert.AreEqual(false, ((AudioDriverMock)audioDrivers[2]).IsPlaying);                               // not playing anymore

        }

        ///<summary>
        /// A test for ConcreteZoneChangeFunction command
        ///</summary>
        [TestMethod()]
        public void ConcreteZoneChangeFunctionCommandTest15()
        {
            List<Command> commands = new List<Command>();
            commands.Add(new SendNuvoCommand(new SimpleId(), false, true, true, true, true, true, true, new Address("100.1"), "OFF", "100.6", 10));
            commands.Add(new PlaySoundCommand(new SimpleId(), false, false, false, true, true, true, true, new Address("100.2"), "http://www.imfeld.net/mp3_stream"));
            ZoneChangeFunction function = new ZoneChangeFunction(new SimpleId(), new Address("100.1"), new Address(), 0, true, true, true, true, null, new TimeSpan(14, 0, 0), new TimeSpan(15, 0, 0), commands);
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = new Dictionary<int, IAudioDriver>();
            audioDrivers.Add(2, new AudioDriverMock());
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);

            target.calculateFunction(new DateTime(2016, 2, 24, 10, 0, 0));
            //Assert.AreEqual(false, target.Active);                                                              // function is not active
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);                                                 // no command in queue
            Assert.AreEqual("", ((AudioDriverMock)audioDrivers[2]).Url);                                        // no URL is playing
            Assert.AreEqual(false, ((AudioDriverMock)audioDrivers[2]).IsPlaying);                               // not playing

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.5"), false, 20, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.4"), true, 20, ZoneQuality.Offline));

            target.calculateFunction(new DateTime(2016, 2, 24, 12, 0, 0));
            Assert.AreEqual(false, target.isActiveAt(new DateTime(2016, 2, 24, 12, 0, 0)));                     // function is not active
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);                                                 // no command in queue
            Assert.AreEqual("", ((AudioDriverMock)audioDrivers[2]).Url);                                        // no URL is playing
            Assert.AreEqual(false, ((AudioDriverMock)audioDrivers[2]).IsPlaying);                               // not playing

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.5"), false, 20, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.4"), true, 20, ZoneQuality.Offline));

            target.calculateFunction(new DateTime(2016, 2, 24, 13, 59, 59));
            Assert.AreEqual(false, target.isActiveAt(new DateTime(2016, 2, 24, 13, 59, 59)));                   // function is not active
            Assert.AreEqual(0, zoneServer.ZoneStateList.Count);                                                 // no command in queue
            Assert.AreEqual("", ((AudioDriverMock)audioDrivers[2]).Url);                                        // no URL is playing
            Assert.AreEqual(false, ((AudioDriverMock)audioDrivers[2]).IsPlaying);                               // not playing

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));  // Source + Volume + Quality change
            zoneServer.distributeZoneState(new ZoneState(new Address("100.5"), false, 20, ZoneQuality.Online)); // Source + Status + Volume change
            zoneServer.distributeZoneState(new ZoneState(new Address("100.4"), true, 20, ZoneQuality.Offline)); // Source + Status + Quality change

            target.calculateFunction(new DateTime(2016, 2, 24, 14, 0, 0));
            Assert.AreEqual(true, target.isActiveAt(new DateTime(2016, 2, 24, 14, 0, 0)));                      // function is active
            Assert.AreEqual(10, zoneServer.ZoneStateList.Count);                                                 // 9 changes + 1 validity change
            Assert.AreEqual("http://www.imfeld.net/mp3_stream", ((AudioDriverMock)audioDrivers[2]).Url);        // URL is playing
            Assert.AreEqual(true, ((AudioDriverMock)audioDrivers[2]).IsPlaying);                                // is playing

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));  // Source + Volume + Quality change
            zoneServer.distributeZoneState(new ZoneState(new Address("100.5"), false, 20, ZoneQuality.Online)); // Source + Status + Volume change
            zoneServer.distributeZoneState(new ZoneState(new Address("100.4"), true, 20, ZoneQuality.Offline)); // Source + Status + Quality change

            target.calculateFunction(new DateTime(2016, 2, 24, 14, 0, 1));
            Assert.AreEqual(true, target.isActiveAt(new DateTime(2016, 2, 24, 14, 0, 1)));                      // function not active
            Assert.AreEqual(19, zoneServer.ZoneStateList.Count);                                                // 9 more command in queue
            Assert.AreEqual("http://www.imfeld.net/mp3_stream", ((AudioDriverMock)audioDrivers[2]).Url);        // URL is still playing
            Assert.AreEqual(true, ((AudioDriverMock)audioDrivers[2]).IsPlaying);                                // is playing

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));  // Source + Volume + Quality change
            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), false, 20, ZoneQuality.Online)); // Status + Volume change
            zoneServer.distributeZoneState(new ZoneState(new Address("100.4"), true, 20, ZoneQuality.Online));  // Source + Status change

            target.calculateFunction(new DateTime(2016, 2, 24, 15, 0, 0));
            Assert.AreEqual(true, target.isActiveAt(new DateTime(2016, 2, 24, 15, 0, 0)));                      // function is active
            Assert.AreEqual(26, zoneServer.ZoneStateList.Count);                                                // 7 more command in queue
            Assert.AreEqual("http://www.imfeld.net/mp3_stream", ((AudioDriverMock)audioDrivers[2]).Url);        // URL is playing
            Assert.AreEqual(true, ((AudioDriverMock)audioDrivers[2]).IsPlaying);                                // is playing

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));  // Source + Volume change
            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), false, 20, ZoneQuality.Online)); // Status + Volume change
            zoneServer.distributeZoneState(new ZoneState(new Address("100.4"), true, 20, ZoneQuality.Online));  // Source + Status change

            target.calculateFunction(new DateTime(2016, 2, 24, 15, 0, 1));
            Assert.AreEqual(false, target.isActiveAt(new DateTime(2016, 2, 24, 15, 0, 1)));                     // function not active anymore
            Assert.AreEqual(27, zoneServer.ZoneStateList.Count);                                                // + validity end command
            Assert.AreEqual("", ((AudioDriverMock)audioDrivers[2]).Url);                                        // no URL is playing
            Assert.AreEqual(false, ((AudioDriverMock)audioDrivers[2]).IsPlaying);                               // not playing anymore

        }


/*
        /// <summary>
        ///A test for Dispose
        ///</summary>
        [TestMethod()]
        public void DisposeTest()
        {
            ZoneChangeFunction function = new ZoneChangeFunction();
            IZoneServer zoneServer = null;
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);
            target.Dispose();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for calculateFunction
        ///</summary>
        [TestMethod()]
        public void calculateFunctionTest()
        {
            ZoneChangeFunction function = new ZoneChangeFunction();
            IZoneServer zoneServer = null;
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);
            DateTime aktTime = new DateTime();
            target.calculateFunction(aktTime);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
*/
  
    }
}

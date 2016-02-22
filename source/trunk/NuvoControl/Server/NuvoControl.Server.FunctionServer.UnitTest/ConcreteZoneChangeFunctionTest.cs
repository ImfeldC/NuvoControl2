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
            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address(), new Address(), 0, dayOfWeek, null );
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
            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address(), new Address(), 0, dayOfWeek, null);
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
            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address(), new Address(), 0, dayOfWeek, null);
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
            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address(), new Address(), 0, null, validFrom, validTo, null);
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
            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address(), new Address(), 0, null, validFrom, validTo, null);
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
            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address(), new Address(), 0, dayOfWeek, validFrom, validTo, null);
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
            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address(), new Address(), 0, dayOfWeek, validFrom, validTo, null);
            IZoneServer zoneServer = null;
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);
            bool actual;
            actual = target.Active;
            Assert.AreEqual(false, actual);
        }

        ///<summary>
        /// A test for Active
        /// Expected result: False, because valid ended 1h ago, even is all days are in list.
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
            TimeSpan validFrom = new TimeSpan(0, 0, 0);
            TimeSpan validTo = new TimeSpan(DateTime.Now.Hour - 1, 59, 59);
            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address(), new Address(), 0, dayOfWeek, validFrom, validTo, null);
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
            commands.Add(new SendNuvoCommand(new Guid(), true, true, true, true, true, true, true, new Address("100.1"), "OFF", "100.6", 10));
            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address(), new Address(), 0, null, commands);
            function.OnStatusChange = true;
            function.OnSourceChange = true;
            function.OnVolumeChange = true;
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
            commands.Add(new SendNuvoCommand(new Guid(), true, true, true, true, true, true, true, new Address("100.1"), "OFF", "100.6", 10));
            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address(), new Address(), 0, null, commands);
            function.OnStatusChange = true;
            function.OnSourceChange = true;
            function.OnVolumeChange = true;
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
            commands.Add(new SendNuvoCommand(new Guid(), true, true, true, true, true, true, true, new Address("100.1"), "OFF", "100.6", 10));
            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address("100.1"), new Address("100.6"), 10, null, commands);
            function.OnStatusChange = true;
            function.OnSourceChange = true;
            function.OnVolumeChange = true;
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));

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
            commands.Add(new SendNuvoCommand(new Guid(), true, true, true, true, true, true, true, new Address("100.1"), "OFF", "100.6", 10));
            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address("100.1"), new Address("100.6"), 10, null, commands);
            function.OnStatusChange = true;
            function.OnSourceChange = true;
            function.OnVolumeChange = true;
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), false, 10, ZoneQuality.Online));

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
            commands.Add(new SendNuvoCommand(new Guid(), true, true, true, true, true, true, true, new Address("100.1"), "OFF", "100.6", 10));
            ZoneChangeFunction function = new ZoneChangeFunction(new Guid(), new Address("100.1"), new Address("100.6"), 10, null, commands);
            function.OnStatusChange = true;
            function.OnSourceChange = true;
            function.OnVolumeChange = true;
            ZoneServerMock zoneServer = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteZoneChangeFunction target = new ConcreteZoneChangeFunction(function, zoneServer, audioDrivers);

            zoneServer.distributeZoneState(new ZoneState(new Address("100.6"), true, 10, ZoneQuality.Online));
            zoneServer.distributeZoneState(new ZoneState(new Address("100.5"), false, 20, ZoneQuality.Online));

            Assert.AreEqual(4, zoneServer.ZoneStateList.Count);     // OnValidityStart & OnFunctionStart with power status, source and volume change
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

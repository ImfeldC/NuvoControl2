using NuvoControl.Server.FunctionServer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ZoneServer;
using System;
using System.Collections.Generic;
using NuvoControl.Common;
using NuvoControl.Server.ProtocolDriver.Interface;

namespace NuvoControl.Server.FunctionServer.UnitTest
{
    
    
    /// <summary>
    ///This is a test class for ConcreteAlarmFunctionTest and is intended
    ///to contain all ConcreteAlarmFunctionTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConcreteAlarmFunctionTest
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
        /// A test for ConcreteAlarmFunction Constructor.
        /// An excpetion is expected, because of the missing function configuration.
        /// </summary>
        [TestMethod()]
        public void ConcreteAlarmFunctionConstructorTest1()
        {
            try
            {
                AlarmFunction function = null;
                IZoneServer zoneServer = null;
                Dictionary<int, IAudioDriver> audioDrivers = null;
                ConcreteAlarmFunction target = new ConcreteAlarmFunction(function, zoneServer,audioDrivers);
            }
            catch (FunctionServerException)
            {
                return;
            }
            Assert.Fail("'FunctionServerException' Exception expected!");
        }

        /// <summary>
        /// A test for ConcreteAlarmFunction Constructor.
        /// No exception is expected.
        /// </summary>
        [TestMethod()]
        public void ConcreteAlarmFunctionConstructorTest2()
        {
            List<DayOfWeek> _dayOfWeeks = new List<DayOfWeek>();
            AlarmFunction function = new AlarmFunction(
                Guid.NewGuid(), 
                new Address(100, 1), new Address(100, 1), 50,
                new TimeSpan(6, 0, 0), new TimeSpan(0, 45, 0), 
                _dayOfWeeks);
            IZoneServer zoneServer = null;
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteAlarmFunction target = new ConcreteAlarmFunction(function, zoneServer,audioDrivers);
        }


        /// <summary>
        /// A test for isFunctionActiveToday
        /// The function is not active, because the list of days is empty.
        /// </summary>
        [TestMethod()]
        public void isFunctionActiveTodayTest1()
        {
            List<DayOfWeek> _dayOfWeeks = new List<DayOfWeek>();
            AlarmFunction function = new AlarmFunction(
                Guid.NewGuid(),
                new Address(100, 1), new Address(100, 1), 50,
                new TimeSpan(6, 0, 0), new TimeSpan(0, 45, 0),
                _dayOfWeeks);
            IZoneServer zoneServer = null;
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteAlarmFunction target = new ConcreteAlarmFunction(function, zoneServer,audioDrivers); 
            DateTime aktTime = DateTime.Now;
            bool actual = target.isFunctionActiveToday(aktTime);
            Assert.AreEqual(false, actual);
        }

        /// <summary>
        /// A test for isFunctionActiveToday
        /// The function is active, because the current day is part of the list.
        /// </summary>
        [TestMethod()]
        public void isFunctionActiveTodayTest2()
        {
            List<DayOfWeek> _dayOfWeeks = new List<DayOfWeek>();
            _dayOfWeeks.Add(DateTime.Now.DayOfWeek);
            AlarmFunction function = new AlarmFunction(
                Guid.NewGuid(),
                new Address(100, 1), new Address(100, 1), 50,
                new TimeSpan(6, 0, 0), new TimeSpan(0, 45, 0),
                _dayOfWeeks);
            IZoneServer zoneServer = null;
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteAlarmFunction target = new ConcreteAlarmFunction(function, zoneServer,audioDrivers); 
            DateTime aktTime = DateTime.Now;
            bool actual = target.isFunctionActiveToday(aktTime);
            Assert.AreEqual(true, actual);
        }

        /// <summary>
        /// A test for isFunctionActiveToday
        /// The function1 is never active, the function2 is always active.
        /// </summary>
        [TestMethod()]
        public void isFunctionActiveTodayTest3()
        {
            IZoneServer zoneServer = null;

            // (1) Empty day list
            List<DayOfWeek> _dayOfWeeks1 = new List<DayOfWeek>();
            AlarmFunction function1 = new AlarmFunction(
                Guid.NewGuid(),
                new Address(100, 1), new Address(100, 1), 50,
                new TimeSpan(6, 0, 0), new TimeSpan(0, 45, 0),
                _dayOfWeeks1);
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteAlarmFunction target1 = new ConcreteAlarmFunction(function1, zoneServer,audioDrivers); 

            // (2) Full day list
            List<DayOfWeek> _dayOfWeeks2 = new List<DayOfWeek>();
            _dayOfWeeks2.Add(DayOfWeek.Monday);
            _dayOfWeeks2.Add(DayOfWeek.Tuesday);
            _dayOfWeeks2.Add(DayOfWeek.Wednesday);
            _dayOfWeeks2.Add(DayOfWeek.Thursday);
            _dayOfWeeks2.Add(DayOfWeek.Friday);
            _dayOfWeeks2.Add(DayOfWeek.Saturday);
            _dayOfWeeks2.Add(DayOfWeek.Sunday);
            AlarmFunction function2 = new AlarmFunction(
                Guid.NewGuid(),
                new Address(100, 1), new Address(100, 1), 50,
                new TimeSpan(6, 0, 0), new TimeSpan(0, 45, 0),
                _dayOfWeeks2);
            ConcreteAlarmFunction target2 = new ConcreteAlarmFunction(function2, zoneServer,audioDrivers); 

            // Monday
            Assert.AreEqual(false, target1.isFunctionActiveToday(new DateTime( 2009, 9, 7, 12, 0,0)));
            Assert.AreEqual(true, target2.isFunctionActiveToday(new DateTime(2009, 9, 7, 12, 0, 0)));

            // Tuesday
            Assert.AreEqual(false, target1.isFunctionActiveToday(new DateTime(2009, 9, 8, 12, 0, 0)));
            Assert.AreEqual(true, target2.isFunctionActiveToday(new DateTime(2009, 9, 8, 12, 0, 0)));

            // Wednesday
            Assert.AreEqual(false, target1.isFunctionActiveToday(new DateTime(2009, 9, 9, 12, 0, 0)));
            Assert.AreEqual(true, target2.isFunctionActiveToday(new DateTime(2009, 9, 9, 12, 0, 0)));

            // Thursday
            Assert.AreEqual(false, target1.isFunctionActiveToday(new DateTime(2009, 9, 10, 12, 0, 0)));
            Assert.AreEqual(true, target2.isFunctionActiveToday(new DateTime(2009, 9, 10, 12, 0, 0)));

            // Friday
            Assert.AreEqual(false, target1.isFunctionActiveToday(new DateTime(2009, 9, 11, 12, 0, 0)));
            Assert.AreEqual(true, target2.isFunctionActiveToday(new DateTime(2009, 9, 11, 12, 0, 0)));

            // Saturday
            Assert.AreEqual(false, target1.isFunctionActiveToday(new DateTime(2009, 9, 12, 12, 0, 0)));
            Assert.AreEqual(true, target2.isFunctionActiveToday(new DateTime(2009, 9, 12, 12, 0, 0)));

            // Sunday
            Assert.AreEqual(false, target1.isFunctionActiveToday(new DateTime(2009, 9, 13, 12, 0, 0)));
            Assert.AreEqual(true, target2.isFunctionActiveToday(new DateTime(2009, 9, 13, 12, 0, 0)));
        }

        /// <summary>
        /// A test for isFunctionActiveToday
        /// The function1 is never active, the function2 is always active.
        /// </summary>
        [TestMethod()]
        public void isFunctionActiveTodayTest4()
        {
            IZoneServer zoneServer = null;

            // (1) Day list: Monday, Wednesday, Friday, Sunday
            List<DayOfWeek> _dayOfWeeks1 = new List<DayOfWeek>();
            _dayOfWeeks1.Add(DayOfWeek.Monday);
            _dayOfWeeks1.Add(DayOfWeek.Wednesday);
            _dayOfWeeks1.Add(DayOfWeek.Friday);
            _dayOfWeeks1.Add(DayOfWeek.Sunday);
            AlarmFunction function1 = new AlarmFunction(
                Guid.NewGuid(),
                new Address(100, 1), new Address(100, 1), 50,
                new TimeSpan(6, 0, 0), new TimeSpan(0, 45, 0),
                _dayOfWeeks1);
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteAlarmFunction target1 = new ConcreteAlarmFunction(function1, zoneServer,audioDrivers);

            // (2) Day list: Tuesday, Thursday, Saturday, Sunday
            List<DayOfWeek> _dayOfWeeks2 = new List<DayOfWeek>();
            _dayOfWeeks2.Add(DayOfWeek.Tuesday);
            _dayOfWeeks2.Add(DayOfWeek.Thursday);
            _dayOfWeeks2.Add(DayOfWeek.Saturday);
            _dayOfWeeks2.Add(DayOfWeek.Sunday);
            AlarmFunction function2 = new AlarmFunction(
                Guid.NewGuid(),
                new Address(100, 1), new Address(100, 1), 50,
                new TimeSpan(6, 0, 0), new TimeSpan(0, 45, 0),
                _dayOfWeeks2);
            ConcreteAlarmFunction target2 = new ConcreteAlarmFunction(function2, zoneServer,audioDrivers);

            // Monday
            Assert.AreEqual(true, target1.isFunctionActiveToday(new DateTime(2009, 9, 7, 12, 0, 0)));
            Assert.AreEqual(false, target2.isFunctionActiveToday(new DateTime(2009, 9, 7, 12, 0, 0)));

            // Tuesday
            Assert.AreEqual(false, target1.isFunctionActiveToday(new DateTime(2009, 9, 8, 12, 0, 0)));
            Assert.AreEqual(true, target2.isFunctionActiveToday(new DateTime(2009, 9, 8, 12, 0, 0)));

            // Wednesday
            Assert.AreEqual(true, target1.isFunctionActiveToday(new DateTime(2009, 9, 9, 12, 0, 0)));
            Assert.AreEqual(false, target2.isFunctionActiveToday(new DateTime(2009, 9, 9, 12, 0, 0)));

            // Thursday
            Assert.AreEqual(false, target1.isFunctionActiveToday(new DateTime(2009, 9, 10, 12, 0, 0)));
            Assert.AreEqual(true, target2.isFunctionActiveToday(new DateTime(2009, 9, 10, 12, 0, 0)));

            // Friday
            Assert.AreEqual(true, target1.isFunctionActiveToday(new DateTime(2009, 9, 11, 12, 0, 0)));
            Assert.AreEqual(false, target2.isFunctionActiveToday(new DateTime(2009, 9, 11, 12, 0, 0)));

            // Saturday
            Assert.AreEqual(false, target1.isFunctionActiveToday(new DateTime(2009, 9, 12, 12, 0, 0)));
            Assert.AreEqual(true, target2.isFunctionActiveToday(new DateTime(2009, 9, 12, 12, 0, 0)));

            // Sunday
            Assert.AreEqual(true, target1.isFunctionActiveToday(new DateTime(2009, 9, 13, 12, 0, 0)));
            Assert.AreEqual(true, target2.isFunctionActiveToday(new DateTime(2009, 9, 13, 12, 0, 0)));
        }


        /// <summary>
        /// A test for calculateFunction
        /// Expected result: Each function sends a command to switch on the zone.
        /// </summary>
        [TestMethod()]
        public void calculateFunctionTest1()
        {

            // (1) Day list: Monday, Wednesday, Friday, Sunday
            // (1) Function: AlarmTime=10:00, AlarmDuration=01:45
            #region Function1
            List<DayOfWeek> _dayOfWeeks1 = new List<DayOfWeek>();
            _dayOfWeeks1.Add(DayOfWeek.Monday);
            _dayOfWeeks1.Add(DayOfWeek.Wednesday);
            _dayOfWeeks1.Add(DayOfWeek.Friday);
            _dayOfWeeks1.Add(DayOfWeek.Sunday);
            AlarmFunction function1 = new AlarmFunction(
                Guid.NewGuid(),
                new Address(100, 1), new Address(100, 3), 50,
                new TimeSpan(10, 0, 0), new TimeSpan(1, 45, 0),
                _dayOfWeeks1);
            ZoneServerMock zoneServer1 = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteAlarmFunction target1 = new ConcreteAlarmFunction(function1, zoneServer1,audioDrivers);
            #endregion

            // (2) Day list: Tuesday, Thursday, Saturday, Sunday
            // (2) Function: AlarmTime=10:00, AlarmDuration=01:45
            #region Function2
            List<DayOfWeek> _dayOfWeeks2 = new List<DayOfWeek>();
            _dayOfWeeks2.Add(DayOfWeek.Tuesday);
            _dayOfWeeks2.Add(DayOfWeek.Thursday);
            _dayOfWeeks2.Add(DayOfWeek.Saturday);
            _dayOfWeeks2.Add(DayOfWeek.Sunday);
            AlarmFunction function2 = new AlarmFunction(
                Guid.NewGuid(),
                new Address(100, 1), new Address(100, 6), 50,
                new TimeSpan(10, 0, 0), new TimeSpan(1, 45, 0),
                _dayOfWeeks2);
            ZoneServerMock zoneServer2 = new ZoneServerMock();
            ConcreteAlarmFunction target2 = new ConcreteAlarmFunction(function2, zoneServer2,audioDrivers);
            #endregion

            // Monday (7.9.2009 12:00)
            target1.calculateFunction(new DateTime(2009, 9, 7, 12, 0, 0));
            target2.calculateFunction(new DateTime(2009, 9, 7, 12, 0, 0));
            Assert.AreEqual(1, zoneServer1._monitoredZones.Count);
            Assert.AreEqual(1, zoneServer2._monitoredZones.Count);
            Assert.AreEqual(0, zoneServer1._zoneStates.Count);
            Assert.AreEqual(0, zoneServer2._zoneStates.Count);

            // Monday (7.9.2009 11:00)
            zoneServer1.distributeZoneState(new ZoneState(new Address(100, 2), false, 20, ZoneQuality.Online));
            target1.calculateFunction(new DateTime(2009, 9, 7, 11, 0, 0));
            zoneServer2.distributeZoneState(new ZoneState(new Address(100, 2), false, 21, ZoneQuality.Online));
            target2.calculateFunction(new DateTime(2009, 9, 7, 11, 0, 0));
            Assert.AreEqual(1, zoneServer1._monitoredZones.Count);
            Assert.AreEqual(1, zoneServer2._monitoredZones.Count);
            Assert.AreEqual(1, zoneServer1._zoneStates.Count);  // 1 command has been sent
            Assert.AreEqual(true, zoneServer1._zoneStates[new Address(100, 1)].PowerStatus);
            Assert.AreEqual(new Address(100,3), zoneServer1._zoneStates[new Address(100, 1)].Source);
            Assert.AreEqual(0, zoneServer2._zoneStates.Count);
            zoneServer1.ClearZoneStateList();

            // Tuesday (8.9.2009 11:00)
            zoneServer1.distributeZoneState(new ZoneState(new Address(100, 2), false, 20, ZoneQuality.Online));
            target1.calculateFunction(new DateTime(2009, 9, 8, 11, 0, 0));
            zoneServer2.distributeZoneState(new ZoneState(new Address(100, 2), false, 21, ZoneQuality.Online));
            target2.calculateFunction(new DateTime(2009, 9, 8, 11, 0, 0));
            Assert.AreEqual(1, zoneServer1._monitoredZones.Count);
            Assert.AreEqual(1, zoneServer2._monitoredZones.Count);
            Assert.AreEqual(0, zoneServer1._zoneStates.Count);
            Assert.AreEqual(1, zoneServer2._zoneStates.Count);  // 1 command has been sent
            Assert.AreEqual(true, zoneServer2._zoneStates[new Address(100, 1)].PowerStatus);
            Assert.AreEqual(new Address(100, 6), zoneServer2._zoneStates[new Address(100, 1)].Source);
            zoneServer2.ClearZoneStateList();

            // Sunday (13.9.2009 11:00)
            zoneServer1.distributeZoneState(new ZoneState(new Address(100, 2), false, 20, ZoneQuality.Online));
            target1.calculateFunction(new DateTime(2009, 9, 13, 11, 0, 0));
            zoneServer2.distributeZoneState(new ZoneState(new Address(100, 2), false, 21, ZoneQuality.Online));
            target2.calculateFunction(new DateTime(2009, 9, 13, 11, 0, 0));
            Assert.AreEqual(1, zoneServer1._monitoredZones.Count);
            Assert.AreEqual(1, zoneServer2._monitoredZones.Count);
            Assert.AreEqual(1, zoneServer1._zoneStates.Count);  // 1 command has been sent
            Assert.AreEqual(true, zoneServer1._zoneStates[new Address(100, 1)].PowerStatus);
            Assert.AreEqual(new Address(100, 3), zoneServer1._zoneStates[new Address(100, 1)].Source);
            Assert.AreEqual(1, zoneServer2._zoneStates.Count);  // 1 command has been sent
            Assert.AreEqual(true, zoneServer2._zoneStates[new Address(100, 1)].PowerStatus);
            Assert.AreEqual(new Address(100, 6), zoneServer2._zoneStates[new Address(100, 1)].Source);
            zoneServer1.ClearZoneStateList();
            zoneServer2.ClearZoneStateList();
        }

        /// <summary>
        /// A test for calculateFunction
        /// The function has been initialized, with 
        /// - 'Off' at 09:00
        /// -  'On' at 09:05
        /// - 'Off' at 09:10
        /// Expected result: Each function sends a command to switch on the zone.
        /// </summary>
        [TestMethod()]
        public void calculateFunctionTest2()
        {

            // (1) Day list: Monday, Wednesday, Friday, Sunday
            // (1) Function: AlarmTime=10:00, AlarmDuration=01:45
            #region Function1
            List<DayOfWeek> _dayOfWeeks1 = new List<DayOfWeek>();
            _dayOfWeeks1.Add(DayOfWeek.Monday);
            _dayOfWeeks1.Add(DayOfWeek.Wednesday);
            _dayOfWeeks1.Add(DayOfWeek.Friday);
            _dayOfWeeks1.Add(DayOfWeek.Sunday);
            AlarmFunction function1 = new AlarmFunction(
                Guid.NewGuid(),
                new Address(100, 1), new Address(100, 3), 50,
                new TimeSpan(10, 0, 0), new TimeSpan(1, 45, 0),
                _dayOfWeeks1);
            ZoneServerMock zoneServer1 = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteAlarmFunction target1 = new ConcreteAlarmFunction(function1, zoneServer1,audioDrivers);
            #endregion

            // (2) Day list: Tuesday, Thursday, Saturday, Sunday
            // (2) Function: AlarmTime=10:00, AlarmDuration=01:45
            #region Function2
            List<DayOfWeek> _dayOfWeeks2 = new List<DayOfWeek>();
            _dayOfWeeks2.Add(DayOfWeek.Tuesday);
            _dayOfWeeks2.Add(DayOfWeek.Thursday);
            _dayOfWeeks2.Add(DayOfWeek.Saturday);
            _dayOfWeeks2.Add(DayOfWeek.Sunday);
            AlarmFunction function2 = new AlarmFunction(
                Guid.NewGuid(),
                new Address(100, 1), new Address(100, 6), 50,
                new TimeSpan(10, 0, 0), new TimeSpan(1, 45, 0),
                _dayOfWeeks2);
            ZoneServerMock zoneServer2 = new ZoneServerMock();
            ConcreteAlarmFunction target2 = new ConcreteAlarmFunction(function2, zoneServer2,audioDrivers);
            #endregion

            ZoneState zoneStateOff1 = new ZoneState(new Address(100, 2), false, 20, ZoneQuality.Online);
            zoneStateOff1.LastUpdate = new DateTime(2009, 9, 13, 09, 0, 0);
            ZoneState zoneStateOn = new ZoneState(new Address(100, 2), true, 20, ZoneQuality.Online);
            zoneStateOn.LastUpdate = new DateTime(2009, 9, 13, 09, 5, 0);
            ZoneState zoneStateOff2 = new ZoneState(new Address(100, 2), false, 20, ZoneQuality.Online);
            zoneStateOff2.LastUpdate = new DateTime(2009, 9, 13, 09, 10, 0);

            // Sunday (13.9.2009 11:00)
            zoneServer1.distributeZoneState(zoneStateOff1);
            zoneServer1.distributeZoneState(zoneStateOn);    // sets the member LastChangeToON
            zoneServer1.distributeZoneState(zoneStateOff2);
            target1.calculateFunction(new DateTime(2009, 9, 13, 11, 0, 0));

            zoneServer2.distributeZoneState(zoneStateOff1);
            zoneServer2.distributeZoneState(zoneStateOn);    // sets the member LastChangeToON
            zoneServer2.distributeZoneState(zoneStateOff2);
            target2.calculateFunction(new DateTime(2009, 9, 13, 11, 44, 59));   // last change before functions ends

            Assert.AreEqual(1, zoneServer1._monitoredZones.Count);
            Assert.AreEqual(1, zoneServer2._monitoredZones.Count);
            Assert.AreEqual(1, zoneServer1._zoneStates.Count);  // 1 command has been sent
            Assert.AreEqual(true, zoneServer1._zoneStates[new Address(100, 1)].PowerStatus);
            Assert.AreEqual(new Address(100, 3), zoneServer1._zoneStates[new Address(100, 1)].Source);
            Assert.AreEqual(1, zoneServer2._zoneStates.Count);  // 1 command has been sent
            Assert.AreEqual(true, zoneServer2._zoneStates[new Address(100, 1)].PowerStatus);
            Assert.AreEqual(new Address(100, 6), zoneServer2._zoneStates[new Address(100, 1)].Source);
            zoneServer1.ClearZoneStateList();
            zoneServer2.ClearZoneStateList();
        }

        /// <summary>
        /// A test for calculateFunction
        /// The function has been initialized, with 
        /// - 'Off' at 09:00
        /// -  'On' at 10:05
        /// - 'Off' at 10:10
        /// Expected result: No command is sent by the function, because the 'on' state arrived 
        /// after the Alarm time.
        /// </summary>
        [TestMethod()]
        public void calculateFunctionTest3()
        {

            // (1) Day list: Monday, Wednesday, Friday, Sunday
            // (1) Function: AlarmTime=10:00, AlarmDuration=01:45
            #region Function1
            List<DayOfWeek> _dayOfWeeks1 = new List<DayOfWeek>();
            _dayOfWeeks1.Add(DayOfWeek.Monday);
            _dayOfWeeks1.Add(DayOfWeek.Wednesday);
            _dayOfWeeks1.Add(DayOfWeek.Friday);
            _dayOfWeeks1.Add(DayOfWeek.Sunday);
            AlarmFunction function1 = new AlarmFunction(
                Guid.NewGuid(),
                new Address(100, 1), new Address(100, 3), 50,
                new TimeSpan(10, 0, 0), new TimeSpan(1, 45, 0),
                _dayOfWeeks1);
            ZoneServerMock zoneServer1 = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteAlarmFunction target1 = new ConcreteAlarmFunction(function1, zoneServer1, audioDrivers);
            #endregion

            // (2) Day list: Tuesday, Thursday, Saturday, Sunday
            // (2) Function: AlarmTime=10:00, AlarmDuration=01:45
            #region Function2
            List<DayOfWeek> _dayOfWeeks2 = new List<DayOfWeek>();
            _dayOfWeeks2.Add(DayOfWeek.Tuesday);
            _dayOfWeeks2.Add(DayOfWeek.Thursday);
            _dayOfWeeks2.Add(DayOfWeek.Saturday);
            _dayOfWeeks2.Add(DayOfWeek.Sunday);
            AlarmFunction function2 = new AlarmFunction(
                Guid.NewGuid(),
                new Address(100, 1), new Address(100, 6), 50,
                new TimeSpan(10, 0, 0), new TimeSpan(1, 45, 0),
                _dayOfWeeks2);
            ZoneServerMock zoneServer2 = new ZoneServerMock();
            ConcreteAlarmFunction target2 = new ConcreteAlarmFunction(function2, zoneServer2,audioDrivers);
            #endregion

            ZoneState zoneStateOff1 = new ZoneState(new Address(100, 2), false, 20, ZoneQuality.Online);
            zoneStateOff1.LastUpdate = new DateTime(2009, 9, 13, 09, 0, 0);
            ZoneState zoneStateOn = new ZoneState(new Address(100, 2), true, 20, ZoneQuality.Online);
            zoneStateOn.LastUpdate = new DateTime(2009, 9, 13, 10, 5, 0);
            ZoneState zoneStateOff2 = new ZoneState(new Address(100, 2), false, 20, ZoneQuality.Online);
            zoneStateOff2.LastUpdate = new DateTime(2009, 9, 13, 10, 10, 0);

            // Sunday (13.9.2009 11:00)
            zoneServer1.distributeZoneState(zoneStateOff1);
            zoneServer1.distributeZoneState(zoneStateOn);    // sets the member LastChangeToON
            zoneServer1.distributeZoneState(zoneStateOff2);
            target1.calculateFunction(new DateTime(2009, 9, 13, 11, 0, 0));

            zoneServer2.distributeZoneState(zoneStateOff1);
            zoneServer2.distributeZoneState(zoneStateOn);    // sets the member LastChangeToON
            zoneServer2.distributeZoneState(zoneStateOff2);
            target2.calculateFunction(new DateTime(2009, 9, 13, 11, 44, 59));

            Assert.AreEqual(1, zoneServer1._monitoredZones.Count);
            Assert.AreEqual(1, zoneServer2._monitoredZones.Count);
            Assert.AreEqual(0, zoneServer1._zoneStates.Count);  // 0 command has been sent
            Assert.AreEqual(0, zoneServer2._zoneStates.Count);  // 0 command has been sent
            zoneServer1.ClearZoneStateList();
            zoneServer2.ClearZoneStateList();
        }


        /// <summary>
        /// A test for calculateFunction
        /// The function has been initialized, with 
        /// - 'Off' at 12.9.2009 23:00
        /// -  'On' at 12.9.2009 23:05
        /// - 'Off' at 12.9.2009 23:10
        /// Expected result: Each function sends a command to switch on the zone.
        /// </summary>
        [TestMethod()]
        public void calculateFunctionTest4()
        {

            // (1) Day list: Monday, Wednesday, Friday, Sunday
            // (1) Function: AlarmTime=10:00, AlarmDuration=01:45
            #region Function1
            List<DayOfWeek> _dayOfWeeks1 = new List<DayOfWeek>();
            _dayOfWeeks1.Add(DayOfWeek.Monday);
            _dayOfWeeks1.Add(DayOfWeek.Wednesday);
            _dayOfWeeks1.Add(DayOfWeek.Friday);
            _dayOfWeeks1.Add(DayOfWeek.Sunday);
            AlarmFunction function1 = new AlarmFunction(
                Guid.NewGuid(),
                new Address(100, 1), new Address(100, 3), 50,
                new TimeSpan(10, 0, 0), new TimeSpan(1, 45, 0),
                _dayOfWeeks1);
            ZoneServerMock zoneServer1 = new ZoneServerMock();
            Dictionary<int, IAudioDriver> audioDrivers = null;
            ConcreteAlarmFunction target1 = new ConcreteAlarmFunction(function1, zoneServer1, audioDrivers);
            #endregion

            // (2) Day list: Tuesday, Thursday, Saturday, Sunday
            // (2) Function: AlarmTime=10:00, AlarmDuration=01:45
            #region Function2
            List<DayOfWeek> _dayOfWeeks2 = new List<DayOfWeek>();
            _dayOfWeeks2.Add(DayOfWeek.Tuesday);
            _dayOfWeeks2.Add(DayOfWeek.Thursday);
            _dayOfWeeks2.Add(DayOfWeek.Saturday);
            _dayOfWeeks2.Add(DayOfWeek.Sunday);
            AlarmFunction function2 = new AlarmFunction(
                Guid.NewGuid(),
                new Address(100, 1), new Address(100, 6), 50,
                new TimeSpan(10, 0, 0), new TimeSpan(1, 45, 0),
                _dayOfWeeks2);
            ZoneServerMock zoneServer2 = new ZoneServerMock();
            ConcreteAlarmFunction target2 = new ConcreteAlarmFunction(function2, zoneServer2,audioDrivers);
            #endregion

            ZoneState zoneStateOff1 = new ZoneState(new Address(100, 2), false, 20, ZoneQuality.Online);
            zoneStateOff1.LastUpdate = new DateTime(2009, 9, 12, 23, 0, 0);
            ZoneState zoneStateOn = new ZoneState(new Address(100, 2), true, 20, ZoneQuality.Online);
            zoneStateOn.LastUpdate = new DateTime(2009, 9, 12, 23, 5, 0);
            ZoneState zoneStateOff2 = new ZoneState(new Address(100, 2), false, 20, ZoneQuality.Online);
            zoneStateOff2.LastUpdate = new DateTime(2009, 9, 12, 23, 10, 0);

            // Sunday (13.9.2009 11:00)
            zoneServer1.distributeZoneState(zoneStateOff1);
            zoneServer1.distributeZoneState(zoneStateOn);    // sets the member LastChangeToON
            zoneServer1.distributeZoneState(zoneStateOff2);
            target1.calculateFunction(new DateTime(2009, 9, 13, 11, 0, 0));

            zoneServer2.distributeZoneState(zoneStateOff1);
            zoneServer2.distributeZoneState(zoneStateOn);    // sets the member LastChangeToON
            zoneServer2.distributeZoneState(zoneStateOff2);
            target2.calculateFunction(new DateTime(2009, 9, 13, 11, 44, 59));   // last change before functions ends

            Assert.AreEqual(1, zoneServer1._monitoredZones.Count);
            Assert.AreEqual(1, zoneServer2._monitoredZones.Count);
            Assert.AreEqual(1, zoneServer1._zoneStates.Count);  // 1 command has been sent
            Assert.AreEqual(true, zoneServer1._zoneStates[new Address(100, 1)].PowerStatus);
            Assert.AreEqual(new Address(100, 3), zoneServer1._zoneStates[new Address(100, 1)].Source);
            Assert.AreEqual(1, zoneServer2._zoneStates.Count);  // 1 command has been sent
            Assert.AreEqual(true, zoneServer2._zoneStates[new Address(100, 1)].PowerStatus);
            Assert.AreEqual(new Address(100, 6), zoneServer2._zoneStates[new Address(100, 1)].Source);
            zoneServer1.ClearZoneStateList();
            zoneServer2.ClearZoneStateList();
        }

    
    }
}

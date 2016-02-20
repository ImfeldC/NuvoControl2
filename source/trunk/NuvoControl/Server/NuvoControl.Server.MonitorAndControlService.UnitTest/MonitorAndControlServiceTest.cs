using System.Threading;
using System.Collections.Generic;

using NuvoControl.Server.MonitorAndControlService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuvoControl.Server.ZoneServer;
using NuvoControl.Server.ProtocolDriver.Interface;
using NuvoControl.Common.Configuration;
using NuvoControl.Common;

namespace NuvoControl.Server.MonitorAndControlService.UnitTest
{
    
    
    /// <summary>
    /// Test class for MonitorAndControlServiceTest 
    /// 
    /// It is intended to contain all MonitorAndControlServiceTest Unit Tests
    /// </summary>
    [TestClass()]
    public class MonitorAndControlServiceTest: IMonitorAndControlNotification
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

/*
 * The SetZoneState method ha snot been implemented in the ProtocolDriverStub
 * 
        /// <summary>
        ///A test for SetZoneState
        /// </summary>
        [TestMethod()]
        public void SetZoneStateTest()
        {
            MonitorAndControlService target = CreateMonitorAndControlService();
            Address zoneId = new Address(1, 1);
            ZoneState stateCommand = new ZoneState();
            target.SetZoneState(zoneId, stateCommand);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }
*/

/*
        /// <summary>
        ///A test for StoreSubscribedZoneId
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.MonitorAndControlService.dll")]
        public void StoreSubscribedZoneIdTest()
        {
            MonitorAndControlService_Accessor target = new MonitorAndControlService_Accessor(); // TODO: Initialize to an appropriate value
            Address zoneId = null; // TODO: Initialize to an appropriate value
            target.StoreSubscribedZoneId(zoneId);
            target.
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        

        /// <summary>
        ///A test for RemoveSubscribedZoneId
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.MonitorAndControlService.dll")]
        public void RemoveSubscribedZoneIdTest()
        {
            MonitorAndControlService_Accessor target = new MonitorAndControlService_Accessor(); // TODO: Initialize to an appropriate value
            Address zoneId = null; // TODO: Initialize to an appropriate value
            target.RemoveSubscribedZoneId(zoneId);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for RemoveMonitorMultiple
        /// </summary>
        [TestMethod()]
        public void RemoveMonitorMultipleTest()
        {
            MonitorAndControlService target = new MonitorAndControlService(); // TODO: Initialize to an appropriate value
            Address[] zoneIds = null; // TODO: Initialize to an appropriate value
            target.RemoveMonitorMultiple(zoneIds);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for RemoveMonitor
        /// </summary>
        [TestMethod()]
        public void RemoveMonitorTest()
        {
            MonitorAndControlService target = new MonitorAndControlService(); // TODO: Initialize to an appropriate value
            Address zoneId = null; // TODO: Initialize to an appropriate value
            target.RemoveMonitor(zoneId);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for OnZoneNotification
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.MonitorAndControlService.dll")]
        public void OnZoneNotificationTest()
        {
            MonitorAndControlService_Accessor target = new MonitorAndControlService_Accessor(); // TODO: Initialize to an appropriate value
            object sender = null; // TODO: Initialize to an appropriate value
            ZoneStateEventArgs e = null; // TODO: Initialize to an appropriate value
            target.OnZoneNotification(sender, e);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for NotifySubscribers
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.MonitorAndControlService.dll")]
        public void NotifySubscribersTest()
        {
            MonitorAndControlService_Accessor target = new MonitorAndControlService_Accessor(); // TODO: Initialize to an appropriate value
            Address zoneId = null; // TODO: Initialize to an appropriate value
            ZoneState zoneState = null; // TODO: Initialize to an appropriate value
            target.NotifySubscribers(zoneId, zoneState);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for MonitorMultiple
        /// </summary>
        [TestMethod()]
        public void MonitorMultipleTest()
        {
            MonitorAndControlService target = new MonitorAndControlService(); // TODO: Initialize to an appropriate value
            Address[] zoneIds = null; // TODO: Initialize to an appropriate value
            target.MonitorMultiple(zoneIds);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Monitor
        /// </summary>
        [TestMethod()]
        public void MonitorTest()
        {
            MonitorAndControlService target = new MonitorAndControlService(); // TODO: Initialize to an appropriate value
            Address zoneId = null; // TODO: Initialize to an appropriate value
            target.Monitor(zoneId);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GetZoneState
        /// </summary>
        [TestMethod()]
        public void GetZoneStateTest()
        {
            MonitorAndControlService target = new MonitorAndControlService(); // TODO: Initialize to an appropriate value
            Address zoneId = null; // TODO: Initialize to an appropriate value
            ZoneState expected = null; // TODO: Initialize to an appropriate value
            ZoneState actual;
            actual = target.GetZoneState(zoneId);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Dispose
        /// </summary>
        [TestMethod()]
        public void DisposeTest()
        {
            MonitorAndControlService target = new MonitorAndControlService(); // TODO: Initialize to an appropriate value
            target.Dispose();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Disconnect
        /// </summary>
        [TestMethod()]
        public void DisconnectTest()
        {
            MonitorAndControlService target = new MonitorAndControlService(); // TODO: Initialize to an appropriate value
            target.Disconnect();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Connect
        /// </summary>
        [TestMethod()]
        public void ConnectTest()
        {
            MonitorAndControlService target = new MonitorAndControlService(); // TODO: Initialize to an appropriate value
            target.Connect();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Cleanup
        /// </summary>
        [TestMethod()]
        [DeploymentItem("NuvoControl.Server.MonitorAndControlService.dll")]
        public void CleanupTest()
        {
            MonitorAndControlService_Accessor target = new MonitorAndControlService_Accessor(); // TODO: Initialize to an appropriate value
            target.Cleanup();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for MonitorAndControlService Constructor
        /// </summary>
        [TestMethod()]
        public void MonitorAndControlServiceConstructorTest1()
        {
            MonitorAndControlService target = new MonitorAndControlService();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for MonitorAndControlService Constructor
        /// </summary>
        [TestMethod()]
        public void MonitorAndControlServiceConstructorTest()
        {
            IMonitorAndControlNofification callback = null; // TODO: Initialize to an appropriate value
            IZoneServer zoneServer = null; // TODO: Initialize to an appropriate value
            MonitorAndControlService target = new MonitorAndControlService(callback, zoneServer);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
         */


        internal MonitorAndControlService CreateMonitorAndControlService()
        {
            ProtocolDriverStub protocolDriver = new ProtocolDriverStub();
            List<IZoneController> zoneControllers = new List<IZoneController>();
            zoneControllers.Add(new ZoneController(new Address(1,1), protocolDriver));
            zoneControllers.Add(new ZoneController(new Address(1,2), protocolDriver));
            zoneControllers.Add(new ZoneController(new Address(2,1), protocolDriver));
            zoneControllers.Add(new ZoneController(new Address(2,2), protocolDriver));
            IZoneServer zoneServer = new NuvoControl.Server.ZoneServer.ZoneServer(zoneControllers);
            return new MonitorAndControlService(this, zoneServer);
        }

        #region IMonitorAndControlNofification Members

        public void OnZoneStateChanged(Address zoneId, ZoneState zoneState)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }

    internal class ProtocolDriverStub : IProtocol
    {
        #region IProtocol Members

        /// <summary>
        /// Public event in case a (single) command is received from the device (zone).
        /// </summary>
        public event ProtocolCommandReceivedEventHandler onCommandReceived;

        /// <summary>
        /// Public event in case a full zone state is received from the device (zone).
        /// </summary>
        public event ProtocolZoneUpdatedEventHandler onZoneStatusUpdate;

        /// <summary>
        /// Public event in case the state of a device has changed.
        /// </summary>
        public event ProtocolDeviceUpdatedEventHandler onDeviceStatusUpdate;

        public void Open(ENuvoSystem system, int deviceId, Communication communicationConfiguration)
        {
            throw new System.NotImplementedException();
        }

        public void Open(ENuvoSystem system, int deviceId, Communication communicationConfiguration, IConcreteProtocol essentiaProtocol)
        {
            throw new System.NotImplementedException();
        }

        public void Close(int deviceId)
        {
            throw new System.NotImplementedException();
        }

        public void ReadZoneState(Address zoneAddress)
        {
            throw new System.NotImplementedException();
        }

        public void SetZoneState(Address zoneAddress, ZoneState zoneState)
        {
            throw new System.NotImplementedException();
        }

        public void CommandSwitchZoneON(Address zoneAddress)
        {
            ThreadPool.QueueUserWorkItem(delegate(object obj)
            {
                Thread.Sleep(2000);
                if (onZoneStatusUpdate != null)
                {
                    onZoneStatusUpdate(this, new ProtocolZoneUpdatedEventArgs(zoneAddress, new ZoneState(), null));
                }
            }, null);
        }

        public void CommandSwitchZoneOFF(Address zoneAddress)
        {
            throw new System.NotImplementedException();
        }

        public void CommandSetSource(Address zoneAddress, Address sourceAddress)
        {
            throw new System.NotImplementedException();
        }

        public void CommandSetVolume(Address zoneAddress, int volumeLevel)
        {
            throw new System.NotImplementedException();
        }

        public void CommandPlaySound(Address sourceAddress, string URL)
        {
            throw new System.NotImplementedException();
        }

        public void SendCommand(Address zoneAddress, INuvoEssentiaSingleCommand command)
        {
            throw new System.NotImplementedException();
        }

        public void SendCommand(Address zoneAddress, INuvoEssentiaCommand command)
        {
            throw new System.NotImplementedException();
        }

        #endregion

    }
}

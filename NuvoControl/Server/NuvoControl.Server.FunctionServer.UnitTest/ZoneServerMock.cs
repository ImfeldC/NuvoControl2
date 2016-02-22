using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Server.ZoneServer;
using NuvoControl.Common.Configuration;
using NuvoControl.Common;
using NuvoControl.Server.ProtocolDriver.Interface;

namespace NuvoControl.Server.FunctionServer.UnitTest
{
    class ZoneServerMock : IZoneServer
    {
        public bool _started = false;
        private Dictionary<Address, ZoneState> _zoneStates = new Dictionary<Address, ZoneState>();
        private List<ZoneState> _zoneStateList = new List<ZoneState>();
        public Dictionary<Address, ZoneNotification> _monitoredZones = new Dictionary<Address, ZoneNotification>();


        public void distributeZoneState( ZoneState zoneState )
        {
            foreach (ZoneNotification zoneNotification in _monitoredZones.Values )
            {
                zoneNotification(this, new ZoneStateEventArgs(zoneState));
            }
        }

        public Dictionary<Address, ZoneState> ZoneStates
        {
            get { return _zoneStates; }
        }

        public List<ZoneState> ZoneStateList
        {
            get { return _zoneStateList; }
        }

        /*
        public void RemoveFromZoneStateList(Address zoneId)
        {
            _zoneStates.Remove(zoneId);
        }
        */

        public void ClearZoneStateList()
        {
            _zoneStates.Clear();
        }

        #region IZoneServer Members

        public void StartUp()
        {
            _started = true;
        }

        public void ShutDown()
        {
            _started = false;
        }

        public ZoneState GetZoneState(Address zoneId)
        {
            return _zoneStates[zoneId];
        }

        public void SetZoneState(Address zoneId, ZoneState zoneState)
        {
            _zoneStates.Remove(zoneId);
            _zoneStates.Add(zoneId, zoneState);
            _zoneStateList.Add(zoneState);
        }

        public void Monitor(Address zoneId, ZoneNotification subscriber)
        {
            _monitoredZones.Add(zoneId,subscriber);
        }

        public void RemoveMonitor(Address zoneId, ZoneNotification subscriber)
        {
            _monitoredZones.Remove(zoneId);
        }

        public IProtocol GetProtocolDriver(Address zoneId)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}

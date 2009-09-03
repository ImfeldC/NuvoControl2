using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Server.ZoneServer;
using NuvoControl.Common.Configuration;
using NuvoControl.Common;

namespace NuvoControl.Server.FunctionServer.UnitTest
{
    class ZoneServerMock : IZoneServer
    {
        public bool _started = false;
        public Dictionary<Address, ZoneState> _zoneStates = new Dictionary<Address, ZoneState>();
        public List<Address> _monitoredZones = new List<Address>();

        #region IZoneServer Members

        public void StartUp()
        {
            _started = true;
        }

        public ZoneState GetZoneState(Address zoneId)
        {
            return _zoneStates[zoneId];
        }

        public void SetZoneState(Address zoneId, ZoneState zoneState)
        {
            _zoneStates.Add(zoneId, zoneState);
        }

        public void Monitor(Address zoneId, ZoneNotification subscriber)
        {
            _monitoredZones.Add(zoneId);
        }

        public void RemoveMonitor(Address zoneId, ZoneNotification subscriber)
        {
            _monitoredZones.Remove(zoneId);
        }

        #endregion
    }
}

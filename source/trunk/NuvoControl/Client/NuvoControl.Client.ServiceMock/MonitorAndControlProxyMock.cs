using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using NuvoControl.Common.Configuration;
using NuvoControl.Common;
using NuvoControl.Client.ServiceAccess.MonitorAndControlService;

namespace NuvoControl.Client.ServiceMock
{
    public class MonitorAndControlProxyMock: IMonitorAndControl
    {
        private Dictionary<Address, Address> _zonesSubscribed = new Dictionary<Address, Address>();
        private Timer _timer;
        private IMonitorAndControlCallback _callback;
        private int _volume = 0;
        private bool _power = false;
        private bool _acknowledged = false;
        private ZoneQuality _quality = ZoneQuality.Offline;
        private int _sourceCounter = 0;
        private Address _source = new Address(100, 0);



        public MonitorAndControlProxyMock()
        {
            _timer = new Timer(OnTimerCallback);
            _timer.Change(5000, Timeout.Infinite);
        }

        public void SetCallback(IMonitorAndControlCallback callback)
        {
            _callback = callback;
        }

        public void OnTimerCallback(object obj)
        {
            lock (_zonesSubscribed)
            {
                foreach (Address zone in _zonesSubscribed.Keys)
                {
                    _callback.OnZoneStateChanged(zone, GetSimulatedZoneState());
                }
            }
            _timer.Change(5000, Timeout.Infinite);
        }

        #region IMonitorAndControl Members

        public void Connect()
        {
            return;
        }

        public void Disconnect()
        {
           return;
        }

        public void SetZoneState(Address zoneId, ZoneState stateCommand)
        {
            lock (_zonesSubscribed)
            {
                if (_zonesSubscribed.ContainsKey(zoneId))
                {
                    _volume = stateCommand.Volume;
                    _power = stateCommand.PowerStatus;
                    _quality = stateCommand.ZoneQuality;
                    _source = new Address(stateCommand.Source);
                    _timer.Change(0, Timeout.Infinite);
                }
            }
        }

        public ZoneState GetZoneState(Address zoneId)
        {
            throw new NotImplementedException();
        }

        public void Monitor(Address zoneId)
        {
            lock (_zonesSubscribed)
            {
                _zonesSubscribed[zoneId] = zoneId;
            }
        }

        public void MonitorMultiple(Address[] zoneIds)
        {
            throw new NotImplementedException();
        }

        public void RemoveMonitor(Address zoneId)
        {
            lock (_zonesSubscribed)
            {

                if (_zonesSubscribed.ContainsKey(zoneId))
                    _zonesSubscribed.Remove(zoneId);
            }
        }

        public void RemoveMonitorMultiple(Address[] zoneIds)
        {
            throw new NotImplementedException();
        }

        private ZoneState GetSimulatedZoneState()
        {
            _sourceCounter++;
            _volume++;
            _power = !_power;
            _acknowledged = !_acknowledged;
            _quality = (_quality == ZoneQuality.Offline) ? ZoneQuality.Online : ZoneQuality.Offline;

            ZoneState zoneState = new ZoneState(new Address(_source.DeviceId, (_sourceCounter % 6) + 1), _power, _volume % 100, ZoneQuality.Online);
            zoneState.ZoneQuality = _quality;
            zoneState.CommandUnacknowledged = _acknowledged;
            return zoneState;
        }

        #endregion
    }
}

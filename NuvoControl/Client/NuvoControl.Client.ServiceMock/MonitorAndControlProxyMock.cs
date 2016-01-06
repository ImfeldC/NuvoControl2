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
        private Dictionary<Address, ZoneState> _zonesStates = new Dictionary<Address, ZoneState>();
        private Timer _timer;
        private IMonitorAndControlCallback _callback;
        private int _sourceCounter = 0;
        private bool _continuousSimulation = false;

        public MonitorAndControlProxyMock()
        {
            _timer = new Timer(OnTimerCallback);

            if (_continuousSimulation)
                _timer.Change(10000, Timeout.Infinite);
        }

        public void SetCallback(IMonitorAndControlCallback callback)
        {
            _callback = callback;
        }

        public bool ContinuousSimulation
        {
            get { return _continuousSimulation; }
            set
            {
                _continuousSimulation = value;
                if (_continuousSimulation)
                    _timer.Change(10000, Timeout.Infinite);
            }
        }

        public void OnTimerCallback(object obj)
        {
            lock (_zonesSubscribed)
            {
                foreach (Address zone in _zonesSubscribed.Keys)
                {
                    _callback.OnZoneStateChanged(zone, GetSimulatedZoneState(zone));
                }
            }
            if (_continuousSimulation)
                _timer.Change(10000, Timeout.Infinite);
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


        public void RenewLease()
        {
            return;
        }


        public void SetZoneState(Address zoneId, ZoneState stateCommand)
        {
            lock (_zonesStates)
            {
                if (_zonesStates.ContainsKey(zoneId) == false)
                    _zonesStates.Add(zoneId, stateCommand);
                    
                _zonesStates[zoneId] = stateCommand;
                _zonesStates[zoneId].CommandUnacknowledged = false;
                ZoneState stateCommandRet = _zonesStates[zoneId];
                ThreadPool.QueueUserWorkItem(delegate(object obj)
                {
                    Thread.Sleep(500);
                    if (_callback != null)
                        _callback.OnZoneStateChanged(zoneId, stateCommandRet);
                }, null);
            }
        }


        public ZoneState GetZoneState(Address zoneId)
        {
            lock (_zonesStates)
            {
                if (_zonesStates.ContainsKey(zoneId) == false)
                    _zonesStates.Add(zoneId, new ZoneState());

                return new ZoneState(_zonesStates[zoneId]);
            }
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


        private ZoneState GetSimulatedZoneState(Address zoneId)
        {
            lock (_zonesStates)
            {
                if (_zonesStates.ContainsKey(zoneId) == false)
                    return null;

                _zonesStates[zoneId].Volume++;
                _zonesStates[zoneId].Volume = _zonesStates[zoneId].Volume % 100;
                _zonesStates[zoneId].PowerStatus = !_zonesStates[zoneId].PowerStatus;
                _zonesStates[zoneId].CommandUnacknowledged = !_zonesStates[zoneId].CommandUnacknowledged;
                _zonesStates[zoneId].ZoneQuality = (_zonesStates[zoneId].ZoneQuality == ZoneQuality.Offline) ? ZoneQuality.Online : ZoneQuality.Offline;
                _zonesStates[zoneId].Source = new Address(100, ((_sourceCounter++) % 6) + 1);
                return new ZoneState(_zonesStates[zoneId]);
            }
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/
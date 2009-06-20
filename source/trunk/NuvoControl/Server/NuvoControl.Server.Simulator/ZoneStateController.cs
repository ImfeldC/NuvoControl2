using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Common;
using NuvoControl.Server.ProtocolDriver.Interface;
using NuvoControl.Common.Configuration;
using Common.Logging;

namespace NuvoControl.Server.Simulator
{
    /// <summary>
    /// Public delegate for the zone state controller event handler.
    /// See <see cref="ZoneStateEventArgs"/> for more information.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ZoneStateUpdated(
          object sender, ZoneStateEventArgs e);

    /// <summary>
    /// Event Handler class for the zone state controller.
    /// Contains the zone id where this event belongs to. In addition it holds
    /// the previous and new zone state.
    /// </summary>
    public class ZoneStateEventArgs : EventArgs
    {
        int _zoneIndex;
        ENuvoEssentiaZones _zoneId;
        ZoneState _prevZoneState;
        ZoneState _newZoneState;

        /// <summary>
        /// Gets the Zone Index. Zero-Based!
        /// </summary>
        public int ZoneIndex
        {
            get { return _zoneIndex; }
        }

        /// <summary>
        /// Gets the zone id (as enumeration, <see cref="ENuvoEssentiaZones"/>). One-Based!
        /// </summary>
        public ENuvoEssentiaZones ZoneId
        {
            get { return _zoneId; }
        }

        /// <summary>
        /// Resturns the previous zone state
        /// </summary>
        public ZoneState PrevZoneState
        {
            get { return _prevZoneState; }
        }

        /// <summary>
        /// Retruns the new zone state
        /// </summary>
        public ZoneState NewZoneState
        {
            get { return _newZoneState; }
        }
        
        /// <summary>
        /// Constructor to create a zone event argument. 
        /// </summary>
        /// <param name="ZoneIndex">Index of the zone. Zero-based!</param>
        /// <param name="prevZoneState">Previous Zone State</param>
        /// <param name="newZoneState">New Zone State</param>
        public ZoneStateEventArgs(int ZoneIndex, ZoneState prevZoneState, ZoneState newZoneState )
        {
            _zoneIndex = ZoneIndex;
            _prevZoneState = prevZoneState;
            _newZoneState = newZoneState;
            _zoneId = (ENuvoEssentiaZones)ZoneIndex+1;  // calculate from index
        }

        /// <summary>
        /// Constructor to create a zone event argument.
        /// </summary>
        /// <param name="ZoneId">Zone id of the zone. Enumeration!</param>
        /// <param name="prevZoneState">Previous Zone State</param>
        /// <param name="newZoneState">New Zone State</param>
        public ZoneStateEventArgs(ENuvoEssentiaZones ZoneId, ZoneState prevZoneState, ZoneState newZoneState )
        {
            _zoneIndex = (int)ZoneId-1;     // calculate from enumeration
            _prevZoneState = prevZoneState;
            _newZoneState = newZoneState;
            _zoneId = ZoneId;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    class ZoneStateController
    {
        #region Common Logger
        /// <summary>
        /// Common logger object.
        /// </summary>
        private ILog _log = LogManager.GetCurrentClassLogger();
        #endregion

        private int _numOfZones;
        private int _deviceId;
        private ZoneState[] _zoneState;

        /// <summary>
        /// Public event notifies the subscriber about changes in the zones.
        /// <remarks>
        /// The subscriber will be notified about each zone state. It is its own
        /// responsibility to check which zone has changed!
        /// </remarks>
        /// </summary>
        public event ZoneStateUpdated onZoneUpdated;

        /// <summary>
        /// Public constructor for the zone state controller.
        /// Creates <c>numOfZones</c> instances of <c>ZoneState</c> objects.
        /// </summary>
        /// <param name="numOfZones">Number of zones.</param>
        /// <param name="deviceId">Device Id, where this zone states belong to.</param>
        public ZoneStateController(int numOfZones, int deviceId)
        {
            _log.Debug(m => m("Create zone state controller, for {0} zones and the device {1}.", numOfZones, deviceId));
            _numOfZones = numOfZones;
            _deviceId = deviceId;
            initZoneState();
        }

        /// <summary>
        /// Sets the Zone State for the corresponding index. One-Based, using the
        /// enumeration <see cref="ENuvoEssentiaZones"/>.!
        /// </summary>
        /// <param name="zoneId">Zone Id. One-Based!</param>
        /// <param name="newZoneState">New zone state.</param>
        public void setZoneState(ENuvoEssentiaZones zoneId, ZoneState newZoneState)
        {
            if (ENuvoEssentiaZones.NoZone != zoneId)
            {
                ZoneState oldState = new ZoneState(_zoneState[(int)zoneId - 1]);
                _zoneState[(int)zoneId - 1] = newZoneState;
                notifyZoneStateSubscribers((int)zoneId - 1, oldState, newZoneState);
            }
        }

        /// <summary>
        /// Gets the Zone State for the corresponding index. Zero-Based!
        /// NOTE: Do not change the object received by this method, use the method
        /// <see cref="setZoneState"/> instead.
        /// Otherwise you won't get update notifications.
        /// </summary>
        /// <param name="index">Index of Zone. Zero-Based!</param>
        /// <returns>Zone State.</returns>
        public ZoneState this[ int index ]
        {
            get { return _zoneState[index]; }
        }

        /// <summary>
        /// Gets the Zone State for the corresponding index. One-Based, using the
        /// enumeration <see cref="ENuvoEssentiaZones"/>.!
        /// NOTE: Do not change the object received by this method, use the method
        /// <see cref="setZoneState"/> instead.
        /// </summary>
        /// <param name="zoneId">Zone Id. One-Based!</param>
        /// <returns>Zone State.</returns>
        public ZoneState this[ENuvoEssentiaZones zoneId]
        {
            get { return _zoneState[(int)zoneId - 1]; }
        }

        /// <summary>
        /// Private method to notify the subscribers about changes in the zone state.
        /// </summary>
        /// <param name="zoneIndex">Index of the changed zone. Zero-Based!</param>
        /// <param name="oldZoneState">Old Zone State.</param>
        /// <param name="newZoneState">New Zone State.</param>
        private void notifyZoneStateSubscribers(int zoneIndex, ZoneState oldZoneState, ZoneState newZoneState)
        {
            _log.Trace(m=>m("Zone State with zoneIndex='{0}' has changed, notify the subscribers. oldState='{1}' newState='{2}'.",zoneIndex, oldZoneState.ToString(), newZoneState.ToString() ));
            if (onZoneUpdated != null)
            {
                if (oldZoneState != newZoneState)
                {
                    onZoneUpdated(this, new ZoneStateEventArgs(zoneIndex, oldZoneState, newZoneState));
                }
            }
        }

        /// <summary>
        /// Initialize the simulated zones with an initial default value,
        /// which is Device id '1' and Source '1', zone power status is ON
        /// and volume is 30dB
        /// </summary>
        private void initZoneState()
        {
            _zoneState = new ZoneState[_numOfZones];
            for (int i = 0; i < _numOfZones; i++)
            {
                _zoneState[i] = new ZoneState(new Address(_deviceId, 1), true, -30);
            }
        }


        /// <summary>
        /// Public override for the <c>ToString</c> method.
        /// </summary>
        /// <returns>String representing the content of this object.</returns>
        public override string ToString()
        {
            int i = 0;
            string strMessage = "";
            foreach (ZoneState zoneState in _zoneState)
            {
                strMessage += string.Format("Zone[{0}]={1} ", i++, zoneState.ToString());
            }
            return strMessage;
        }
    
    }
}

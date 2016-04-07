using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using NuvoControl.Common.Configuration;



namespace NuvoControl.Server.ProtocolDriver.Interface
{
    public enum eOscEvent
    {
        Ping = 0,
        TabChange = 1,
        SwitchOn = 2,
        SwitchOff = 3,
        ValueUp = 4,
        ValueDown = 5,
        SetValue = 6,
        SetValues = 7,
        NuvoControl = 8
    }

    public class OscEvent
    {
        private eOscEvent _oscEvent;
        private string _oscLabel;
        private int _zoneId;
        private int _sourceId;
        /// <summary>
        /// The contents of the packet.
        /// </summary>
        protected List<object> mData;


        private void initMembers()
        {
            _zoneId = -1;
            _sourceId = -1;
            mData = new List<object>();
            mData.Clear();
        }

        public OscEvent(eOscEvent oscEvent, string oscLabel)
        {
            initMembers();
            _oscEvent = oscEvent;
            _oscLabel = oscLabel;
            _zoneId = parseZoneId(_oscLabel);
            _sourceId = parseSourceId(_oscLabel);
        }

        public OscEvent(eOscEvent oscEvent, string oscLabel, int value)
        {
            initMembers();
            _oscEvent = oscEvent;
            _oscLabel = oscLabel;
            mData.Add(value);
            _zoneId = parseZoneId(_oscLabel);
            _sourceId = parseSourceId(_oscLabel);
        }

        public OscEvent(eOscEvent oscEvent, string oscLabel, double value)
        {
            initMembers();
            _oscEvent = oscEvent;
            _oscLabel = oscLabel;
            mData.Add(value);
            _zoneId = parseZoneId(_oscLabel);
            _sourceId = parseSourceId(_oscLabel);
        }

        public OscEvent(eOscEvent oscEvent, string oscLabel, double value1, double value2)
        {
            initMembers();
            _oscEvent = oscEvent;
            _oscLabel = oscLabel;
            mData.Add(value1);
            mData.Add(value2);
            _zoneId = parseZoneId(_oscLabel);
            _sourceId = parseSourceId(_oscLabel);
        }

        public eOscEvent OscCommand
        {
            get { return _oscEvent; }
        }

        public string OscLabel
        {
            get { return _oscLabel; }
        }

        public IList<object> OscData
        {
            get { return mData.AsReadOnly(); }
        }

        public int getOscData
        {
            get { return Convert.ToInt32(mData[0]); }
        }

        public int getZoneId
        {
            get { return _zoneId; }
        }

        public int getSourceId
        {
            get { return _sourceId; }
        }

        /// <summary>
        /// Returns string representative for the osc event.
        /// </summary>
        /// <returns>String of the osc event.</returns>
        public override string ToString()
        {
            return String.Format("[{0}-{1}]", _oscEvent.ToString(), _oscLabel);
        }

        /// <summary>
        /// Private method, which retrievs the zone id from the osc message string
        /// </summary>
        /// <param name="oscLabel">Osc message string.</param>
        /// <returns>Zone id found in the message, otherwise -1 is returned.</returns>
        private int parseZoneId( string oscLabel )
        {
            int len = 4;    // "Zone" = 4 characters
            string[] parts = oscLabel.Split('/');
            foreach (string part in parts)
            {
                if (part.IndexOf("ZoneSelection") == 0)
                {
                    return int.Parse(parts[4]);
                }
                if ((part.IndexOf("Zone") == 0) && (part.Length == len + 1 || part.Length == len + 2))
                {
                    return int.Parse(part.Substring(len));
                }
            }
            return -1;
        }

        /// <summary>
        /// Private method, which retrievs the source id from the osc message string
        /// </summary>
        /// <param name="oscLabel">Osc message string.</param>
        /// <returns>Source id found in the message, otherwise -1 is returned.</returns>
        private int parseSourceId(string oscLabel)
        {
            int len = 6;    // "Source" = 6 characters
            string[] parts = oscLabel.Split('/');
            foreach (string part in parts)
            {
                if (part.IndexOf("SourceSelection") == 0)
                {
                    return int.Parse(parts[5]);
                }
                if ((part.IndexOf("Source") == 0) && (part.Length == len + 1 || part.Length == len + 2))
                {
                    return int.Parse(part.Substring(len));
                }
            }
            return -1;
        }

    }

    #region OscEventReceived

    /// <summary>
    /// Public delegate used in case a osc event is received.
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Additional information passed by the Sender</param>
    public delegate void OscEventReceivedEventHandler(
              object sender, OscEventReceivedEventArgs e);

    /// <summary>
    /// Argument class, used in the delegate <c>OscEventReceivedEventHandler</c>.
    /// Inherits from base class <c>EventArgs</c>.
    /// </summary>
    public class OscEventReceivedEventArgs : EventArgs
    {
        private Address _oscDevice;
        private OscEvent _oscEvent;
        private IPEndPoint _SourceEndPoint;

        /// <summary>
        /// Constructor for the argument class.
        /// </summary>
        public OscEventReceivedEventArgs(Address oscDevice, OscEvent oscEvent, IPEndPoint SourceEndPoint)
        {
            _oscDevice = oscDevice;
            _oscEvent = oscEvent;
            _SourceEndPoint = SourceEndPoint;
        }

        /// <summary>
        /// Returns the Device Id, as part of the Osc device Address.
        /// </summary>
        public int OscDeviceId
        {
            get { return _oscDevice.DeviceId; }
        }

        /// <summary>
        /// Returns the osc device address.
        /// </summary>
        public Address OscDevice
        {
            get { return _oscDevice; }
        }

        public OscEvent OscEvent
        {
            get { return _oscEvent; }
        }

        public IPEndPoint SourceEndPoint
        {
            get { return _SourceEndPoint; }
        }

    }

    #endregion

    public interface IOscDriver
    {
        /// Server (receiver) methods

        /// <summary>
        /// This event is raised in case a osc event has been recieved from the underlying device.
        /// </summary>
        event OscEventReceivedEventHandler onOscEventReceived;

        /// <summary>
        /// This event is raised in case a osc event (from Nuvo layout) has been recieved from the underlying device.
        /// </summary>
        event OscEventReceivedEventHandler onOscNuvoEventReceived;


        /// <summary>
        /// Register an Osc method.
        /// </summary>
        /// <param name="oscEvent">The Osc event to register.</param>
        void RegisterMethod(OscEvent oscEvent);

		/// <summary>
		/// Unregister an Osc method.
		/// </summary>
        /// <param name="oscEvent">The Osc event to unregister.</param>
        void UnRegisterMethod(OscEvent oscEvent);

		/// <summary>
		/// Unregister all Osc events.
		/// </summary>
		void ClearMethods();


        /// <summary>
        /// Start listening for incoming Osc packets.
        /// </summary>
        /// <remarks>This is a non-blocking (asynchronous) call.</remarks>
        void Start();

        /// <summary>
        /// Stop listening for Osc packets.
        /// </summary>
        void Stop();


        /// Client (sender) methods

        /// <summary>
        /// Send an osc message to the client.
        /// </summary>
        /// <param name="address">Address to send the message.</param>
        /// <param name="value">Message or value to send.</param>
        void SendMessage(string address, object value);

        /// <summary>
        /// Send an osc message to the client.
        /// </summary>
        /// <param name="oscEvent">Osc event to send (incl. message and values).</param>
        void SendMessage(OscEvent oscEvent);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Common.Configuration;


namespace NuvoControl.Server.ProtocolDriver.Interface
{

    public class OscEvent
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
            SetValues = 7
        }

        private eOscEvent _oscEvent;
        private string _oscLabel;
        private int _intValue;
        private double _doubleValue;
        private double[] _doubleValues = new double[2];

        public OscEvent(eOscEvent oscEvent, string oscLabel)
        {
            _oscEvent = oscEvent;
            _oscLabel = oscLabel;
            _intValue = -1;
            _doubleValue = -1.0;
            _doubleValues[0] = -1.0;
            _doubleValues[1] = -1.0;
        }

        public OscEvent(eOscEvent oscEvent, string oscLabel, int value)
        {
            _oscEvent = oscEvent;
            _oscLabel = oscLabel;
            _intValue = value;
            _doubleValue = -1.0;
            _doubleValues[0] = -1.0;
            _doubleValues[1] = -1.0;
        }

        public OscEvent(eOscEvent oscEvent, string oscLabel, double value)
        {
            _oscEvent = oscEvent;
            _oscLabel = oscLabel;
            _intValue = -1;
            _doubleValue = value;
            _doubleValues[0] = -1.0;
            _doubleValues[1] = -1.0;
        }

        public OscEvent(eOscEvent oscEvent, string oscLabel, double value1, double value2)
        {
            _oscEvent = oscEvent;
            _oscLabel = oscLabel;
            _intValue = -1;
            _doubleValue = -1.0;
            _doubleValues[0] = value1;
            _doubleValues[1] = value2;
        }

        public eOscEvent OscCommand
        {
            get { return _oscEvent; }
        }

        public string OscLabel
        {
            get { return _oscLabel; }
        }

        /// <summary>
        /// Returns string representative for the osc event.
        /// </summary>
        /// <returns>String of the osc event.</returns>
        public override string ToString()
        {
            return String.Format("[{0}-{1}-{2}]", _oscEvent.ToString(), _oscLabel, (_intValue>-1?_intValue.ToString():(_doubleValue>-1.0?_doubleValue.ToString():(_doubleValues[0]>-1.0?String.Format("{0}+{1}",_doubleValues[0],_doubleValues[1]):"na"))));
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
        private int _oscValue = -1;

        /// <summary>
        /// Constructor for the argument class.
        /// </summary>
        public OscEventReceivedEventArgs(Address oscDevice, OscEvent oscEvent)
        {
            _oscDevice = oscDevice;
            _oscEvent = oscEvent;
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

        public int OscValue
        {
            get { return _oscValue; }
        }

    }
    #endregion

    public interface IOscDriver
    {
        /// <summary>
        /// This event is raised in case a osc event has been recieved from the underlying device.
        /// </summary>
        event OscEventReceivedEventHandler onOscEventReceived;


        void RegisterMethod( OscEvent oscEvent );

        void Start();

        void Stop();


    }
}

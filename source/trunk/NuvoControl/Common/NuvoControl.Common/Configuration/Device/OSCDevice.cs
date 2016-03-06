using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace NuvoControl.Common.Configuration
{

    public enum eOSCDeviceType
    {
        OSCServer = 0,
        OSCClient = 1
    }

    
    /// <summary>
    /// This is a system configuration class. 
    /// 
    /// It is a data structurer.
    /// It defines a OSC device.
    /// </summary>
    [DataContract]
    public class OSCDevice
    {

        #region Private Members

        /// <summary>
        /// The address of the osc device.
        /// </summary>
        [DataMember]
        private Address _id = new Address(SystemConfiguration.ID_UNDEFINED, SystemConfiguration.ID_UNDEFINED);

        private eOSCDeviceType _deviceType = eOSCDeviceType.OSCServer;

        private Address _deviceId = new Address();

        private string _name = "";

        private IPAddress _ipAddress = null;

        private int _port = 0;

        #endregion


        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public OSCDevice()
        {
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        public OSCDevice(Address id, eOSCDeviceType deviceType, Address deviceId, string name, IPAddress ipAddress, int port)
        {
            _id = id;
            _deviceType = deviceType;
            _deviceId = deviceId;
            _name = name;
            _ipAddress = ipAddress;
            _port = port;
        }

        #endregion


        #region Public Interface

        /// <summary>
        /// The address of the osc device.
        /// </summary>
        public Address Id
        {
            get { return _id; }
        }

        public eOSCDeviceType DeviceType
        {
            get { return _deviceType; }
        }

        public Address DeviceId
        {
            get { return _deviceId; }
        }

        public string Name
        {
            get { return _name; }
        }

        public IPAddress IpAddress
        {
            get { return _ipAddress; }
        }

        public int Port
        {
            get { return _port; }
        }

        #endregion


        /// <summary>
        /// Returns string representative of this class.
        /// </summary>
        /// <returns>String representative of this class.</returns>
        public override string ToString()
        {
            return String.Format("Name={0}, Id={1}, Type={2}, IPAddress={3}, Port={4}", _name, _deviceId.ToString(), _deviceType.ToString(), _ipAddress.ToString(), _port);
        }

    }
}

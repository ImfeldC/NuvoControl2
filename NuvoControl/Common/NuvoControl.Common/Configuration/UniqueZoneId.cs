using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Common.Configuration
{
    [Serializable]
    public class UniqueZoneId
    {
        #region Private Members

        private int _zoneId = SystemConfiguration.ID_UNDEFINED;
        private int _deviceId = SystemConfiguration.ID_UNDEFINED;

        #endregion

        #region Constructors

        public UniqueZoneId(int deviceId, int zoneId)
        {
            this._deviceId = deviceId;
            this._zoneId = zoneId;
        }

        #endregion

        #region Public Interface

        public int ZoneId
        {
            get { return _zoneId; }
            set { _zoneId = value; }
        }

        public int DeviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; }
        }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            UniqueZoneId id = obj as UniqueZoneId;
            if ((object)id == null)
                return false;

            return (_deviceId == id._deviceId) && (_zoneId == id._zoneId);
        }


        public bool Equals(UniqueZoneId id)
        {
            if ((object)id == null)
                return false;

            return (_deviceId == id._deviceId) && (_zoneId == id._zoneId);
        }


        public override int GetHashCode()
        {
            return _deviceId ^ _zoneId;
        }


        #endregion
    }
}

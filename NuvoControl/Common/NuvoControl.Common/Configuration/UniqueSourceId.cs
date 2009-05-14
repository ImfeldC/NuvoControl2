using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Common.Configuration
{
    [Serializable]
    public class UniqueSourceId
    {
        #region Private Members

        private int _sourceId = -1;
        private int _deviceId = -1;

        #endregion

        #region Constructors

        public UniqueSourceId(int sourceId, int deviceId)
        {
            this._sourceId = sourceId;
            this._deviceId = deviceId;
        }

        #endregion

        #region Public Interface

        public int SourceId
        {
            get { return _sourceId; }
            set { _sourceId = value; }
        }

        public int DeviceId
        {
            get { return _deviceId; }
            set { _deviceId = value; }
        }

        #endregion
    }
}

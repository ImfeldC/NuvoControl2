/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.05.2009
 *   File Name:      Address.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 12.05.2009, Bernhard Limacher: First implementation.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Common.Configuration
{
    [Serializable]
    public class Address
    {
        #region Private Members

        private int _objectId = SystemConfiguration.ID_UNDEFINED;
        private int _deviceId = SystemConfiguration.ID_UNDEFINED;

        #endregion

        #region Constructors

        public Address(int deviceId, int objectId)
        {
            this._deviceId = deviceId;
            this._objectId = objectId;
        }

        #endregion

        #region Public Interface

        public int ObjectId
        {
            get { return _objectId; }
        }

        public int DeviceId
        {
            get { return _deviceId; }
        }


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Address id = obj as Address;
            if ((object)id == null)
                return false;

            return (_deviceId == id._deviceId) && (_objectId == id._objectId);
        }


        public bool Equals(Address id)
        {
            if ((object)id == null)
                return false;

            return (_deviceId == id._deviceId) && (_objectId == id._objectId);
        }


        public override int GetHashCode()
        {
            return _deviceId ^ _objectId;
        }

        public override string ToString()
        {
            return String.Format("{0}.{1}", _deviceId, _objectId);
        }


        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
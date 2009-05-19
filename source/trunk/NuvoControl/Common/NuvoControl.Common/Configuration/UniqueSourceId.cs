/**************************************************************************************************
 * 
 *   Copyright (C) Siemens AG 2006 All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.05.2009
 *   File Name:      UniqueSourceId.cs
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
    public class UniqueSourceId
    {
        #region Private Members

        private int _sourceId = SystemConfiguration.ID_UNDEFINED;
        private int _deviceId = SystemConfiguration.ID_UNDEFINED;

        #endregion

        #region Constructors

        public UniqueSourceId(int deviceId, int sourceId)
        {
            this._deviceId = deviceId;
            this._sourceId = sourceId;
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


        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            UniqueSourceId id = obj as UniqueSourceId;
            if ((object)id == null)
                return false;

            return (_deviceId == id._deviceId) && (_sourceId == id._sourceId);
        }


        public bool Equals(UniqueSourceId id)
        {
            if ((object)id == null)
                return false;

            return (_deviceId == id._deviceId) && (_sourceId == id._sourceId);
        }


        public override int GetHashCode()
        {
            return _deviceId ^ _sourceId;
        }


        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/

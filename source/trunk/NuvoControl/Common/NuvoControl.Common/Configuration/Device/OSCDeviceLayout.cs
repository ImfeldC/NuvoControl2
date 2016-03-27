/**************************************************************************************************
 * 
 *   Copyright (C) 2016 by Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Common.Configuration
{

    public enum eOSCDeviceLayoutType
    {
        Undefined = 0,
        Zone = 1,
        Source = 2
    }

    public enum eOSCDeviceLayoutAddress
    {
        ZoneName = 0,
        ZoneStatus = 1,
        ZoneSourceName = 2,
        ZoneSourceSelection = 3,
        ZoneVolume = 4,
        ZoneBass = 5,
        ZoneTreble = 6,
        ZoneKeyPadLock = 7,

        SourceName = 8
    }

    public class OSCDeviceLayout
    {

        private eOSCDeviceLayoutType _oscDeviceLayoutType = eOSCDeviceLayoutType.Undefined;
        private int _id = -1;

        private Dictionary<eOSCDeviceLayoutAddress,string>  _oscDeviceLayout = new Dictionary<eOSCDeviceLayoutAddress,string>();


        public OSCDeviceLayout(eOSCDeviceLayoutType oscDeviceLayoutType, int id)
        {
            _oscDeviceLayoutType = oscDeviceLayoutType;
            _id = id;
        }

        public void AddLayoutEntry(eOSCDeviceLayoutAddress addressIdentifier, string address)
        {
            _oscDeviceLayout.Add(addressIdentifier, address);
        }
    }
}

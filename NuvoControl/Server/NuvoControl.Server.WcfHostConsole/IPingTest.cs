/**************************************************************************************************
 * 
 *   Copyright (C) C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.WcfHostConsole
 *   Author:         Christian Imfeld
 *   Creation Date:  09.03.2015
 *   File Name:      IPingTest.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 09.03.2015, Christian Imfeld: Definition of the interface.
 * 
 **************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace NuvoControl.Server.WcfHostConsole
{
    interface IPingTest
    {
    }

    [ServiceContract]
    public interface IWcfPingTest
    {
        [OperationContract]
        string Ping();
    }
}

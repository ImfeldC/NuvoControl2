/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Test.NuvoClient
 *   Author:         Ch.Imfeld
 *   Creation Date:  6/12/2009 11:02:29 PM
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 6/12/2009 11:02:29 PM, Ch.Imfeld: Initial implementation.
 * 
 **************************************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Common.Logging;

namespace NuvoControl.Test.NuvoClient
{
    /// <summary>
    /// Program. Main program class of COMListener
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application. 
        /// Uses the ILog intercface for common logging.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ILog log = LogManager.GetCurrentClassLogger();
            log.Debug( m=>m("Start of COMListener! (Version={0})", Application.ProductVersion) );
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new NuvoClient());

            log.Debug(m => m("End of COMListener! (Version={0})", Application.ProductVersion));
        }
    }
}

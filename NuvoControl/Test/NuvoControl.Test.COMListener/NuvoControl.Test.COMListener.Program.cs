/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Test.COMListener
 *   Author:         Ch.Imfeld
 *   Creation Date:  6/12/2009 11:02:29 PM
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 6/12/2009 11:02:29 PM, Ch.Imfeld: Initial implementation.
 * 
 **************************************************************************************************/

/*!

 \page pageTool_COMListener COM Listener

 This tool is a helper tool, used in case of debug and maintance tasks.
 
 It is \b not used for the regular system runtime.
 
 This tool is able to connect to a regular serial COM port.
 It allows to send data to the connected end-point, and to receive its answer.
 
 With the drop-down box at the top the required serial port device can be selected.
 At start-up the tool reads all available serial COM ports on the machine and adds them to the list 
 (e.g. COM1, COM2, etc.).
 
 To connect to the selected COM port, press the 'Connect' button.
 
 To send any text, enter the text in the text field and press the 'Send' button. 
 

 <BR>
 Back to \ref main "main page"
 
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Common.Logging;

namespace NuvoControl.Test.COMListener
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// \class	Program
    ///
    /// \brief	Program. Main program class of COMListener
    ///
    /// \author	Administrator
    /// \date	17.05.2009
    ////////////////////////////////////////////////////////////////////////////////////////////////////

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
            Application.Run(new COMListener());

            log.Debug(m => m("End of COMListener! (Version={0})", Application.ProductVersion));
        }
    }
}

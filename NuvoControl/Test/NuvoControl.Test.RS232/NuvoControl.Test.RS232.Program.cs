/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Test.RS232
 *   Author:         Ch.Imfeld
 *   Creation Date:  6/12/2009 11:02:29 PM
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 6/12/2009 11:02:29 PM, Ch.Imfeld: Initial implementation.
 * 
 **************************************************************************************************/
//
// http://www.codeproject.com/KB/system/rs232ThreadSafe.aspx
//

/*!

 \page pageTool_RS232Tester RS232 Tester

 This tool is a helper tool, used in case of debug and maintance tasks.
 
 It is \b not used for the regular system runtime.
 
 This tool is able to connect to a regular serial COM port.
 It allows to send data to the connected end-point, and to receive its answer.
 
 With the drop-down box at the top the required serial port device can be selected.
 At start-up the tool reads all available serial COM ports on the machine and adds them to the list 
 (e.g. COM1, COM2, etc.).
 
 As soon the COM port has been selected, a connection is established. You do not need to press any additional button.
 
 To send any text, enter the text in the text field and press the 'Send' button. 

 Any incoming data is automatically written to the text box. 

 <BR>
 Back to \ref main "main page"
 
*/





using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NuvoControl.Test.RS232
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new fclsRS232Tester());
        }
    }
}
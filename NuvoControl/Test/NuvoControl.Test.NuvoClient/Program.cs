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

/*!

 \page pageTool_NuvoClient NuvoClient

 This tool is a helper tool, used in case of debug and maintance tasks.
 
 It is \b not used for the regular system runtime.
 
 This tool is able to connect to several serial port types, like regular COM port, the MSMQ system and
 in-built simulator object.
 It allows to send and/or receive data to/from the connected end-point.
 
 \section secToolNC_StartConnection Description: Start a connection
 
 With the drop-down box at the top the required serial port device can be selected.
 At start-up the tool reads all available serial COM ports on the machine and adds them to the list 
 (e.g. COM1, COM2, etc.).
 
 In addition it adds also the two queues used by NuvoControl to communicate between the simulator
 and the NuvoControl server. You can either select a queue directly, or with QUEUE you can select 
 the serial port class implementing the queue access (see \ref SerialPortQueue ).
 
 Beside that it allows also to connect to the in-built simulator class (see \ref ProtocolDriverSimulator )
 
 In the future you can easely add any other class which implements the ISerialPort interface.

 With the checkbox 'Receive' you can enable/disable incoming messages.
 
 With the checkbox 'Send' you can enable/disobale outgoing messages.
 
 To connect press the 'Connect' button.
 
 The lower part conatins a text area, where all incoming and outgoing data is traced.
 
 
 \section secToolNC_SendAnyText Description: Send any text
 
 The area 'Send Text' allows to send any text you like to the connected end-point.
 
 Type your message in the text field and press 'Send' to send the content of the text field.
 
 \section secToolNC_SendCommand Description: Send a Nuvo  command

 The second area 'Send Nuvo Command' allows to send a Nuvo command to the connected end-point.
 
 Select the data like Zone, Source, etc. and press 'Send' to send the corresponding command.

 <BR>
 Back to \ref main "main page"
 
*/

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

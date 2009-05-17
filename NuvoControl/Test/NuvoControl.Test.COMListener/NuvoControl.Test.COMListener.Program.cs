////////////////////////////////////////////////////////////////////////////////////////////////////
/// \file	NuvoControl.Test.COMListener\NuvoControl.Test.COMListener.Program.cs
///
/// \brief	Implements the NuvoControl.Test.COMListener.Program class. 
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Common.Logging;

////////////////////////////////////////////////////////////////////////////////////////////////////
/// \namespace	NuvoControl.Test.COMListener
///
/// \brief	Contains test client "COMListener". 
/// 
////////////////////////////////////////////////////////////////////////////////////////////////////

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
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn	[STAThread] static void Main()
        ///
        /// \brief	
        /// 		The main entry point for the application. 
        /// 		Uses the ILog intercface for common logging.
        ///
        /// \author	Administrator
        /// \date	17.05.2009
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        [STAThread]
        static void Main()
        {
            ILog log = LogManager.GetCurrentClassLogger();
            log.Debug( m=>m("Start of COMListener! (Version={0})", Application.ProductVersion) );
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new COMListener());
        }
    }
}

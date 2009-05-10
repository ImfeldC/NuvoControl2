//
// http://www.codeproject.com/KB/system/rs232ThreadSafe.aspx
//

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
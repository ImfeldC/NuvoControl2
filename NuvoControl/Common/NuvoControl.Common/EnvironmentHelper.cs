using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Common
{
    public class EnvironmentHelper
    {
        /// <summary>
        /// Returns a a string about the detected operating system.
        /// See http://mono.wikia.com/wiki/Detecting_the_execution_platform 
        /// </summary>
        /// <returns>Operating System.</returns>
        public static string getOperatingSystem()
        {
            string msg1 = "This is a Windows operating system.";
            string msg2 = "This is a Unix operating system.";
            string msg3 = "ERROR: This platform identifier is invalid.";
            string msg = null;

            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;
            switch (pid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    msg = msg1;
                    break;
                case PlatformID.Unix:
                    msg = msg2;
                    break;
                default:
                    msg = msg3;
                    break;
            }
            //Console.WriteLine(msg1);
            return msg;
        }

        /// <summary>
        /// Returns true if running on Linux system.
        /// See http://mono.wikia.com/wiki/Detecting_the_execution_platform
        /// </summary>
        /// <returns>True, if running on Linux.</returns>
        public static bool isRunningOnLinux()
        {
            int p = (int)Environment.OSVersion.Platform;
            if ((p == 4) || (p == 6) || (p == 128))
            {
                //Console.WriteLine("Running on Unix");
                return true;
            }
            else
            {
                //Console.WriteLine("NOT running on Unix");
                return false;
            }
        }
    
    }
}

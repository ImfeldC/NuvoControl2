using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

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

        // See http://stackoverflow.com/questions/11779143/run-a-python-script-from-c-sharp
        public static Process run_cmd(string cmd, string args)
        {
            Process process = null;
            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = cmd;       // e.g. cmd is full path to python.exe
                start.Arguments = args;     // e.g. args is path to .py file and any cmd line args
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                process = Process.Start(start);
                Console.WriteLine("   Process {0} {1} started .... id={2} [{3}]", cmd, args, process.Id, process.ToString());

                /*
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.Write(result);
                }
                */
            }
            catch (Exception exc)
            {
                Console.WriteLine("   EXCEPTION to start Process {0} {1} started .... exc={2}", cmd, args, exc.ToString());
                process = null;
            }

            return process;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ServiceModel;
using NuvoControl.Server.WcfService;
using NuvoControl.Server.Service;
using NuvoControl.Server.Service.Configuration;

namespace NuvoControl.Server.WcfService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("**** Console based WCF host started. *******");

            /*
            using (ServiceHost serviceHost = new ServiceHost(typeof(Service1)))
            {
                serviceHost.Open();
                Console.WriteLine("**** WCF Service is running. *******");
                Console.WriteLine("URI: {0}", serviceHost.BaseAddresses[0].AbsoluteUri);

                using (ServiceHost serviceHost2 = new ServiceHost(typeof(Service2)))
                {
                    serviceHost2.Open();
                    Console.WriteLine("**** WCF Service is running. *******");
                    Console.WriteLine("URI: {0}", serviceHost2.BaseAddresses[0].AbsoluteUri);

                    Console.WriteLine("**** Press <Enter> to terminate. *******");
                    Console.ReadLine();
                }
            } 
            */

            NuvoControlService ncService = new NuvoControlService();
            ncService.StartUp(@"..\..\..\..\Config\NuvoControlKonfiguration.xml");
            

            using (ServiceHost serviceHost = new ServiceHost(ncService))
            {
                serviceHost.Open();
                Console.WriteLine("**** WCF Service is running. *******");
                Console.WriteLine("URI: {0}", serviceHost.BaseAddresses[0].AbsoluteUri);

                using (ServiceHost serviceHost2 = new ServiceHost(ncService.IConfigureIfc()))
                {
                    serviceHost2.Open();
                    Console.WriteLine("**** WCF Service is running. *******");
                    Console.WriteLine("URI: {0}", serviceHost2.BaseAddresses[0].AbsoluteUri);

                    Console.WriteLine("**** Press <Enter> to terminate. *******");
                    Console.ReadLine();
                }

            }
    
        }
    }
}

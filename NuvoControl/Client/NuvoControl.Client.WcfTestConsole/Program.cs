using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

//using NuvoControl.Client.WcfTestConsole.ServiceReference1;
using NuvoControl.Client.WcfTestConsole.NuvoControlReference;
using NuvoControl.Client.WcfTestConsole.NuvoControlConfigurationReference;

namespace NuvoControl.Client.WcfTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("**** Console client started. *******");

            /*
            using (Service1Client serviceProxy = new Service1Client())
            {
                string returnValue = serviceProxy.GetData(3);
                CompositeType type = new CompositeType();
                type.BoolValue = true;
                type.StringValue = "Test";
                CompositeType returnValue2 = serviceProxy.GetDataUsingDataContract(type);
                Console.WriteLine("Value: {0}", returnValue);
                Console.WriteLine("Value2: {0}", returnValue2.StringValue);

                Zone zone = serviceProxy.GetData2();
                Console.WriteLine("Zone name: {0}", zone.Name);
                Console.WriteLine("Picture type: {0}", zone.PictureType);
                //zone.FloorPlanCoordinates[0] = new System.Drawing.Point();

                Console.ReadLine();


            }*/

            NuvoControlClient ncClient = new NuvoControlClient();
            Guid id = Guid.NewGuid();
            bool success = ncClient.StartSession(id);
            //object configureIfc = ncClient.IConfigureIfc();
            ConfigureClient cfgIfc = new ConfigureClient();
            Zone zone = cfgIfc.GetZoneKonfiguration(2);

            Console.WriteLine("Zone name: {0}", zone.Name);
            Console.WriteLine("Picture type: {0}", zone.PictureType);
            //zone.FloorPlanCoordinates[0] = new System.Drawing.Point();

            Console.ReadLine();

        }
    }
}

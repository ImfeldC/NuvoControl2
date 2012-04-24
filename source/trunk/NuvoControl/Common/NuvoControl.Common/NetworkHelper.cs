using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace NuvoControl.Common
{
    /// <summary>
    /// Helper methods to retrieve infôrmation about the network of the current host.
    /// More information at http://www.google.ch/url?sa=t&source=web&cd=1&ved=0CBUQFjAA&url=http%3A%2F%2Fmsdn.microsoft.com%2Fde-de%2Flibrary%2Fbb979288.aspx&ei=fA1oTJKBN5icOLD-yLkF&usg=AFQjCNGBGiUVEnjRHLPbE-PvQk_0Y2GJmw
    /// </summary>
    public class NetworkHelper
    {
        /// <summary>
        /// Returns the host name.
        /// </summary>
        /// <returns>Host name.</returns>
        public static string getHostName()
        {
            return Dns.GetHostName();
        }

        /// <summary>
        /// Returns the first IP address of the current host.
        /// </summary>
        /// <returns>IP address.</returns>
        public static string getIPAddress()
        {
            if (Dns.GetHostEntry(getHostName()).AddressList.Length > 0)
            {
                return Dns.GetHostEntry(getHostName()).AddressList[0].ToString();
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Replaces the placedolder 'localhost' in the endpoint address with the name (or IP address) of the
        /// current host.
        /// 
        /// More information at http://social.msdn.microsoft.com/Forums/en-US/wcf/thread/843ef9fd-f48e-42ed-ab7c-2baf68d6f0d1
        /// </summary>
        /// <param name="configuredEndpointAddress">Configured endpoint address, with placeholder 'localhost' in it.</param>
        /// <returns>Endpoint address with host name.</returns>
        public static string buildEndpointAddress(string configuredEndpointAddress)
        {
            string endpointAdress = configuredEndpointAddress;
            endpointAdress = endpointAdress.Replace("localhost", getHostName());
            return endpointAdress;
        }


        /// <summary>
        /// Replaces the placedolder 'localhost' in the endpoint address with the name of the
        /// current host, passed as argument.
        /// </summary>
        /// <param name="configuredEndpointAddress">Configured endpoint address, with placeholder 'localhost' in it.</param>
        /// <param name="hostname">Hostname which should be used instead of 'localhost'.</param>
        /// <returns>Endpoint address with host name.</returns>
        public static string buildEndpointAddress(string configuredEndpointAddress, string hostname)
        {
            string endpointAdress = configuredEndpointAddress;
            endpointAdress = endpointAdress.Replace("localhost", hostname);
            return endpointAdress;
        }


    
    }
}

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text;
using System.Net;
using System.IO;
 
namespace NuvoControl.ThingSpeak
{
    public class ThingSpeak
    {
        private const string _url = "http://api.thingspeak.com/";
        //private const string _APIKey = "10BLEA0XICFWMWW1";         // e.g. 10BLEA0XICFWMWW1

        /// <summary>
        /// Static Method to write 1 ... 8 data fields to ThingSpeak
        /// </summary>
        /// <param name="APIKey">Key to access ThingSpeak channel</param>
        /// <param name="field1">Value 1 to write, omit with null</param>
        /// <param name="field2">Value 2 to write, omit with null</param>
        /// <param name="field3">Value 3 to write, omit with null</param>
        /// <param name="field4">Value 4 to write, omit with null</param>
        /// <param name="field5">Value 5 to write, omit with null</param>
        /// <param name="field6">Value 6 to write, omit with null</param>
        /// <param name="field7">Value 7 to write, omit with null</param>
        /// <param name="field8">Value 8 to write, omit with null</param>
        /// <param name="TSResponse">Response from ThingSpeak</param>
        /// <returns>True, if write was ok</returns>
        public static Boolean SendDataToThingSpeak(string APIKey, string field1, string field2, string field3, string field4, string field5, string field6, string field7, string field8, out Int16 TSResponse)
        {
            StringBuilder sbQS = new StringBuilder();
 
            // Build the querystring
            sbQS.Append(_url + "update?key=" + APIKey);
            if (field1 != null) sbQS.Append("&field1=" + HttpUtility.UrlEncode(field1));
            if (field2 != null) sbQS.Append("&field2=" + HttpUtility.UrlEncode(field2));
            if (field3 != null) sbQS.Append("&field3=" + HttpUtility.UrlEncode(field3));
            if (field4 != null) sbQS.Append("&field4=" + HttpUtility.UrlEncode(field4));
            if (field5 != null) sbQS.Append("&field5=" + HttpUtility.UrlEncode(field5));
            if (field6 != null) sbQS.Append("&field6=" + HttpUtility.UrlEncode(field6));
            if (field7 != null) sbQS.Append("&field7=" + HttpUtility.UrlEncode(field7));
            if (field8 != null) sbQS.Append("&field8=" + HttpUtility.UrlEncode(field8));
 
            // The response will be a "0" if there is an error or the entry_id if > 0
            TSResponse = Convert.ToInt16(PostToThingSpeak(sbQS.ToString()));
 
            if (TSResponse > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
 
        }

        /// <summary>
        /// Statis method to write one field to ThingSpeak
        /// </summary>
        /// <param name="APIKey">Key to access ThingSpeak channel</param>
        /// <param name="field_id">Field id: 1...8</param>
        /// <param name="field_value">Field Value to write</param>
        /// <param name="TSResponse">Response from ThingSpeak</param>
        /// <returns>True, if write was ok</returns>
        public static Boolean SendDataToThingSpeak(string APIKey, string field_id, string field_value, out Int16 TSResponse)
        {
            StringBuilder sbQS = new StringBuilder();

            // Build the querystring
            sbQS.Append(_url + "update?key=" + APIKey);
            sbQS.Append("&field" + HttpUtility.UrlEncode(field_id) + "=" + HttpUtility.UrlEncode(field_value));

            // The response will be a "0" if there is an error or the entry_id if > 0
            TSResponse = Convert.ToInt16(PostToThingSpeak(sbQS.ToString()));

            if (TSResponse > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Static method to update status on ThingSpeak
        /// </summary>
        /// <param name="APIKey">Key to access ThingSpeak channel</param>
        /// <param name="status">Status to update</param>
        /// <param name="TSResponse">Response from ThingSpeak</param>
        /// <returns>True, if update was ok</returns>
        public static Boolean UpdateThingkSpeakStatus(string APIKey, string status, out Int16 TSResponse)
        {
            StringBuilder sbQS = new StringBuilder();
            sbQS.Append(_url + "update?key=" + APIKey + "&status=" + HttpUtility.UrlEncode(status));
 
            TSResponse = Convert.ToInt16(PostToThingSpeak(sbQS.ToString()));
 
            if (TSResponse > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
 
        /// <summary>
        /// Static method to update location on ThingSpeak
        /// </summary>
        /// <param name="APIKey">Key to access ThingSpeak channel</param>
        /// <param name="TSLat">Latitude to update</param>
        /// <param name="TSLong">Longitude to update</param>
        /// <param name="TSElevation">Elevation to update</param>
        /// <param name="TSResponse">Response from ThingSpeak</param>
        /// <returns>True, if update was ok</returns>
        public static Boolean UpdateThingSpeakLocation(string APIKey, string TSLat, string TSLong, string TSElevation, out Int16 TSResponse)
        {
            StringBuilder sbQS = new StringBuilder();
            sbQS.Append(_url + "update?key=" + APIKey);
 
            if (TSLat != null) sbQS.Append("&lat=" + TSLat);
            if (TSLong != null) sbQS.Append("&long=" + TSLong);
            if (TSElevation != null) sbQS.Append("&elevation=" + TSElevation);
 
            TSResponse = Convert.ToInt16(PostToThingSpeak(sbQS.ToString()));
 
            if (TSResponse > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
 
        /// <summary>
        /// Private method to post query string to ThingSpeak
        /// </summary>
        /// <param name="QueryString">Query string to post</param>
        /// <returns>Response from ThingSpeak</returns>
        private static string PostToThingSpeak(string QueryString)
        {
            StringBuilder sbResponse = new StringBuilder();
            byte[] buf = new byte[8192];
 
            // Hit the URL with the querystring and put the response in webResponse
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(QueryString);
            HttpWebResponse webResponse = (HttpWebResponse)myRequest.GetResponse();
            try
            {
                Stream myResponse = webResponse.GetResponseStream();
 
                int count = 0;
 
                // Read the response buffer and return
                do
                {
                    count = myResponse.Read(buf, 0, buf.Length);
                    if (count != 0)
                    {
                        sbResponse.Append(Encoding.ASCII.GetString(buf, 0, count));
                    }
                }
                while (count > 0);
                return sbResponse.ToString();
            }
            catch (WebException ex)
            {
                return "0";
            }
 
        }
    }
}

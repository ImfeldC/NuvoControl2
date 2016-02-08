using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Mail;


namespace NuvoControl.Common
{
    // Example from http://stackoverflow.com/questions/9201239/send-e-mail-via-smtp-using-c-sharp
    // Setup on Raspberry Pi, see http://stackoverflow.com/questions/4148019/authentication-or-decryption-has-failed-when-sending-mail-to-gmail-using-ssl
    //   mozroots --import --ask-remove --machine
    //   certmgr -ssl smtps://smtp.gmail.com:465
    public class MailHelper
    {
        // TODO: store connection settings in a proper way
        private const string your_id = "imfeldc@gmail.com";
        private const string your_password = "DIGITAL";

        /// <summary>
        /// Sends mail message
        /// </summary>
        /// <param name="recipient">Recipient mail address.</param>
        /// <param name="subject">Mail subject.</param>
        /// <param name="body">Mail body.</param>
        /// <returns>true in case send was succesful, otherwise false.</returns>
        public static bool SendMail(string recipient, string subject, string body)
        {
            return SendMail(recipient, subject, body, your_id, your_password);
        }

        /// <summary>
        /// Sends mail message
        /// </summary>
        /// <param name="recipient">Recipient mail address.</param>
        /// <param name="subject">Mail subject.</param>
        /// <param name="body">Mail body.</param>
        /// <param name="myid">GMail username.</param>
        /// <param name="mypassword">GMail password.</param>
        /// <returns>true in case send was succesful, otherwise false.</returns>
        public static bool SendMail(string recipient, string subject, string body, string myid, string mypassword)
        {
            List<MailAddress> recipients = new List<MailAddress>();
            recipients.Add(new MailAddress(recipient));
            return SendMail(recipients, subject, body, myid, mypassword);
        }


        /// <summary>
        /// Sends mail message
        /// </summary>
        /// <param name="recipients">List of recipients..</param>
        /// <param name="subject">Mail subject.</param>
        /// <param name="body">Mail body.</param>
        /// <returns>true in case send was succesful, otherwise false.</returns>
        public static bool SendMail(List<MailAddress> recipients, string subject, string body)
        {
            return SendMail(recipients, subject, body, your_id, your_password);
        }

        /// <summary>
        /// Sends mail message
        /// </summary>
        /// <param name="recipients">List of recipients.</param>
        /// <param name="subject">Mail subject.</param>
        /// <param name="body">Mail body.</param>
        /// <param name="myid">GMail username.</param>
        /// <param name="mypassword">GMail password.</param>
        /// <returns>true in case send was succesful, otherwise false.</returns>
        public static bool SendMail(List<MailAddress> recipients, string subject, string body, string myid, string mypassword)
        {
            try
            {
                SmtpClient client = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(myid, mypassword),
                    Timeout = 10000,
                };
                MailMessage mm = new MailMessage();
                mm.From = new MailAddress(your_id);
                foreach (MailAddress addr in recipients)
                {
                    mm.To.Add(addr);
                }
                mm.Subject = subject.Trim();
                mm.Body = body;
                client.Send(mm);

                //LogHelper.Log(LogLevel.Info, String.Format("Email Sent"));
                return true;
            }
            catch (Exception e)
            {
                //LogHelper.Log(LogLevel.Fatal, String.Format("Could not send email\n\n" + e.ToString()));
                return false;
            }
        }

        /// <summary>
        /// Method to replace defined placeholders.
        /// {DateTime.Now}: current date and time
        /// {MachineName}: will be replaced with the machine name
        /// {OperatingSystem}: will be repalced with a text of the OS
        /// </summary>
        /// <param name="strMessage">Message string with the placeholders to be replaced.</param>
        /// <returns>Message string with replaced placeholders.</returns>
        public static string replacePlaceHolders(string strMessage)
        {
            string strNewMessage = strMessage;
            strNewMessage = strNewMessage.Replace("{DateTime.Now}", DateTime.Now.ToString());
            strNewMessage = strNewMessage.Replace("{MachineName}", System.Environment.MachineName);
            strNewMessage = strNewMessage.Replace("{OperatingSystem}", EnvironmentHelper.getOperatingSystem());
            return strNewMessage;
        }
    }
}

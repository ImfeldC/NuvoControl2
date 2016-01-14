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
        private const string your_id = "imfeldc@gmail.com";
        private const string your_password = "DIGITAL";

        /// <summary>
        /// Sends mail message
        /// </summary>
        /// <param name="recepient">Recepient mail address.</param>
        /// <param name="subject">Mail subject.</param>
        /// <param name="body">Mail body.</param>
        /// <returns>true in case send was succesful, otherwise false.</returns>
        public static bool SendMail(string recepient, string subject, string body)
        {
            return SendMail(recepient, subject, body, your_id, your_password);
        }

        /// <summary>
        /// Sends mail message
        /// </summary>
        /// <param name="recepient">Recepient mail address.</param>
        /// <param name="subject">Mail subject.</param>
        /// <param name="body">Mail body.</param>
        /// <param name="myid">GMail username.</param>
        /// <param name="mypassword">GMail password.</param>
        /// <returns>true in case send was succesful, otherwise false.</returns>
        public static bool SendMail(string recepient, string subject, string body, string myid, string mypassword)
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
                MailMessage mm = new MailMessage(your_id, recepient, subject, body);
                client.Send(mm);

                //Console.WriteLine("Email Sent");
                return true;
            }
            catch (Exception e)
            {
                //Console.WriteLine("Could not send email\n\n" + e.ToString());
                return false;
            }
        }

    }
}

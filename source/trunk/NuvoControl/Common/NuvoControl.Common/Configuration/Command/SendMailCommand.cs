using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Mail;


namespace NuvoControl.Common.Configuration
{
    public class SendMailCommand : Command
    {

        private List<MailAddress> _toAddress = new List<MailAddress>();
        public List<MailAddress> ToAddress
        {
            get { return _toAddress; }
        }

        private List<MailAddress> _ccAddress = new List<MailAddress>();
        public List<MailAddress> CcAddress
        {
            get { return _ccAddress; }
        }

        private List<MailAddress> _bccAddress = new List<MailAddress>();
        public List<MailAddress> BccAddress
        {
            get { return _bccAddress; }
        }

        private string _configuredSubject = "";
        public string Subject
        {
            get { return _configuredSubject; }
        }

        private string _configuredBody = "";
        public string Body
        {
            get { return _configuredBody; }
        }


        /// <summary>
        /// Default constructor
        /// </summary>
        public SendMailCommand()
        {
            Initialize();
        }

        /// <summary>
        /// Standard constructor, used by configuration loader.
        /// </summary>
        /// <param name="id">Id of the command.</param>
        /// <param name="onFunctionError">True, if command shall be executed in case of an error.</param>
        /// <param name="onFunctionStart">True, if command shall be executed at function start.</param>
        /// <param name="onFunctionEnd">True, if command shall be executed at function end.</param>
        /// <param name="onValidityStart">True, if command shall be executed at validity start.</param>
        /// <param name="onValidityEnd">True, if command shall be executed at validity end.</param>
        /// <param name="onUnix">True, if command shall be exceuted on Unix systems. Default=True</param>
        /// <param name="onWindows">True, if command shall be executed on Windows systems. Default=True</param>
        /// <param name="toAddress"></param>
        /// <param name="ccAddress"></param>
        /// <param name="bccAddress"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public SendMailCommand(SimpleId id, 
            bool onFunctionError, bool onFunctionStart, bool onFunctionEnd, bool onValidityStart, bool onValidityEnd, 
            bool onUnix, bool onWindows,
            IEnumerable<MailAddress> toAddress, IEnumerable<MailAddress> ccAddress, IEnumerable<MailAddress> bccAddress,
            string subject, string body )
            : base(id, eCommand.SendMail, onFunctionError, onFunctionStart, onFunctionEnd, onValidityStart, onValidityEnd, onUnix, onWindows)
        {
            _toAddress = toAddress.ToList<MailAddress>();
            if (ccAddress != null)
                _ccAddress = ccAddress.ToList<MailAddress>();
            if (bccAddress != null)
                _bccAddress = bccAddress.ToList<MailAddress>();
            _configuredSubject = subject;
            _configuredBody = body;
            Initialize();
        }

        private void Initialize()
        {
            if (_configuredSubject == "")
            {
                // Initialize default subject
                _configuredSubject = "SendMailCommand: {OnFunction} Id={Command.Id} Machine={MachineName}";
            }
            if (_configuredBody == "")
            {
                // Initialize default body
                _configuredBody = "SendMailCommand Body: {Command.Id} on Machine={MachineName} \nOS={OperatingSystem} \nFunction={Function} \n({DateTime.Now})";
            }
        }

        /// <summary>
        /// Public override of ToString() method.
        /// </summary>
        /// <returns>Returns string representative.</returns>
        public override string ToString()
        {
            return String.Format("SendMail=[ Command={0}, ToCount={1}, CcCount={2}, BccCount={3} ]",
                base.ToString(), _toAddress.Count(), _ccAddress.Count(), _bccAddress.Count() );
        }

    }
}

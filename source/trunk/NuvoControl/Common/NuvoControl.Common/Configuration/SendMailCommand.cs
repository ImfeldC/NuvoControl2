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

        public SendMailCommand(Guid id, bool onFunctionError, bool onFunctionStart, bool onFunctionEnd,
            IEnumerable<MailAddress> toAddress, IEnumerable<MailAddress> ccAddress, IEnumerable<MailAddress> bccAddress,
            string subject, string body )
            : base( id, eCommand.SendMail, onFunctionError, onFunctionStart, onFunctionEnd )
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
                _configuredSubject = String.Format("SendMailCommand: {0}", base.Id);
            }
            if (_configuredBody == "")
            {
                // Initialize default body
                _configuredBody = String.Format("SendMailCommand Body: {0} ({1})", ToString(), DateTime.Now.ToString());
            }
        }

        /// <summary>
        /// Public override of ToString() method.
        /// </summary>
        /// <returns>Returns string representative.</returns>
        public override string ToString()
        {
            return String.Format("Command={0}, ToCount={1}, CcCount={2}, BccCount={3}",
                base.ToString(), _toAddress.Count(), _ccAddress.Count(), _bccAddress.Count() );
        }

    }
}

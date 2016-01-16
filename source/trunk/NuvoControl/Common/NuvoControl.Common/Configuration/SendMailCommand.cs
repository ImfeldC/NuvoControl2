using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Mail;


namespace NuvoControl.Common.Configuration
{
    public class SendMailCommand : Command
    {

        private List<MailAddress>   _toAddress = new List<MailAddress>();

        private List<MailAddress> _ccAddress = new List<MailAddress>();

        private List<MailAddress> _bccAddress = new List<MailAddress>();


        /// <summary>
        /// Default constructor
        /// </summary>
        public SendMailCommand()
        {
        }

        public SendMailCommand(Guid id, bool onFunctionError, bool onFunctionStart, bool onFunctionEnd,
            IEnumerable<MailAddress> toAddress, IEnumerable<MailAddress> ccAddress, IEnumerable<MailAddress> bccAddress )
            : base( id, eCommand.SendMail, onFunctionError, onFunctionStart, onFunctionEnd )
        {
            _toAddress = toAddress.ToList<MailAddress>();
            if (ccAddress != null)
                _ccAddress = ccAddress.ToList<MailAddress>();
            if (bccAddress != null)
                _bccAddress = bccAddress.ToList<MailAddress>();
        }

        public SendMailCommand(Guid id, bool onFunctionError, bool onFunctionStart, bool onFunctionEnd,
            List<MailAddress> toAddress, List<MailAddress> ccAddress, List<MailAddress> bccAddress)
            : base(id, eCommand.SendMail, onFunctionError, onFunctionStart, onFunctionEnd)
        {
            _toAddress = toAddress;
            if (ccAddress != null)
                _ccAddress = ccAddress;
            if (bccAddress != null)
                _bccAddress = bccAddress;
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

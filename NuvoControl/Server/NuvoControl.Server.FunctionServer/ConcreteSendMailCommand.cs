using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;


namespace NuvoControl.Server.FunctionServer
{
    class ConcreteSendMailCommand : ConcreteCommand
    {
        private SendMailCommand _sendMailCommand = null;

        public ConcreteSendMailCommand(SendMailCommand command)
            :base( command )
        {
            _sendMailCommand = (SendMailCommand)Command;
        }

        public override void execCommand(eCommandType cmdType)
        {
            LogHelper.Log(String.Format(">>> Execute SendMail command on event {0}: {1}", cmdType, _sendMailCommand.ToString()));

            // Send mail ...
            MailHelper.SendMail(_sendMailCommand.ToAddress, _sendMailCommand.Subject, _sendMailCommand.Body);
        }

    }
}

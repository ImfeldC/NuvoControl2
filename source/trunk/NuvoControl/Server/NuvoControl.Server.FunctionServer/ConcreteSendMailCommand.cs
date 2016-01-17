using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;


namespace NuvoControl.Server.FunctionServer
{
    /// <summary>
    /// Concrete class to implement the SendMail command.
    /// </summary>
    class ConcreteSendMailCommand : ConcreteCommand
    {
        private SendMailCommand _sendMailCommand = null;

        public ConcreteSendMailCommand(SendMailCommand command)
            :base( command )
        {
            _sendMailCommand = (SendMailCommand)Command;
        }

        public override void execCommand(eCommandType cmdType, Function function)
        {
            LogHelper.Log(String.Format(">>> Execute SendMail command on event {0}: SendMailCommand={1} / Function={2}", cmdType, _sendMailCommand.ToString(), function.ToString()));

            // Send mail ...
            MailHelper.SendMail(_sendMailCommand.ToAddress, 
                replacePlaceHolders(_sendMailCommand.Subject,cmdType,function), 
                replacePlaceHolders(_sendMailCommand.Body,cmdType,function));
        }

        /// <summary>
        /// Method to replace defined placeholders.
        /// {OnFunction}: will be replaced with command type (e.g. OnFunctionStart)
        /// {Function}: will be replaced with a string representative of the function, which caused this command to execute.
        /// {Command.Id}: Id of the command.
        /// </summary>
        /// <param name="strMessage">Message string with the placeholders to be replaced.</param>
        /// <param name="cmdType">Type of the command (e.g. OnFunctionStart)</param>
        /// <param name="function">Function which caused to execute this command.</param>
        /// <returns>Message string with replaced placeholders.</returns>
        private string replacePlaceHolders(string strMessage, eCommandType cmdType, Function function)
        {
            String strNewMessage = strMessage;
            // replace system wide placeholders
            strNewMessage = MailHelper.replacePlaceHolders(strNewMessage);
            // replace function- and command specific placeholders
            strNewMessage = strNewMessage.Replace("{OnFunction}", cmdType.ToString());
            strNewMessage = strNewMessage.Replace("{Function}", function.ToString());
            strNewMessage = strNewMessage.Replace("{Command.Id}", this.Command.Id.ToString());

            return strNewMessage;
        }
    }
}

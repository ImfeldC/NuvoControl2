/**************************************************************************************************
 * 
 *   Copyright (C) 2016 by Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.FunctionServer
 * 
 **************************************************************************************************/

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
            _sendMailCommand = command;
        }

        public override void execCommand(eCommandType cmdType, Function function)
        {
            if (checkCommandType(cmdType))
            {
                LogHelper.Log(LogLevel.Info, String.Format(">>> Execute SendMail command on event {0} ", cmdType));
                LogHelper.Log(LogLevel.Trace, String.Format("      SendMailCommand={0} / Function={1}", _sendMailCommand.ToString(), function.ToString()));
                // Send mail ...
                bool bOk = MailHelper.SendMail(_sendMailCommand.ToAddress,
                    replacePlaceHolders(_sendMailCommand.Subject, cmdType, function),
                    replacePlaceHolders(_sendMailCommand.Body, cmdType, function));
                if (!bOk)
                {
                    LogHelper.Log(LogLevel.Error, String.Format("      Execute SendMail FAILED!"));
                }
            }
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

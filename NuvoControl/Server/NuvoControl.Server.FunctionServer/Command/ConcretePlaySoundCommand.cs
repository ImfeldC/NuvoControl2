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
using NuvoControl.Server.ZoneServer;
using NuvoControl.Server.ProtocolDriver.Interface;


namespace NuvoControl.Server.FunctionServer
{
    class ConcretePlaySoundCommand : ConcreteCommand, IDisposable
    {
        #region Private Fields

        /// <summary>
        /// Private member to store audio driver.
        /// </summary>
        protected IAudioDriver _audioDriver = null;


        private PlaySoundCommand _playSoundCommand = null;

        #endregion

        public ConcretePlaySoundCommand(PlaySoundCommand command, IZoneServer zoneServer, IAudioDriver audioDriver)
            :base( command )
        {
            _playSoundCommand = command;
            _audioDriver = audioDriver;
        }

        /// <summary>
        /// Method called for execCommand
        /// </summary>
        /// <param name="cmdType">Command type.</param>
        /// <param name="function">Related fucntion.</param>
        public override void execCommand(eCommandType cmdType, Function function)
        {
            // onFunctionStart & onValidityStart configured to run ...
            if (checkCommandType(cmdType) && (cmdType == eCommandType.onFunctionStart | cmdType == eCommandType.onValidityStart) )
            {
                LogHelper.Log(LogLevel.Info, String.Format(">>> Execute PlaySound@START command on event {0}", cmdType));
                LogHelper.Log(LogLevel.Trace, String.Format("      PlaySoundCommand={0} / Function={1}", _playSoundCommand.ToString(), function.ToString()));

                _audioDriver.CommandPlaySound(_playSoundCommand.Url);           
            }

            // onFunctionEnd & onValidityEnd configured to run ...
            if (checkCommandType(cmdType) && (cmdType == eCommandType.onFunctionEnd | cmdType == eCommandType.onValidityEnd) )
            {
                LogHelper.Log(LogLevel.Info, String.Format(">>> Execute PlaySound@END command on event {0}", cmdType));
                LogHelper.Log(LogLevel.Trace, String.Format("      PlaySoundCommand={0} / Function={1}", _playSoundCommand.ToString(), function.ToString()));

                _audioDriver.Close();
            }

            // onFunctionError configured to run ...
            // ... stop any running process in case of an error
            if (checkCommandType(cmdType) && cmdType == eCommandType.onFunctionError)
            {
                LogHelper.Log(LogLevel.Info, String.Format(">>> Execute PlaySound@ERROR command on event {0}", cmdType));
                LogHelper.Log(LogLevel.Trace, String.Format("      PlaySoundCommand={0} / Function={1}", _playSoundCommand.ToString(), function.ToString()));

                _audioDriver.Close();
            }
        }

        /// <summary>
        /// Dispose method, to esnure that play sound command (process) gets killed at shutdown
        /// </summary>
        public new void Dispose()
        {
            // close any sound playing, before shutdown
            _audioDriver.Close();
            base.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NuvoControl.Common.Configuration
{
    /// <summary>
    /// Enum of supported commands
    /// </summary>
    public enum eCommand
    {
        /// <summary>
        /// Send mail
        /// </summary>
        SendMail = 0,
        /// <summary>
        /// Play sound
        /// </summary>
        PlaySound = 1,
        /// <summary>
        /// Start a process
        /// </summary>
        StartProcess = 2,
        /// <summary>
        /// Send Nuvo Command
        /// </summary>
        SendNuvoCommand = 3
    }

    /// <summary>
    /// Events in case the command shall be executed.
    /// </summary>
    public enum eCommandType
    {
        onFunctionError = 0,    // in case an error occurs
        onFunctionStart = 1,    // in case function starts (e.g. alarm time reached)
        onFunctionEnd = 2,      // in case function ends (e.g. sleep duration reached)
        onValidityStart = 3,    // in case validity of a function starts
        onValidityEnd = 4       // in case valididty of a function ends
    }

    [DataContract]
    public abstract class Command
    {


        /// <summary>
        /// The id of the command.
        /// </summary>
        [DataMember]
        private SimpleId _id = SimpleId.NewGuid();

        /// <summary>
        /// The command to be executed.
        /// </summary>
        [DataMember]
        private eCommand _command;

        /// <summary>
        /// True, if command shall be executed on Unix platform
        /// </summary>
        [DataMember]
        private bool _onUnix = false;

        /// <summary>
        /// True, if command shall be executed on Unix platform
        /// </summary>
        public bool OnUnix
        {
            get { return _onUnix; }
            set { _onUnix = value; }
        }


        /// <summary>
        /// True, if command shall be executed on Windows platform
        /// </summary>
        [DataMember]
        private bool _onWindows = false;

        /// <summary>
        /// True, if command shall be executed on Windows platform
        /// </summary>
        public bool OnWindows
        {
            get { return _onWindows; }
            set { _onWindows = value; }
        }


        [DataMember]
        private bool _onFunctionError = false;

        /// <summary>
        /// True, if command shall be executed in case of an error.
        /// </summary>
        public bool OnFunctionError
        {
            get { return _onFunctionError; }
            set { _onFunctionError = value; }
        }


        [DataMember]
        private bool _onFunctionStart = false;

        /// <summary>
        /// True, if command shall be executed at function start.
        /// </summary>
        public bool OnFunctionStart
        {
            get { return _onFunctionStart; }
            set { _onFunctionStart = value; }
        }


        [DataMember]
        private bool _onFunctionEnd = false;

        /// <summary>
        /// True, if command shall be executed at function end.
        /// </summary>
        public bool OnFunctionEnd
        {
            get { return _onFunctionEnd; }
            set { _onFunctionEnd = value; }
        }


        [DataMember]
        private bool _onValidityStart = false;

        /// <summary>
        /// True, if command shall be executed at function validity start.
        /// </summary>
        public bool OnValidityStart
        {
            get { return _onValidityStart; }
            set { _onValidityStart = value; }
        }


        [DataMember]
        private bool _onValidityEnd = false;

        /// <summary>
        /// True, if command shall be executed at validity end.
        /// </summary>
        public bool OnValidityEnd
        {
            get { return _onValidityEnd; }
            set { _onValidityEnd = value; }
        }


        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Command()
        {
        }

        /// <summary>
        /// Standard constructor, used by configuration loader.
        /// </summary>
        /// <param name="id">Id of the command.</param>
        /// <param name="command">Command itself (e.g. SendMail)</param>
        /// <param name="onFunctionError">True, if command shall be executed in case of an error.</param>
        /// <param name="onFunctionStart">True, if command shall be executed at function start.</param>
        /// <param name="onFunctionEnd">True, if command shall be executed at function end.</param>
        /// <param name="onValidityStart">True, if command shall be executed at validity start.</param>
        /// <param name="onValidityEnd">True, if command shall be executed at validity end.</param>
        /// <param name="onUnix">True, if command shall be exceuted on Unix systems. Default=True</param>
        /// <param name="onWindows">True, if command shall be executed on Windows systems. Default=True</param>
        public Command(SimpleId id, eCommand command, 
            bool onFunctionError, bool onFunctionStart, bool onFunctionEnd, bool onValidityStart, bool onValidityEnd,
            bool onUnix, bool onWindows)
        {
            _id = id;
            _command = command;
            _onFunctionError = onFunctionError;
            _onFunctionStart = onFunctionStart;
            _onFunctionEnd = onFunctionEnd;
            _onValidityStart = onValidityStart;
            _onValidityEnd = onValidityEnd;
            _onUnix = onUnix;
            _onWindows = onWindows;
        }

        /// <summary>
        /// Accessor for the command id.
        /// </summary>
        public SimpleId Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Public override of ToString() method.
        /// </summary>
        /// <returns>Returns string representative.</returns>
        public override string ToString()
        {
            return String.Format("Command: {0} -- [OS: Unix={7}, Windows={8}] -- [EVENTS: OnError={2}, OnFuncStart={3}, OnFuncEnd={4}, OnValStart={5}, OnValEnd={6}] -- Id={1}", 
                _command, Id,
                (_onFunctionError ? "Yes" : "No"), (_onFunctionStart ? "Yes" : "No"), (_onFunctionEnd ? "Yes" : "No"), (_onValidityStart ? "Yes" : "No"), (_onValidityEnd ? "Yes" : "No"),
                (_onUnix ? "Yes" : "No"), (_onWindows ? "Yes" : "No") );
        }

    }
}

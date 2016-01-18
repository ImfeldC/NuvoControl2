using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NuvoControl.Common.Configuration
{
    public enum eCommand
    {
        SendMail = 0,
        PlaySound = 1
    }

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
        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// The command to be executed.
        /// </summary>
        [DataMember]
        private eCommand _command;


        [DataMember]
        private bool _onFunctionError = false;

        public bool OnFunctionError
        {
            get { return _onFunctionError; }
            set { _onFunctionError = value; }
        }


        [DataMember]
        private bool _onFunctionStart = false;

        public bool OnFunctionStart
        {
            get { return _onFunctionStart; }
            set { _onFunctionStart = value; }
        }


        [DataMember]
        private bool _onFunctionEnd = false;

        public bool OnFunctionEnd
        {
            get { return _onFunctionEnd; }
            set { _onFunctionEnd = value; }
        }


        [DataMember]
        private bool _onValidityStart = false;

        public bool OnValidityStart
        {
            get { return _onValidityStart; }
            set { _onValidityStart = value; }
        }


        [DataMember]
        private bool _onValidityEnd = false;

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
        public Command(Guid id, eCommand command, bool onFunctionError, bool onFunctionStart, bool onFunctionEnd, bool onValidityStart, bool onValidityEnd)
        {
            _id = id;
            _command = command;
            _onFunctionError = onFunctionError;
            _onFunctionStart = onFunctionStart;
            _onFunctionEnd = onFunctionEnd;
            _onValidityStart = onValidityStart;
            _onValidityEnd = onValidityEnd;
        }

        /// <summary>
        /// Accessor for the command id.
        /// </summary>
        public Guid Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Public override of ToString() method.
        /// </summary>
        /// <returns>Returns string representative.</returns>
        public override string ToString()
        {
            return String.Format("Command: {0}, OnError={2}, OnFuncStart={3}, OnFuncEnd={4}, OnValStart={5}, OnValEnd={6}, Id={1}", 
                _command, Id,
                (_onFunctionError ? "Yes" : "No"), (_onFunctionStart ? "Yes" : "No"), (_onFunctionEnd ? "Yes" : "No"), (_onValidityStart ? "Yes" : "No"), (_onValidityEnd ? "Yes" : "No"));
        }

    }
}

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


    [DataContract]
    public class Command
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
        [DataMember]
        private bool _onFunctionStart = false;
        [DataMember]
        private bool _onFunctionEnd = false;


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
        public Command(Guid id, eCommand command, bool onFunctionError, bool onFunctionStart, bool onFunctionEnd)
        {
            _id = id;
            _command = command;
            _onFunctionError = onFunctionError;
            _onFunctionStart = onFunctionStart;
            _onFunctionEnd = onFunctionEnd;
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
            return String.Format("Command={0}, OnError={2}, OnStart={3}, OnEnd={4}, Guid={1}", 
                _command, Id, 
                (_onFunctionError?"Yes":"No"), (_onFunctionStart?"Yes":"No"), (_onFunctionEnd?"Yes":"No"));
        }

    }
}

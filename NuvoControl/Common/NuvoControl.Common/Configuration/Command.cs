using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NuvoControl.Common.Configuration
{
    [DataContract]
    public class Command
    {
        /// <summary>
        /// The id of the command.
        /// </summary>
        //[DataMember]
        //private Guid _id = Guid.NewGuid();

        [DataMember]
        private string _command;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Command()
        {
        }

        public Command(string command)
        {
            _command = command;
        }

        /// <summary>
        /// Accessor for the function id.
        /// </summary>
        //public Guid Id
        //{
        //    get { return _id; }
        //}

        /// <summary>
        /// Public override of ToString() method.
        /// </summary>
        /// <returns>Returns string representative.</returns>
        public override string ToString()
        {
            return String.Format("Command={0}", _command);
        }

    }
}

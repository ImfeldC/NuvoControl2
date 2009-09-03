using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Server.FunctionServer
{
    class FunctionServerException : Exception
    {
        /// <summary>
        /// Public constructor for the function server excpetion.
        /// A message is expected to describe the cause of the exception.
        /// </summary>
        /// <param name="message">Cause message of the exception.</param>
        public FunctionServerException(string message)
            : base(message)
        {
        }
    }
}

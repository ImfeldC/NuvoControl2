using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Common.Configuration
{
    public class PlaySoundCommand : Command
    {

        /// <summary>
        /// Standard constructor, used by configuration loader.
        /// </summary>
        /// <param name="id">Id of the command.</param>
        /// <param name="onFunctionError">True, if command shall be executed in case of an error.</param>
        /// <param name="onFunctionStart">True, if command shall be executed at function start.</param>
        /// <param name="onFunctionEnd">True, if command shall be executed at function end.</param>
        /// <param name="onValidityStart">True, if command shall be executed at validity start.</param>
        /// <param name="onValidityEnd">True, if command shall be executed at validity end.</param>
        public PlaySoundCommand(Guid id, bool onFunctionError, bool onFunctionStart, bool onFunctionEnd, bool onValidityStart, bool onValidityEnd)
            :base(id, eCommand.PlaySound, onFunctionError, onFunctionStart, onFunctionEnd, onValidityStart, onValidityEnd)
        {
        }

    }
}

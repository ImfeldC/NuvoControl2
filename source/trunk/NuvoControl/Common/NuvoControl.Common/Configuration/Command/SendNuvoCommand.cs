using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Common.Configuration
{
    public class SendNuvoCommand : Command
    {
        Address _zoneId;
        public Address ZoneId
        {
            get { return _zoneId; }
        }

        Address _sourceId = null;
        public Address SourceId
        {
            get { return _sourceId; }
        }

        string _powerStatus = "";
        public bool PowerStatus
        {
            get { return (_powerStatus=="ON"?true:false); }
        }

        int _volume = -1;
        public int Volume
        {
            get { return _volume; }
        }

        public SendNuvoCommand(Guid id, bool onFunctionError, bool onFunctionStart, bool onFunctionEnd, bool onValidityStart, bool onValidityEnd, bool onUnix, bool onWindows,
            Address ZoneId, string PowerStatus, string SourceId, int Volume)
            :base(id, eCommand.PlaySound, onFunctionError, onFunctionStart, onFunctionEnd, onValidityStart, onValidityEnd, onUnix, onWindows)
        {
            _zoneId = ZoneId;
            // check source id
            if( SourceId != "" )
            {
                _sourceId = new Address(int.Parse(SourceId.Split(new char[] { SystemConfiguration.ID_SEPARATOR })[0]),
                        int.Parse(SourceId.Split(new char[] { SystemConfiguration.ID_SEPARATOR })[1]));
            }
            _powerStatus = PowerStatus;
            _volume = Volume;
        }

        /// <summary>
        /// Public override of ToString() method.
        /// </summary>
        /// <returns>Returns string representative.</returns>
        public override string ToString()
        {
            return String.Format("SendNuvoCommand=[ Command={0}, ZoneId={1}, SourceId={2}, PowerStatus={3}, Volume={4} ]",
                base.ToString(), _zoneId.ToString(), _sourceId.ToString(), _powerStatus, _volume );
        }
    }
}

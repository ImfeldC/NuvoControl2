using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Server.ProtocolDriver.Interface;

namespace NuvoControl.Server.ProtocolDriver
{
    public class NuvoEssentiaCommand : INuvoEssentiaCommand
    {
        private NuvoEssentiaSingleCommand[] _commandList;

        #region INuvoEssentiaCommand Members

        public Guid Guid
        {
            get { throw new NotImplementedException(); }
        }

        public ENuvoEssentiaCommands Command
        {
            get { throw new NotImplementedException(); }
        }

        public bool Valid
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime CreatedDateTime
        {
            get { throw new NotImplementedException(); }
        }

        public DateTime SendDateTime
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DateTime ReceiveDateTime
        {
            get { throw new NotImplementedException(); }
        }

        public string OutgoingCommandTemplate
        {
            get { throw new NotImplementedException(); }
        }

        public string OutgoingCommand
        {
            get { throw new NotImplementedException(); }
        }

        public string IncomingCommandTemplate
        {
            get { throw new NotImplementedException(); }
        }

        public string IncomingCommand
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ENuvoEssentiaZones ZoneId
        {
            get { throw new NotImplementedException(); }
        }

        public ENuvoEssentiaSources SourceId
        {
            get { throw new NotImplementedException(); }
        }

        public EZonePowerStatus PowerStatus
        {
            get { throw new NotImplementedException(); }
        }

        public EIRCarrierFrequency IrCarrierFrequencySource(ENuvoEssentiaSources source)
        {
            throw new NotImplementedException();
        }

        public int VolumeLevel
        {
            get { throw new NotImplementedException(); }
        }

        public int BassLevel
        {
            get { throw new NotImplementedException(); }
        }

        public int TrebleLevel
        {
            get { throw new NotImplementedException(); }
        }

        public EVolumeResetStatus VolumeResetStatus
        {
            get { throw new NotImplementedException(); }
        }

        public EDIPSwitchOverrideStatus DIPSwitchOverrideStatus
        {
            get { throw new NotImplementedException(); }
        }

        public ESourceGroupStatus SourceGrupStatus
        {
            get { throw new NotImplementedException(); }
        }

        public string FirmwareVersion
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

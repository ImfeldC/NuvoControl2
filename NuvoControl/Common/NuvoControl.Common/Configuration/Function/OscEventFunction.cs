/**************************************************************************************************
 * 
 *   Copyright (C) 2016 by Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common.Configuration
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NuvoControl.Common.Configuration
{
    /// <summary>
    /// This is a system configuration class. 
    /// 
    /// It is a data structurer.
    /// It defines attributes, specifying a osc event function.
    /// </summary>
    [DataContract]
    public class OscEventFunction : Function
    {
        #region Private Members

        /// <summary>
        /// Address of the osc device related to this function.
        /// </summary>
        [DataMember]
        private Address _oscDevice = new Address(SystemConfiguration.ID_UNDEFINED, SystemConfiguration.ID_UNDEFINED);

        /// <summary>
        /// Related osc message (event)
        /// </summary>
        [DataMember]
        private string _onEvent = "";

        /// <summary>
        /// Values related to the osc event
        /// </summary>
        [DataMember]
        private int _oscValue = 0;

        #endregion


        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public OscEventFunction()
        {
        }

        public OscEventFunction(SimpleId id, Address oscDevice, string oscEvent, int oscValue, List<DayOfWeek> validOnDays, List<Command> commands)
            : base(id, new Address(), commands)
        {
            initMembers(oscDevice, oscEvent, oscValue, validOnDays, new TimeSpan(), new TimeSpan(), commands);
        }

        public OscEventFunction(SimpleId id, Address oscDevice, string oscEvent, int oscValue, List<DayOfWeek> validOnDays, TimeSpan validFrom, TimeSpan validTo, List<Command> commands)
            : base(id, new Address(), commands)
        {
            initMembers(oscDevice, oscEvent, oscValue, validOnDays, validFrom, validTo, commands);
        }

        private void initMembers(Address oscDevice, string oscEvent, int oscValue, List<DayOfWeek> validOnDays, TimeSpan validFrom, TimeSpan validTo, List<Command> commands)
        {
            _oscDevice = oscDevice;
            _oscValue = oscValue;
            if (validOnDays != null)
                this._validOnDays = validOnDays;
            if (validFrom != null && validFrom.Ticks > 0 )
                this._validFrom = validFrom;
            if (validTo != null && validTo.Ticks > 0)
                this._validTo = validTo;
        }

        #endregion

        #region Public Interface

        public Address OscDevice
        {
            get { return _oscDevice; }
        }

        public string OnEvent
        {
            get { return _onEvent; }
        }

        public int OscValue
        {
            get { return _oscValue; }
        }

        #endregion

        
        /// <summary>
        /// Public override of ToString() method.
        /// </summary>
        /// <returns>Returns string representative.</returns>
        public override string ToString()
        {
            return String.Format("OscEventFunction: OscDevice={0}, OscEvent={1}, OscValue={2}, Valid from={3} to={4}, {5}", OscDevice.ToString(), OnEvent, OscValue, ValidFrom, ValidTo, base.ToString());
        }

    }
}

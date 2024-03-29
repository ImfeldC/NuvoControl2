﻿/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.05.2009
 *   File Name:      Function.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 12.05.2009, Bernhard Limacher: First implementation.
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
    /// Is used as base class for all function classes.
    /// 
    /// NOTE: Due subclassing issues with WCF, you need to specify the allowed subclasses.
    /// In this case: SleepFunction and AlarmFunction
    /// See also http://msdn.microsoft.com/en-us/magazine/gg598929.aspx
    /// </summary>
    [DataContract]
    [KnownType(typeof(SleepFunction))]
    [KnownType(typeof(AlarmFunction))]
    public abstract class Function
    {
        #region Private Members

        /// <summary>
        /// The id of the function.
        /// </summary>
        [DataMember]
        private SimpleId _id = SimpleId.NewGuid();

        /// <summary>
        /// The address of the zone, which this function is applied for.
        /// </summary>
        [DataMember]
        private Address _zoneId = new Address(SystemConfiguration.ID_UNDEFINED, SystemConfiguration.ID_UNDEFINED);

        /// <summary>
        /// Start time in which a function can be triggered.
        /// </summary>
        [DataMember]
        protected TimeSpan _validFrom = new TimeSpan(0, 0, 0);

        /// <summary>
        /// End time in which a function can be triggered.
        /// </summary>
        [DataMember]
        protected TimeSpan _validTo = new TimeSpan(23,59,59);

        /// <summary>
        /// The days, on which this functions is valid.
        /// </summary>
        [DataMember]
        //protected List<DayOfWeek> _validOnDays = new List<DayOfWeek>();
        protected List<DayOfWeek> _validOnDays = null;

        [DataMember]
        private List<Command> _commands = new List<Command>();

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Function()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">The function id.</param>
        /// <param name="zoneId">The address of the zone, which this function is applied for.</param>
        public Function(SimpleId id, Address zoneId)
        {
            this._id = id;
            this._zoneId = zoneId;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">The function id.</param>
        /// <param name="zoneId">The address of the zone, which this function is applied for.</param>
        public Function(SimpleId id, Address zoneId, List<Command> commands)
        {
            this._id = id;
            this._zoneId = zoneId;
            if( commands != null ) this._commands.AddRange( commands );
        }

        #endregion

        #region Public Interface


        /// <summary>
        /// Accessor for the function id.
        /// </summary>
        public SimpleId Id
        {
            get { return _id; }
        }


        /// <summary>
        /// Accessor for the zone address which this function is applied for.
        /// </summary>
        public Address ZoneId
        {
            get { return _zoneId; }
        }

        /// <summary>
        /// Start time in which a sleep function can be triggered.
        /// </summary>
        public TimeSpan ValidFrom
        {
            get { return _validFrom; }
        }

        /// <summary>
        /// End time in which a sleep function can be triggered.
        /// </summary>
        public TimeSpan ValidTo
        {
            get { return _validTo; }
        }

        /// <summary>
        /// The days, on which this alarm is valid.
        /// </summary>
        public List<DayOfWeek> ValidOnDays
        {
            get { return _validOnDays; }
        }


        public List<Command> Commands
        {
            get { return _commands; }
        }


        /// <summary>
        /// Public override of ToString() method.
        /// </summary>
        /// <returns>Returns string representative.</returns>
        public override string ToString()
        {
            string strCommand = "";
            if (_commands != null && _commands.Count() > 0)
            {
                foreach (Command command in _commands)
                {
                    strCommand += command.ToString();
                    strCommand += ",";
                }
            }
            return String.Format("Zone={0}, Id={1}, Commands={2} [{3}]", ZoneId, Id, (_commands != null ? _commands.Count().ToString() : "None"), strCommand);
        }

        #endregion

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/

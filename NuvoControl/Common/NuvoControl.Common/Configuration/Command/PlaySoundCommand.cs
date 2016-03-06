/**************************************************************************************************
 * 
 *   Copyright (C) 2016 by Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Common.Configuration
{
    /// <summary>
    /// This is a system configuration class. 
    /// 
    /// It is a data structurer.
    /// It conatins the defintion of the play sound command.
    /// </summary>
    public class PlaySoundCommand : Command
    {

        #region Private Members

        /// <summary>
        /// The address of the source.
        /// </summary>
        private Address _sourceId = new Address(SystemConfiguration.ID_UNDEFINED, SystemConfiguration.ID_UNDEFINED);

        /// <summary>
        /// The URL to play.
        /// </summary>
        private string _url = "";

        #endregion


        #region Constructors

        /// <summary>
        /// Standard constructor, used by configuration loader.
        /// </summary>
        /// <param name="id">Id of the command.</param>
        /// <param name="onFunctionError">True, if command shall be executed in case of an error.</param>
        /// <param name="onFunctionStart">True, if command shall be executed at function start.</param>
        /// <param name="onFunctionEnd">True, if command shall be executed at function end.</param>
        /// <param name="onValidityStart">True, if command shall be executed at validity start.</param>
        /// <param name="onValidityEnd">True, if command shall be executed at validity end.</param>
        /// <param name="onUnix">True, if command shall be exceuted on Unix systems. Default=True</param>
        /// <param name="onWindows">True, if command shall be executed on Windows systems. Default=True</param>
        /// <param name="sourceId">Source Id, on which to play the sound.</param>
        /// <param name="url">URL to play.</param>
        public PlaySoundCommand(SimpleId id, bool onFunctionError, bool onFunctionStart, bool onFunctionEnd, bool onValidityStart, bool onValidityEnd, bool onUnix, bool onWindows,
            Address sourceId, string url)
            : base(id, eCommand.PlaySound, onFunctionError, onFunctionStart, onFunctionEnd, onValidityStart, onValidityEnd, onUnix, onWindows)
        {
            _sourceId = sourceId;
            _url = url;
        }

        #endregion


        #region Public Interface

        /// <summary>
        /// Source id where to play the sound.
        /// </summary>
        public Address SourceId
        {
            get { return _sourceId; }
            set { _sourceId = value; }
        }

        /// <summary>
        /// URL to play.
        /// </summary>
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }


        /// <summary>
        /// Public override of ToString() method.
        /// </summary>
        /// <returns>Returns string representative.</returns>
        public override string ToString()
        {
            return String.Format("PlaySound=[ url={1}, Command={0} ]",
                base.ToString(), _url);
        }

        #endregion

    }
}

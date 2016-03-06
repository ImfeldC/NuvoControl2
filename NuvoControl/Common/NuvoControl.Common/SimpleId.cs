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

namespace NuvoControl.Common
{
    /// <summary>
    /// Class to hide Guid class, allows also "simple" ids
    /// </summary>
    public class SimpleId 
    {

        private Guid _guid = Guid.Empty;
        private string _simpleId = "-";

        public SimpleId()
        {
            _guid = Guid.NewGuid();
            _simpleId = _guid.ToString();
        }

        public SimpleId( string guid)
        {
            try 
            {
                _guid = new Guid(guid);
                _simpleId = _guid.ToString();
            }
            catch( System.FormatException exc)
            {
                // This is not a "valid" guid, handle as "simple id"
                _guid = Guid.Empty;
                _simpleId = guid;
            }
        }

        public static SimpleId NewGuid()
        {
            return new SimpleId();
        }

        /// <summary>
        /// Public override of ToString() method.
        /// </summary>
        /// <returns>Returns string representative.</returns>
        public override string ToString()
        {
            return _simpleId;
        }


        /// <summary>
        /// Determines wether the specified object equals the current object.
        /// </summary>
        /// <param name="other">The object to compare with.</param>
        /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object other)
        {
            if (other == null)
                return false;

            SimpleId id = other as SimpleId;
            if ((object)id == null)
                return false;

            int iRes = String.Compare(_simpleId, id._simpleId);

            return (iRes == 0);
        }

        /// <summary>
        /// Determines wether the specified simple id equals the current address.
        /// </summary>
        /// <param name="other">The simple id to compare with.</param>
        /// <returns>True if the specified simple id is equal to the current simple id; otherwise, false.</returns>
        public bool Equals(SimpleId other)
        {
            if ((object)other == null)
                return false;

            int iRes = String.Compare(_simpleId, other._simpleId);

            return (iRes == 0);
        }


        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="id1">Left hand side parameter.</param>
        /// <param name="id2">Reight hand side parameter</param>
        /// <returns>True, if the specified addresses are equal; otherwise false.</returns>
        public static bool operator ==(SimpleId id1, SimpleId id2)
        {
            if ((object)id1 == null)
                return (object)id2 == null;

            return id1.Equals(id2);
        }


        /// <summary>
        /// Unequality operator
        /// </summary>
        /// <param name="id1">Left hand side parameter.</param>
        /// <param name="id2">Reight hand side parameter</param>
        /// <returns>True, if the specified addresses are unequal; otherwise false.</returns>
        public static bool operator !=(SimpleId id1, SimpleId id2)
        {
            return !(id1 == id2);
        }
    
    }
}

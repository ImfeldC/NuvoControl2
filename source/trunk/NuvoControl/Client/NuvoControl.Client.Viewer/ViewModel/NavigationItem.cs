/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Client.Viewer
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.07.2009
 *   File Name:      NavigationItem.cs
 * 
 ***************************************************************************************************
 * 
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Common.Configuration;

namespace NuvoControl.Client.Viewer.ViewModel
{
    /// <summary>
    /// Specifies a all attributes of a view at a point of time.
    /// 
    /// With this data, it is possible to navigate within the application and within history as well.
    /// </summary>
    public class NavigationItem
    {
        /// <summary>
        /// The hierarchy. Right now this is either the main context, the floor context or the zone context.
        /// </summary>
        private IHierarchyContext _context;

        /// <summary>
        /// The address of the item shown in the view.
        /// </summary>
        private Address _id;

        /// <summary>
        /// The name of the item shown in the view.
        /// </summary>
        private string _name;


        /// <summary>
        /// Specifies a all attributes of a view at a point of time.
        /// </summary>
        /// <param name="context">The hierarchy. Right now this is either the main context, the floor context or the zone context</param>
        /// <param name="id">The address of the item shown in the view.</param>
        /// <param name="name">The name of the item shown in the view.</param>
        public NavigationItem(IHierarchyContext context, Address id, string name)
        {
            this._context = context;
            this._id = id;
            this._name = name;
        }


        /// <summary>
        /// The hierarchy. Right now this is either the main context, the floor context or the zone context
        /// </summary>
        public IHierarchyContext Context
        {
            get { return _context; }
        }


        /// <summary>
        /// The address of the item shown in the view.
        /// </summary>
        public Address Id
        {
            get { return _id; }
        }


        /// <summary>
        /// The name of the item shown in the view.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }


        /// <summary>
        /// Determines wether the specified object equals the current object.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            NavigationItem item = obj as NavigationItem;
            if ((object)item == null)
                return false;

            return (_id == item._id);
        }


        /// <summary>
        /// Determines wether the specified address equals the current address.
        /// </summary>
        /// <param name="item">The navigation item to compare with.</param>
        /// <returns>True if the specified navigation item is equal to the current navigation item; otherwise, false.</returns>
        public bool Equals(NavigationItem item)
        {
            if ((object)item == null)
                return false;

            return (_id == item._id);
        }


        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="item1">Left hand side parameter.</param>
        /// <param name="item2">Reight hand side parameter</param>
        /// <returns>True, if the specified items are equal; otherwise false.</returns>
        public static bool operator ==(NavigationItem item1, NavigationItem item2)
        {
            if ((object)item1 == null)
                return (object)item2 == null;

            return item1.Equals(item2);
        }


        /// <summary>
        /// Unequality operator
        /// </summary>
        /// <param name="item1">Left hand side parameter.</param>
        /// <param name="item2">Reight hand side parameter</param>
        /// <returns>True, if the specified items are unequal; otherwise false.</returns>
        public static bool operator !=(NavigationItem item1, NavigationItem item2)
        {
            return !(item1 == item2);
        }


        /// <summary>
        /// Hash function for the address type.
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }


        /// <summary>
        /// ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _name;
        }
    }
}


/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/

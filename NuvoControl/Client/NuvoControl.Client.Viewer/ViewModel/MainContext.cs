﻿/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Client.Viewer
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.07.2009
 *   File Name:      MainContext.cs
 * 
 ***************************************************************************************************
 * 
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows;
using System.IO;

using NuvoControl.Common.Configuration;

namespace NuvoControl.Client.Viewer.ViewModel
{
    /// <summary>
    /// This is the logical context of the main view (top/main hierarchy) of the viewer.
    /// It contains the logical context of the building/house.
    /// </summary>
    public class MainContext : INotifyPropertyChanged, IHierarchyContext
    {
        #region Fields

        /// <summary>
        /// Configuration data of the building.
        /// </summary>
        private Building _building;

        /// <summary>
        /// The parent hierarchy.
        /// </summary>
        private IHierarchyContext _parent = null;

        /// <summary>
        /// The child hierarchy. This is the floor context.
        /// </summary>
        private IHierarchyContext _child = null;

        /// <summary>
        /// If this hierarchy is visible or not.
        /// </summary>
        private Visibility _visibility = Visibility.Visible;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="building">Configuration data of the building.</param>
        public MainContext(Building building)
        {
            this._building = building;
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// The image shown if this hierarchy is visible.
        /// </summary>
        public BitmapImage MainImage
        {
            get
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), _building.PicturePath);
                return new BitmapImage(new Uri(path));
            }
        }


        /// <summary>
        /// Tooltip of for the view of the context.
        /// </summary>
        public string ToolTip
        {
            get { return ("Click to browse down to " + (_child==null?"(n/a)":_child.ObjectName)); }
        }


        #endregion

        #region IHierarchyContext Members

        /// <summary>
        /// The name of the object, shown in this hierarchy. Here it is the name of the building.
        /// </summary>
        public string ObjectName
        {
            get { return _building.Name; }
        }


        /// <summary>
        /// The address of the object, shown in the current hierarchy.
        /// Here it is the address of the building.
        /// </summary>
        public Address ObjectId
        {
            get { return _building.Id; }
        }


        /// <summary>
        /// The parent hierarchy context.
        /// </summary>
        public IHierarchyContext Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }


        /// <summary>
        /// The child hierarchy context.
        /// Here it is the floor context.
        /// </summary>
        public IHierarchyContext Child
        {
            get { return _child; }
            set { _child = value; }
        }


        /// <summary>
        /// The visibility of the hierarchy context. 
        /// Only one can be visible at a time.
        /// </summary>
        public Visibility ContextVisibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                NotifyPropertyChanged(new PropertyChangedEventArgs("ContextVisibility"));
            }
        }


        /// <summary>
        /// True, if the context has a next object to show.
        /// Here, there is nothing more to show.
        /// </summary>
        public bool HasNextObject
        {
            get { return false; }
        }


        /// <summary>
        /// The name of the next object.
        /// Here, there is nothing more to show.
        /// </summary>
        public string NextObjectName
        {
            get { return String.Empty; }
        }


        /// <summary>
        /// Go to next object of this context.
        /// Here, there is nothing more to show.
        /// </summary>
        public void NextObject()
        {
            // nothing to implement
        }


        /// <summary>
        /// True, if the context has a next object to show.
        /// Here, there is nothing more to show.
        /// </summary>
        public bool HasPreviousObject
        {
            get { return false; }
        }


        /// <summary>
        /// The name of the previous object.
        /// Here, there is nothing more to show.
        /// </summary>
        public string PreviousObjectName
        {
            get { return String.Empty; }
        }


        /// <summary>
        /// Go to previous object of this context.
        /// Here, there is nothing more to show.
        /// </summary>
        public void PreviousObject()
        {
            // nothing to implement
        }


        /// <summary>
        /// Navigate to a specified object of this context.
        /// </summary>
        /// <param name="id">The id of the object to navigate to. Here, there is no object to nvaigate to.</param>
        public void Navigate(Address id)
        {
            // nothing to implement
        }


        /// <summary>
        /// Get all navigations objects of this context.
        /// Here, we have just one object. This is the top object (building).
        /// </summary>
        public List<NavigationItem> NavigationObjects
        {
            get
            {
                List<NavigationItem> items = new List<NavigationItem>();
                items.Add(new NavigationItem(this, _building.Id, _building.Name));
                return items;
            }
        }


        /// <summary>
        /// Called, when this context got activated. i.e. the corresponding active object is visible.
        /// </summary>
        /// <param name="id">The id of the object to navigate to.</param>
        public void OnHierarchyActivated(Address id)
        {
            // nothing to do
        }


        /// <summary>
        /// Called, when this context is deactivated. i.e. this corresponding active object is not visible.
        /// </summary>
        public void OnHierarchyDeactivated()
        {
            // nothing to do
        }


        /// <summary>
        /// Updates the context with the specified parameter.
        /// </summary>
        /// <param name="context"></param>
        public void UpdateContext(IHierarchyContext context)
        {
            // nothing to do
        }        
        

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/
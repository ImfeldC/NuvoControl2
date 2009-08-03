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
 *   File Name:      HistoryList.cs
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
    /// Stores navigation items of previously vistited views.
    /// </summary>
    public class HistoryList
    {
        #region Fields

        /// <summary>
        /// The navigation items.
        /// </summary>
        private List<NavigationItem> _history = new List<NavigationItem>();

        /// <summary>
        /// The currently shown navigation item (view).
        /// </summary>
        private int _currentIndex = -1;

        /// <summary>
        /// Maximum numbers of history items
        /// </summary>
        private const int MAX_ITEMS = 10;

        #endregion

        #region Public Interface

        /// <summary>
        /// The currently shown navigation item (view).
        /// </summary>
        public int CurrentIndex
        {
            get { return _currentIndex; }
        }


        /// <summary>
        /// Adds a navigation item to the history list.
        /// </summary>
        /// <param name="item"></param>
        public void Append(NavigationItem item)
        {
            if (_history.Count >= MAX_ITEMS)
            {
                _history.RemoveAt(0);
                _currentIndex--;
            }
            else if (_history.Count > _currentIndex + 1)
                _history.RemoveRange(_currentIndex + 1, _history.Count - (_currentIndex + 1));

            _history.Add(item);
            _currentIndex++;
        }


        /// <summary>
        /// Returns true, if browse back within the history can be done.
        /// </summary>
        public bool CanBrowseBack
        {
            get { return (_currentIndex <= 0)? false: true; }
        }


        /// <summary>
        /// Browses one item back within the history. Returns the corresponding item.
        /// </summary>
        /// <returns></returns>
        public NavigationItem BrowseBack()
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                return _history[_currentIndex];
            }
            else
                return null;
        }


        /// <summary>
        /// The name of the navigation item (view), which will be returned if BrowseBack() is called.
        /// </summary>
        public string BrowseBackName
        {
            get
            {
                if (_currentIndex > 0)
                    return _history[_currentIndex - 1].Name;
                else
                    return String.Empty;
            }
        }


        /// <summary>
        /// Returns true, if browse forward within the history can be done.
        /// </summary>
        public bool CanBrowseForward
        {
            get { return (_currentIndex < _history.Count - 1) ? true : false; }
        }


        /// <summary>
        /// Browses one item forward within the history. Returns the corresponding item.
        /// </summary>
        /// <returns></returns>
        public NavigationItem BrowseForward()
        {
            if (_currentIndex < MAX_ITEMS - 1)
            {
                _currentIndex++;
                return _history[_currentIndex];
            }
            else
                return null;
        }


        /// <summary>
        /// The name of the navigation item (view), which will be returned if BrowseForward() is called.
        /// </summary>
        public string BrowseForwardName
        {
            get
            {
                if (_currentIndex < _history.Count - 1)
                    return _history[_currentIndex + 1].Name;
                else
                    return String.Empty;
            }
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
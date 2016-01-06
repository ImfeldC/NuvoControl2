/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Client.Viewer
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.07.2009
 *   File Name:      Navigator.cs
 * 
 ***************************************************************************************************
 * 
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;

using NuvoControl.Client.Viewer.Commands;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Client.Viewer.ViewModel
{
    /// <summary>
    /// Handles navigation within the application views.
    /// </summary>
    internal class Navigator: INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// The current context of the view.
        /// </summary>
        private IHierarchyContext _context;

        /// <summary>
        /// The history list. Stores navigation items which have been visited lately.
        /// </summary>
        private HistoryList _historsList = new HistoryList();

        /// <summary>
        /// The command bindings of all navigation commands.
        /// </summary>
        private CommandBindingCollection _bindings = new CommandBindingCollection();

        /// <summary>
        /// A ListCollectionView on all views of the current hierarchy.
        /// E.g. For the floor hierarchy, there are two views: The one for the main floor and the one for the gallery.
        /// (Of course, there can be more views (floors) configured.)
        /// </summary>
        private ListCollectionView _views;

        /// <summary>
        /// Helper variable to suppress view selection  changes
        /// </summary>
        private bool _ignoreViewSelectionChange = false;

        #endregion

        #region Constructors

        /// <summary>
        /// The navigation object.
        /// </summary>
        /// <param name="context"></param>
        public Navigator(IHierarchyContext context)
        {
            this._context = context;

            CommandBinding binding = new CommandBinding(CustomCommands.BrowseNext, NextViewCommand_Executed, NextViewCommand_CanExecute);
            _bindings.Add(binding);
            binding = new CommandBinding(CustomCommands.BrowsePrevious, PreviousViewCommand_Executed, PreviousViewCommand_CanExecute);
            _bindings.Add(binding);
            binding = new CommandBinding(NavigationCommands.BrowseForward, BrowseForwardCommand_Executed, BrowseForwardCommand_CanExecute);
            _bindings.Add(binding);
            binding = new CommandBinding(NavigationCommands.BrowseBack, BrowseBackCommand_Executed, BrowseBackCommand_CanExecute);
            _bindings.Add(binding);
            binding = new CommandBinding(CustomCommands.BrowseDown, BrowseDownCommand_Executed, BrowseDownCommand_CanExecute);
            _bindings.Add(binding);
            binding = new CommandBinding(CustomCommands.BrowseUp, BrowseUpCommand_Executed, BrowseUpCommand_CanExecute);
            _bindings.Add(binding);
        }

        #endregion

        #region Public Interface


        /// <summary>
        /// The list collection view to the slectable views (navigation items) of the current hierarchy.
        /// </summary>
        public ListCollectionView NavigationItems
        {
            get
            {
                _views = (ListCollectionView)CollectionViewSource.GetDefaultView(_context.NavigationObjects);
                _views.CurrentChanged += new EventHandler(_views_CurrentChanged);
                return _views;
            }
        }


        /// <summary>
        /// The active view (specified by a navigation item).
        /// </summary>
        public NavigationItem SelectedNavigationItem
        {
            get { return new NavigationItem(_context, new Address(_context.ObjectId), _context.ObjectName); }
            set
            {
                if (_ignoreViewSelectionChange == false)
                {
                    //_context.Navigate((value as NavigationItem).Id);
                }
            }
        }


        void _views_CurrentChanged(object sender, EventArgs e)
        {
            ListCollectionView view = sender as ListCollectionView;
            if ((view != null) && (_ignoreViewSelectionChange == false))
            {
                _historsList.Append(view.CurrentItem as NavigationItem);
                _context.Navigate((view.CurrentItem as NavigationItem).Id);
            }
        }


        /// <summary>
        /// The command bindings of the naviagtion commands
        /// </summary>
        public CommandBindingCollection CommandBindingsDynamic
        {
            get { return _bindings; }
        }


        /// <summary>
        /// Tooltip for the browse back navigation command.
        /// </summary>
        public string BackToolTip
        {
            get
            {
                if (_historsList.BrowseBackName != String.Empty)
                    return NavigationCommands.BrowseBack.Text + " to " + _historsList.BrowseBackName;
                else
                    return NavigationCommands.BrowseBack.Text;
            }
        }


        /// <summary>
        /// Tooltip for the browse forward navigation command.
        /// </summary>
        public string ForwardToolTip
        {
            get
            {
                if (_historsList.BrowseForwardName != String.Empty)
                    return NavigationCommands.BrowseForward.Text + " to " + _historsList.BrowseForwardName;
                else
                    return NavigationCommands.BrowseForward.Text;
            }
        }


        /// <summary>
        /// Tooltip for the browse up navigation command.
        /// </summary>
        public string UpToolTip
        {
            get
            {
                if (_context.Parent != null)
                    return CustomCommands.BrowseUp.Text + " to " + _context.Parent.ObjectName;
                else
                    return CustomCommands.BrowseUp.Text;
            }
        }


        /// <summary>
        /// Tooltip for the browse down navigation command.
        /// </summary>
        public string DownToolTip
        {
            get
            {
                if (_context.Child != null)
                    return CustomCommands.BrowseDown.Text + " to " + _context.Child.ObjectName;
                else
                    return CustomCommands.BrowseDown.Text;
            }
        }


        /// <summary>
        /// Tooltip for the browse previous navigation command.
        /// </summary>
        public string PreviousToolTip
        {
            get
            {
                return CustomCommands.BrowsePrevious.Text + " to " + _context.PreviousObjectName;
            }
        }


        /// <summary>
        /// Tooltip for the browse next navigation command.
        /// </summary>
        public string NextToolTip
        {
            get
            {
                return CustomCommands.BrowseNext.Text + " to " + _context.NextObjectName;
            }
        }

        #endregion

        #region Navigation Command Handlers


        /// <summary>
        /// CanExecute handler for the browse forward command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseForwardCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _historsList.CanBrowseForward;
        }

        /// <summary>
        /// Execute handler for the browse forward command.
        /// Retrieves the next navigation item from the history list and navigates to it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseForwardCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ignoreViewSelectionChange = true;

            NavigationItem targetItem = _historsList.BrowseForward();
            _context.ContextVisibility = Visibility.Collapsed;
            targetItem.Context.ContextVisibility = Visibility.Visible;
            _context.OnHierarchyDeactivated();
            _context = targetItem.Context;
            _context.OnHierarchyActivated(targetItem.Id);

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
            _ignoreViewSelectionChange = false;
        }



        /// <summary>
        /// CanExecute handler for the browse back command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseBackCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _historsList.CanBrowseBack;
        }

        /// <summary>
        /// Execute handler for the browse back command.
        /// Retrieves the previous navigation item from the history list and navigates to it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseBackCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ignoreViewSelectionChange = true;

            NavigationItem targetItem = _historsList.BrowseBack();
            _context.ContextVisibility = Visibility.Collapsed;
            targetItem.Context.ContextVisibility = Visibility.Visible;
            _context.OnHierarchyDeactivated();
            _context = targetItem.Context;
            _context.OnHierarchyActivated(targetItem.Id);

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
            _ignoreViewSelectionChange = false;
        }



        /// <summary>
        /// CanExecute handler for the browse up command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseUpCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (_context.Parent != null);
        }

        /// <summary>
        /// Execute handler for the browse up command.
        /// Navigates to the upper hiearchy.
        /// E.g. from zone view to floor view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseUpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ignoreViewSelectionChange = true;

            _context.ContextVisibility = Visibility.Collapsed;
            _context.Parent.ContextVisibility = Visibility.Visible;
            _context.OnHierarchyDeactivated();
            _context = _context.Parent;
            _context.OnHierarchyActivated(e.Parameter as Address);
            //if ((e.Parameter != null) && (e.Parameter is Address))
            //    _context.Navigate(e.Parameter as Address);
            //else
            //    _context.Navigate(null);

            _historsList.Append(new NavigationItem(_context, new Address(_context.ObjectId), _context.ObjectName));
            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
            _ignoreViewSelectionChange = false;
        }



        /// <summary>
        /// CanExecute handler for the browse down command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseDownCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (_context.Child != null);
        }

        /// <summary>
        /// Execute handler for the browse down command.
        /// Navigates to the lower hiearchy.
        /// E.g. from floor view to zone view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseDownCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ignoreViewSelectionChange = true;
            _context.ContextVisibility = Visibility.Collapsed;
            _context.Child.ContextVisibility = Visibility.Visible;
            _context.OnHierarchyDeactivated();
            _context = _context.Child;
            _context.OnHierarchyActivated(e.Parameter as Address);
            //if ((e.Parameter != null) && (e.Parameter is Address))
            //    _context.Navigate(e.Parameter as Address);
            //else
            //    _context.Navigate(null);

            _historsList.Append(new NavigationItem(_context, new Address(_context.ObjectId), _context.ObjectName));
            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
            _ignoreViewSelectionChange = false;
        }



        /// <summary>
        /// CanExecute handler for the browse next command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextViewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _context.HasNextObject;
        }

        /// <summary>
        /// Execute handler for the browse next command.
        /// Navigates to the next view within the same hierarchy.
        /// E.g. from main floor to second floor (depending on the configuration).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextViewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ignoreViewSelectionChange = true;

            _context.NextObject();

            _historsList.Append(new NavigationItem(_context, new Address(_context.ObjectId), _context.ObjectName));
            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
            _ignoreViewSelectionChange = false;
        }



        /// <summary>
        /// CanExecute handler for the browse previous command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviousViewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _context.HasPreviousObject;
        }        
        
        /// <summary>
        /// Execute handler for the browse previous command.
        /// Navigates to the previous view within the same hierarchy.
        /// E.g. from second floor to main floor (depending on the configuration).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PreviousViewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ignoreViewSelectionChange = true;

            _context.PreviousObject();

            _historsList.Append(new NavigationItem(_context, new Address(_context.ObjectId), _context.ObjectName));
            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
            _ignoreViewSelectionChange = false;
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
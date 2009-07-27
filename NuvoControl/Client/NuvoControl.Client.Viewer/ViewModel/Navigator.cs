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
    public class NavigationItem
    {
        private IHierarchyContext _context;
        private Address _id;
        private string _name;

        public NavigationItem(IHierarchyContext context, Address id, string name)
        {
            this._context = context;
            this._id = id;
            this._name = name;
        }

        public IHierarchyContext Context
        {
            get { return _context; }
        }

        public Address Id
        {
            get { return _id; }
        }

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
        /// <param name="id">The navigation item to compare with.</param>
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
        /// <param name="id1">Left hand side parameter.</param>
        /// <param name="id2">Reight hand side parameter</param>
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
        /// <param name="id1">Left hand side parameter.</param>
        /// <param name="id2">Reight hand side parameter</param>
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


        public override string ToString()
        {
            return _name;
        }
    }


    internal class Navigator: INotifyPropertyChanged
    {
        private IHierarchyContext _context;
        private HistoryList _historsList = new HistoryList();
        private CommandBindingCollection _bindings = new CommandBindingCollection();
        private ListCollectionView _views;
        private bool _ignoreViewSelectionChange = false;



        public Navigator(IHierarchyContext context)
        {

            this._context = context;
            UpdateViews();
            //_historsList.Append(new NavigationItem(_context, new Address(_context.Id), _context.Name));

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

        //public List<NavigationItem> NavigationItems
        //{
        //    get
        //    {
        //        _views = (ListCollectionView)CollectionViewSource.GetDefaultView(_context.NavigationItems);
        //        _views.CurrentChanged += new EventHandler(_views_CurrentChanged);

        //        return _context.NavigationItems;
        //    }
        //}

        public ListCollectionView NavigationItems
        {
            get
            {
                _views = (ListCollectionView)CollectionViewSource.GetDefaultView(_context.NavigationItems);
                _views.CurrentChanged += new EventHandler(_views_CurrentChanged);
                return _views;
            }
        }

        public NavigationItem SelectedNavigationItem
        {
            get { return new NavigationItem(_context, new Address(_context.Id), _context.Name); }
            set
            {
                if (_ignoreViewSelectionChange == false)
                {
                    //_context.Navigate((value as NavigationItem).Id);
                }
            }
        }


        private void UpdateViews()
        {
        }


        //public List<NavigationItem> NavigationItems
        //{
        //    get
        //    {
        //        return _context.NavigationItems;
        //    }
        //}

        void _views_CurrentChanged(object sender, EventArgs e)
        {
            ListCollectionView view = sender as ListCollectionView;
            if ((view != null) && (_ignoreViewSelectionChange == false))
            {
                _historsList.Append(view.CurrentItem as NavigationItem);
                _context.Navigate((view.CurrentItem as NavigationItem).Id);
            }
        }

        public CommandBindingCollection CommandBindingsDynamic
        {
            get { return _bindings; }
        }

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

        public string UpToolTip
        {
            get
            {
                if (_context.Parent != null)
                    return CustomCommands.BrowseUp.Text + " to " + _context.Parent.Name;
                else
                    return CustomCommands.BrowseUp.Text;
            }
        }

        public string DownToolTip
        {
            get
            {
                if (_context.Child != null)
                    return CustomCommands.BrowseDown.Text + " to " + _context.Child.Name;
                else
                    return CustomCommands.BrowseDown.Text;
            }
        }

        public string PreviousToolTip
        {
            get
            {
                return CustomCommands.BrowsePrevious.Text + " to " + _context.PreviousName;
            }
        }

        public string NextToolTip
        {
            get
            {
                return CustomCommands.BrowseNext.Text + " to " + _context.NextName;
            }
        }

        private void BrowseForwardCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ignoreViewSelectionChange = true;
            NavigationItem targetItem = _historsList.BrowseForward();
            _context.Visibility1 = Visibility.Collapsed;
            targetItem.Context.Visibility1 = Visibility.Visible;
            _context.OnHierarchyDeactivated();
            _context = targetItem.Context;
            _context.OnHierarchyActivated();
            _context.Navigate(targetItem.Id);

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
            _ignoreViewSelectionChange = false;
        }

        private void BrowseForwardCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _historsList.CanBrowseForward;
        }

        private void BrowseBackCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ignoreViewSelectionChange = true;
            NavigationItem targetItem = _historsList.BrowseBack();
            _context.Visibility1 = Visibility.Collapsed;
            targetItem.Context.Visibility1 = Visibility.Visible;
            _context.OnHierarchyDeactivated();
            _context = targetItem.Context;
            _context.Navigate(targetItem.Id);
            _context.OnHierarchyActivated();

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
            _ignoreViewSelectionChange = false;
        }

        private void BrowseBackCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _historsList.CanBrowseBack;
        }

        private void BrowseUpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ignoreViewSelectionChange = true;
            _context.Visibility1 = Visibility.Collapsed;
            _context.Parent.Visibility1 = Visibility.Visible;
            _context.OnHierarchyDeactivated();
            _context = _context.Parent;
            _context.OnHierarchyActivated();
            if ((e.Parameter != null) && (e.Parameter is Address))
                _context.Navigate(e.Parameter as Address);
            else
                _context.Navigate(null);

            _historsList.Append(new NavigationItem(_context, new Address(_context.Id), _context.Name));

            UpdateViews();
            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
            _ignoreViewSelectionChange = false;
        }

        private void BrowseUpCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (_context.Parent != null);
        }

        private void BrowseDownCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ignoreViewSelectionChange = true;
            _context.Visibility1 = Visibility.Collapsed;
            _context.Child.Visibility1 = Visibility.Visible;
            _context.OnHierarchyDeactivated();
            _context = _context.Child;
            _context.OnHierarchyActivated();
            if ((e.Parameter != null) && (e.Parameter is Address))
                _context.Navigate(e.Parameter as Address);
            else
                _context.Navigate(null);

            _historsList.Append(new NavigationItem(_context, new Address(_context.Id), _context.Name));

            UpdateViews();
            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
            _ignoreViewSelectionChange = false;
        }

        private void BrowseDownCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (_context.Child != null);
        }

        private void NextViewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ignoreViewSelectionChange = true;
            _context.Next();
            //_views.MoveCurrentTo(new NavigationItem(_context, new Address(_context.Id), _context.Name));

            _historsList.Append(new NavigationItem(_context, new Address(_context.Id), _context.Name));
            UpdateViews();

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
            _ignoreViewSelectionChange = false;
        }

        private void NextViewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _context.HasNext;
        }

        private void PreviousViewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _ignoreViewSelectionChange = true;
            _context.Previous();
            //_views.MoveCurrentTo(new NavigationItem(_context, new Address(_context.Id), _context.Name));

            _historsList.Append(new NavigationItem(_context, new Address(_context.Id), _context.Name));

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
            _ignoreViewSelectionChange = false;
        }

        private void PreviousViewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _context.HasPrevious;
        }

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

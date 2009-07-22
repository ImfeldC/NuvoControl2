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



        public Navigator(IHierarchyContext context)
        {
            this._context = context;
            _historsList.Append(new NavigationItem(_context, new Address(_context.Id), _context.Name));

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

        void _views_CurrentChanged(object sender, EventArgs e)
        {
            ListCollectionView view = sender as ListCollectionView;
            if (view != null)
            {
                _context.Navigate((view.CurrentItem as NavigationItem).Id);
            }
        }

        public CommandBindingCollection CommandBindingsDynamic
        {
            get { return _bindings; }
        }

        public string BackToolTip
        {
            get { return NavigationCommands.BrowseBack.Text + " to " + _historsList.BrowseBackName; }
        }

        public string ForwardToolTip
        {
            get { return NavigationCommands.BrowseForward.Text + " to " + _historsList.BrowseForwardName; }
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
            NavigationItem targetItem = _historsList.BrowseForward();
            _context.Visibility1 = Visibility.Collapsed;
            targetItem.Context.Visibility1 = Visibility.Visible;
            _context = targetItem.Context;
            _context.Navigate(targetItem.Id);

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
        }

        private void BrowseForwardCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _historsList.CanBrowseForward;
        }

        private void BrowseBackCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            NavigationItem targetItem = _historsList.BrowseBack();
            _context.Visibility1 = Visibility.Collapsed;
            targetItem.Context.Visibility1 = Visibility.Visible;
            _context = targetItem.Context;
            _context.Navigate(targetItem.Id);

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
        }

        private void BrowseBackCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _historsList.CanBrowseBack;
        }

        private void BrowseUpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _context.Visibility1 = Visibility.Collapsed;
            _context.Parent.Visibility1 = Visibility.Visible;
            _context = _context.Parent;
            _context.OnHierarchyChanged();

            _historsList.Append(new NavigationItem(_context, new Address(_context.Id), _context.Name));

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
        }

        private void BrowseUpCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (_context.Parent != null);
        }

        private void BrowseDownCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _context.Visibility1 = Visibility.Collapsed;
            _context.Child.Visibility1 = Visibility.Visible;
            _context = _context.Child;
            _context.OnHierarchyChanged();
            if ((e.Parameter != null) && (e.Parameter is Address))
                _context.Navigate(e.Parameter as Address);

            _historsList.Append(new NavigationItem(_context, new Address(_context.Id), _context.Name));

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
        }

        private void BrowseDownCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (_context.Child != null);
        }

        private void NextViewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _context.Next();

            _historsList.Append(new NavigationItem(_context, new Address(_context.Id), _context.Name));

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
        }

        private void NextViewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _context.HasNext;
        }

        private void PreviousViewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _context.Previous();

            _historsList.Append(new NavigationItem(_context, new Address(_context.Id), _context.Name));

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
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

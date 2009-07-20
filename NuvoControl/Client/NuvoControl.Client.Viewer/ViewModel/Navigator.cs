using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;

using NuvoControl.Client.Viewer.Commands;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Client.Viewer.ViewModel
{
    internal class Navigator: INotifyPropertyChanged
    {
        private IHierarchyContext _context;
        private HistoryList _historsList = new HistoryList();
        private CommandBindingCollection _bindings = new CommandBindingCollection();


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

        public CommandBindingCollection CommandBindingsDynamic
        {
            get { return _bindings; }
        }

        public string BackToolTip
        {
            get { return NavigationCommands.BrowseBack.Text + " to " + "Viewname"; }
        }

        public string ForwardToolTip
        {
            get { return NavigationCommands.BrowseForward.Text + " to " + "Viewname"; }
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
            HistoryList.HistoryListItem targetItem = _historsList.BrowseForward();
            _historsList.Append(new HistoryList.HistoryListItem(_context, new Address(_context.Id)));

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
        }

        private void BrowseForwardCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _historsList.CanBrowseForward;
        }

        private void BrowseBackCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            HistoryList.HistoryListItem targetItem = _historsList.BrowseBack();
            _historsList.Append(new HistoryList.HistoryListItem(_context, new Address(_context.Id)));

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

            _historsList.Append(new HistoryList.HistoryListItem(_context, new Address(_context.Id)));

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

            _historsList.Append(new HistoryList.HistoryListItem(_context, new Address(_context.Id)));

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
        }

        private void BrowseDownCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (_context.Child != null);
        }

        private void NextViewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _context.Next();

            _historsList.Append(new HistoryList.HistoryListItem(_context, new Address(_context.Id)));

            NotifyPropertyChanged(new PropertyChangedEventArgs(""));
        }

        private void NextViewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _context.HasNext;
        }

        private void PreviousViewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _context.Previous();

            _historsList.Append(new HistoryList.HistoryListItem(_context, new Address(_context.Id)));

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

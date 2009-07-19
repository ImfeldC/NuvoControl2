using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

using NuvoControl.Client.Viewer.Commands;

namespace NuvoControl.Client.Viewer.ViewModel
{
    internal class Navigator
    {
        private IHierarchyContext _context;

        CommandBindingCollection _bindings = new CommandBindingCollection();


        public Navigator(IHierarchyContext context)
        {
            this._context = context;

            CommandBinding binding = new CommandBinding(NavigationCommands.NextPage, NextViewCommand_Executed, NextViewCommand_CanExecute);
            _bindings.Add(binding);
            binding = new CommandBinding(NavigationCommands.PreviousPage, PreviousViewCommand_Executed, PreviousViewCommand_CanExecute);
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

        private void BrowseForwardCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void BrowseForwardCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void BrowseBackCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void BrowseBackCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void BrowseUpCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _context.Visibility1 = Visibility.Collapsed;
            _context.Parent.Visibility1 = Visibility.Visible;
            _context = _context.Parent;
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
        }

        private void BrowseDownCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (_context.Child != null);
        }

        private void NextViewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _context.Next();
        }

        private void NextViewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _context.HasNext;
        }

        private void PreviousViewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _context.Previous();

        }

        private void PreviousViewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _context.HasPrevious;
        }
    }
}

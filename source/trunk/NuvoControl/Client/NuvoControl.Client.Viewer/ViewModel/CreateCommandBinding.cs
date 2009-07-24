using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

using NuvoControl.Client.Viewer.Commands;

namespace NuvoControl.Client.Viewer.Commands
{
    /// <summary>
    /// Attached property that can be used to create a binding for a CommandModel. Set the
    /// CreateCommandBinding.Command property to a CommandModel.
    /// </summary>
    public static class CreateCommandBinding
    {
        public static readonly DependencyProperty CommandProperty
           = DependencyProperty.RegisterAttached("Command", typeof(CommandBinding), typeof(CreateCommandBinding),
                new PropertyMetadata(new PropertyChangedCallback(OnCommandInvalidated)));

        public static RoutedUICommand GetCommand(DependencyObject sender)
        {
            return (RoutedUICommand)sender.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject sender, CommandBinding commandBinding)
        {
            sender.SetValue(CommandProperty, commandBinding);
        }

        /// <summary>
        /// Callback when the Command property is set or changed.
        /// </summary>
        private static void OnCommandInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            // Clear the exisiting bindings on the element we are attached to.
            UIElement element = (UIElement)dependencyObject;
            //element.CommandBindings.Clear();

            // If we're given a command model, set up a binding
            CommandBinding commandBinding = e.NewValue as CommandBinding;
            if (commandBinding != null)
            {
                element.CommandBindings.Add(commandBinding);
            }

            // Suggest to WPF to refresh commands
            CommandManager.InvalidateRequerySuggested();
        }

    }


    public static class TestDepProp
    {
        public static readonly DependencyProperty CommandProperty
           = DependencyProperty.RegisterAttached("TestDep", typeof(string), typeof(TestDepProp),
                new PropertyMetadata(new PropertyChangedCallback(OnTestDepInvalidated)));

        public static string GetTestDep(DependencyObject sender)
        {
            return (string)sender.GetValue(CommandProperty);
        }

        public static void SetTestDep(DependencyObject sender, string command)
        {
            sender.SetValue(CommandProperty, command);
        }

        /// <summary>
        /// Callback when the Command property is set or changed.
        /// </summary>
        private static void OnTestDepInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            // Clear the exisiting bindings on the element we are attached to.
            UIElement element = (UIElement)dependencyObject;
            //element.CommandBindings.Clear();

            // If we're given a command model, set up a binding
            string command = e.NewValue as string;
            //if (commandBinding != null)
            //{
            //    element.CommandBindings.Add(commandBinding);
            //}

            // Suggest to WPF to refresh commands
            CommandManager.InvalidateRequerySuggested();
        }

    }


}

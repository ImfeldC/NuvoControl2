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
    public static class CreateCommandBindingCollection
    {
        public static readonly DependencyProperty CommandBindingsDynamicProperty
           = DependencyProperty.RegisterAttached("CommandBindingsDynamic", typeof(CommandBindingCollection), typeof(CreateCommandBindingCollection),
                new PropertyMetadata(new PropertyChangedCallback(OnCommandBindingsInvalidated)));

        public static RoutedUICommand GetCommandBindingsDynamic(DependencyObject sender)
        {
            return (RoutedUICommand)sender.GetValue(CommandBindingsDynamicProperty);
        }

        public static void SetCommandBindingsDynamic(DependencyObject sender, CommandBindingCollection commandBindings)
        {
            sender.SetValue(CommandBindingsDynamicProperty, commandBindings);
        }

        /// <summary>
        /// Callback when the Command property is set or changed.
        /// </summary>
        private static void OnCommandBindingsInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement)dependencyObject;

            // If we're given a command model, set up a binding
            CommandBindingCollection commandBindings = e.NewValue as CommandBindingCollection;
            if (commandBindings != null)
            {
                foreach (CommandBinding binding in commandBindings)
                {
                    element.CommandBindings.Add(binding);
                }
            }

            // Suggest to WPF to refresh commands
            CommandManager.InvalidateRequerySuggested();
        }

    }

}

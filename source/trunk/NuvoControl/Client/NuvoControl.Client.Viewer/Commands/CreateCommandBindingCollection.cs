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
 *   File Name:      CreateCommandBindingCollection.cs
 * 
 ***************************************************************************************************
 * 
 * 
 **************************************************************************************************/

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
    /// An attached property.
    /// 
    /// It is used, to store command bindings dynamically within the commandbindings collection of UIElements.
    /// This allows defining command bindings (with their handlers) in view model classes.
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
        /// Callback when the command bindings collection property is set or changed.
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


/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/
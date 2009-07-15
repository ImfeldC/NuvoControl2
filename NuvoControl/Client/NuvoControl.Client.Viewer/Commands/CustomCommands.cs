using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Input;

namespace NuvoControl.Client.Viewer.Commands
{
    public class CustomCommands
    {
        private static RoutedUICommand _browseDown;
        private static RoutedUICommand _browseUp;

        static CustomCommands()
        {
            InputGestureCollection inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.D, ModifierKeys.Control, "Ctrl+D"));
            _browseDown = new RoutedUICommand("Browse Down", "BrowseDown", typeof(CustomCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.U, ModifierKeys.Control, "Ctrl+U"));
            _browseUp = new RoutedUICommand("Browse Up", "BrowseUp", typeof(CustomCommands), inputs);

        }

        public static RoutedUICommand BrowseDown
        {
            get { return _browseDown; }
        }

        public static RoutedUICommand BrowseUp
        {
            get { return _browseUp; }
        }
    }
}

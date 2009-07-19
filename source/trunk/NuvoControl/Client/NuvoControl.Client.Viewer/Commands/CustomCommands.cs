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
        private static RoutedUICommand _volumeDown;
        private static RoutedUICommand _volumeUp;

        private static RoutedUICommand _test;


        static CustomCommands()
        {
            InputGestureCollection inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.D, ModifierKeys.Control, "Ctrl+D"));
            _browseDown = new RoutedUICommand("Browse Down", "BrowseDown", typeof(CustomCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.U, ModifierKeys.Control, "Ctrl+U"));
            _browseUp = new RoutedUICommand("Browse Up", "BrowseUp", typeof(CustomCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.D, ModifierKeys.Control, "Ctrl+D"));
            _volumeDown = new RoutedUICommand("Volume Down", "VolumeDown", typeof(CustomCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.U, ModifierKeys.Control, "Ctrl+U"));
            _volumeUp = new RoutedUICommand("Volunme Up", "VolumeUp", typeof(CustomCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.U, ModifierKeys.Control, "Ctrl+U"));
            _test = new RoutedUICommand("Test", "Test", typeof(CustomCommands), inputs);


        }

        public static RoutedUICommand BrowseDown
        {
            get { return _browseDown; }
        }

        public static RoutedUICommand BrowseUp
        {
            get { return _browseUp; }
        }

        public static RoutedUICommand VolumeDown
        {
            get { return _volumeDown; }
        }

        public static RoutedUICommand VolumeUp
        {
            get { return _volumeUp; }
        }

        public static RoutedUICommand Test
        {
            get { return _test; }
        }
    }


    //public class RoutedUICommandEx : RoutedUICommand
    //{
    //    public RoutedUICommandEx(string text, string name, Type ownerType, InputGestureCollection inputs)
    //        : base(text, name, ownerType, inputs)
    //    {

    //    }

    //    public void ExecutedEx(object sender, ExecutedRoutedEventArgs e)
    //    {
    //    }


    //    public void CanExecuteEx(object sender, CanExecuteRoutedEventArgs e)
    //    {
    //        e.CanExecute = true;
    //    }

    //}

}

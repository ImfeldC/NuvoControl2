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
        private static RoutedUICommand _browsePrevious;
        private static RoutedUICommand _browseNext;

        private static RoutedUICommand _volumeDown;
        private static RoutedUICommand _volumeUp;
        private static RoutedUICommand _power;

        private static RoutedUICommand _appExit;


        static CustomCommands()
        {
            InputGestureCollection inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.Down, ModifierKeys.Control, "Ctrl+Down"));
            _browseDown = new RoutedUICommand("Down", "BrowseDown", typeof(CustomCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.Up, ModifierKeys.Control, "Ctrl+Up"));
            _browseUp = new RoutedUICommand("Up", "BrowseUp", typeof(CustomCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.Left, ModifierKeys.Control, "Ctrl+Left"));
            _browsePrevious = new RoutedUICommand("Previous", "BrowsePrevious", typeof(CustomCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.Right, ModifierKeys.Control, "Ctrl+Right"));
            _browseNext = new RoutedUICommand("Next", "BrowseNext", typeof(CustomCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.D, ModifierKeys.Control, "Ctrl+D"));
            _volumeDown = new RoutedUICommand("Volume Down", "VolumeDown", typeof(CustomCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.U, ModifierKeys.Control, "Ctrl+U"));
            _volumeUp = new RoutedUICommand("Volunme Up", "VolumeUp", typeof(CustomCommands), inputs);

            inputs = new InputGestureCollection();
            inputs.Add(new KeyGesture(Key.M, ModifierKeys.Control, "Ctrl+M"));
            _power = new RoutedUICommand("Power", "Power", typeof(CustomCommands), inputs);

            _appExit = new RoutedUICommand("Exit", "Exit", typeof(CustomCommands), null);


        }

        public static RoutedUICommand BrowseDown
        {
            get { return _browseDown; }
        }

        public static RoutedUICommand BrowseUp
        {
            get { return _browseUp; }
        }

        public static RoutedUICommand BrowsePrevious
        {
            get { return _browsePrevious; }
        }

        public static RoutedUICommand BrowseNext
        {
            get { return _browseNext; }
        }

        public static RoutedUICommand VolumeDown
        {
            get { return _volumeDown; }
        }

        public static RoutedUICommand VolumeUp
        {
            get { return _volumeUp; }
        }

        public static RoutedUICommand Power
        {
            get { return _power; }
        }

        public static RoutedUICommand AppExit
        {
            get { return _appExit; }
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

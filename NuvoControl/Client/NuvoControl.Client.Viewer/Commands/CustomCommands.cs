/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Client.Viewer
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.07.2009
 *   File Name:      CustomCommands.cs
 * 
 ***************************************************************************************************
 * 
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Input;

namespace NuvoControl.Client.Viewer.Commands
{
    /// <summary>
    /// This class defines commands, used in various places within the application.
    /// E.g. Navigation commands, Zone commands.
    /// All commands support shortcuts.
    /// </summary>
    public class CustomCommands
    {
        #region Fields

        /// <summary>
        /// Command, to browse down within the viewer hierarchies.
        /// E.g. from the floor view to a zone view.
        /// </summary>
        private static RoutedUICommand _browseDown;

        /// <summary>
        /// Command, to browse up within the viewer hierarchies.
        /// E.g. from a zone view to its floor view.
        /// </summary>
        private static RoutedUICommand _browseUp;

        /// <summary>
        /// Command, to browse to the next view within the same hierarchy.
        /// E.g. from a floor view to the next floor view..
        /// </summary>
        private static RoutedUICommand _browsePrevious;

        /// <summary>
        /// Command, to browse to the prvious view within the same hierarchy.
        /// E.g. from a floor view to the prvious floor view..
        /// </summary>
        private static RoutedUICommand _browseNext;

        /// <summary>
        /// Command, used to control decreasing the audio volume.
        /// </summary>
        private static RoutedUICommand _volumeDown;

        /// <summary>
        /// Command, used to control increasing the audio volume.
        /// </summary>
        private static RoutedUICommand _volumeUp;

        /// <summary>
        /// Command, used to control the enabling/disabling the audio source
        /// </summary>
        private static RoutedUICommand _power;

        /// <summary>
        /// Application exit command
        /// </summary>
        private static RoutedUICommand _appExit;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
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

        #endregion

        #region Public Interface

        /// <summary>
        /// Command, to browse down within the viewer hierarchies.
        /// E.g. from the floor view to a zone view.
        /// </summary>
        public static RoutedUICommand BrowseDown
        {
            get { return _browseDown; }
        }


        /// <summary>
        /// Command, to browse up within the viewer hierarchies.
        /// E.g. from a zone view to its floor view.
        /// </summary>
        public static RoutedUICommand BrowseUp
        {
            get { return _browseUp; }
        }


        /// <summary>
        /// Command, to browse to the next view within the same hierarchy.
        /// E.g. from a floor view to the next floor view..
        /// </summary>
        public static RoutedUICommand BrowsePrevious
        {
            get { return _browsePrevious; }
        }


        /// <summary>
        /// Command, to browse to the prvious view within the same hierarchy.
        /// E.g. from a floor view to the prvious floor view..
        /// </summary>
        public static RoutedUICommand BrowseNext
        {
            get { return _browseNext; }
        }


        /// <summary>
        /// Command, used to control decreasing the audio volume.
        /// </summary>
        public static RoutedUICommand VolumeDown
        {
            get { return _volumeDown; }
        }


        /// <summary>
        /// Command, used to control increasing the audio volume.
        /// </summary>
        public static RoutedUICommand VolumeUp
        {
            get { return _volumeUp; }
        }


        /// <summary>
        /// Command, used to control the enabling/disabling the audio source
        /// </summary>
        public static RoutedUICommand Power
        {
            get { return _power; }
        }


        /// <summary>
        /// Application exit command
        /// </summary>
        public static RoutedUICommand AppExit
        {
            get { return _appExit; }
        }

        #endregion

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
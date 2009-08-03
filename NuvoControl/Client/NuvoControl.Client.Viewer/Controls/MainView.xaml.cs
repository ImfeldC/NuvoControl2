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
 *   File Name:      MainView.cs
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
using System.Windows.Controls;

using NuvoControl.Client.Viewer.Commands;

namespace NuvoControl.Client.Viewer.Controls
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// This view is the top view of the application.
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Left mouse button up handler. Triggers the 'Browse down' command (Browse down to the floor view).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void _imageBuilding_OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            if (CustomCommands.BrowseDown.CanExecute(null, null))
                CustomCommands.BrowseDown.Execute(null, null);
        }
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
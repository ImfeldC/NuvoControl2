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
 *   File Name:      ZoneControl.cs
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
using NuvoControl.Client.Viewer.ViewModel;

namespace NuvoControl.Client.Viewer.Controls
{
    /// <summary>
    /// Interaction logic for ZoneControl.xaml
    /// This is the zone control instantiated per zone in the floor view.
    /// </summary>
    public partial class ZoneControl : UserControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ZoneControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Button click handler. Opens the zone commander popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnOpenCommander_Click(object sender, RoutedEventArgs e)
        {
            _popupCommander.IsOpen = true;
        }


        /// <summary>
        /// Button click handler. Closes the zone commander popup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnCloseCommander_Click(object sender, RoutedEventArgs e)
        {
            if ((string)((FrameworkElement)e.OriginalSource).Tag == "ClosePopupButton")
            {
                _popupCommander.IsOpen = false;
            }
        }


        /// <summary>
        /// Left mouse button up handler. Triggers the 'Browse down' command (Browse down to the zone view).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void _polygonArea_OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            ZoneContext zoneContext = this.DataContext as ZoneContext;
            if ((zoneContext != null) && ((string)(e.Source as FrameworkElement).Tag == "Zone"))
            {
                if (CustomCommands.BrowseDown.CanExecute(zoneContext.ObjectId, null))
                    CustomCommands.BrowseDown.Execute(zoneContext.ObjectId, null);
            }
        }
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
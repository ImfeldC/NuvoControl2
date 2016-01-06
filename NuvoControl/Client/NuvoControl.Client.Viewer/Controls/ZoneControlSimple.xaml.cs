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
 *   File Name:      ZoneControlSimple.cs
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

namespace NuvoControl.Client.Viewer.Controls
{
    /// <summary>
    /// Interaction logic for ZoneControlSimple.xaml
    /// This is the zone control instantiated in the zone view.
    /// </summary>
    public partial class ZoneControlSimple : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ZoneControlSimple()
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
        /// Button click handler. Closes the zone commander popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnCloseCommander_Click(object sender, RoutedEventArgs e)
        {
            if ((string)((FrameworkElement)e.OriginalSource).Tag == "ClosePopupButton")
                _popupCommander.IsOpen = false;
        }
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/
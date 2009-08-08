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
 *   File Name:      MainWindow.cs
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using NuvoControl.Client.Viewer.Controls;
using NuvoControl.Client.Viewer.ViewModel;
using NuvoControl.Common.Configuration;
using NuvoControl.Common;
using NuvoControl.Client.Viewer.ServiceAccess;
using NuvoControl.Client.ServiceAccess;

namespace NuvoControl.Client.Viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Startup code.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        /// <summary>
        /// The main/top view.
        /// </summary>
        private MainView _mainView = new MainView();

        /// <summary>
        /// The floor view.
        /// </summary>
        private FloorView _floorView = new FloorView();

        /// <summary>
        /// The zone view.
        /// </summary>
        private ZoneView _zoneView = new ZoneView();

        /// <summary>
        /// The graphic configuration
        /// </summary>
        private Graphic _graphic;

        /// <summary>
        /// The navigation object.
        /// </summary>
        private Navigator _navigator;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            ServiceConfigurator.Configure(false);
            ReadConfiguration();
            InitializeViews();
            InitializeViewModel();
        }

        #endregion

        #region Non-Public Interface


        /// <summary>
        /// Reads the configuration from the service.
        /// </summary>
        private void ReadConfiguration()
        {
            _graphic = ServiceProxy.ConfigurationProxy.GetGraphicConfiguration();
        }


        /// <summary>
        /// Initializes all view.
        /// </summary>
        private void InitializeViews()
        {
            _mainGrid.Children.Add(_floorView);
            _mainGrid.Children.Add(_mainView);
            _mainGrid.Children.Add(_zoneView);
        }


        /// <summary>
        /// Initializes the view model.
        /// </summary>
        private void InitializeViewModel()
        {
            MainContext mainContext = new MainContext(_graphic.Building);
            _mainView.DataContext = mainContext;

            FloorContext floorContext = new FloorContext(_graphic.Building.Floors, _graphic.Sources, _floorView);
            floorContext.Parent = mainContext;
            mainContext.Child = floorContext;
            _floorView.DataContext = floorContext;

            ZoneContext zoneContext = new ZoneContext(_graphic.Building.Floors[0].Zones, _graphic.Sources);
            zoneContext.Parent = floorContext;
            floorContext.Child = zoneContext;
            _zoneView.DataContext = zoneContext;

            _navigator = new Navigator(mainContext);
            this.DataContext = _navigator;

            _mainView.Focus();
            _floorView.Focus();
            _zoneView.Focus();

        }


        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            ServiceProxy.MonitorAndControlProxy.Dispose();
            ServiceProxy.ConfigurationProxy.Dispose();
            base.OnClosing(e);
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
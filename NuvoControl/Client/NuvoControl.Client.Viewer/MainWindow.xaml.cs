﻿/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
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
using System.Threading;
using System.Diagnostics;
using System.Drawing;

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
using Common.Logging;
using System.IO;

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
        /// Trace logger
        /// </summary>
        private static ILog _log = LogManager.GetCurrentClassLogger();

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

            //MessageBox.Show("Starting up the client...", "NuvoControl");
            StartupWindow startupWindow = new StartupWindow();
            startupWindow.Show();
            Thread.Sleep(1000);

            System.Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            _log.Trace(m => m("*********** NuvoControl Viewer started *********** (Version={0})", version.ToString() ));

            bool serviceDiscovered = false;
            try
            {
                DiscoverServices();
                serviceDiscovered = true;
            }
            catch (Exception exc)
            {
                StringBuilder message = new StringBuilder("Failed to start up the viewer.\nThe viewer could not discover the service.\n\n");
                message.Append("\n\nException message:\n");
                message.Append(exc.Message);

                _log.Fatal(message.ToString());

                MessageBox.Show(message.ToString(), "Nuvo Control Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (serviceDiscovered && ServiceProxy.ServiceDiscovery.DiscoveredServers.Count > 0)
            {
                _textServerName.Text = "";

                // Select server in case more than one service is available ...
                if (ServiceProxy.ServiceDiscovery.DiscoveredServers.Count > 1)
                {
                    // Display Server Dialog to select server to connect
                    ServerWindow serverWindow = new ServerWindow();
                    serverWindow.ShowDialog();
                    _textServerName.Text = serverWindow._ctrlServerConnect.SelectedServer;
                }
                else
                {
                    _textServerName.Text = ServiceProxy.ServerName;
                }
                _textServerName.LostFocus += new RoutedEventHandler(_textServerName_LostFocus);

                ServiceProxy.ServiceDiscovery.SelectedServer = _textServerName.Text;

                if (_textServerName.Text.Length>0)
                {
                    bool configRead = false;
                    try
                    {
                        ServiceConfigurator.Configure(Properties.Settings.Default.TestMode);   // set true in case of 'test' mode
                        ReadConfiguration();
                        configRead = true;
                    }
                    catch (Exception exc)
                    {
                        StringBuilder message = new StringBuilder("Failed to start up the viewer.\nThe viewer could not retrieve the configuration from the service.\n\n");
                        message.Append("Shut down the viewer and check following points:");
                        message.Append("\n- The service is running.");
                        message.Append("\n- The configured service address and port is proper.");
                        message.Append("\n- The configured viewer IP or name is proper.");
                        message.Append("\n\nException message:\n");
                        message.Append(exc.Message);

                        _log.Fatal(message.ToString());

                        MessageBox.Show(message.ToString(), "Nuvo Control Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    if (configRead && serviceDiscovered)
                    {
                        InitializeViews();
                        InitializeViewModel();
                    }
                }
            }

            startupWindow.Close();
        }

        /// <summary>
        /// Lost focus handler for server name text field. It stores the new server name.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _textServerName_LostFocus(object sender, RoutedEventArgs e)
        {
            ServiceProxy.ServerName = _textServerName.Text;
            _log.Trace(m => m("Set server name to {0}. Set client name to {1}", ServiceProxy.ServerName, ServiceProxy.ClientIpOrName));
        }

        #endregion

        #region Non-Public Interface

        /// <summary>
        /// Method to discover the available services in the network.
        /// </summary>
        private void DiscoverServices()
        {
            _log.Debug(m => m("MainWindow.DiscoverServices() ..."));
            ServiceProxy.DiscoverServices();

            _log.Debug(m => m("MainWindow.DiscoverServices() {0} server found ...", ServiceProxy.ServiceDiscovery.DiscoveredServers.Count));
        }


        /// <summary>
        /// Reads the configuration from the service.
        /// </summary>
        private void ReadConfiguration()
        {
            _log.Debug(m => m("MainWindow.ReadConfiguration() ..."));
            _graphic = ServiceProxy.ConfigurationProxy.GetGraphicConfiguration();

            ReadImages();
        }

        /// <summary>
        /// Private method to read all images specified in the configuration
        /// from configuration service.
        /// </summary>
        private void ReadImages()
        {
            _log.Debug(m => m("MainWindow.ReadImages() ..."));
            if (_graphic != null)
            {
                ReadImage(_graphic.Building.PicturePath);
                foreach (SourceGraphic src in _graphic.Sources)
                {
                    //ReadImage(src.PicturePath);
                }
                foreach (Floor floor in _graphic.Building.Floors)
                {
                    ReadImage(floor.FloorPlanPath);
                    foreach (ZoneGraphic zone in floor.Zones)
                    {
                        ReadImage(zone.PicturePath);
                    }
                }
            }
        }

        /// <summary>
        /// Private method to read an image from configuration service.
        /// </summary>
        /// <param name="picturePath">Image to read.</param>
        private void ReadImage(string picturePath)
        {
            NuvoImage image;
            string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), picturePath);
            _log.Debug(m => m("MainWindow.ReadImage( {0} ) ...", picturePath));
            try
            {
                image = ServiceProxy.ConfigurationProxy.GetImage(picturePath);
                _log.Debug(m => m("MainWindow.ReadImage( {0} ) ... loaded with Size={1}", picturePath, image.Picture.Size.ToString()));
                image.Picture.Save(path);
                _log.Debug(m => m("MainWindow.ReadImage( {0} ) ... saved to {1}", picturePath, path));
            }
            catch (Exception exc)
            {
                _log.Error(m => m("MainWindow.ReadImage( {0} ) ... exception to load/save image! Exception={1}", picturePath, exc.ToString()));
            }
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


        /// <summary>
        /// Handler for Classic style selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnMenuClassic_Checked(object sender, RoutedEventArgs e)
        {
            LoadNewSkin(@"../Skins/NuvoControlStylesClassic.xaml");
            if (_btnMenuSmooth != null)
                _btnMenuSmooth.IsChecked = false;
            if (_btnMenuSteel != null)
                _btnMenuSteel.IsChecked = false;
            if (_btnMenuTechno != null)
                _btnMenuTechno.IsChecked = false;
            if (_btnMenuFreak != null)
                _btnMenuFreak.IsChecked = false;
        }


        /// <summary>
        /// Handler for Smooth style selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnMenuSmooth_Checked(object sender, RoutedEventArgs e)
        {
            LoadNewSkin(@"../Skins/NuvoControlStylesSmooth.xaml");
            if (_btnMenuClassic != null)
                _btnMenuClassic.IsChecked = false;
            if (_btnMenuSteel != null)
                _btnMenuSteel.IsChecked = false;
            if (_btnMenuTechno != null)
                _btnMenuTechno.IsChecked = false;
            if (_btnMenuFreak != null)
                _btnMenuFreak.IsChecked = false;
        }


        /// <summary>
        /// Handler for Techno style selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnMenuTechno_Checked(object sender, RoutedEventArgs e)
        {
            LoadNewSkin(@"../Skins/NuvoControlStylesTechno.xaml");
            if (_btnMenuClassic != null)
                _btnMenuClassic.IsChecked = false;
            if (_btnMenuSmooth != null)
                _btnMenuSmooth.IsChecked = false;
            if (_btnMenuSteel != null)
                _btnMenuSteel.IsChecked = false;
            if (_btnMenuFreak != null)
                _btnMenuFreak.IsChecked = false;
        }


        /// <summary>
        /// Handler for Steel style selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnMenuSteel_Checked(object sender, RoutedEventArgs e)
        {
            LoadNewSkin(@"../Skins/NuvoControlStylesSteel.xaml");
            if (_btnMenuClassic != null)
                _btnMenuClassic.IsChecked = false;
            if (_btnMenuSmooth != null)
                _btnMenuSmooth.IsChecked = false;
            if (_btnMenuTechno != null)
                _btnMenuTechno.IsChecked = false;
            if (_btnMenuFreak != null)
                _btnMenuFreak.IsChecked = false;
        }


        /// <summary>
        /// Handler for Freak style selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnMenuFreak_Checked(object sender, RoutedEventArgs e)
        {
            LoadNewSkin(@"../Skins/NuvoControlStylesFreak.xaml");
            if (_btnMenuClassic != null)
                _btnMenuClassic.IsChecked = false;
            if (_btnMenuSmooth != null)
                _btnMenuSmooth.IsChecked = false;
            if (_btnMenuSteel != null)
                _btnMenuSteel.IsChecked = false;
            if (_btnMenuTechno != null)
                _btnMenuTechno.IsChecked = false;
        }

        /// <summary>
        /// Laods the new skin into the resources.
        /// </summary>
        /// <param name="relativeSkinName"></param>
        private void LoadNewSkin(string relativeSkinName)
        {
            ResourceDictionary newDictionary = new ResourceDictionary();
            newDictionary.Source = new Uri(relativeSkinName, UriKind.Relative);
            this.Resources.MergedDictionaries[0] = newDictionary;
        }


        /// <summary>
        /// Handler for About Box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _btnAboutBox_Click(object sender, RoutedEventArgs e)
        {
            // Uses a constructor that takes a parent window for the AboutBox.
            AboutBox about = new AboutBox(this);
            about.ShowDialog();
        }


        /// <summary>
        /// The window is about to close. Disposes the service proxies.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                ServiceProxy.Dispose();
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception in OnClosing: " + exc.Message);
            }
            base.OnClosing(e);
        }

        #endregion

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/
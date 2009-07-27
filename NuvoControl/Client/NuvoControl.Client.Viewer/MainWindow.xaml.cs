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
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainView _mainView = new MainView();
        private FloorView _floorView = new FloorView();
        private ZoneView _zoneView = new ZoneView();

        private Graphic _graphic;
        private Navigator _navigator;

        public MainWindow()
        {
            InitializeComponent();

            ServiceConfigurator.Configure(false);
            ReadConfiguration();
            InitializeViews();
            InitializeViewModel();
        }


        private void ReadConfiguration()
        {
            _graphic = ServiceProxy.ConfigurationProxy.GetGraphicConfiguration();
        }


        private void InitializeViews()
        {
            _mainGrid.Children.Add(_floorView);
//            _floorView.Visibility = Visibility.Collapsed;

            _mainGrid.Children.Add(_mainView);
 //           _mainView.Visibility = Visibility.Visible;

            _mainGrid.Children.Add(_zoneView);
  //          _zoneView.Visibility = Visibility.Collapsed;


        }


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
            _zoneView.Focus();

        }


        /*
                private void BrowseDownCommand_Executed(object sender, ExecutedRoutedEventArgs e)
                {
                    _mainView.Visibility = Visibility.Collapsed;
                    _floorView.Visibility = Visibility.Visible;


                    _floorView.CreateZone(_graphic.Building.Floors[0].Zones[0], _graphic.Sources);
                    _floorView.CreateZone(_graphic.Building.Floors[0].Zones[1], _graphic.Sources);
                    _floorView.CreateZone(_graphic.Building.Floors[0].Zones[2], _graphic.Sources);
                    _floorView.CreateZone(_graphic.Building.Floors[0].Zones[3], _graphic.Sources);
                    _floorView.CreateZone(_graphic.Building.Floors[0].Zones[4], _graphic.Sources);
                    _floorView.CreateZone(_graphic.Building.Floors[0].Zones[5], _graphic.Sources);
                    _floorView.CreateZone(_graphic.Building.Floors[0].Zones[6], _graphic.Sources);
                    _floorView.CreateZone(_graphic.Building.Floors[0].Zones[7], _graphic.Sources);
                    _floorView.CreateZone(_graphic.Building.Floors[0].Zones[8], _graphic.Sources);

                }


                private void BrowseDownCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
                {
                    e.CanExecute = true;
                }


                private void _btnFwd_Click(object sender, RoutedEventArgs e)
                {
                    //NavigationService nav = NavigationService.GetNavigationService((Page)frame1.Content);
                    //Page page = new PageZones();
                    //page.KeepAlive = true;
                    //page.Name = "PageZones" + counter.ToString();
                    //counter++;

                    //nav.Navigate(page);

                    //MainView mainView = new MainView();
                    //_mainGrid.Children.Add(mainView);
                    //Grid.SetColumn(mainView, 0);
                    //Grid.SetRow(mainView, 0);



                }


                private void _btnBack_Click(object sender, RoutedEventArgs e)
                {
                    _mainView.Visibility = Visibility.Collapsed;

                    ZoneView zoneView = new ZoneView();
                    _mainGrid.Children.Add(zoneView);
                    Grid.SetColumn(zoneView, 0);
                    Grid.SetRow(zoneView, 0);

                }
        */

        private void MouseUp_Event(object sender, RoutedEventArgs e)
        {

        }

        private void NewExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void NewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


    }
}

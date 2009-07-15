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

using NuvoControl.Client.Viewer.Pages;
using NuvoControl.Client.Viewer.Controls;

namespace NuvoControl.Client.Viewer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainView _mainView = new MainView();

        public MainWindow()
        {
            InitializeComponent();


            _mainView.Source = new BitmapImage(new Uri(@"C:\Diplomarbeit\SVN\NuvoControl_Trunk\NuvoControl\Client\NuvoControl.Client.Viewer\bin\Debug\MainView.jpg"));
            _mainGrid.Children.Add(_mainView);
            Grid.SetColumn(_mainView, 0);
            Grid.SetRow(_mainView, 0);
        }


        private void BrowseBackCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }


        private void BrowseBackCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        private void BrowseDownCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _mainView.Visibility = Visibility.Collapsed;

            FloorView floorView = new FloorView();
            floorView.Source = new BitmapImage(new Uri(@"C:\Diplomarbeit\SVN\NuvoControl_Trunk\NuvoControl\Client\NuvoControl.Client.Viewer\bin\Debug\Floor1.bmp"));
            _mainGrid.Children.Add(floorView);
            Grid.SetColumn(floorView, 0);
            Grid.SetRow(floorView, 0);

            floorView.CreateZone();

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


        private void _btnPrev_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _btnBack_Click(object sender, RoutedEventArgs e)
        {
            _mainView.Visibility = Visibility.Collapsed;

            ZoneView zoneView = new ZoneView();
            _mainGrid.Children.Add(zoneView);
            Grid.SetColumn(zoneView, 0);
            Grid.SetRow(zoneView, 0);

        }

        private void _btnUp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _btnDown_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _btnNext_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

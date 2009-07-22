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

using NuvoControl.Client.Viewer.Commands;

namespace NuvoControl.Client.Viewer.Controls
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }


        /// <summary>
        /// TODO remove:
        /// </summary>
        public ImageSource Source2
        {
            set { this._imageBuilding.Source = value; }
        }

        public void _imageBuilding_OnMouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            if (CustomCommands.BrowseDown.CanExecute(null, null))
                CustomCommands.BrowseDown.Execute(null, null);
        }
    }
}

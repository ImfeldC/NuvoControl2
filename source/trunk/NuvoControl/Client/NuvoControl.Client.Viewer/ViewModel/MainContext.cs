using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows;
using System.IO;

namespace NuvoControl.Client.Viewer.ViewModel
{
    public class MainContext : INotifyPropertyChanged, IHierarchyContext
    {
        private string _imagePath;
        private IHierarchyContext _parent = null;
        private IHierarchyContext _child = null;
        private Visibility _visibility = Visibility.Visible;

        public MainContext(string imagePath)
        {
            this._imagePath = imagePath;
        }

        public BitmapImage MainImage
        {
            get
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), _imagePath);
                return new BitmapImage(new Uri(path));
            }
        }

        #region IHierarchyContext Members

        public IHierarchyContext Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public IHierarchyContext Child
        {
            get { return _child; }
            set { _child = value; }
        }

        public bool HasNext
        {
            get { return false; }
        }

        public void Next()
        {
        }

        public bool HasPrevious
        {
            get { return false; }
        }

        public void Previous()
        {
        }

        public Visibility Visibility1
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                NotifyPropertyChanged(new PropertyChangedEventArgs("Visibility1"));
            }
       }

        #endregion




        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        #endregion
    }
}

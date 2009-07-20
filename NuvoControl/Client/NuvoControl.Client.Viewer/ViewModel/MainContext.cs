using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows;
using System.IO;

using NuvoControl.Common.Configuration;

namespace NuvoControl.Client.Viewer.ViewModel
{
    public class MainContext : INotifyPropertyChanged, IHierarchyContext
    {
        private Building _building;
        private IHierarchyContext _parent = null;
        private IHierarchyContext _child = null;
        private Visibility _visibility = Visibility.Visible;

        public MainContext(Building building)
        {
            this._building = building;
        }

        public BitmapImage MainImage
        {
            get
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), _building.PicturePath);
                return new BitmapImage(new Uri(path));
            }
        }

        #region IHierarchyContext Members

        public string Name
        {
            get { return _building.Name; }
        }

        public Address Id
        {
            get { return _building.Id; }
        }

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

        public string NextName
        {
            get { return String.Empty; }
        }

        public void Next()
        {
        }

        public bool HasPrevious
        {
            get { return false; }
        }

        public string PreviousName
        {
            get { return String.Empty; }
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

        public void OnHierarchyChanged()
        {
            // noting to do
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Common.Configuration;

namespace NuvoControl.Client.Viewer.ViewModel
{
    public class HistoryList
    {
        private List<NavigationItem> _history = new List<NavigationItem>();
        private int _currentIndex = -1;
        private const int MAX_ITEMS = 10;

        public int CurrentIndex
        {
            get { return _currentIndex; }
        }

        public void Append(NavigationItem item)
        {
            if (_history.Count >= MAX_ITEMS)
            {
                _history.RemoveAt(0);
                _currentIndex--;
            }
            else if (_history.Count > _currentIndex + 1)
                _history.RemoveRange(_currentIndex + 1, _history.Count - (_currentIndex + 1));

            _history.Add(item);
            _currentIndex++;
        }

        public bool CanBrowseBack
        {
            get { return (_currentIndex <= 0)? false: true; }
        }

        public NavigationItem BrowseBack()
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                return _history[_currentIndex];
            }
            else
                return null;
        }

        public string BrowseBackName
        {
            get
            {
                if (_currentIndex > 0)
                    return _history[_currentIndex - 1].Name;
                else
                    return null;
            }
        }


        public bool CanBrowseForward
        {
            get { return (_currentIndex < _history.Count - 1) ? true : false; }
        }

        public NavigationItem BrowseForward()
        {
            if (_currentIndex < MAX_ITEMS - 1)
            {
                _currentIndex++;
                return _history[_currentIndex];
            }
            else
                return null;
        }


        public string BrowseForwardName
        {
            get
            {
                if (_currentIndex < MAX_ITEMS - 1)
                    return _history[_currentIndex + 1].Name;
                else
                    return null;
            }
        }
    }
}

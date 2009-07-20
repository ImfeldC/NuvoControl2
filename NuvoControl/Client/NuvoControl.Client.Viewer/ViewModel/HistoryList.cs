using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Common.Configuration;

namespace NuvoControl.Client.Viewer.ViewModel
{
    public class HistoryList
    {
        public class HistoryListItem
        {
            private IHierarchyContext _context;
            private Address _id;

            public HistoryListItem(IHierarchyContext context, Address id)
            {
                this._context = context;
                this._id = id;
            }

            public IHierarchyContext Context
            {
                get { return _context; }
            }

            public Address Id
            {
                get { return _id; }
            }
        }

        private List<HistoryListItem> _history = new List<HistoryListItem>();
        private int _currentIndex = 0;
        private const int MAX_ITEMS = 10;

        public int CurrentIndex
        {
            get { return _currentIndex; }
        }

        public void Append(HistoryListItem item)
        {
            if (_history.Count >= MAX_ITEMS)
                _history.RemoveAt(0);
            else if (_history.Count > _currentIndex + 1)
                _history.RemoveRange(_currentIndex + 1, _history.Count - _currentIndex);

            _history.Add(item);
            _currentIndex++;
        }

        public bool CanBrowseBack
        {
            get { return (_currentIndex <= 0)? false: true; }
        }

        public HistoryListItem BrowseBack()
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                return _history[_currentIndex + 1];
            }
            else
                return null;
        }

        public bool CanBrowseForward
        {
            get { return (_currentIndex >= MAX_ITEMS-1) ? false : true; }
        }

        public HistoryListItem BrowseForward()
        {
            if (_currentIndex < MAX_ITEMS - 1)
            {
                _currentIndex++;
                return _history[_currentIndex - 1];
            }
            else
                return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using NuvoControl.Common.Configuration;

namespace NuvoControl.Client.Viewer.ViewModel
{
    public interface IHierarchyContext
    {
        string Name { get; }
        Address Id { get; }
        List<NavigationItem> NavigationItems { get; }
        IHierarchyContext Parent { get; set; }
        IHierarchyContext Child { get; set; }
        bool HasNext { get; }
        string NextName { get; }
        void Next();
        bool HasPrevious { get; }
        string PreviousName { get; }
        void Previous();
        void Navigate(Address id);
        Visibility Visibility1 { get; set; }
        void OnHierarchyChanged();
        void UpdateContext(IHierarchyContext context);


    }


}

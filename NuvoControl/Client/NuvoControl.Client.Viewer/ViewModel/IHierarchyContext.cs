using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace NuvoControl.Client.Viewer.ViewModel
{
    public interface IHierarchyContext
    {
        IHierarchyContext Parent { get; set; }
        IHierarchyContext Child { get; set; }

        bool HasNext { get; }
        void Next();

        bool HasPrevious { get; }
        void Previous();

        Visibility Visibility1 { get; set; }


    }


}

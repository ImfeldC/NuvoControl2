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

        /// <summary>
        /// Called, when this context got activated. i.e. the corresponding view is visible.
        /// </summary>
        void OnHierarchyActivated();

        /// <summary>
        /// Called, when this context is deactivated. i.e. this corresponding view is not visible.
        /// </summary>
        void OnHierarchyDeactivated();


        void UpdateContext(IHierarchyContext context);

        //void Restore(Address id);


    }


}

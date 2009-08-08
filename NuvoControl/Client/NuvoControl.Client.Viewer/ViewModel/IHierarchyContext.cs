using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

using NuvoControl.Common.Configuration;

namespace NuvoControl.Client.Viewer.ViewModel
{
    /// <summary>
    /// Any context (floor, zone, ...) implements this interface, if this context is part of a hierarchy.
    /// I.e. The floor is child of the building, the zone is child of the floor.
    /// </summary>
    public interface IHierarchyContext
    {
        /// <summary>
        /// The name of the object, shown in the current hierarchy.
        /// </summary>
        string ObjectName { get; }

        /// <summary>
        /// The address of the object, shown in the current hierarchy.
        /// </summary>
        Address ObjectId { get; }

        /// <summary>
        /// The parent hierarchy context.
        /// </summary>
        IHierarchyContext Parent { get; set; }

        /// <summary>
        /// The child hierarchy context.
        /// </summary>
        IHierarchyContext Child { get; set; }

        /// <summary>
        /// The visibility of the hierarchy context. 
        /// Only one can be visible at a time.
        /// </summary>
        Visibility ContextVisibility { get; set; }

        /// <summary>
        /// True, if the context has a next object to show.
        /// </summary>
        bool HasNextObject { get; }

        /// <summary>
        /// The name of the next object.
        /// </summary>
        string NextObjectName { get; }

        /// <summary>
        /// Go to next object of this context.
        /// </summary>
        void NextObject();

        /// <summary>
        /// True, if the context has a previous object to show.
        /// </summary>
        bool HasPreviousObject { get; }

        /// <summary>
        /// The name of the previous object.
        /// </summary>
        string PreviousObjectName { get; }

        /// <summary>
        /// Go to previous object of this context.
        /// </summary>
        void PreviousObject();

        /// <summary>
        /// Navigate to a specified object of this context.
        /// </summary>
        /// <param name="id">The id of the object to navigate to.</param>
        void Navigate(Address id);

        /// <summary>
        /// Get all navigations objects of this context.
        /// </summary>
        List<NavigationItem> NavigationObjects { get; }

        /// <summary>
        /// Called, when this context got activated. i.e. the corresponding acitve object is visible.
        /// </summary>
        /// <param name="id">The id of the object to navigate to.</param>
        void OnHierarchyActivated(Address id);

        /// <summary>
        /// Called, when this context is deactivated. i.e. this corresponding active object is not visible.
        /// </summary>
        void OnHierarchyDeactivated();

        /// <summary>
        /// Updates the context with the specified parameter.
        /// </summary>
        /// <param name="context"></param>
        void UpdateContext(IHierarchyContext context);
    }


}

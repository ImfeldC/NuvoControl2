﻿using System;
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
        IHierarchyContext Parent { get; set; }
        IHierarchyContext Child { get; set; }
        bool HasNext { get; }
        string NextName { get; }
        void Next();
        bool HasPrevious { get; }
        string PreviousName { get; }
        void Previous();
        Visibility Visibility1 { get; set; }
        void OnHierarchyChanged();


    }


}
﻿// XAML Map Control - http://xamlmapcontrol.codeplex.com/
// Copyright © Clemens Fischer 2012-2013
// Licensed under the Microsoft Public License (Ms-PL)

using System.Windows;
using System.Windows.Controls;

namespace MapControl
{
    /// <summary>
    /// Displays a pushpin at a geographic location provided by the MapPanel.Location attached property.
    /// </summary>
    public class Pushpin : ContentControl
    {
        static Pushpin()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(Pushpin), new FrameworkPropertyMetadata(typeof(Pushpin)));
        }
    }
}

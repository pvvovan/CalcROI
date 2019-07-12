﻿// XAML Map Control - http://xamlmapcontrol.codeplex.com/
// Copyright © Clemens Fischer 2012-2013
// Licensed under the Microsoft Public License (Ms-PL)

using System.Windows;
using System.Windows.Input;

namespace MapControl
{
    /// <summary>
    /// Default input event handling.
    /// </summary>
    public class Map : MapBase
    {
        public static readonly DependencyProperty ManipulationModeProperty = DependencyProperty.Register(
            "ManipulationMode", typeof(ManipulationModes), typeof(Map), new PropertyMetadata(ManipulationModes.All));

        public static readonly DependencyProperty MouseWheelZoomChangeProperty = DependencyProperty.Register(
            "MouseWheelZoomChange", typeof(double), typeof(Map), new PropertyMetadata(1d));

        private Point? mousePosition;

        static Map()
        {
            IsManipulationEnabledProperty.OverrideMetadata(typeof(Map), new FrameworkPropertyMetadata(true));
        }

        /// <summary>
        /// Gets or sets a value that specifies how the map control handles manipulations.
        /// </summary>
        public ManipulationModes ManipulationMode
        {
            get { return (ManipulationModes)GetValue(ManipulationModeProperty); }
            set { SetValue(ManipulationModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the amount by which the ZoomLevel property changes during a MouseWheel event.
        /// </summary>
        public double MouseWheelZoomChange
        {
            get { return (double)GetValue(MouseWheelZoomChangeProperty); }
            set { SetValue(MouseWheelZoomChangeProperty, value); }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            var zoomChange = MouseWheelZoomChange * (double)e.Delta / 120d;
            ZoomMap(e.GetPosition(this), TargetZoomLevel + zoomChange);
        }
              

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (CaptureMouse())
            {
                mousePosition = e.GetPosition(this);
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (mousePosition.HasValue)
            {
                mousePosition = null;
                ReleaseMouseCapture();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (mousePosition.HasValue)
            {
                var position = e.GetPosition(this);
                TranslateMap((Point)(position - mousePosition));
                mousePosition = position;
            }
        }

        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            base.OnManipulationStarted(e);

            Manipulation.SetManipulationMode(this, ManipulationMode);
        }

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            base.OnManipulationDelta(e);

            TransformMap(e.ManipulationOrigin,
                (Point)e.DeltaManipulation.Translation, e.DeltaManipulation.Rotation,
                (e.DeltaManipulation.Scale.X + e.DeltaManipulation.Scale.Y) / 2d);
        }
    }
}

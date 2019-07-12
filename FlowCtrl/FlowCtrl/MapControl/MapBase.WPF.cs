﻿// XAML Map Control - http://xamlmapcontrol.codeplex.com/
// Copyright © Clemens Fischer 2012-2013
// Licensed under the Microsoft Public License (Ms-PL)

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MapControl
{
    public partial class MapBase
    {
        public static readonly DependencyProperty ForegroundProperty =
            System.Windows.Controls.Control.ForegroundProperty.AddOwner(typeof(MapBase));

        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(
            "Center", typeof(Location), typeof(MapBase), new FrameworkPropertyMetadata(
                new Location(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (o, e) => ((MapBase)o).CenterPropertyChanged((Location)e.NewValue)));

        public static readonly DependencyProperty TargetCenterProperty = DependencyProperty.Register(
            "TargetCenter", typeof(Location), typeof(MapBase), new FrameworkPropertyMetadata(
                new Location(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (o, e) => ((MapBase)o).TargetCenterPropertyChanged((Location)e.NewValue)));

        public static readonly DependencyProperty ZoomLevelProperty = DependencyProperty.Register(
            "ZoomLevel", typeof(double), typeof(MapBase), new FrameworkPropertyMetadata(
                1d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (o, e) => ((MapBase)o).ZoomLevelPropertyChanged((double)e.NewValue)));

        public static readonly DependencyProperty TargetZoomLevelProperty = DependencyProperty.Register(
            "TargetZoomLevel", typeof(double), typeof(MapBase), new FrameworkPropertyMetadata(
                1d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (o, e) => ((MapBase)o).TargetZoomLevelPropertyChanged((double)e.NewValue)));

        public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register(
            "Heading", typeof(double), typeof(MapBase), new FrameworkPropertyMetadata(
                0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (o, e) => ((MapBase)o).HeadingPropertyChanged((double)e.NewValue)));

        public static readonly DependencyProperty TargetHeadingProperty = DependencyProperty.Register(
            "TargetHeading", typeof(double), typeof(MapBase), new FrameworkPropertyMetadata(
                0d, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (o, e) => ((MapBase)o).TargetHeadingPropertyChanged((double)e.NewValue)));

        static MapBase()
        {
            UIElement.ClipToBoundsProperty.OverrideMetadata(
                typeof(MapBase), new FrameworkPropertyMetadata(true));

            Panel.BackgroundProperty.OverrideMetadata(
                typeof(MapBase), new FrameworkPropertyMetadata(Brushes.Transparent));
        }

        partial void Initialize()
        {
            AddVisualChild(tileContainer);
        }

        partial void RemoveAnimation(DependencyProperty property)
        {
            BeginAnimation(property, null);
        }

        protected override int VisualChildrenCount
        {
            get { return base.VisualChildrenCount + 1; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
            {
                return tileContainer;
            }

            return base.GetVisualChild(index - 1);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            ResetTransformOrigin();
            UpdateTransform();
        }

        private void SetTransformMatrixes(double scale)
        {
            Matrix rotateMatrix = Matrix.Identity;
            rotateMatrix.Rotate(Heading);
            rotateTransform.Matrix = rotateMatrix;
            scaleTransform.Matrix = new Matrix(scale, 0d, 0d, scale, 0d, 0d);
            scaleRotateTransform.Matrix = scaleTransform.Matrix * rotateMatrix;
        }

        internal void OnLeftMouseButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            OnMouseLeftButtonDown(e);
        }
    }
}

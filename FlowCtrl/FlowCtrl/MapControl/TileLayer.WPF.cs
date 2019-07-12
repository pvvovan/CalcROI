﻿// XAML Map Control - http://xamlmapcontrol.codeplex.com/
// Copyright © Clemens Fischer 2012-2013
// Licensed under the Microsoft Public License (Ms-PL)

using System.Windows;
using System.Windows.Media;

namespace MapControl
{
    public partial class TileLayer : DrawingVisual
    {
        partial void Initialize()
        {
            VisualTransform = transform;
            VisualEdgeMode = EdgeMode.Aliased;
        }

        public Brush Background { get; set; }

        protected ContainerVisual TileContainer
        {
            get { return Parent as ContainerVisual; }
        }

        protected void RenderTiles()
        {
            using (var drawingContext = RenderOpen())
            {
                foreach (var tile in tiles)
                {
                    var tileSize = TileSource.TileSize << (zoomLevel - tile.ZoomLevel);
                    var tileRect = new Rect(
                        tileSize * tile.X - TileSource.TileSize * grid.X,
                        tileSize * tile.Y - TileSource.TileSize * grid.Y,
                        tileSize, tileSize);

                    drawingContext.DrawRectangle(tile.Brush, null, tileRect);

                    //if (tile.ZoomLevel == zoomLevel)
                    //    drawingContext.DrawText(new FormattedText(string.Format("{0}-{1}-{2}", tile.ZoomLevel, tile.X, tile.Y),
                    //        System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 14, Brushes.Black), tileRect.TopLeft);
                }
            }
        }
    }
}

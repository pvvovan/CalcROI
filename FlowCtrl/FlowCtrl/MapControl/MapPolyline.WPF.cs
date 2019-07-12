// XAML Map Control - http://xamlmapcontrol.codeplex.com/
// Copyright © Clemens Fischer 2012-2013
// Licensed under the Microsoft Public License (Ms-PL)

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace MapControl
{
    public partial class MapPolyline
    {
        private const double GeometryScale = 1e6;

        private static readonly ScaleTransform geometryScaleTransform = new ScaleTransform
        {
            ScaleX = 1d / GeometryScale,
            ScaleY = 1d / GeometryScale
        };


        public MapPolyline()
        {
            Data = new StreamGeometry();
            this.PreviewMouseLeftButtonDown += MapPolyline_PreviewMouseLeftButtonDown;
        }

        void MapPolyline_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ParentMap.OnLeftMouseButtonDown(e);
            ExecuteCommand();
        }

        protected override void UpdateData()
        {
            var geometry = (StreamGeometry)Data;
            
            var locations = Locations;
            Location first;

            if (ParentMap != null && locations != null && (first = locations.FirstOrDefault()) != null)
            {
                using (var context = geometry.Open())
                {
                    var startPoint = ParentMap.MapTransform.Transform(first);
                    var points = locations.Skip(1).Select(l => ParentMap.MapTransform.Transform(l)).ToList();

                    //context.BeginFigure(startPoint, IsClosed, IsClosed);
                    //context.PolyLineTo(points, true, false);


                    //geometry.Rect = new Rect(p1.X * GeometryScale, p1.Y * GeometryScale,
                    //(p2.X - p1.X) * GeometryScale, (p2.Y - p1.Y) * GeometryScale);
                    Point scaledStartPoint = new Point(startPoint.X * GeometryScale, startPoint.Y * GeometryScale);
                    List<Point> scaledPoints = new List<Point>(points.Count);
                    foreach (var p in points)
                        scaledPoints.Add(new Point(p.X * GeometryScale, p.Y * GeometryScale));

                //var pathF = new PathFigure();
                //pathF.IsFilled = true;
                //pathF.StartPoint = scaledStartPoint;
                //for (int i = 1; i < scaledPoints.Count; i++)
                //    pathF.Segments.Add(new LineSegment() { Point = scaledPoints[i] });
                //geometry.Figures.Add(pathF);
                    context.BeginFigure(scaledStartPoint, IsClosed, IsClosed);
                    context.PolyLineTo(scaledPoints, true, false);
                }

                var transform = new TransformGroup();
                transform.Children.Add(geometryScaleTransform); // revert scaling
                transform.Children.Add(ParentMap.ViewportTransform);
                //RenderTransform = transform;
                geometry.Transform = transform;

                //geometry.Transform = ParentMap.ViewportTransform;
            }
            else
            {
                geometry.Clear();
                geometry.ClearValue(Geometry.TransformProperty);
            }
        }
    }
}

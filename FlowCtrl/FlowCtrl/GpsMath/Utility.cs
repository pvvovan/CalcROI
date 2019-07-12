using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCtrl.GpsMath
{
    public static class Utility
    {
        public static List<MyPoint3D> GetPolygonIntersections(List<MyPoint3D> polygon, LineDefinition line)
        {
            List<MyPoint3D> intersections = new List<MyPoint3D>();
            if (polygon.Last().X != polygon.First().X || polygon.Last().Y != polygon.First().Y)
            {
                var original = polygon;
                polygon = new List<MyPoint3D>();
                foreach (var p in original)
                    polygon.Add(p);
                polygon.Add(original.First());
            }
            for (int i = 1; i < polygon.Count; i++)
            {
                var p1 = polygon[i - 1];
                var p2 = polygon[i];
                var intersection = LineDefinition.GetLineIntersection(LineDefinition.GetLineDefinition(p1, p2), line);
                if (Utility.IsInLineSegment(p1, p2, intersection))
                    intersections.Add(intersection);
            }
            if (intersections.Count % 2 != 0)
            { }
            return intersections;
        }

        public static List<MyPoint3D> GetHeadlandIntersections(List<MyPoint3D> heanland, LineDefinition line, double width)
        {
            List<MyPoint3D> intersections = new List<MyPoint3D>();
            if (heanland.Last().X != heanland.First().X || heanland.Last().Y != heanland.First().Y)
            {
                var original = heanland;
                heanland = new List<MyPoint3D>();
                foreach (var p in original)
                    heanland.Add(p);
                heanland.Add(original.First());
            }
            for (int i = 1; i < heanland.Count; i++)
            {
                var p1 = heanland[i - 1];
                var p2 = heanland[i];
                var outerLine = LineDefinition.GetLineDefinition(p1, p2);
                var intersection = LineDefinition.GetLineIntersection(outerLine, line);
                if (Utility.IsInLineSegment(p1, p2, intersection))
                {
                    intersections.Add(ChooseHeadlandIntersection(heanland, line, width, outerLine, intersection));
                }
            }
            if (intersections.Count % 2 != 0)
            { }
            return intersections;
        }

        public static MyPoint3D ChooseHeadlandIntersection(List<MyPoint3D> heanland, LineDefinition line, double width, LineDefinition outerLine, MyPoint3D intersection)
        {
            double phiOl = Math.Atan(outerLine.k);
            double phiL = Math.Atan(line.k);
            double alpha = phiOl - phiL;
            if (Math.Abs(alpha) < WGS84.Pi / 2)
                alpha = WGS84.Pi - Math.Abs(alpha);
            double d = width / 2 / Math.Tan(alpha / 2);
            double dx = d * Math.Cos(phiL);
            MyPoint3D intersection1 = new MyPoint3D(intersection.X + dx, line.k * (intersection.X + dx) + line.b, 0);
            if (IsInsidePolygon(heanland, intersection1))
            {
                return (intersection1);
            }
            else
            {
                MyPoint3D intersection2 = new MyPoint3D(intersection.X - dx, line.k * (intersection.X - dx) + line.b, 0);
                return (intersection2);
            }
        }

        public static bool IsInsidePolygon(List<MyPoint3D> polygon, MyPoint3D point)
        {
            if ((polygon.First().X != polygon.Last().X) || (polygon.First().Y != polygon.Last().Y))
            {
                var inputPoly = polygon;
                polygon = new List<MyPoint3D>(inputPoly.Count + 1);
                foreach (var p in inputPoly)
                    polygon.Add(p);
                polygon.Add(inputPoly.First());
            }

            int IntersectionCounter = 0;
            var Pc = point;

            for (int i = 1; i < polygon.Count; i++)
            {
                var p1 = polygon[i - 1];
                var p2 = polygon[i];

                if (p1.X <= Pc.X)
                    if (p2.X < Pc.X)
                        continue;

                if (p1.X >= Pc.X)
                    if (p2.X > Pc.X)
                        continue;

                if (p1.Y <= Pc.Y)
                    if (p2.Y < Pc.Y)
                        continue;

                if (p1.Y >= Pc.Y)
                    if (p2.Y > Pc.Y)
                    {
                        IntersectionCounter++;
                        continue;
                    }

                double k21 = (p2.Y - p1.Y) / (p2.X - p1.X);
                double kc1 = (Pc.Y - p1.Y) / (Pc.X - p1.X);

                if (p2.X > p1.X)
                {
                    if (kc1 <= k21)
                        IntersectionCounter++;
                }
                else
                {
                    if (kc1 >= k21)
                        IntersectionCounter++;
                }
            }


            if (IntersectionCounter % 2 == 0) // is even
                return false;
            else
                return true;
        }

        public static bool IsInLineSegment(MyPoint3D p1, MyPoint3D p2, MyPoint3D point)
        {
            double maxX = Math.Max(p1.X, p2.X) + 0.000001;
            double minX = Math.Min(p1.X, p2.X) - 0.000001;
            double maxY = Math.Max(p1.Y, p2.Y) + 0.000001;
            double minY = Math.Min(p1.Y, p2.Y) - 0.000001;
            if (point.X >= minX && point.X <= maxX && point.Y >= minY && point.Y <= maxY)
            {
                if (point.X == p2.X && point.Y == p2.Y)
                    return false;
                return true;
            }
            else
                return false;
        }

        public static List<MyPoint3D> ShiftBoundary(double length, List<MyPoint3D> boundary)
        {
            List<MyPoint3D> headLand = new List<MyPoint3D>();
            headLand.Add(ShiftPoint(boundary.Last(), boundary[0], boundary[1], length));
            for (int i = 1; i < boundary.Count - 1; i++)
                headLand.Add(ShiftPoint(boundary[i - 1], boundary[i], boundary[i + 1], length));
            headLand.Add(ShiftPoint(boundary[boundary.Count - 2], boundary.Last(), boundary[0], length));
            return headLand;
        }

        public static MyPoint3D ShiftPoint(MyPoint3D p1, MyPoint3D p2, double Length)
        {
            MyPoint3D p = new MyPoint3D();

            double gamma = Math.Atan2((p2.Y - p1.Y), (p2.X - p1.X));
            double alpha = gamma - WGS84.Pi / 2.0;
            double dx = Length * Math.Cos(alpha);
            double dy = Length * Math.Sin(alpha);

            p.X = p2.X + dx;
            p.Y = p2.Y + dy;

            return p;
        }

        public static MyPoint3D ShiftPoint(MyPoint3D p1, MyPoint3D p2, MyPoint3D p3, double Length)
        {
            LineDefinition line1;
            if (p1.X > p2.X)
                line1 = LineDefinition.GetParallelLine(LineDefinition.GetLineDefinition(p1, p2), -Length);
            else
                line1 = LineDefinition.GetParallelLine(LineDefinition.GetLineDefinition(p1, p2), Length);

            LineDefinition line2;
            if (p2.X > p3.X)
                line2 = LineDefinition.GetParallelLine(LineDefinition.GetLineDefinition(p2, p3), -Length);
            else
                line2 = LineDefinition.GetParallelLine(LineDefinition.GetLineDefinition(p2, p3), Length);

            if (line1.IsVertical && line2.IsVertical)
            {
                MyPoint3D p = new MyPoint3D();
                p.Y = p2.Y;
                if (p1.Y < p2.Y)
                    p.X = p2.X - Length;
                else
                    p.X = p2.X + Length;
                return p;
            }
            else if (line1.k == line2.k)
                return ShiftPoint(p1, p2, -Length);
            else
                return LineDefinition.GetLineIntersection(line1, line2);
        }

        public static double CalculateArea(List<GeoPoint> points, bool returnPositive = true)
        {
            List<MyPoint3D> polygon = new List<MyPoint3D>();
            foreach (var p in points)
                polygon.Add(p.ToCart());

            if (polygon.Count < 3)
                return 0;

            if ((points.First().Longitude != points.Last().Longitude) ||
                (points.First().Latitude != points.Last().Latitude) ||
                (points.First().Elevation != points.Last().Elevation))
                polygon.Add(points.First().ToCart());


            int NumOfPoints = polygon.Count;
            double area = 0;

            double[] X = new double[NumOfPoints];
            double[] Y = new double[NumOfPoints];
            double[] Z = new double[NumOfPoints];

            for (int i = 0; i < NumOfPoints; i++)
            {
                X[i] = polygon[i].X;
                Y[i] = polygon[i].Y;
                Z[i] = polygon[i].Z;
            }

            for (int i = 1; i < NumOfPoints; i++)
            {
                area = area + (X[i] - X[i - 1]) * Y[i - 1] + 0.5 * (Y[i] - Y[i - 1]) * (X[i] - X[i - 1]);
            }
            if (returnPositive)
                area = (Math.Abs(area));
            if (Double.IsNaN(area))
                return -1;
            return area;
        }
    }
}

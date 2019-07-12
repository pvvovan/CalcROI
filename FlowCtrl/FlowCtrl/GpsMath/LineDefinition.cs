using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCtrl.GpsMath
{
    public class LineDefinition
    {
        public double k;
        public double b;
        public bool IsVertical;
        public double X;

        public static LineDefinition GetLineDefinition(MyPoint3D p1, MyPoint3D p2)
        {
            LineDefinition def = new LineDefinition();
            if (p2.X == p1.X)
            {
                def.IsVertical = true;
                def.X = p1.X;
                if (p2.Y > p1.Y)
                    def.k = double.PositiveInfinity;
                else
                    def.k = double.NegativeInfinity;
                return def;
            }
            def.k = (p2.Y - p1.Y) / (p2.X - p1.X);
            def.b = p1.Y - def.k * p1.X;
            return def;
        }

        public static MyPoint3D GetLineIntersection(LineDefinition l1, LineDefinition l2)
        {
            MyPoint3D point = new MyPoint3D();
            if (l1.IsVertical)
            {
                point.X = l1.X;
                point.Y = l2.k * point.X + l2.b;
                return point;
            }
            if (l2.IsVertical)
            {
                point.X = l2.X;
                point.Y = l1.k * point.X + l1.b;
                return point;
            }
            point.X = (l2.b - l1.b) / (l1.k - l2.k);
            point.Y = point.X * l1.k + l1.b;
            return point;
        }

        public static LineDefinition GetParallelLine(LineDefinition line, double distance)
        {
            if (line.IsVertical)
            {
                if (line.k == double.PositiveInfinity)
                    return new LineDefinition() { IsVertical = true, X = line.X - distance, k = double.PositiveInfinity };
                else
                    return new LineDefinition() { IsVertical = true, X = line.X + distance, k = double.NegativeInfinity };
            }
            LineDefinition parallelLine = new LineDefinition() { k = line.k, b = line.b };
            double alpha = Math.Atan(line.k);
            parallelLine.b += distance / Math.Cos(alpha);
            return parallelLine;
        }

        internal static LineDefinition TurnLine(LineDefinition line, double angle, MyPoint3D origin)
        {
            double alpha = Math.Atan(line.k) + angle;
            LineDefinition turnedLine = new LineDefinition() { k = Math.Tan(alpha) };
            turnedLine.b = origin.Y - origin.X * turnedLine.k;
            return turnedLine;
        }

        internal static LineDefinition GetBestLine(List<MyPoint3D> points)
        {
            LineDefinition bestLine = new LineDefinition();

            double[,] A_A = new double[2, 2] { { 0, 0 }, { 0, 0 } };
            foreach (var p in points)
            {
                A_A[0, 0] += p.X * p.X;
                A_A[0, 1] += p.X;
            }
            A_A[1, 0] = A_A[0, 1];
            A_A[1, 1] = points.Count;

            double overK = 1.0 / (A_A[0, 0] * A_A[1, 1] - A_A[1, 0] * A_A[0, 1]);
            double[,] A_minus = new double[2, 2];
            A_minus[0, 0] = A_A[1, 1] * overK;
            A_minus[0, 1] = -A_A[0, 1] * overK;
            A_minus[1, 0] = -A_A[1, 0] * overK;
            A_minus[1, 1] = A_A[0, 0] * overK;

            foreach (var p in points)
            {
                bestLine.k += (A_minus[0, 0] * p.X + A_minus[0, 1] * 1) * p.Y;
                bestLine.b += (A_minus[1, 0] * p.X + A_minus[1, 1] * 1) * p.Y;
            }

            return bestLine;
        }
    }
}

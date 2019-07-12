using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace FlowCtrl.GpsMath
{
    public class MyPoint3D
    {
        public double X;

        public double Y;

        public double Z;

        public MyPoint3D() { }

        public MyPoint3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public MyPoint3D(MyPoint3D point)
            : this(point.X, point.Y, point.Z)
        {

        }
        public double Norm
        {
            get { return (Math.Sqrt(X * X + Y * Y + Z * Z)); }
        }
        public MyPoint3D Normalize()
        {
            MyPoint3D Vec = new MyPoint3D(X, Y, Z);
            return Vec / this.Norm;
        }

        public static MyPoint3D operator +(MyPoint3D p1, MyPoint3D p2)
        {
            return new MyPoint3D(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        }
        public static MyPoint3D operator -(MyPoint3D p1, MyPoint3D p2)
        {
            return new MyPoint3D(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        }
        public static double operator *(MyPoint3D p1, MyPoint3D p2)
        {
            return ((p1.X * p2.X) + (p1.Y * p2.Y) + (p1.Z * p2.Z));
        }
        public static MyPoint3D operator *(MyPoint3D p1, double p2)
        {
            return new MyPoint3D((p1.X * p2), (p1.Y * p2), (p1.Z * p2));
        }
        public static MyPoint3D operator *(double p2, MyPoint3D p1)
        {
            return new MyPoint3D((p1.X * p2), (p1.Y * p2), (p1.Z * p2));
        }
        public static MyPoint3D operator /(MyPoint3D p1, double p2)
        {
            return new MyPoint3D((p1.X / p2), (p1.Y / p2), (p1.Z / p2));
        }
        public static MyPoint3D operator /(double p2, MyPoint3D p1)
        {
            return new MyPoint3D((p1.X / p2), (p1.Y / p2), (p1.Z / p2));
        }



        internal class MyPoint3D_ComparerByY : IComparer<MyPoint3D>
        {
            public int Compare(MyPoint3D x, MyPoint3D y)
            {
                if (x.Y > y.Y)
                    return 1;
                if (x.Y < y.Y)
                    return -1;
                return 0;
            }
        }

        internal class MyPoint3D_ComparerByX : IComparer<MyPoint3D>
        {
            public int Compare(MyPoint3D x, MyPoint3D y)
            {
                if (x.X > y.X)
                    return 1;
                if (x.X < y.X)
                    return -1;
                return 0;
            }
        }

        internal class MyPoint3D_EqualityComparer : IEqualityComparer<MyPoint3D>
        {
            public bool Equals(MyPoint3D x, MyPoint3D y)
            {
                return ((x - y).Norm < 0.000001);
            }

            public int GetHashCode(MyPoint3D obj)
            {
                return (obj.X.ToString() + obj.Y.ToString() + obj.Z.ToString()).GetHashCode();
            }
        }
    }
}

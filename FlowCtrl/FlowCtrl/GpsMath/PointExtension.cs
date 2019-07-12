using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCtrl.GpsMath
{
    public static class PointExtension
    {
        static public double LocalLatitude = 50;

        public static MyPoint3D ToCart(this GeoPoint p)
        {
            double LatitudeDegCoef = WGS84.Pi * WGS84.a / 180.0;
            double LongitudeDegCoef = WGS84.Pi * WGS84.a * Math.Cos(LocalLatitude / 180.0 * WGS84.Pi) / 180.0;
            MyPoint3D cart = new MyPoint3D();
            cart.Y = p.Latitude * LatitudeDegCoef;
            cart.X = p.Longitude * LongitudeDegCoef;
            return cart;
        }

        public static GeoPoint ToGeo(this MyPoint3D p)
        {
            double LatitudeDegCoef = WGS84.Pi * WGS84.a / 180.0;
            double LongitudeDegCoef = WGS84.Pi * WGS84.a * Math.Cos(LocalLatitude / 180.0 * WGS84.Pi) / 180.0;
            GeoPoint geo = new GeoPoint();
            geo.Latitude = p.Y / LatitudeDegCoef;
            geo.Longitude = p.X / LongitudeDegCoef;
            return geo;
        }
    }
}

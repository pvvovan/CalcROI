using MapControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCtrl.GpsMath
{
    static class LocationExtension
    {
        public static GeoPoint ToGeoPoint(this Location location)
        {
            return new GeoPoint() { Latitude = location.Latitude, Longitude = location.Longitude };
        }

        public static Location ToLocation(this GeoPoint geoPoint)
        {
            return new Location(geoPoint.Latitude, geoPoint.Longitude);
        }
    }
}

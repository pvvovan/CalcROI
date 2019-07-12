using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlowCtrl.GpsMath;

namespace FlowCtrl.Tests
{
    [TestClass]
    public class TestUtility
    {
        [TestMethod]
        public void TestIsInLineSegment()
        {
            MyPoint3D p1 = new MyPoint3D(1, 3, 0);
            MyPoint3D p2 = new MyPoint3D(0, -1, 0);

            Assert.IsTrue (Utility.IsInLineSegment(p1, p2, new MyPoint3D(0.25, 0, 0)));
            Assert.IsTrue (Utility.IsInLineSegment(p2, p1, new MyPoint3D(0.25, 0, 0)));
            Assert.IsFalse(Utility.IsInLineSegment(p2, p1, new MyPoint3D(0.25, 4, 0)));
        }

        [TestMethod]
        public void TestPolygonIntersections()
        {
            List<MyPoint3D> polygon = new List<MyPoint3D>();
            polygon.Add(new MyPoint3D(-1, 0, 0));
            polygon.Add(new MyPoint3D(0, 3, 0));
            polygon.Add(new MyPoint3D(3, 0, 0));

            var inters = Utility.GetPolygonIntersections(polygon, new LineDefinition() { k = 1, b = 0 });
            Assert.AreEqual(2, inters.Count);
            Assert.AreEqual(0, Math.Min(inters[0].X, inters[1].X));
            Assert.AreEqual(1.5, Math.Max(inters[0].X, inters[1].X));
            Assert.AreEqual(0, Math.Min(inters[0].Y, inters[1].Y));
            Assert.AreEqual(1.5, Math.Max(inters[0].Y, inters[1].Y));

            inters = Utility.GetPolygonIntersections(polygon, new LineDefinition() { k = 0.1, b = -4 });
            Assert.AreEqual(0, inters.Count);
        }

        [TestMethod]
        public void TestIsInsidePolygon()
        {
            List<MyPoint3D> polygon = new List<MyPoint3D>();
            polygon.Add(new MyPoint3D(-1, -1, 0));
            polygon.Add(new MyPoint3D(0, 2, 0));
            polygon.Add(new MyPoint3D(3, 0, 0));

            Assert.IsTrue  (Utility.IsInsidePolygon(polygon, new MyPoint3D(   1,   1, 0 )));
            Assert.IsTrue  (Utility.IsInsidePolygon(polygon, new MyPoint3D( 0.1, 0.1, 0 )));
            Assert.IsFalse (Utility.IsInsidePolygon(polygon, new MyPoint3D(   0,  -1, 0 )));
            Assert.IsFalse (Utility.IsInsidePolygon(polygon, new MyPoint3D(   1,  -1, 0 )));
            Assert.IsTrue  (Utility.IsInsidePolygon(polygon, new MyPoint3D(  -1,  -1, 0 )));
            Assert.IsTrue  (Utility.IsInsidePolygon(polygon, new MyPoint3D(   0,   2, 0 )));
            Assert.IsFalse (Utility.IsInsidePolygon(polygon, new MyPoint3D(   3,   1, 0 )));
            Assert.IsTrue  (Utility.IsInsidePolygon(polygon, new MyPoint3D(   3,   0, 0 )));
            Assert.IsFalse (Utility.IsInsidePolygon(polygon, new MyPoint3D(   4,   0, 0 )));
            Assert.IsFalse (Utility.IsInsidePolygon(polygon, new MyPoint3D(  -4,   0, 0 )));
            Assert.IsTrue  (Utility.IsInsidePolygon(polygon, new MyPoint3D(   2,   0, 0 )));
            Assert.IsTrue  (Utility.IsInsidePolygon(polygon, new MyPoint3D(   0,   0, 0 )));
            Assert.IsTrue  (Utility.IsInsidePolygon(polygon, new MyPoint3D(   0,   1, 0 )));
        }

        [TestMethod]
        public void TestCalculateArea()
        {
            List<GeoPoint> polygon = new List<GeoPoint>();
            polygon.Add(new MyPoint3D(0, 0, 0).ToGeo());
            polygon.Add(new MyPoint3D(1, 0, 0).ToGeo());
            polygon.Add(new MyPoint3D(1, 1, 0).ToGeo());

            Assert.AreEqual(0.5, Utility.CalculateArea(polygon));
            polygon.Add(new MyPoint3D(0, 0, 0).ToGeo());
            Assert.AreEqual(0.5, Utility.CalculateArea(polygon));
            polygon.Add(new MyPoint3D(0, 0, 0).ToGeo());
            Assert.AreEqual(0.5, Utility.CalculateArea(polygon));


            polygon = new List<GeoPoint>();
            polygon.Add(new MyPoint3D(0, 0, 0).ToGeo());
            polygon.Add(new MyPoint3D(1, 0, 0).ToGeo());
            polygon.Add(new MyPoint3D(1, 1, 0).ToGeo());
            polygon.Add(new MyPoint3D(0, 1, 0).ToGeo());
            Assert.AreEqual(1, Utility.CalculateArea(polygon));
            polygon.Add(new MyPoint3D(0, 0, 0).ToGeo());
            Assert.AreEqual(1, Utility.CalculateArea(polygon));
        }

        [TestMethod]
        public void TestShiftPoint()
        {
            MyPoint3D p1 = new MyPoint3D(1, -1, 0);
            MyPoint3D p2 = new MyPoint3D(0, 0, 0);
            MyPoint3D p3 = new MyPoint3D(1, 1, 0);


            var p = Utility.ShiftPoint(p1, p2, p3, 12);
            Assert.AreEqual(0, p.Y);
            Assert.AreEqual(true, Math.Abs(-12 * Math.Sqrt(2) - p.X) < 0.00000000001);

            List<MyPoint3D> cartPolygon = new List<MyPoint3D>();
            cartPolygon.Add(new MyPoint3D(0, 1, 0));
            cartPolygon.Add(new MyPoint3D(1, 1, 0));
            cartPolygon.Add(new MyPoint3D(1, 0, 0));
            cartPolygon.Add(new MyPoint3D(0, 0, 0));
            cartPolygon.Add(new MyPoint3D(0, 1, 0));

            int i = 1;
            p = Utility.ShiftPoint(cartPolygon[i - 1], cartPolygon[i], cartPolygon[i + 1], 0.1);
            Assert.AreEqual(1.1, p.X);
            Assert.AreEqual(1.1, p.Y);

            i = 2;
            p = Utility.ShiftPoint(cartPolygon[i - 1], cartPolygon[i], cartPolygon[i + 1], 0.1);
            Assert.AreEqual(1.1, p.X);
            Assert.AreEqual(-0.1, p.Y);

            i = 3;
            p = Utility.ShiftPoint(cartPolygon[i - 1], cartPolygon[i], cartPolygon[i + 1], 0.1);
            Assert.AreEqual(-0.1, p.X);
            Assert.AreEqual(-0.1, p.Y);
        }

        [TestMethod]
        public void TestChooseHeadlandInterstion()
        {
            List<MyPoint3D> headland = new List<MyPoint3D>();
            //headland.Add(new MyPoint3D());
            headland.Add(new MyPoint3D() { X = 10 });
            headland.Add(new MyPoint3D() { Y = 10 });
            headland.Add(new MyPoint3D() { X = -10 });
            headland.Add(new MyPoint3D() { Y = -10 });
            headland.Add(new MyPoint3D() { X = 10 });
            //headland.Add(new MyPoint3D());

            LineDefinition line = new LineDefinition() { k = 1 };
            LineDefinition headlandLine = new LineDefinition() { k = -1, b = 10 };
            var intersection = Utility.ChooseHeadlandIntersection(headland, line, 2, headlandLine, new MyPoint3D() { X = 5, Y = 5 });
            Assert.AreEqual(5 - Math.Sqrt(2) / 2, intersection.X);
            Assert.AreEqual(5 - Math.Sqrt(2) / 2, intersection.Y);

            line = new LineDefinition() { k = -1 };
            headlandLine = new LineDefinition() { k = 1, b = -10 };
            intersection = Utility.ChooseHeadlandIntersection(headland, line, 2, headlandLine, new MyPoint3D() { X = 5, Y = -5 });
            Assert.AreEqual(true, Math.Abs(5 - Math.Sqrt(2) / 2 - intersection.X) < 0.00000001);
            Assert.AreEqual(true, Math.Abs(-5 + Math.Sqrt(2) / 2 - intersection.Y) < 0.00000001);

            line = new LineDefinition() { k = -1 };
            headlandLine = new LineDefinition() { k = 1, b = 10 };
            intersection = Utility.ChooseHeadlandIntersection(headland, line, 2, headlandLine, new MyPoint3D() { X = -5, Y = 5 });
            Assert.AreEqual(true, Math.Abs(-5 + Math.Sqrt(2) / 2 - intersection.X) < 0.00000001);
            Assert.AreEqual(true, Math.Abs(5 - Math.Sqrt(2) / 2 - intersection.Y) < 0.00000001);

            line = new LineDefinition() { k = 1 };
            headlandLine = new LineDefinition() { k = -1, b = -10 };
            intersection = Utility.ChooseHeadlandIntersection(headland, line, 2, headlandLine, new MyPoint3D() { X = -5, Y = -5 });
            Assert.AreEqual(true, Math.Abs(-5 + Math.Sqrt(2) / 2 - intersection.X) < 0.00000001);
            Assert.AreEqual(true, Math.Abs(-5 + Math.Sqrt(2) / 2 - intersection.Y) < 0.00000001);



            headland = new List<MyPoint3D>();
            headland.Add(new MyPoint3D() { X = 25.0 / 3 });
            headland.Add(new MyPoint3D() { Y = 25.0 / 4 });
            headland.Add(new MyPoint3D() { X = -25.0 / 3 });
            headland.Add(new MyPoint3D() { Y = -25.0 / 4 });
            headland.Add(new MyPoint3D() { X = 25.0 / 3 });

            line = new LineDefinition() { k = 4.0 / 3 };
            headlandLine = new LineDefinition() { k = -3.0 / 4, b = 25.0 / 4 };
            intersection = Utility.ChooseHeadlandIntersection(headland, line, 2, headlandLine, new MyPoint3D() { X = 3, Y = 4 });
            Assert.AreEqual(true, Math.Abs(3 - 3.0 / 5 - intersection.X) < 0.00000001);
            Assert.AreEqual(true, Math.Abs(4 - 4.0 / 5 - intersection.Y) < 0.00000001);

            line = new LineDefinition() { k = 4.0 / 3 };
            headlandLine = new LineDefinition() { k = -3.0 / 4, b = -25.0 / 4 };
            intersection = Utility.ChooseHeadlandIntersection(headland, line, 2, headlandLine, new MyPoint3D() { X = -3, Y = -4 });
            Assert.AreEqual(true, Math.Abs(-3 + 3.0 / 5 - intersection.X) < 0.00000001);
            Assert.AreEqual(true, Math.Abs(-4 + 4.0 / 5 - intersection.Y) < 0.00000001);



            headland = new List<MyPoint3D>();
            headland.Add(new MyPoint3D() { X = 90, Y = 10 });
            headland.Add(new MyPoint3D() { X = -30, Y = 90 });
            headland.Add(new MyPoint3D() { X = 70, Y = 125 });
            headland.Add(new MyPoint3D() { X = 90, Y = 10 });

            line = LineDefinition.GetLineDefinition(new MyPoint3D(), new MyPoint3D() { X = 15, Y = 60 });
            headlandLine = LineDefinition.GetLineDefinition(new MyPoint3D() { X = 90, Y = 10 }, new MyPoint3D() { X = -30, Y = 90 });
            intersection = Utility.ChooseHeadlandIntersection(headland, line, 10, headlandLine, new MyPoint3D() { X = 15, Y = 60 });
        }


    }
}

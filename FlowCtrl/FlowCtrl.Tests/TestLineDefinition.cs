using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TrimbleFIQ = FlowCtrl;

namespace FlowCtrl.Tests
{
    [TestClass]
    public class LineDefinitionTest
    {
        [TestMethod]
        public void TestLineDefinition()
        {
            TrimbleFIQ.GpsMath.MyPoint3D p1 = new TrimbleFIQ.GpsMath.MyPoint3D(0, 0, 0);
            TrimbleFIQ.GpsMath.MyPoint3D p2 = new TrimbleFIQ.GpsMath.MyPoint3D(1, 1, 0);

            var line = TrimbleFIQ.GpsMath.LineDefinition.GetLineDefinition(p1, p2);
            Assert.AreEqual(1, line.k);
            Assert.AreEqual(0, line.b);

            p1.X = 3;
            line = TrimbleFIQ.GpsMath.LineDefinition.GetLineDefinition(p1, p2);
            Assert.AreEqual(-0.5, line.k);
            Assert.AreEqual(1.5, line.b);
        }

        [TestMethod]
        public void TestLineIntersection()
        {
            TrimbleFIQ.GpsMath.MyPoint3D p1 = new TrimbleFIQ.GpsMath.MyPoint3D(0, 0, 0);
            TrimbleFIQ.GpsMath.MyPoint3D p2 = new TrimbleFIQ.GpsMath.MyPoint3D(1, 0.5, 0);
            var line1 = TrimbleFIQ.GpsMath.LineDefinition.GetLineDefinition(p1, p2);

            p1.X = 3;
            p2.Y = 1;
            var line2 = TrimbleFIQ.GpsMath.LineDefinition.GetLineDefinition(p1, p2);

            var intersection = TrimbleFIQ.GpsMath.LineDefinition.GetLineIntersection(line2, line1);
            Assert.AreEqual(1.5, intersection.X);
            Assert.AreEqual(0.75, intersection.Y);
        }

        [TestMethod]
        public void TestParallelLine()
        {
            TrimbleFIQ.GpsMath.LineDefinition line = new TrimbleFIQ.GpsMath.LineDefinition() { k = 0, b = 0 };
            var parallel = TrimbleFIQ.GpsMath.LineDefinition.GetParallelLine(line, 12);
            Assert.AreEqual(12, parallel.b);

            parallel = TrimbleFIQ.GpsMath.LineDefinition.GetParallelLine(line, -20);
            Assert.AreEqual(-20, parallel.b);

            line.k = 3;
            line.b = 4;
            parallel = TrimbleFIQ.GpsMath.LineDefinition.GetParallelLine(line, -3);
            double h = line.b - parallel.b;
            double expected = Math.Atan(3);
            expected = 3 / Math.Cos(expected);
            Assert.AreEqual(expected, h);


            line = TrimbleFIQ.GpsMath.LineDefinition.GetLineDefinition(
                new TrimbleFIQ.GpsMath.MyPoint3D(0, 0, 0), new TrimbleFIQ.GpsMath.MyPoint3D(0, 1, 0));
            Assert.AreEqual(-10, TrimbleFIQ.GpsMath.LineDefinition.GetParallelLine(line, 10).X);

            line = TrimbleFIQ.GpsMath.LineDefinition.GetLineDefinition(
                new TrimbleFIQ.GpsMath.MyPoint3D(0, 1, 0), new TrimbleFIQ.GpsMath.MyPoint3D(0, 0, 0));
            Assert.AreEqual(10, TrimbleFIQ.GpsMath.LineDefinition.GetParallelLine(line, 10).X);
        }
    }
}

using FlowCtrl.GpsMath;
using MapControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FlowCtrl.VM
{
    public class MainMapVM : INotifyPropertyChanged
    {
        ViewModelBase vm = new ViewModelBase();

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        ObservableCollection<FieldVM> _Fields;
        public ObservableCollection<FieldVM> Fields
        {
            get { return _Fields; }
            set { _Fields = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => Fields)); }
        }

        ViewModelCommand _SelectCommand;
        public ViewModelCommand SelectCommand
        {
            get
            {
                if (_SelectCommand == null)
                {
                    _SelectCommand = new ViewModelCommand(
                        p => Select(p),
                        p => CanSelect);
                }
                return _SelectCommand;
            }
        }
        void Select(object polygon)
        {
            if (selectedField != null)
                selectedField.Fill = greenBrush;
            FieldVM f = (FieldVM)polygon;
            f.Fill = darkBrush;
            selectedField = f;
            setCanProcessField();
        }
        bool _CanSelect = true;
        public bool CanSelect
        {
            get { return _CanSelect; }
            set { _CanSelect = value; SelectCommand.OnCanExecuteChanged(); }
        }

        SolidColorBrush greenBrush = new SolidColorBrush(Colors.LightGreen);
        SolidColorBrush darkBrush = new SolidColorBrush(Colors.DarkGreen);

        FieldVM selectedField;


        int _NumberOfSections = 6;
        public int NumberOfSections
        {
            get { return _NumberOfSections; }
            set { _NumberOfSections = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => NumberOfSections)); }
        }

        double _ImplementWidth = 30.5;
        public double ImplementWidth
        {
            get { return _ImplementWidth; }
            set 
            { 
                _ImplementWidth = value; 
                PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => ImplementWidth));
                PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => NumberOfNozzles));
            }
        }


        ObservableCollection<FlagVM> _Flags;
        public ObservableCollection<FlagVM> Flags
        {
            get { return _Flags; }
            set { _Flags = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => Flags)); }
        }

        FlagVM flagA;
        public void SetFlagA(Location location)
        {
            if (flagA != null)
                Flags.Remove(flagA);
            flagA = new FlagVM()
            {
                Location = location,
                Text = "A"
            };
            Flags.Add(flagA);
            setCanProcessField();
        }



        FlagVM flagB;
        public void SetFlagB(Location location)
        {
            if (flagB != null)
                Flags.Remove(flagB);
            flagB = new FlagVM()
            {
                Location = location,
                Text = "B"
            };
            Flags.Add(flagB);
            setCanProcessField();
        }

        ObservableCollection<VM.LineVM> _GuidanceLines = new ObservableCollection<LineVM>();
        public ObservableCollection<VM.LineVM> GuidanceLines
        {
            get { return _GuidanceLines; }
            set { _GuidanceLines = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => GuidanceLines)); }
        }

        ObservableCollection<VM.LineVM> _AB_Lines = new ObservableCollection<LineVM>();
        public ObservableCollection<VM.LineVM> AB_Lines
        {
            get { return _AB_Lines; }
            set { _AB_Lines = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => AB_Lines)); }
        }

        void setCanProcessField()
        {
            if (selectedField == null || flagA == null || flagB == null)
                CanProcessField = false;
            else
                CanProcessField = true;

            if (flagA != null && flagB != null)
            {
                AB_Lines.Clear();
                VM.LineVM line = new LineVM();
                line.Locations = new LocationCollection();
                line.Locations.Add(flagA.Location);
                line.Locations.Add(flagB.Location);
                AB_Lines.Add(line);
            }
        }
        ViewModelCommand _ProcessFieldCmd;
        public ViewModelCommand ProcessFieldCmd
        {
            get 
            {
                if (_ProcessFieldCmd == null)
                    _ProcessFieldCmd = new ViewModelCommand(
                        p => processField(),
                        p => CanProcessField);
                return _ProcessFieldCmd; 
            }
            set { _ProcessFieldCmd = value; }
        }
        bool _CanProcessField;
        public bool CanProcessField
        {
            get { return _CanProcessField; }
            set { _CanProcessField = value; ProcessFieldCmd.OnCanExecuteChanged(); }
        }
        void processField()
        {
            PointExtension.LocalLatitude = selectedField.Locations.Min(p => p.Latitude);
            LineDefinition lineDef_AB = LineDefinition.GetLineDefinition(flagA.Location.ToGeoPoint().ToCart(), flagB.Location.ToGeoPoint().ToCart());
            GuidanceLines.Clear();
            Swathes.Clear();
            AnalysisResult res = AnalyzeFieldAndShowSwaths(selectedField, lineDef_AB);

            AppliedArea = res.AppliedArea / 10000;
            FieldArea = res.FieldArea / 10000;
            //Savings = (res.AppliedArea - res.FieldArea) / 10000 * CostHa;
            SectionSaving = (res.AppliedArea - res.FieldArea) / 10000 * CostHa * (1 - 1.0 / NumberOfSections);
            SectionLoss = (res.AppliedArea - res.FieldArea) / 10000 * CostHa - SectionSaving;

            double totalNozzleSaving = (res.AppliedArea - res.FieldArea) / 10000 * CostHa * (1 - 1.0 / NumberOfNozzles);
            NozzleSaving = totalNozzleSaving - SectionSaving;
            NozzleLoss = (res.AppliedArea - res.FieldArea) / 10000 * CostHa - totalNozzleSaving;

            SectionPaybackHa = SectionControlPrice / (SectionSaving / AppliedArea);
            NozzlePaybackHa = (NozzleControlPrice - SectionControlPrice) / (NozzleSaving / AppliedArea);
        }

        private AnalysisResult AnalyzeFieldAndShowSwaths(FieldVM polygon, LineDefinition lineDef)
        {
            double processedArea = 0;
            double width = ImplementWidth;
            var geoPolygon = new List<GeoPoint>();
            foreach (var p in polygon.Locations)
                geoPolygon.Add(p.ToGeoPoint());

            List<MyPoint3D> cartPolygon = new List<MyPoint3D>();
            foreach (var p in polygon.Locations)
                cartPolygon.Add(p.ToGeoPoint().ToCart());

            double length = width / 2.0;
            if (Utility.CalculateArea(geoPolygon, false) < 0)
                length = -length;


            List<MyPoint3D> boundary = new List<MyPoint3D>();
            MyPoint3D prevP = cartPolygon[0];
            boundary.Add(cartPolygon[0]);
            for (int i = 1; i < cartPolygon.Count; i++)
            {
                if ((prevP - cartPolygon[i]).Norm < 3)
                    continue;
                prevP = cartPolygon[i];
                boundary.Add(cartPolygon[i]);
            }
            if ((boundary.First() - boundary.Last()).Norm < 3)
                boundary.Remove(boundary.Last());

            length = -length;
            List<MyPoint3D> headLand = Utility.ShiftBoundary(length, boundary);
            headLand.Add(headLand.First());
            List<MyPoint3D> innerBoundary = Utility.ShiftBoundary(2 * length, boundary);

            AddHeadLandToMap(headLand);
            AddOuterSwathToMap(boundary, innerBoundary);

            var intersections = Utility.GetHeadlandIntersections(headLand, lineDef, width);

            processedArea += width * ComputeParallelLines(width, headLand, lineDef, intersections, true).LinesLength;

            lineDef = LineDefinition.GetParallelLine(lineDef, -width);
            intersections = Utility.GetHeadlandIntersections(headLand, lineDef, width);
            processedArea += width * ComputeParallelLines(-width, headLand, lineDef, intersections, true).LinesLength;
            for (int i = 1; i < headLand.Count; i++)
                processedArea += width * (headLand[i - 1] - headLand[i]).Norm;

            AnalysisResult res = new AnalysisResult();
            res.AppliedArea = processedArea;
            res.FieldArea = Utility.CalculateArea(geoPolygon);
            return res;
        }

        private void AddHeadLandToMap(List<MyPoint3D> headLand)
        {
            VM.LineVM line = new LineVM();
            line.Locations = new LocationCollection();
            foreach (var p in headLand)
                line.Locations.Add(p.ToGeo().ToLocation());
            GuidanceLines.Add(line);
        }

        private void AddOuterSwathToMap(List<MyPoint3D> boundary, List<MyPoint3D> innerBoundary)
        {
            VM.PolygonVM polygon = new VM.PolygonVM();
            polygon.Fill = new SolidColorBrush(Colors.Blue);
            polygon.Fill.Opacity = 0.3;
            polygon.Locations = new LocationCollection();
            foreach (var p in boundary)
                polygon.Locations.Add(p.ToGeo().ToLocation());
            polygon.Locations.Add(polygon.Locations.First());

            polygon.Locations.Add(innerBoundary.First().ToGeo().ToLocation());
            for (int i = innerBoundary.Count - 1; i >= 0; i--)
                polygon.Locations.Add(innerBoundary[i].ToGeo().ToLocation());

            Swathes.Add(polygon);
        }

        private CompResult ComputeParallelLines(double width, List<MyPoint3D> cartPolygon, LineDefinition lineDef, List<MyPoint3D> intersections, bool addLinesToMap)
        {
            CompResult result = new CompResult();

            while (intersections.Count > 1)
            {
                intersections = intersections.Distinct(new MyPoint3D.MyPoint3D_EqualityComparer()).ToList();
                if (intersections.Min(p => p.Y) == intersections.Max(p => p.Y))
                    intersections.Sort(new MyPoint3D.MyPoint3D_ComparerByX());
                else
                    intersections.Sort(new MyPoint3D.MyPoint3D_ComparerByY());

                for (int i = 0; i < intersections.Count - 1; i += 2)
                {
                    var p1 = intersections[i + 1];
                    var p2 = intersections[i];
                    result.LinesLength += (p1 - p2).Norm;
                    result.LinesCount++;
                    if (addLinesToMap)
                    {
                        AddLineToMap(p1, p2);
                        AddSwathToMap(p1, p2, width);
                    }
                }
                lineDef = LineDefinition.GetParallelLine(lineDef, width);
                intersections = Utility.GetHeadlandIntersections(cartPolygon, lineDef, width);
            }
            return result;
        }

        private void AddSwathToMap(MyPoint3D p1, MyPoint3D p2, double width)
        {
            VM.PolygonVM polygon = new VM.PolygonVM();
            polygon.Fill = new SolidColorBrush(Colors.Blue);
            polygon.Fill.Opacity = 0.3;
            polygon.Locations = new LocationCollection();

            MyPoint3D n = (p2 - p1).Normalize();
            MyPoint3D np = new MyPoint3D(n.Y, -n.X, 0);
            polygon.Locations.Add((p1 + (np * (width / 2))).ToGeo().ToLocation());
            polygon.Locations.Add((p2 + (np * (width / 2))).ToGeo().ToLocation());
            polygon.Locations.Add((p2 - (np * (width / 2))).ToGeo().ToLocation());
            polygon.Locations.Add((p1 - (np * (width / 2))).ToGeo().ToLocation());

            Swathes.Add(polygon);
        }

        private void AddLineToMap(MyPoint3D p1, MyPoint3D p2)
        {
            VM.LineVM l = new VM.LineVM();
            l.Locations = new LocationCollection();
            l.Locations.Add(p1.ToGeo().ToLocation());
            l.Locations.Add(p2.ToGeo().ToLocation());
            GuidanceLines.Add(l);
        }

        ObservableCollection<VM.PolygonVM> _Swathes = new ObservableCollection<PolygonVM>();
        public ObservableCollection<VM.PolygonVM> Swathes
        {
            get { return _Swathes; }
            set { _Swathes = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => Swathes)); }
        }

        int _SectionControlPrice = 2700;
        public int SectionControlPrice
        {
            get { return _SectionControlPrice; }
            set { _SectionControlPrice = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => SectionControlPrice)); }
        }

        int _NozzleControlPrice = 30000;
        public int NozzleControlPrice
        {
            get { return _NozzleControlPrice; }
            set { _NozzleControlPrice = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => NozzleControlPrice)); }
        }

        public int NumberOfNozzles
        {
            get { return (int)(ImplementWidth * 2); }
        }

        int _CostHa = 50;
        public int CostHa
        {
            get { return _CostHa; }
            set { _CostHa = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => CostHa)); }
        }

        double _FieldArea;
        public double FieldArea
        {
            get { return _FieldArea; }
            set { _FieldArea = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => FieldArea)); }
        }
        double _AppliedArea;
        public double AppliedArea
        {
            get { return _AppliedArea; }
            set { _AppliedArea = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => AppliedArea)); }
        }

        double _SectionSaving;
        public double SectionSaving
        {
            get { return _SectionSaving; }
            set { _SectionSaving = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => SectionSaving)); }
        }

        double _SectionLoss;
        public double SectionLoss
        {
            get { return _SectionLoss; }
            set { _SectionLoss = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => SectionLoss)); }
        }

        double _NozzleSaving;
        public double NozzleSaving
        {
            get { return _NozzleSaving; }
            set { _NozzleSaving = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => NozzleSaving)); }
        }

        double _NozzleLoss;
        public double NozzleLoss
        {
            get { return _NozzleLoss; }
            set { _NozzleLoss = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => NozzleLoss)); }
        }




        VM.ViewModelCommand _DoAllFieldCmd;
        public VM.ViewModelCommand DoAllFieldCmd
        {
            get 
            {
                if (_DoAllFieldCmd == null)
                    _DoAllFieldCmd = new ViewModelCommand(
                        p => doAllFields(),
                        p => CanDoAllFields);
                return _DoAllFieldCmd; 
            }
            set { _DoAllFieldCmd = value; }
        }
        bool _CanDoAllFields = true;
        public bool CanDoAllFields
        {
            get { return _CanDoAllFields; }
            set { _CanDoAllFields = value; DoAllFieldCmd.OnCanExecuteChanged(); }
        }
        void doAllFields()
        {
            if (selectedField == null)
                if (Fields.Count > 0)
                    selectedField = Fields.First();

            GuidanceLines.Clear();
            Swathes.Clear();
            AnalysisResult.Clear();
            foreach (var poly in Fields)
            {
                PointExtension.LocalLatitude = poly.Locations.Min(p => p.Latitude);

                var geoPolygon = new List<GeoPoint>();
                foreach (var p in poly.Locations)
                    geoPolygon.Add(p.ToGeoPoint());

                List<MyPoint3D> cartPolygon = new List<MyPoint3D>();
                foreach (var p in poly.Locations)
                    cartPolygon.Add(p.ToGeoPoint().ToCart());

                var lineDef = LineDefinition.GetBestLine(cartPolygon);
                var res = AnalyzeFieldAndShowSwaths(poly, lineDef);
                if (double.IsNaN(res.AppliedArea))
                    continue;
                res.AppliedArea = res.AppliedArea / 10000;
                res.FieldArea = res.FieldArea / 10000;
                //res.Savings = (res.AppliedArea - res.FieldArea) * CostHa; 
                res.SectionSaving = (res.AppliedArea - res.FieldArea) * CostHa * (1 - 1.0 / NumberOfSections);
                res.SectionLoss = (res.AppliedArea - res.FieldArea) * CostHa - res.SectionSaving;
                res.FieldName = (AnalysisResult.Count + 1).ToString();
                AnalysisResult.Add(res);
            }
            AppliedArea = AnalysisResult.Sum(r => r.AppliedArea);
            FieldArea = AnalysisResult.Sum(r => r.FieldArea);
            SectionSaving = AnalysisResult.Sum(r => r.SectionSaving);
            SectionLoss = AnalysisResult.Sum(r => r.SectionLoss);

            double totalNozzleSaving = (AppliedArea - FieldArea) * CostHa * (1 - 1.0 / NumberOfNozzles);
            NozzleSaving = totalNozzleSaving - SectionSaving;
            NozzleLoss = (AppliedArea - FieldArea) * CostHa - totalNozzleSaving;

            SectionPaybackHa = SectionControlPrice / (SectionSaving / AppliedArea);
            NozzlePaybackHa = (NozzleControlPrice - SectionControlPrice) / (NozzleSaving / AppliedArea);
        }

        ObservableCollection<AnalysisResult> _AnalysisResult = new ObservableCollection<AnalysisResult>();
        public ObservableCollection<AnalysisResult> AnalysisResult
        {
            get { return _AnalysisResult; }
            set { _AnalysisResult = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => AnalysisResult)); }
        }

        double _SectionPaybackHa;
        public double SectionPaybackHa
        {
            get { return _SectionPaybackHa; }
            set { _SectionPaybackHa = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => SectionPaybackHa)); }
        }

        double _NozzlePaybackHa;
        public double NozzlePaybackHa
        {
            get { return _NozzlePaybackHa; }
            set { _NozzlePaybackHa = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => NozzlePaybackHa)); }
        }
    }


    public class AnalysisResult
    {
        public string FieldName { get; set; }
        public double FieldArea { get; set; }
        public double AppliedArea { get; set; }
        public double SectionSaving { get; set; }
        public double SectionLoss { get; set; }
    }

    class CompResult
    {
        public double LinesLength;
        public double LinesCount;
    }
}

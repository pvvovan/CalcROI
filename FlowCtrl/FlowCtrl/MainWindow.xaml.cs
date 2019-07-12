using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace FlowCtrl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm;
            vm.Flags = new System.Collections.ObjectModel.ObservableCollection<VM.FlagVM>();
            vm.Fields = new System.Collections.ObjectModel.ObservableCollection<VM.FieldVM>();
        }

        VM.MainMapVM vm = new VM.MainMapVM();
        SolidColorBrush greenBrush = new SolidColorBrush(Colors.LightGreen);

        private void btnPlus_Click(object sender, RoutedEventArgs e)
        {
            mainMap.ZoomMap(new Point(mainMap.ActualWidth / 2, mainMap.ActualHeight / 2), mainMap.ZoomLevel + 1);
        }
        private void btnMinus_Click(object sender, RoutedEventArgs e)
        {
            mainMap.ZoomMap(new Point(mainMap.ActualWidth / 2, mainMap.ActualHeight / 2), mainMap.ZoomLevel - 1);
        }

        private void OpenKml(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opfd = new OpenFileDialog();
            opfd.Filter = "Google Earth files *.kml|*.kml";
            var res = opfd.ShowDialog();
            if (res == true)
            {
                vm.Fields = new System.Collections.ObjectModel.ObservableCollection<VM.FieldVM>();

                XmlDocument document = new XmlDocument();
                document.Load(opfd.FileName);
                var places = document.GetElementsByTagName("Placemark");
                foreach (XmlElement place in places)
                {
                    var el = place.GetElementsByTagName("coordinates")[0];
                    try
                    {
                        VM.FieldVM polygon = new VM.FieldVM();
                        vm.Fields.Add(polygon);
                        polygon.Fill = greenBrush;
                        polygon.Locations = new MapControl.LocationCollection();
                        string[] coordStrs = ((System.Xml.XmlElement)el).InnerText.Split(' ');
                        CultureInfo ci = new CultureInfo("en-US");
                        foreach (var pointStr in coordStrs)
                        {
                            try
                            {
                                if (string.IsNullOrEmpty(pointStr))
                                    continue;
                                double longitude;
                                string[] pCoords = pointStr.Split(',');
                                longitude = double.Parse(pCoords[0].Trim(), ci);
                                double latitude = double.Parse(pCoords[1], ci);
                                polygon.Locations.Add(new MapControl.Location(latitude, longitude));
                            }
                            catch { }
                        }

                        var southWest = new MapControl.Location(polygon.Locations.Min(p => p.Latitude), polygon.Locations.Min(p => p.Longitude));
                        var northEast = new MapControl.Location(polygon.Locations.Max(p => p.Latitude), polygon.Locations.Max(p => p.Longitude));
                        mainMap.ZoomToBounds(southWest, northEast);
                    }
                    catch
                    { }
                }
            }
        }

        private void btnA_Checked(object sender, RoutedEventArgs e)
        {
            mainMap.Cursor = Cursors.Cross;
            mainMap.PreviewMouseLeftButtonUp += mainMap_MouseLeftUp_SetA;
            vm.CanSelect = false;
            btnB.IsEnabled = false;
        }

        void mainMap_MouseLeftUp_SetA(object sender, MouseButtonEventArgs e)
        {
            var l = mainMap.ViewportPointToLocation(e.GetPosition(mainMap));
            vm.SetFlagA(l);
            btnA.IsChecked = false;
        }
        private void btnA_Unchecked(object sender, RoutedEventArgs e)
        {
            mainMap.Cursor = Cursors.Arrow;
            mainMap.PreviewMouseLeftButtonUp -= mainMap_MouseLeftUp_SetA;
            vm.CanSelect = true;
            btnB.IsEnabled = true;
        }



        private void btnB_Checked(object sender, RoutedEventArgs e)
        {
            mainMap.Cursor = Cursors.Cross;
            mainMap.PreviewMouseLeftButtonUp += mainMap_MouseLeftUp_SetB;
            vm.CanSelect = false;
            btnA.IsEnabled = false;
        }

        void mainMap_MouseLeftUp_SetB(object sender, MouseButtonEventArgs e)
        {
            var l = mainMap.ViewportPointToLocation(e.GetPosition(mainMap));
            vm.SetFlagB(l);
            btnB.IsChecked = false;
        }
        private void btnB_Unchecked(object sender, RoutedEventArgs e)
        {
            mainMap.Cursor = Cursors.Arrow;
            mainMap.PreviewMouseLeftButtonUp -= mainMap_MouseLeftUp_SetB;
            vm.CanSelect = true;
            btnA.IsEnabled = true;
        }


        VM.FieldVM fieldDrawn;
        private void DrawField(object sender, RoutedEventArgs e)
        {
            menuDrawField.IsEnabled = false;
            menuStopDrawField.IsEnabled = true;
            mainMap.Cursor = Cursors.Cross;
            mainMap.PreviewMouseLeftButtonUp += mainMap_MouseLeftUp_AddPointToField;
            fieldDrawn = new VM.FieldVM();
            fieldDrawn.Locations = new MapControl.LocationCollection();
            fieldDrawn.Fill = greenBrush;
            vm.Fields.Add(fieldDrawn);
        }

        void mainMap_MouseLeftUp_AddPointToField(object sender, MouseButtonEventArgs e)
        {
            MapControl.Location l = mainMap.ViewportPointToLocation(e.GetPosition(mainMap));
            fieldDrawn.Locations.Add(l);
        }

        private void StopDrawingField(object sender, RoutedEventArgs e)
        {
            menuDrawField.IsEnabled = true;
            menuStopDrawField.IsEnabled = false;
            mainMap.Cursor = Cursors.Arrow;
            mainMap.PreviewMouseLeftButtonUp -= mainMap_MouseLeftUp_AddPointToField;
            if (fieldDrawn.Locations.Count < 3)
                vm.Fields.Remove(fieldDrawn);
        }

        private void menuEnglish_Click(object sender, RoutedEventArgs e)
        {
            menuEnglish.IsChecked = true;
            menuRussian.IsChecked = false;
            menuLanguage.Header = "Language";
            menuFile.Header = "File";
            menuOpen.Header = "Open";
            menuField.Header = "Field";
            menuDrawField.Header = "Draw field";
            menuStopDrawField.Header = "Complete drawing";
            lblImplementWidth.Text = "Implement width, m";
            lblMaterial.Text = "Material, $/ha";
            btnSelectedField.Content = "Selected field";
            lblFieldArea.Text = "Field area, ha: ";
            lblAppliedArea.Text = "Applied area, ha: ";
            lblSavings.Text = "Saving, $:";
            lblLoss.Text = "Loss, $:";
            btnAllFields.Content = "All fields";
            columnName.Header = "Field";
            columnArea.Header = "Area, ha";
            columnApplied.Header = "Applied, ha";
            columnSavings.Header = "Saving, $";
            columnLoss.Header = "Loss, $";
            lblAutoSecCtrl.Content = "Automatic Section Control";
            txtNumberOfSections.Text = "Number of sections:";
            lblSectionControl.Text = "Section control:";
            lblNozzleControl.Text = "Nozzle control:";
            lblPayBack.Text = "Payback, ha";
            lblSectionCtrlPrice.Text = "Section control price, $:";
            lblNozzleCtrlPrice.Text = "Nozzle control price, $:";
            lblAdditional.Text = "additional ";
        }

        private void menuRussian_Click(object sender, RoutedEventArgs e)
        {
            menuEnglish.IsChecked = false;
            menuRussian.IsChecked = true;
            menuLanguage.Header = "Язык";
            menuFile.Header = "Файл";
            menuOpen.Header = "Открыть";
            menuField.Header = "Поле";
            menuDrawField.Header = "Нарисовать поле";
            menuStopDrawField.Header = "Закончить рисование";
            lblImplementWidth.Text = "Ширина орудия, м";
            lblMaterial.Text = "Материал, $/га";
            btnSelectedField.Content = "Выбраное поле";
            lblFieldArea.Text = "Площадь поля (полей), га:";
            lblAppliedArea.Text = "Площадь обработки, га:";
            lblSavings.Text = "Экономия, $:";
            lblLoss.Text = "Потери, $:";
            btnAllFields.Content = "Все поля";
            columnName.Header = "Поле";
            columnArea.Header = "Площадь, га";
            columnApplied.Header = "Обработано, га";
            columnSavings.Header = "Экономия, $";
            columnLoss.Header = "Потери, $";
            lblAutoSecCtrl.Content = "Авто управление секциями";
            txtNumberOfSections.Text = "Количество секций:";
            lblSectionControl.Text = "Количество секций:";
            lblNozzleControl.Text = "форсунок:";
            lblPayBack.Text = "Окупаемость, га";
            lblSectionCtrlPrice.Text = "Цена контроля секций, $:";
            lblNozzleCtrlPrice.Text = "Цена контроля форсунок, $:";
            lblAdditional.Text = "дополнит. ";
        }
    }
}

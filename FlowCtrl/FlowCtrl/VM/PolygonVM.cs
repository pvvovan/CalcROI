using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FlowCtrl.VM
{
    public class PolygonVM : INotifyPropertyChanged
    {
        protected ViewModelBase vm = new ViewModelBase();
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        MapControl.LocationCollection _Locations;
        public MapControl.LocationCollection Locations
        {
            get { return _Locations; }
            set { _Locations = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => Locations)); }
        }

        Brush _Fill;
        public Brush Fill
        {
            get { return _Fill; }
            set { _Fill = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => Fill)); }
        }

        protected void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged(this, e);
        }
    }
}

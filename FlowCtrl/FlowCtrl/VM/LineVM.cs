using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCtrl.VM
{
    public class LineVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        ViewModelBase vm = new ViewModelBase();

        MapControl.LocationCollection _Locations;
        public MapControl.LocationCollection Locations
        {
            get { return _Locations; }
            set { _Locations = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => Locations)); }
        }
    }
}

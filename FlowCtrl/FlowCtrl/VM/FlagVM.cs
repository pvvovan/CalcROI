using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCtrl.VM
{
    public class FlagVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        ViewModelBase vm = new ViewModelBase();

        MapControl.Location _Location;
        public MapControl.Location Location
        {
            get { return _Location; }
            set { _Location = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => Location)); }
        }


        string _Text;
        public string Text
        {
            get { return _Text; }
            set { _Text = value; PropertyChanged(this, vm.GetPropertyChangedEventArgs(() => Text)); }
        }
    }
}

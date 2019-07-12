using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FlowCtrl.VM
{
    public class FieldVM : PolygonVM
    {

        string _FieldCaption;
        public string FieldCaption
        {
            get { return _FieldCaption; }
            set { _FieldCaption = value; InvokePropertyChanged(vm.GetPropertyChangedEventArgs(() => FieldCaption)); }
        }

        MapControl.Location _CaptionLocation;
        public MapControl.Location CaptionLocation
        {
            get { return _CaptionLocation; }
            set { _CaptionLocation = value; InvokePropertyChanged(vm.GetPropertyChangedEventArgs(() => CaptionLocation)); }
        }
        
    }
}

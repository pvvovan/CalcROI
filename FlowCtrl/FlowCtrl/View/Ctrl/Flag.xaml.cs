using MapControl;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace FlowCtrl.View.Ctrl
{
    /// <summary>
    /// Interaction logic for Pushin.xaml
    /// </summary>
    public partial class Flag : UserControl, IMapElement
    {
        public Flag()
        {
            InitializeComponent();
            MouseLeftButtonDown += Flag_MouseLeftButtonDown;
        }

        void Flag_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ParentMap.OnLeftMouseButtonDown(e);
        }

        public static readonly DependencyProperty FlagTextProperty = DependencyProperty.Register("FlagText", typeof(string), typeof(Flag));
        public string FlagText
        {
            get
            {
                return (string)GetValue(FlagTextProperty);
            }
            set
            {
                SetValue(FlagTextProperty, value);
            }
        }

        public MapBase ParentMap
        {
            get;
            set;
        }
    }
}

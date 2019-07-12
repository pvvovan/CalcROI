using System.Windows;
using System.Windows.Controls;

namespace MapControl
{
    public class MapLabel : ContentControl
    {
        static MapLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(MapLabel), new FrameworkPropertyMetadata(typeof(MapLabel)));
        }
    }
}

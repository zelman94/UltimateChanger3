using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WPF.Bootstrap.Controls
{
    public class TabInPanel : TabControl
    {
        public Brush PanelBackground
        {
            get { return (Brush)GetValue(PanelBackgroundProperty); }
            set { SetValue(PanelBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PanelBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PanelBackgroundProperty =
            DependencyProperty.Register("PanelBackground", typeof(Brush), typeof(TabInPanel),
                (PropertyMetadata)new FrameworkPropertyMetadata((object)null, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));



    }
}

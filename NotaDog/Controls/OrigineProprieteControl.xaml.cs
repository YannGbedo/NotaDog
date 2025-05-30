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
using NotaDog.Data;
using UserControl = System.Windows.Controls.UserControl;

namespace NotaDog.Controls
{
    /// <summary>
    /// Logique d'interaction pour OrigineProprieteControl.xaml
    /// </summary>
    public partial class OrigineProprieteControl : UserControl
    {
        public OrigineProprieteControl()
        {
            InitializeComponent();

            OwnershipOrigin ??= new OwnershipOrigin();
        }

        public OwnershipOrigin OwnershipOrigin
        {
            get { return (OwnershipOrigin)GetValue(OwnershipOriginProperty); }
            set { SetValue(OwnershipOriginProperty, value); }
        }

        public static readonly DependencyProperty OwnershipOriginProperty =
            DependencyProperty.Register("OwnershipOrigin", typeof(OwnershipOrigin), typeof(OrigineProprieteControl), new PropertyMetadata(new OwnershipOrigin()));
    }
}

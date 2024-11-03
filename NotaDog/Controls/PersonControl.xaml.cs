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
using UserControl = System.Windows.Controls.UserControl;
using NotaDog.Data;

namespace NotaDog.Controls
{
    /// <summary>
    /// Logique d'interaction pour PersonControl.xaml
    /// </summary>
    public partial class PersonControl : UserControl
    {
        public PersonControl()
        {
            InitializeComponent();
        }

        // Propriété de dépendance pour la liaison de données
        public Person Person
        {
            get { return (Person)GetValue(PersonProperty); }
            set { SetValue(PersonProperty, value); }
        }

        public static readonly DependencyProperty PersonProperty =
            DependencyProperty.Register("Person", typeof(Person), typeof(PersonControl), new PropertyMetadata(new Person()));
    }
}

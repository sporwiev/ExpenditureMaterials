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

namespace BifServiceExpenditureMaterials.Controls
{
    /// <summary>
    /// Логика взаимодействия для Card.xaml
    /// </summary>
    public partial class Card : UserControl
    {
        public Card()
        {
            InitializeComponent();
            DataContext = this;
        }
        public static DependencyProperty TitleCardProperty = DependencyProperty.Register(
            "TitleCard",
            typeof(string),
            typeof(Card),
            new PropertyMetadata(null)
        );
        public static DependencyProperty DateCardProperty = DependencyProperty.Register(
            "Date",
            typeof(string),
            typeof(Card),
            new PropertyMetadata(null)
        );
        public static DependencyProperty CountCardProperty = DependencyProperty.Register(
            "Count",
            typeof(string),
            typeof(Card),
            new PropertyMetadata(null)
        );
        public static DependencyProperty StatusCardProperty = DependencyProperty.Register(
            "Status",
            typeof(string),
            typeof(Card),
            new PropertyMetadata(null)
        );
        public new string TitleCard
        { 
            get { return (string)GetValue(TitleCardProperty); } 
            set { SetValue(TitleCardProperty,value); } 
        }
        public string Date
        {
            get { return (string)GetValue(DateCardProperty); }
            set { SetValue(DateCardProperty, value); }
        }
        public string Count
        {
            get { return (string)GetValue(CountCardProperty); }
            set { SetValue(CountCardProperty, value); }
        }

        public string Status 
        {
            get { return (string)GetValue(StatusCardProperty); }
            set { SetValue(StatusCardProperty, value); }
        }
    }
}

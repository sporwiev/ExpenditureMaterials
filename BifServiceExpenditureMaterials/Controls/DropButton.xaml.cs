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
    /// Логика взаимодействия для DropButton.xaml
    /// </summary>
    public partial class DropButton : UserControl
    {
        public DropButton()
        {

            InitializeComponent();
            HighPanel = panel1.Children;
            LowPanel = panel2.Children;

        }
        DependencyProperty HighPanelproperty = DependencyProperty.Register(
        
            "HighPanel",
            typeof(UIElementCollection),
            typeof(DropButton),
            new PropertyMetadata(null) 
            );
        DependencyProperty LowPanelproperty = DependencyProperty.Register(

            "LowPanel",
            typeof(UIElementCollection),
            typeof(DropButton),
            new PropertyMetadata(null)
            );
        public UIElementCollection HighPanel
        {

            get { return (UIElementCollection)GetValue(HighPanelproperty); }

            set { SetValue(HighPanelproperty, value); }

        }
        public UIElementCollection LowPanel
        {

            get { return (UIElementCollection)GetValue(LowPanelproperty); }

            set { SetValue(LowPanelproperty, value); }

        }

    }
}

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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BifServiceExpenditureMaterials.Helpers;

namespace BifServiceExpenditureMaterials.Controls
{
    /// <summary>
    /// Логика взаимодействия для OliControl.xaml
    /// </summary>
    [ContentProperty("SubPanel")]
    public partial class OliControl : UserControl
    {

        private bool isOpen = false;

        public static DependencyProperty brendTitleProperty = DependencyProperty.Register(

            "brendTitle",
            typeof(string),
            typeof(OliControl),
            new PropertyMetadata(null)

            );

        public string brendTitle { get { return (string)GetValue(brendTitleProperty); } set { SetValue(brendTitleProperty, value); } }

        public static DependencyProperty vilocityTitleProperty = DependencyProperty.Register(

            "vilocityTitle",
            typeof(string),
            typeof(OliControl),
            new PropertyMetadata(null)

            );

        public string vilocityTitle { get { return (string)GetValue(vilocityTitleProperty); } set { SetValue(vilocityTitleProperty, value); } }

        public static DependencyProperty valueTitleProperty = DependencyProperty.Register(

            "valueTitle",
            typeof(string),
            typeof(OliControl),
            new PropertyMetadata(null)

            );

        public string valueTitle { get { return (string)GetValue(valueTitleProperty); } set { SetValue(valueTitleProperty, value); } }

        public static DependencyProperty subunitNameTitleProperty = DependencyProperty.Register(

            "subunitnameTitle",
            typeof(string),
            typeof(OliControl),
            new PropertyMetadata(null)

            );

        public string subunitnameTitle { get { return (string)GetValue(subunitNameTitleProperty); } set { SetValue(subunitNameTitleProperty, value); } }

        public static DependencyProperty subunitCountTitleProperty = DependencyProperty.Register(

            "subunitcountTitle",
            typeof(string),
            typeof(OliControl),
            new PropertyMetadata(null)

            );
        

        public string subunitcountTitle { get { return (string)GetValue(subunitCountTitleProperty); } set { SetValue(subunitCountTitleProperty, value); } }

        public static DependencyProperty SubPanelProperty = DependencyProperty.Register(

            "SubPanel",
            typeof(UIElementCollection),
            typeof(OliControl),
            new PropertyMetadata(null)

            );

        public UIElementCollection SubPanel { get { return (UIElementCollection)GetValue(SubPanelProperty); } set { SetValue(SubPanelProperty, value); } }

        public OliControl()
        {
            InitializeComponent();
            DataContext = this;
            SubPanel = panel.Children;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isOpen)
            {
                VisiblePanel.Visibility = Visibility.Visible;
                trsnalateImage.Angle = -90;
            }
            else
            {
                VisiblePanel.Visibility = Visibility.Collapsed;
                trsnalateImage.Angle = 90;
            }
            isOpen = !isOpen;
        }
    }
}

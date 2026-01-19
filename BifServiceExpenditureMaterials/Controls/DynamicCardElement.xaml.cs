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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BifServiceExpenditureMaterials.Views.Pages;
using BifServiceExpenditureMaterials.Views.Windows;

namespace BifServiceExpenditureMaterials.Controls
{
    /// <summary>
    /// Логика взаимодействия для DynamicCardElement.xaml
    /// </summary>
    public partial class DynamicCardElement : UserControl
    {
        public bool isOpenPanel = false;
        public static DependencyProperty TypeBackgroundOnMaterialProperty = DependencyProperty.Register(
            "TypeBackgroundOnMaterial",
            typeof(SolidColorBrush),
            typeof(DynamicCardElement),
            new PropertyMetadata(null)

            );
        public static DependencyProperty TitleCardProperty = DependencyProperty.Register(
            "TitleCardMaterial",
            typeof(string),
            typeof(DynamicCardElement),
            new PropertyMetadata(null)

            );
        public static DependencyProperty ResponsibleProperty = DependencyProperty.Register(
            
            "Responsible",
            typeof(string),
            typeof(DynamicCardElement),
            new PropertyMetadata(null)

            );
        public static DependencyProperty YearProperty = DependencyProperty.Register(
            
            "Year",
            typeof(string),
            typeof(DynamicCardElement),
            new PropertyMetadata(null)

            );
        public static DependencyProperty MonthProperty = DependencyProperty.Register(
            
            "Month",
            typeof(string),
            typeof(DynamicCardElement),
            new PropertyMetadata(null)
            );

        public static DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message",
            typeof(string),
            typeof(DynamicCardElement),
            new PropertyMetadata(null)
            );
        public SolidColorBrush TypeBackgroundOnMaterial { get { return (SolidColorBrush)GetValue(TypeBackgroundOnMaterialProperty); } set { SetValue(TypeBackgroundOnMaterialProperty, value); } }

        public static Point CoordinatesPoint { get; set; } 
        public string TitleCard { get { return (string)GetValue(TitleCardProperty); } set { SetValue(TitleCardProperty, value); } }
        public string Responsible { get { return (string)GetValue(ResponsibleProperty); } set { SetValue(ResponsibleProperty, value); } }
        public string Year { get { return (string)GetValue(YearProperty); } set { SetValue(YearProperty, value); } }
        public string Month { get { return (string)GetValue(MonthProperty); } set { SetValue(MonthProperty, value); } }
        public string Message { get { return (string)GetValue(MessageProperty); } set { SetValue(MessageProperty, value); } }

        


        public DynamicCardElement(Color backgroundCard)
        {
            InitializeComponent();
            DataContext = this;
            typebackgroundcolor.Color = backgroundCard;
   
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animationHeightGridOpen = new DoubleAnimation()
            {
                From = isOpenPanel ? 52 : 200,
                To = isOpenPanel ? 200 : 52

            };
            
            panelMessage.BeginAnimation(RowDefinition.HeightProperty, animationHeightGridOpen);
            isOpenPanel = !isOpenPanel;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            HomePage.isUpdate = true;
            MainWindow.window.RootNavigation.Navigate(typeof(HomePage));
            //HomePage.GetTabItemByHeader(HomePage.tab,)
           // HomePage.SelectedTab(HomePage.GetIndexByHeaderOnTab(Month));

            HomePage.ViewOnPoint((int)CoordinatesPoint.X, (int)CoordinatesPoint.Y);
        }
    }
}

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
using BifServiceExpenditureMaterials.Controls;
using BifServiceExpenditureMaterials.Database;

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для DynamicViewMaterialElement.xaml
    /// </summary>
    public partial class DynamicViewMaterialElement : UserControl
    {
        private int index = 0;

        public DynamicViewMaterialElement()
        {
            InitializeComponent();
        }
       
        private void Border_GotFocusLeft(object sender, RoutedEventArgs e)
        {
            OpacityAnimationOpen(sender);
        }
        private void Border_GotFocusRight(object sender, RoutedEventArgs e)
        {
            OpacityAnimationOpen(sender);
        }
        private void Border_LostFocusRight(object sender,RoutedEventArgs e)
        {
            OpacityAnimationClose(sender);
        }
        private void Border_LostFocusLeft(object sender, RoutedEventArgs e)
        {
            OpacityAnimationClose(sender);
        }
        public void OpacityAnimationOpen(object sender)
        {
            var border = (Button)sender;
            DoubleAnimation animation = new DoubleAnimation() 
            { 
                From = 0,
                To = 1,
            };
            border.BeginAnimation(Border.OpacityProperty, animation);
        }
        public void OpacityAnimationClose(object sender)
        {
            var border = (Button)sender;
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = 1,
                To = 0,
            };
            border.BeginAnimation(Border.OpacityProperty, animation);
        }

        private void Button_ClickLeft(object sender, RoutedEventArgs e)
        {

            if (index - 1 < 0)
                index = 0;
            else
                index--;
            CreateAnimate();
            UpdateCardPanel(index);
            CloseAnimate();
        }
        private void Button_ClickRight(object sender, RoutedEventArgs e)
        {
            var outcount = App.dBcontext.Materials.Count();
            if (index + 1 > outcount)
                index = outcount;
            else
                index++;
            
            CreateAnimate();
            UpdateCardPanel(index);
            CloseAnimate();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCardPanel(index);
        }
        private void CreateAnimate()
        {
            var animate = new DoubleAnimation()
            {
                From = 1,
                To = 0,
            };
            panelDynamicCards.BeginAnimation(StackPanel.OpacityProperty, animate);

        }
        private void CloseAnimate()
        {
            var animate = new DoubleAnimation()
            {
                From = 0,
                To = 1,
            };
            panelDynamicCards.BeginAnimation(StackPanel.OpacityProperty, animate);
        }
        private void UpdateCardPanel(int i)
        {
            if (panelDynamicCards.Children.Count > 0)
                panelDynamicCards.Children.Clear();
            var material = App.dBcontext.Materials.Where(s => s.Год == DateTime.Now.Year).ToList()[i];
            var color = material.ТипТраты == "ТО" ? Color.FromRgb(220, 20, 60) : Color.FromRgb(0, 250, 154);
            var x = Convert.ToDouble(material.Ячейка.Split(":")[0]);
            var y = Convert.ToDouble(material.Ячейка.Split(":")[1]);
            DynamicCardElement.CoordinatesPoint = new Point(x,y);
            panelDynamicCards.Children.Add(new DynamicCardElement(color) { Responsible = material.Ответственный, Year = material.Год.ToString(), Month = material.Месяц, Message = material.message, TitleCard = material.НомерЯчейки });
        }
    }
}

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

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для VisualizeExpenditure.xaml
    /// </summary>
    public partial class VisualizeExpenditure : UserControl
    {
        private bool isDragging = false;
        private Point mouseOffset;
        private Point position;
        public VisualizeExpenditure()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        private void UserControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double scaleStep = 0.1;
            double newScaleX = CanvasScale.ScaleX;
            double newScaleY = CanvasScale.ScaleY;
            
            if (e.Delta > 0)
            {
                newScaleX += scaleStep;
                newScaleY += scaleStep;
                scaleStep -= 0.01;
            }
            else
            {
                newScaleX -= scaleStep;
                newScaleY -= scaleStep;
                scaleStep += 0.01;

            }
            CanvasScale.ScaleX = newScaleX;
            CanvasScale.ScaleY = newScaleY;
        }

        

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var element = sender as UIElement;
                position = e.GetPosition(mainCanvas);
                CanvasTranslate.X = position.X - mouseOffset.X;
                CanvasTranslate.Y = position.Y - mouseOffset.Y;
                    //Canvas.SetLeft(element, position.X - mouseOffset.X);
                    //Canvas.SetTop(element, position.Y - mouseOffset.Y);
            }
        }
        private void mainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as UIElement;
            isDragging = true;
            mouseOffset = e.GetPosition(mainCanvas);
            element.CaptureMouse();
        }

        private void mainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var element = sender as UIElement;
            isDragging = false;
            CanvasTranslate.X = position.X - mouseOffset.X;
            CanvasTranslate.Y = position.Y - mouseOffset.Y;
            element.ReleaseMouseCapture();
        }
    }
}

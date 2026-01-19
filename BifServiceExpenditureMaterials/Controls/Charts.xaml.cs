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
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.Helpers;
using BifServiceExpenditureMaterials.Views.Pages;
using BifServiceExpenditureMaterials.Views.Windows;

namespace BifServiceExpenditureMaterials.Controls
{
    /// <summary>
    /// Логика взаимодействия для Charts.xaml
    /// </summary>
    public partial class Charts : UserControl
    {
        private List<int?,string?,string?,string?> _valuesOldMonth = new List<int?,string?,string?,string?>();
        private List<int?, string?, string?, string?> _valuesNewMonth = new List<int?, string?, string?, string?>();
        private ScaleTransform ScaleTransformLineEllipse = new ScaleTransform();
        private TranslateTransform TranslateTransformEllipse = new TranslateTransform();
        private double pointX = 0;

        // Свойство для установки данных
        //public List<int?,string,string> Values
        //{
        //    get => (_values,_names_dates);
        //    set
        //    {
        //        _values = value ?? new List<int?>();
        //        DrawChart();
        //    }
        //}
        //public List<string?> Dates
        //{
        //    get => _dates;
        //    set
        //    {
        //        _dates = value ?? new List<string?>();
        //        DrawChart();
        //    }
        //}
        public List<int?,string?,string?,string?> ValuesOldMonth
        {
            get => _valuesOldMonth;
            set
            {
                _valuesOldMonth = value ?? new List<int?, string?, string?, string?>();
                DrawChart();
            }
        }
        public List<int?, string?, string?, string?> ValuesNewMonth
        {
            get => _valuesNewMonth;
            set
            {
                _valuesNewMonth = value ?? new List<int?, string?, string?, string?>();
                DrawChart();
            }
        }
        //public List<string?> Names
        //{
        //    get => _names;
        //    set
        //    {
        //        _names = value ?? new List<string?>();
        //        DrawChart();
        //    }
        //}
        public Charts()
        {
            InitializeComponent();
            this.SizeChanged += (s, e) => DrawChart();
        }
        public void DrawChart()
        {
            ChartCanvas.Children.Clear();

           // if (_valuesOldMonth == null || _valuesOldMonth.Count == 0 || _valuesNewMonth == null || _valuesNewMonth.Count == 0)
            //    return;

            double width = ChartCanvas.ActualWidth;
            double height = ChartCanvas.ActualHeight;

            if (width == 0 || height == 0)
                return;

            double minValOld = double.MaxValue;
            double maxValOld = double.MinValue;
            double minValNew = double.MaxValue;
            double maxValNew = double.MinValue;

            foreach (var val in _valuesOldMonth)
            {
                if (val.Item1 < minValOld) minValOld = (double)val.Item1;
                if (val.Item1 > maxValOld) maxValOld = (double)val.Item1;
            }
            foreach (var val in _valuesNewMonth)
            {
                if (val.Item1 < minValNew) minValNew = (double)val.Item1;
                if (val.Item1 > maxValNew) maxValNew = (double)val.Item1;
            }
            if (maxValNew == minValNew)
                maxValNew = minValNew + 1;
            if (maxValOld == minValOld)
                maxValOld = minValOld + 1; // избежать деления на 0

            double margin = 20;
            double usableWidth = width - 2 * margin;
            double usableHeight = height - 2 * margin;

            // Расстояние между точками по X
            double stepXOld = usableWidth / (_valuesOldMonth.Count - 1);
            double stepXNew = usableWidth / (_valuesNewMonth.Count - 1);
            // Функция перевода значения Y (значение -> координата)
            Func<double, double> valueToYOld = valOld =>
                margin + usableHeight - ((valOld - minValOld) / (maxValOld - minValOld)) * usableHeight;

            Func<double, double> valueToYNew = valNew =>
               margin + usableHeight - ((valNew - minValNew) / (maxValNew - minValNew)) * usableHeight;

            // Рисуем линии
            for (int i = 0; i < _valuesOldMonth.Count - 1; i++)
            {

                var line = new Line
                {
                    X1 = margin + i * stepXOld ,
                    Y1 = valueToYOld((double)_valuesOldMonth[i].Item1),
                    X2 = margin + (i + 1) * stepXOld ,
                    Y2 = valueToYOld((double)_valuesOldMonth[i + 1].Item1),
                    Stroke = Brushes.BlueViolet,
                    StrokeThickness = 2,
                    //RenderTransform = ScaleTransformLineEllipse
                };
                
                ChartCanvas.Children.Add(line);
            }
            for (int i = 0; i < _valuesNewMonth.Count - 1; i++)
            {

                var line = new Line
                {
                    X1 = margin + i * stepXNew,
                    Y1 = valueToYNew((double)_valuesNewMonth[i].Item1),
                    X2 = margin + (i + 1) * stepXNew,
                    Y2 = valueToYNew((double)_valuesNewMonth[i + 1].Item1),
                    Stroke = Brushes.OrangeRed,
                    StrokeThickness = 2,
                    //RenderTransform = ScaleTransformLineEllipse
                };

                ChartCanvas.Children.Add(line);
            }
            // Рисуем точки с цветом в зависимости от значения
            for (int i = 0; i < _valuesOldMonth.Count; i++)
            {
                ToolTip tool = new ToolTip();
                tool.Content = "Дата: " + (_valuesOldMonth[i].Item3 == null ? "" : _valuesOldMonth[i].Item3) + "\r\nМашина:" + (_valuesOldMonth[i].Item2 == null ? "" : _valuesOldMonth[i].Item2) + "\r\nИспользовано продукта: " + (_valuesOldMonth[i].Item1 == null ? "" : _valuesOldMonth[i].Item1);
                var value = _valuesOldMonth[i];
                Ellipse ellipse = new Ellipse
                {
                    
                    Width = 12,
                    Height = 12,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Fill = GetColorForValue((double)value.Item1, minValOld, maxValOld),
                    ToolTip = tool,
                    //RenderTransform = ScaleTransformLineEllipse
                };
                ellipse.MouseRightButtonDown += (sender, e) => Ellipse_MouseRightButtonDown(sender,e, value.Item2, value.Item3,value.Item4);
                

                double cx = margin + i * stepXOld;
                double cy = valueToYOld((double)value.Item1);

                Canvas.SetLeft(ellipse, cx - ellipse.Width / 2);
                Canvas.SetTop(ellipse, cy - ellipse.Height / 2);

                ChartCanvas.Children.Add(ellipse);
            }
            for (int i = 0; i < _valuesNewMonth.Count; i++)
            {
                ToolTip tool = new ToolTip();
                tool.Content = "Дата: " + (_valuesNewMonth[i].Item3 == null ? "" : _valuesNewMonth[i].Item3) + "\r\nМашина:" + (_valuesNewMonth[i].Item2 == null ? "" : _valuesNewMonth[i].Item2) + "\r\nИспользовано продукта: " + (_valuesNewMonth[i].Item1 == null ? "" : _valuesNewMonth[i].Item1);
                var value = _valuesNewMonth[i];
                Ellipse ellipse = new Ellipse
                {

                    Width = 12,
                    Height = 12,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Fill = GetColorForValue((double)value.Item1, minValNew, maxValNew),
                    ToolTip = tool,
                    //RenderTransform = ScaleTransformLineEllipse
                };
                ellipse.MouseRightButtonDown += (sender, e) => Ellipse_MouseRightButtonDown(sender, e, value.Item2, value.Item3, value.Item4);


                double cx = margin + i * stepXNew;
                double cy = valueToYNew((double)value.Item1);

                Canvas.SetLeft(ellipse, cx - ellipse.Width / 2);
                Canvas.SetTop(ellipse, cy - ellipse.Height / 2);

                ChartCanvas.Children.Add(ellipse);
            }
            var max = maxValNew > maxValOld ? maxValNew : maxValOld;
            foreach (var item in panelindexY.Children)
            {
                if(item is TextBlock text)
                {
                    if (max < 0)
                    {
                        text.Text = "0";
                    }
                    else
                    {
                        text.Text = max.ToString();
                    }
                    max /= 2;
                    
                }
            }
        }

        private void Ellipse_MouseRightButtonDown(object sender, MouseButtonEventArgs e,string number,string date,string Ячейка)
        {
            //MainWindow.window.RootNavigation.Navigate(typeof(HomePage));
            //var datagrid = HomePage.GetTable().Datagrid;
            //HomePage._home.UpdateData(HomePage.GetTabItemByHeader(HomePage.tab, BifServiceExpenditureMaterials.Helpers.Other.GetMouthNumber(DateTime.Now.Month)), true);
            //HomePage.GetTable().SearchCell(datagrid, Convert.ToInt32(Ячейка.Split(":")[0]), Convert.ToInt32(Ячейка.Split(":")[1]));
        }

        // Логика определения цвета точки по значению
        private Brush GetColorForValue(double value, double min, double max)
        {
            double range = max - min;
            if (value < min + range / 3)
                return Brushes.Green;     // низкий уровень
            else if (value < min + 2 * range / 3)
                return Brushes.Blue;      // средний уровень
            else
                return Brushes.Red;       // высокий уровень
        }

        //private void Border_MouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    if (e.Delta > 0)
        //    {
        //        pointX += 0.01;

        //    }
        //    else
        //    {
        //        pointX -= 0.01;
        //    }
        //    foreach (var item in ChartCanvas.Children)
        //        {
        //            if (item is Ellipse ellipse)
        //            {
        //                if (e.Delta > 0)
        //                {
        //                    TranslateTransformEllipse.X += 0.01;
        //                }
        //                else
        //                {
        //                    TranslateTransformEllipse.X -= 0.01;
        //                }
        //                ellipse.RenderTransform = TranslateTransformEllipse;
        //            }
        //            if (item is Line line)
        //            {
        //                if (e.Delta > 0)
        //                {
        //                    line.X2 += 1 + pointX;
        //                    line.X1 += 1;
        //                }
        //                else
        //                {
        //                    line.X2 -= 1 - pointX;
        //                    line.X1 -= 1;
        //                }
        //            }
        //        }
            
        //}
    }
}

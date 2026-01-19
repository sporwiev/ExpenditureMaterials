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
using System.Windows.Shapes;
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.material;
using static System.Net.Mime.MediaTypeNames;

namespace BifServiceExpenditureMaterials.Forms
{
    /// <summary>
    /// Логика взаимодействия для FormAddOil.xaml
    /// </summary>
    public partial class FormAddOil : Window
    {
        material.Oil oil { get; set; }

        public material.Oil SelectedOil => oil;

        public int index = 0;
        public FormAddOil()
        {
            InitializeComponent();
            Loaded += FormAddOil_Loaded;
        }

        private void FormAddOil_Loaded(object sender, RoutedEventArgs e)
        {
            
            {
                {
                    OilTypeComboBox.ItemsSource = App.dBcontext.oiltypefields.Select(s => s.name).ToList();
      
                    OilTypeComboBox.SelectionChanged += OilTypeComboBox_SelectionChanged;
                    OilBrandComboBox.SelectionChanged += OilBrandComboBox_SelectionChanged;
                }
            }
            
        }

        private void OilTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (OilTypeComboBox.SelectedValue != null)
                    
                    {
                    var b = OilTypeComboBox.SelectedValue.ToString();
                        var code1 = App.dBcontext.oiltypefields.Where(s => s.name == b).First().Id;
                    //MessageBox.Show(code1.name.ToString());
                    var a = App.dBcontext.oilbrandfields.Where(a => a.code.ToString() == code1.ToString()).Select(a => a.name).ToList();// Select(a => a.name).ToList();
                    OilBrandComboBox.ItemsSource = a;

                    }
            }
            catch
            {
                return;
            }


        }
        private void OilBrandComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (OilBrandComboBox.SelectedValue != null)
                    
                    {
                        var code = App.dBcontext.oilbrandfields.Where(s => s.name == OilBrandComboBox.SelectedValue.ToString()).First().Id;
                        //MessageBox.Show(code1.name.ToString());
                        var a = App.dBcontext.oilvilocityfields.OrderBy(s => s.Id).Where(s => s.code.ToString() == code.ToString()).Select(s => s.name).ToList();
                    OilViscosityComboBox.ItemsSource = a;

                    }
            }
            catch
            {
                return;
            }
            finally
            {

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            oil = new material.Oil();
            int i = 0;
            foreach (var item in panelSubunit.Children)
            {
                if (i >= 1)
                {
                    var stack = item as StackPanel;
                    var name = (stack.Children[1] as ComboBox).Text;
                    var value = Convert.ToInt32((stack.Children[2] as TextBox).Text);
                    oil.subUnits.Add(new material.Subunit() { Count = value, Name = name });
                }
                i++;
            }
                
            oil.type = OilTypeComboBox.Text;
            oil.brend = OilBrandComboBox.Text;
            oil.vilocity = OilViscosityComboBox.Text;
            oil.value = OilLitersTextBox.Text;
            DialogResult = true;

        }
        public StackPanel CreateUiElement()
        {
            var stackpanel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
            
            {
                var label = new TextBlock()
                {
                    Width = 90,
                    Text = "Часть",
                    VerticalAlignment = VerticalAlignment.Center,
                };
                var combo = new ComboBox()
                {
                    ItemsSource = App.dBcontext.subunit.Select(s => s.Name).ToList(),
                    Background = Brushes.LightGray,
                    Width = 140,
                    SelectedIndex = 0,

                };
                var input = new TextBox()
                {
                    Margin = new Thickness(10, 0, 0, 0),
                    Width = 60,
                    Text = "0"
                };
                
                var button = new Button()
                {
                    Margin = new Thickness(10, 0, 0, 0),
                    Content = "❌",
                    VerticalAlignment = VerticalAlignment.Center,
                };
                button.Click += DeleteItem_Click;
                stackpanel.Children.Add(label);
                stackpanel.Children.Add(combo);
                stackpanel.Children.Add(input);
                stackpanel.Children.Add(button);
            }
            return stackpanel;
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            int i = 0;
            var result = 0;
            foreach (var item in panelSubunit.Children)
            {
                if (i >= 1)
                {
                    var stack = item as StackPanel;
                    var inputval = string.IsNullOrEmpty((stack.Children[2] as TextBox).Text) || !int.TryParse((stack.Children[2] as TextBox).Text,out int num) ? "0" : (stack.Children[2] as TextBox).Text;
                    var value = Convert.ToInt32(inputval);
                    result += value;
                }
                i++;
            }
            OilLitersTextBox.Text = result.ToString();
        }

        public StackPanel CreateUiElement(int i,int countvalue)
        {
            var stackpanel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
            
            {
                var label = new TextBlock()
                {
                    Width = 90,
                    Text = "Часть",
                    VerticalAlignment = VerticalAlignment.Center,
                };
                var combo = new ComboBox()
                {
                    ItemsSource = App.dBcontext.subunit.Select(s => s.Name).ToList(),
                    Background = Brushes.LightGray,
                    Width = 140,
                    SelectedIndex = i,

                };
                var input = new TextBox()
                {
                    Margin = new Thickness(10, 0, 0, 0),
                    Width = 60,
                    Text = countvalue.ToString()
                };
                input.TextChanged += Input_TextChanged;
                var button = new Button()
                {
                    Margin = new Thickness(10, 0, 0, 0),
                    Content = "❌",
                    VerticalAlignment = VerticalAlignment.Center,
                };
                button.Click += DeleteItem_Click;
                stackpanel.Children.Add(label);
                stackpanel.Children.Add(combo);
                stackpanel.Children.Add(input);
                stackpanel.Children.Add(button);
            }
            return stackpanel;
        }
        private int Getindex(object sender)
        {
            int index = 0;
            foreach(var item in panelSubunit.Children)
            {
                var stack = (StackPanel)item;
                if(stack.Children[3] is Button && (Button)stack.Children[3] == (Button)sender)
                {
                    return index;
                }
                index++;
            }
            return 0;
            
        }
        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            panelSubunit.Children.RemoveAt(Getindex(sender));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            panelSubunit.Children.Add(CreateUiElement());
        }

        private void OilLitersTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            
            {
                
                var count = App.dBcontext.subunit.Count();
                if (!string.IsNullOrEmpty(OilLitersTextBox.Text)) {
                    var subvalue = Convert.ToInt32(OilLitersTextBox.Text) / count;
                    for (int i = 0; i < count - 1; i++)
                    {
                        panelSubunit.Children.Add(CreateUiElement(i, subvalue));
                    }
                }
            }
        }
    }
}

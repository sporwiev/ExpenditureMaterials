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
using Wpf.Ui.Controls;

namespace BifServiceExpenditureMaterials.Forms
{
    /// <summary>
    /// Логика взаимодействия для FormAddFilters.xaml
    /// </summary>
    public partial class FormAddFilters : Window
    {
        List<string> activeFiltersName = new List<string>();
        List<string> activeFiltersCount = new List<string>();

        public int MaxHeightWindow = 0; 
        public List<string> SelectedFiltersName => activeFiltersName;
        public List<string> SelectedFiltersCount => activeFiltersCount;
        public FormAddFilters()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
        }
        private void CollectActiveFilters()
        {
            activeFiltersName.Clear();
            activeFiltersCount.Clear();

            void FindToggleSwitches(DependencyObject parent)
            {
                int count = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);

                    if (child is StackPanel panel)
                    {
                        ToggleSwitch? toggle = null;
                        ComboBox? combo = null;

                        foreach (var element in panel.Children)
                        {
                            if (element is ToggleSwitch ts) toggle = ts;
                            if (element is ComboBox cb) combo = cb;
                        }

                        if (toggle?.IsChecked == true)
                        {
                            string toggleLabel = toggle.OffContent?.ToString() ?? "Без названия";
                            string comboText = combo?.Text ?? "";
                            activeFiltersName.Add(toggleLabel);
                            activeFiltersCount.Add(comboText);
                        }
                    }

                    // рекурсивно
                    FindToggleSwitches(child);
                }
            }

            FindToggleSwitches(this);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            FillWindowsOnFields();
            FillAllComboBoxes();
            Height += 50;
        }

        private void FillWindowsOnFields()
        {
            
            {
                var filters = App.dBcontext.Filter.Select(x => x.Name).ToList();
                foreach (var filter in filters)
                {
                    glpan.Children.Add(CreateField(filter));
                    Height += 40;
                }
            }
        }
        private StackPanel CreateField(string text)
        {
            StackPanel panel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(5),};
            ToggleSwitch switches = new ToggleSwitch() { OffContent = text, OnContent = text, Width = 220, Margin = new Thickness(0,0,30,0) };

            ComboBox cb = new ComboBox() { Width = 120, Background = Brushes.Gray, Foreground = Brushes.White};
            //switches.Checked += (sender,e) => Switches_Checked(sender,e,cb);
            switches.Click += (sender,e) => Switches_Click(sender, e, cb);
            panel.Children.Add(switches);
            panel.Children.Add(cb);
            return panel;
        }

        private void Switches_Click(object sender, RoutedEventArgs e, ComboBox combo)
        {
            ToggleSwitch switches = (ToggleSwitch)sender;
            if (switches.IsChecked == true)
            {
                
                combo.SelectedIndex = 0;
            }
            else
            {
               
                combo.SelectedIndex = -1;
            }
            //e.Handled = true;
        }

        

        private void FillAllComboBoxes()
        {
            void Fill(DependencyObject parent)
            {
                int count = VisualTreeHelper.GetChildrenCount(parent);
                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(parent, i);

                    if (child is ComboBox comboBox)
                    {
                        comboBox.ItemsSource = Enumerable.Range(1, 100).ToList();
                    }

                    Fill(child); // рекурсивно
                }
            }

            Fill(this);
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            activeFiltersName.Clear();
            activeFiltersCount.Clear();
            CollectActiveFilters();
            DialogResult = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

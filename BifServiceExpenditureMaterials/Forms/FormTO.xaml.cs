using BifServiceExpenditureMaterials.Autopattern;
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.Helpers;
using BifServiceExpenditureMaterials.Views.Pages;
using BifServiceExpenditureMaterials.Views.Windows;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BifServiceExpenditureMaterials.Controls;
using BifServiceExpenditureMaterials.material;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections;
using BifServiceExpenditureMaterials.Models;
using Microsoft.AspNetCore.SignalR;


namespace BifServiceExpenditureMaterials.Forms
{
    /// <summary>
    /// Логика взаимодействия для FormTO.xaml
    /// </summary>
    public partial class FormTO : Window
    {
        System.Timers.Timer timer;
        public string nomer { get; set; }
        public string yache { get; set; }
        public int year { get; set; }

        [DllImport("user32.dll")]
        private static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;

        private List<material.Oil> listOils = new List<material.Oil>();

        private List<string> listSubs = new List<string>();

        private int oilindexpanel = 1;

        public FormTO(string НомерЯчейки, string Ячейка, bool isWrite, int year)
        {
            Closing += FormTO_Closing;
            InitializeComponent();
            this.year = year;
            if (isWrite)
            {
                nomer = НомерЯчейки;
                yache = Ячейка;

            }
            else
            {

                GetData(НомерЯчейки);
            }

        }

        private void FormTO_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            HomePage.RefreshDataGrid();
        }

        public FormTO(Material material, CountMaterials countmaterial, string nomer, string yache)
        {
            InitializeComponent();
            SetData(material, countmaterial);
            this.nomer = nomer;
            this.yache = yache;
        }
        public async void SetData(Material? material, CountMaterials countmaterial)
        {
            
            {

                {
                    AntifreezeColorComboBox.ItemsSource = App.dBcontext.antifreezecolorfields.Select(s => s.name).ToList();
                    AntifreezeColorComboBox.SelectionChanged += AntifreezeColorComboBox_SelectionChanged;
                    AntifreezeBrandComboBox.SelectionChanged += AntifreezeBrandComboBox_SelectionChanged;
                }
                {
                    GreaseBrandComboBox.ItemsSource = App.dBcontext.greasebrandfields.Select(s => s.name).ToList();
                    GreaseBrandComboBox.SelectionChanged += GreaseBrandComboBox_SelectionChanged;
                    GreaseTypeComboBox.SelectionChanged += GreaseTypeComboBox_SelectionChanged;
                }
                {
                    MotorTypeComboBox.ItemsSource = new List<string>() { "Низ", "Верх", "Низ-верх" };
                }
                {
                    ResponsibleComboBox.ItemsSource = GetPersone();
                }

                string[][] oil = null;
                string[] anti;
                string[] grease;
                int indexoil = 0;
                foreach (var item in material.oilCode.Split(","))
                {
                    //if ((bool)(App.dBcontext?.Oil?.Where(s => s.Id == material.oil_id).Any()))
                    //{
                    //    var oilobject = App.dBcontext?.Oil?.Where(s => s.Id == material.oil_id).Select(s => s.Name).First().Split("_");
                    //    oil = [[indexoil.ToString()],[oilobject[0],oilobject[1],oilobject[2]]];
                    //}
                    //else
                    //{
                    //    oil = [[""], ["","",""]];

                    //}
                    indexoil++;
                }
                if ((bool)(App.dBcontext?.Antifreeze?.Where(s => s.Id.ToString() == material.antifreeze_id.ToString()).Any()))
                {
                    anti = App.dBcontext?.Antifreeze?.Where(s => s.Id.ToString() == material.antifreeze_id.ToString()).Select(s => s.Name).First().Split("_");
                }
                else
                {
                    anti = ["", "", ""];
                }
                if ((bool)(App.dBcontext?.Grease.Where(s => s.Id.ToString() == material.grease_id.ToString()).Any()))
                {
                    grease = App.dBcontext?.Grease.Where(s => s.Id.ToString() == material.grease_id.ToString()).Select(s => s.Name).First().Split("_");
                }
                else
                {
                    grease = ["", "", ""];
                }

                #region Oil
                foreach (var item in oil)
                {
                    MessageBox.Show(item.ToString());
                }
                #endregion Oil
                #region Antifreeze
                for (int i = 0; i < AntifreezeColorComboBox.Items.Count; i++)
                {
                    if (AntifreezeColorComboBox.Items[i].ToString() == anti?[0])
                    {
                        AntifreezeColorComboBox.SelectedIndex = i;
                        for (int j = 0; j < AntifreezeBrandComboBox.Items.Count; j++)
                        {
                            if (AntifreezeBrandComboBox.Items[j].ToString() == anti?[1])
                            {
                                AntifreezeBrandComboBox.SelectedIndex = j;
                                for (int t = 0; t < AntifreezeTypeComboBox.Items.Count; t++)
                                {
                                    if (AntifreezeTypeComboBox.Items[t].ToString() == anti?[2])
                                    {
                                        AntifreezeTypeComboBox.SelectedIndex = t;
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
                AntifreezeLitersTextBox.Text = countmaterial.count_antifreeze.ToString();
                #endregion Antifreeze
                #region Grease 
                for (int i = 0; i < GreaseBrandComboBox.Items.Count; i++)
                {
                    if (GreaseBrandComboBox.Items[i].ToString() == grease?[0])
                    {
                        GreaseBrandComboBox.SelectedIndex = i;
                        for (int j = 0; j < GreaseTypeComboBox.Items.Count; j++)
                        {
                            if (GreaseTypeComboBox.Items[j].ToString() == grease?[1])
                            {
                                GreaseTypeComboBox.SelectedIndex = j;
                                for (int t = 0; i < GreaseViscosityComboBox.Items.Count; t++)
                                {
                                    if (GreaseViscosityComboBox.Items[t].ToString() == grease?[2])
                                    {
                                        GreaseViscosityComboBox.SelectedIndex = t;
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    }
                }


                GreaseAmountTextBox.Text = countmaterial.count_grease.ToString();
                #endregion Grease
                #region Filters
                var filterscount = countmaterial?.count_filter?.Split(":").ToList();
                var filtersname = countmaterial?.count_filtername?.Split(":").ToList();
                foreach (var filter in filterscount ?? new List<string>() { "" })
                {
                    FilterListViewCount.Items.Add(filter);
                }

                foreach (var filter in filtersname ?? new List<string>() { "" })
                {
                    FilterListViewName.Items.Add(filter);
                }
                #endregion Filters
                #region Other
                MotoHoursTextBox.Text = countmaterial?.count_other_clock?.ToString();
                MileageTextBox.Text = countmaterial?.count_other_milesage?.ToString();
                for (int i = 0; i < MotorTypeComboBox.Items.Count - 1; i++)
                {
                    if (MotorTypeComboBox.Items[i].ToString() == countmaterial?.count_motors)
                    {
                        MotorTypeComboBox.SelectedIndex = i;
                    }
                }
                #endregion Other
                #region Responsible
                int l = 0;
                stackResponseble.Children.Clear();
                for (int i = 0; i < material?.Ответственный?.Split("|").Count() - 1; i++)
                {
                    var otvets = material?.Ответственный?.Split("|")[i];
                    StackPanel panel = new() { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
                    TextBlock text = new() { Margin = new Thickness(20, 0, 40, 0), VerticalAlignment = VerticalAlignment.Center, Text = "Ответственный" };
                    ComboBox combo = new() { ItemsSource = GetPersone(), Background = Brushes.LightGray, Width = 430 };
                    for (int j = 0; j < combo.Items.Count; j++)
                    {
                        var imy = combo.Items[j].ToString();
                        if (otvets == imy)
                        {
                            combo.SelectedIndex = j;
                            break;
                        }
                    }
                    panel.Children.Add(text);
                    panel.Children.Add(combo);
                    stackResponseble.Children.Add(panel);

                    l++;
                }
                #endregion Responsible
            }
        }

        public void addOilCard(string names,Material material)
        {
            int index = 0;
            int i = 1;
            int j = 1;
            names = names.Substring(0, names.Length - 1);
            
            foreach (var item in names.Split("*"))
            {

                OliControl control = new OliControl()
                {
                    Margin = new Thickness(0, 3, 0, 3),
                    brendTitle = item.Split("_")[1],
                    vilocityTitle = item.Split("_")[2],
                    valueTitle = App.dBcontext.CountMaterials.Where(s => s.Id == material.countmaterial_id).First().count_oil.Split(",")[index++],
                };
                if (material.subunit_id != null)
                {
                    foreach (var item2 in material.subunit_id.Split("*"))
                    {
                        if(i == material.subunit_id.Split("*").Length)
                        {
                            break;
                        }
                        StackPanel stack = new StackPanel();
                        foreach (var item3 in item2.Split(","))
                        {
                            if(j == item2.Split(",").Length)
                            {
                                break;
                            }
                            stack = new StackPanel() { Margin = new Thickness(3) };
                            stack.Children.Add(new TextBlock() { Text = App.dBcontext.subunit.Where(s => s.Id == Convert.ToInt32(item3)).First().Name });
                            stack.Children.Add(new TextBlock() { Text = material.CountMaterials.count_subunit.Split("*")[i].Split(",")[j] });
                            control.SubPanel.Add(stack);
                            j++;
                        }
                        i++;

                    }
                }
                control.VisiblePanel.Visibility = Visibility.Visible;
                panelOil.Children.Add(control);
                //index++;
            }
        }

        public async void GetData(string НомерЯчейки)
        {
            Loaded -= Window_Loaded;
            
            {
                int? id = 0;
                var material = App.dBcontext?.Materials?.ToList().Where(e => e.НомерЯчейки == НомерЯчейки).ToList()[0];
                MessageButton.ToolTip = material.message == null ? "" : material.message;
                string names = "";

                if (material.oilCode != null && material.oilCode != "")
                {
                    if (material.oilCode.IndexOf(",") != -1 )
                    {
                        foreach (var item in material.oilCode.Split(","))
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                names += App.dBcontext.Oil.Where(s => s.Id.ToString() == item).First().Name.ToString() + "*";
                            }
                        }
                        var countMat = App.dBcontext.CountMaterials.Where(s => s.Id == material.countmaterial_id);
                        addOilCard(names, material);
                    }
                    else
                    {
                        names = App.dBcontext.Oil.Where(s => s.Id.ToString() == material.oilCode).First().Name.ToString();
                        OliControl control = new OliControl()
                        {

                            brendTitle = names.Split("_")[1],
                            vilocityTitle = names.Split("_")[2],
                            valueTitle = App.dBcontext.CountMaterials.Where(s => s.Id.ToString() == material.countmaterial_id.ToString()).First().count_oil
                        };
                        panelOil.Children.Add(control);
                    }

                }
                else
                {
                    if(material.oil_id != null && material.oil_id.ToString() != "")
                    {
                        
                            names += App.dBcontext.Oil.Where(s => s.Id.ToString() == material.oil_id.ToString()).First().Name.ToString() + "*";
                            var countMat = App.dBcontext.CountMaterials.Where(s => s.Id.ToString() == material.countmaterial_id.ToString());
                            OliControl control = new OliControl()
                            {

                                brendTitle = names.Split("_")[1],
                                vilocityTitle = names.Split("_")[2],
                                valueTitle = App.dBcontext.CountMaterials.Where(s => s.Id.ToString() == material.countmaterial_id.ToString()).First().count_oil
                            };
                            panelOil.Children.Add(control);
                        
                    }
                }
                    //var oil = material.oil_id == null ? ["", "", "", "", "", "", ""] : App.dBcontext.Oil.OrderBy(e => e.Id).Where(e => e.Id == material.oil_id).First().Name.Split('_');
                var anti = (material.antifreeze_id == null || material.antifreeze_id == 0) ? ["", "", "", "", "", "", ""] : App.dBcontext.Antifreeze.OrderBy(e => e.Id).Where(e => e.Id.ToString() == material.antifreeze_id.ToString()).ToList()[0].Name.Split('_');
                var grease = (material.grease_id == null || material.grease_id == 0) ? ["", "", "", "", "", "", ""] : App.dBcontext.Grease.OrderBy(e => e.Id).Where(e => e.Id.ToString() == material.grease_id.ToString()).ToList()[0].Name.Split('_');
                //var clock = App.dBcontext.CountMaterials.OrderBy(e => e.Id).Where(e => e.Id == material.countmaterial_id).ToList()[0].count_other_clock;
                var countMaterial = App.dBcontext.CountMaterials.FirstOrDefault(e => e.Id.ToString() == material.countmaterial_id.ToString());
                if (countMaterial == null) return;
                var clock = countMaterial.count_other_clock;
                var mileage = App.dBcontext.CountMaterials.OrderBy(e => e.Id).Where(e => e.Id.ToString() == material.countmaterial_id.ToString()).ToList()[0].count_other_milesage;
                var motors = App.dBcontext.CountMaterials.OrderBy(e => e.Id).Where(e => e.Id.ToString() == material.countmaterial_id.ToString()).ToList()[0].count_motors;
                var respone = material?.Ответственный == null ? "" : material?.Ответственный;
                if (respone != null)
                {
                    var text = "";

                    for (int i = 0; i < respone.Split("|").Count(); i++)// var item in respone.Split("|"))
                    {
                        var tr = respone.Split("|")[i];
                        text += tr + "\r\n";

                    }
                    stackResponseble.Children.Clear();
                    stackResponseble.Children.Add(new TextBlock() { Text = text, Margin = new Thickness(20, 0, 0, 0) });
                }
                id = material?.Id;

                AntifreezeColorComboBox.Items.Add(anti[0]);
                AntifreezeColorComboBox.SelectedIndex = 0;
                AntifreezeBrandComboBox.Items.Add(anti[1]);
                AntifreezeBrandComboBox.SelectedIndex = 0;
                AntifreezeTypeComboBox.Items.Add(anti[2]);
                AntifreezeTypeComboBox.SelectedIndex = 0;
                AntifreezeLitersTextBox.Text = material?.CountMaterials?.count_antifreeze?.ToString();
                GreaseBrandComboBox.Items.Add(grease[0]);
                GreaseBrandComboBox.SelectedIndex = 0;
                GreaseTypeComboBox.Items.Add(grease[1]);
                GreaseTypeComboBox.SelectedIndex = 0;
                GreaseViscosityComboBox.Items.Add(grease[2]);
                GreaseViscosityComboBox.SelectedIndex = 0;
                GreaseAmountTextBox.Text = material?.CountMaterials?.count_grease?.ToString();
                MotoHoursTextBox.Text = clock.ToString();
                MileageTextBox.Text = mileage.ToString();
                MotorTypeComboBox.Items.Add(motors);
                MotorTypeComboBox.SelectedIndex = 0;
                ResponsibleComboBox.Items.Add(respone);
                ResponsibleComboBox.SelectedIndex = 0;
                var filters = App.dBcontext.CountMaterials.OrderBy(e => e.Id).Where(e => e.Id.ToString() == material.countmaterial_id.ToString()).ToList();
                if (filters.Count != 0)
                {
                    foreach (var filter in filters)
                    {
                        if (filter.count_filterids.Length == 1)
                        {
                            FilterListViewCount.Items.Add(filter.count_filter);
                            FilterListViewName.Items.Add(filter.count_filtername);
                        }
                        else
                        {
                            for (int i = 0; i < filter.count_filterids.Split(":").Count(); i++)
                            {
                                FilterListViewCount.Items.Add(filter.count_filter.Split(":")[i]);
                                FilterListViewName.Items.Add(filter.count_filtername.Split(":")[i]);
                            }
                        }
                    }
                }
                //await App.dBcontext.DisposeAsync();

            }
        }
        public List<string> GetPersone()
        {
            return new List<string>()
            {
                    "Пятых Андрей Анатольевич",
                    "Джонни",
                    "Илья",
                    "Артем",
                    "Антон",
                    "Аксас Абдельхафид",
                    "Муравкин Юрий",
                    "Павлов Игорь",
                    "Плотников Иван",
                    "Резников Степан",
                    "Сергеев Денис",
                    "Степанов Максим",
                    "Цыба Валентин",

            };
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            {


                {
                    AntifreezeColorComboBox.ItemsSource = App.dBcontext.antifreezecolorfields.Select(s => s.name).ToList();
                    AntifreezeColorComboBox.SelectionChanged += AntifreezeColorComboBox_SelectionChanged;
                    AntifreezeBrandComboBox.SelectionChanged += AntifreezeBrandComboBox_SelectionChanged;
                }
                {
                    GreaseBrandComboBox.ItemsSource = App.dBcontext.greasebrandfields.Select(s => s.name).ToList();
                    GreaseBrandComboBox.SelectionChanged += GreaseBrandComboBox_SelectionChanged;
                    GreaseTypeComboBox.SelectionChanged += GreaseTypeComboBox_SelectionChanged;
                }
                {
                    MotorTypeComboBox.ItemsSource = new List<string>() { "Низ", "Верх", "Низ-верх" };
                }
                {
                    ResponsibleComboBox.ItemsSource = GetPersone();
                }
            }
        }

        private void GreaseTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (GreaseTypeComboBox.SelectedValue != null)
                {
                    if (GreaseTypeComboBox.SelectedValue.ToString() == "Смазка_стрелы_Т")
                    {
                        GreaseViscosityComboBox.ItemsSource = new List<string> { "NULL" };
                        GreaseViscosityComboBox.SelectedIndex = 0;
                        return;
                    }
                    
                    {

                        if (GreaseTypeComboBox.SelectedValue != null)
                        {
                            var code = App.dBcontext.greasetypefields.Where(s => s.name == GreaseTypeComboBox.SelectedValue.ToString()).First().Id;
                            GreaseViscosityComboBox.ItemsSource = App.dBcontext.greasevilocityfields.OrderBy(s => s.Id).Where(s => s.code.ToString() == code.ToString()).Select(s => s.name).ToList();
                        }
                    }
                }
            }
            catch
            {
                return;
            }
        }

        private void GreaseBrandComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (GreaseBrandComboBox.SelectedValue != null)
                    
                    {
                        var code1 = App.dBcontext.greasebrandfields.Where(s => s.name == GreaseBrandComboBox.SelectedValue.ToString()).First().Id;
                        //MessageBox.Show(code1.name.ToString());
                        GreaseTypeComboBox.ItemsSource = App.dBcontext.greasetypefields.OrderBy(s => s.Id).Where(s => s.code.ToString() == code1.ToString()).Select(s => s.name).ToList();

                    }
            }
            catch
            {
                return;
            }
        }

        private void AntifreezeBrandComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (AntifreezeBrandComboBox.SelectedValue != null)
                    
                    {
                        var code = App.dBcontext.antifreezebrandfields.Where(s => s.name == AntifreezeBrandComboBox.SelectedValue.ToString()).First().Id;
                        //MessageBox.Show(code1.name.ToString());
                        AntifreezeTypeComboBox.ItemsSource = App.dBcontext.antifreezetypefields.OrderBy(s => s.Id).Where(s => s.code.ToString() == code.ToString()).Select(s => s.name).ToList();

                    }
            }
            catch
            {
                return;
            }
        }




        private void AntifreezeColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (AntifreezeColorComboBox.SelectedValue != null)
                    
                    {
                        var code = App.dBcontext.antifreezecolorfields.Where(s => s.name == AntifreezeColorComboBox.SelectedValue.ToString()).First().Id;
                        //MessageBox.Show(code1.name.ToString());
                        AntifreezeBrandComboBox.ItemsSource = App.dBcontext.antifreezebrandfields.OrderBy(s => s.Id).Where(s => s.code.ToString() == code.ToString()).Select(s => s.name).ToList();

                    }
            }
            catch
            {
                return;
            }

        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MotoHoursTextBox.Text) || string.IsNullOrEmpty(MileageTextBox.Text))
            {
                MessageBox.Show("Заполните поля: Мото-часы и Пробег");
                return;
            }
            else
            {
                AddData();
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void MessageButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Wpf.Ui.Controls.Button)sender;

            Clipboard.SetText(button.ToolTip.ToString());
            flyout.IsOpen = true;
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
            timer.Start();
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {

            flyout.Dispatcher.Invoke(() =>
            {
                flyout.IsOpen = false;
            });
            timer.Stop();
            timer.Dispose();
        }

        private void SelectFilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterListViewName.Items.Clear();
            var filterWindow = new FormAddFilters();
            if (filterWindow.ShowDialog() == true)
            {
                // Очистим старый список


                // Запишем новые
                foreach (var filter in filterWindow.SelectedFiltersName)
                {
                    FilterListViewName.Items.Add(filter);
                }
                foreach (var filter in filterWindow.SelectedFiltersCount)
                {
                    FilterListViewCount.Items.Add(filter);
                }
            }
        }
        private async void AddData()
        {

            
            {
                var greaseBrand = GreaseBrandComboBox.Text == null ? "" : GreaseBrandComboBox.Text + "_";
                var greaseType = GreaseTypeComboBox.Text == null ? "" : GreaseTypeComboBox.Text + "_";
                var greaseViscosity = GreaseViscosityComboBox.Text == null ? "" : GreaseViscosityComboBox.Text + "_";
                var grease = (greaseBrand + greaseType + greaseViscosity).Length > 0 ? (greaseBrand + greaseType + greaseViscosity).Substring(0, (greaseBrand + greaseType + greaseViscosity).Length - 1) : "";




                var anticolor = AntifreezeColorComboBox.Text == null ? "" : AntifreezeColorComboBox.Text + "_";
                var antibrand = AntifreezeBrandComboBox.Text == null ? "" : AntifreezeBrandComboBox.Text + "_";
                var antitype = AntifreezeTypeComboBox.Text == null ? "" : AntifreezeTypeComboBox.Text + "_";
                var anti = (anticolor + antibrand + antitype).Length > 0 ? (anticolor + antibrand + antitype).Substring(0, (anticolor + antibrand + antitype).Length - 1) : "";

                var otvets = "";
                foreach (var item in stackResponseble.Children)
                {
                    if (item is StackPanel stak)
                    {
                        var text = ((ComboBox)stak.Children[1]).Text;
                        otvets += text + "|";
                    }

                }

                var motor = MotorTypeComboBox.Text;
                var filtersName = "";
                var filtersCount = "";
                var filtersIds = "";
                foreach (var item in FilterListViewName.Items)
                {
                    if (item != "")
                    {
                        filtersName += item + ":";
                        filtersIds += App.dBcontext.Filter.OrderBy(e => e.id).Where(e => e.Name == item.ToString()).First().id + ":";
                    }
                }
                foreach (var item in FilterListViewCount.Items)
                {
                    if (item != "")
                    {
                        filtersCount += item + ":";
                    }
                }


                string oil_ids = "";
                string valueOil = "";
                var oilmess = "";
                var sub_ids = "";
                var sub_ids_count = ""; 
                int h = 0;
                var subvalue = "";
                foreach (var item in listOils)
                {
                    if (listOils.Count() == 1)
                    {
                        
                        
                        oil_ids += (item.ToString() == null ? null : App.dBcontext.Oil.Where(e => e.Name == item.ToString()).First().Id);
                    }
                    else
                    {
                        oil_ids += (item.ToString() == null ? null : App.dBcontext.Oil.Where(e => e.Name == item.ToString()).First().Id + ",");
                    }
                    valueOil += item.value + ",";
                    oilmess += item.ToString() != null ? item.ToString().Replace("_", " ") + ": " + item.value + ", " : null;

                    
                    //foreach (var sub in item.subUnits)
                    //{
                    //    subvalue += sub.Name + ": " + sub.Count + ", "; 
                    //    sub_ids += App.dBcontext.subunit.Where(s => s.Name == sub.Name).First().Id + ",";
                    //    sub_ids_count += sub.Count + ",";
                    //}
                    //subvalue = "Масло залитое в запчасти: " + subvalue.Substring(0, subvalue.Length - 2);
                    //sub_ids = sub_ids.Substring(0, sub_ids.Length - 1);
                    //sub_ids_count = sub_ids_count.Substring(0, sub_ids_count.Length - 1);
                    //sub_ids_count += "*";
                    //sub_ids += "*";
                    
                    h++;
                }

                //sub_ids = sub_ids.Substring(0, sub_ids.Length - 1);
                //sub_ids_count = sub_ids_count.Substring(0, sub_ids_count.Length - 1);
                //MessageBox.Show(oil_ids.Substring(0,oil_ids.Length - 1));
                //return;
                oilmess = oilmess.Substring(0, oilmess.Length - 2);
                valueOil = valueOil.Substring(0, valueOil.Length - 1);
                filtersName = filtersName == "" ? "" : filtersName.Substring(0, filtersName.Length - 1);
                filtersCount = filtersCount == "" ? "" : filtersCount.Substring(0, filtersCount.Length - 1);
                filtersIds = filtersIds == "" ? "" : filtersIds.Substring(0, filtersIds.Length - 1);
                object oilid = "";
  
                if(listOils.Count() != 1)
                {
                   oilid = oil_ids.Substring(0, oil_ids.Length - 1);

                }
                else
                {
                    oilid = oil_ids;
                }
                object greaseid = grease == "__" ? "NULL" : App.dBcontext.Grease.OrderBy(e => e.Id).Where(e => e.Name == grease).First().Id;
                object antifreezeid = anti == "__" ? "NULL" : App.dBcontext.Antifreeze.OrderBy(e => e.Id).Where(e => e.Name == anti).First().Id;
                object motorid = motor == "" ? "NULL" : App.dBcontext.Motors.OrderBy(e => e.id).Where(e => e.Name == motor).First().id;




                Material material = new Material
                {
                    Ячейка = (string?)yache,
                    ТипТраты = "ТО",
                    НомерЯчейки = nomer,
                    Ответственный = otvets,
                    Месяц = HomePage.CurrentMounth == null ? DateTime.Now.ToString("M") : HomePage.CurrentMounth,
                    Год = year == 0 ? DateTime.Now.Year : year,
                    oilCode = oil_ids.Length == 1 ? null : oil_ids,
                    oil_id = oil_ids.Length == 1 ? Convert.ToInt32(oil_ids) : null,
                    subunit_id = sub_ids,
                    motor_id = motorid == "NULL" ? null : Convert.ToInt32(motorid),
                    grease_id = greaseid == "NULL" ? null : Convert.ToInt32(greaseid),
                    antifreeze_id = antifreezeid == "NULL" ? null : Convert.ToInt32(antifreezeid),
                    CountMaterials = new CountMaterials()
                    {
                        count_other_clock = int.TryParse(MotoHoursTextBox.Text, out var temp5) ? temp5 : 0,
                        count_other_milesage = int.TryParse(MileageTextBox.Text, out var temp6) ? temp6 : 0,
                        count_antifreeze = int.TryParse(AntifreezeLitersTextBox.Text, out var temp2) ? temp2 : 0,
                        count_filter = filtersCount,
                        count_subunit = sub_ids_count,
                        count_filterids = filtersIds,
                        count_filtername = filtersName,
                        count_grease = int.TryParse(GreaseAmountTextBox.Text, out var temp3) ? temp3 : 0,
                        count_oil = valueOil,
                        count_motors = MotorTypeComboBox.Text
                    }

                };

                if (greaseid != "NULL")
                {
                    // App.dBcontext.greasecount.Where(s => s.id_grease == Convert.ToInt32(greaseid)).First().count -= int.TryParse(GreaseAmountTextBox.Text, out var itoggrease) ? itoggrease : 0;
                }
                if (antifreezeid != "NULL")
                {
                    // App.dBcontext.antifreezecount.Where(s => s.id_antifreeze == Convert.ToInt32(antifreezeid)).First().count -= int.TryParse(AntifreezeLitersTextBox.Text, out var itogantifreeze) ? itogantifreeze : 0;
                }


                //await App.dBcontext.SaveChangesAsync();


                //await App.dBcontext.DisposeAsync();
                var filters = "";
                if (filtersName != null)
                {
                    for (int i = 0; i < filtersName.Split(":").Count(); i++)
                    {
                        if (filtersName.Split(":")[i] != "")
                        {
                            filters += filtersName.Split(":")[i] + " " + filtersCount.Split(":")[i] + "шт; ";
                        }
                    }
                }
                var otvet = "";
                foreach (var item in otvets.Split("|"))
                {
                    otvet += item + ", ";
                }
                var chas = int.TryParse(MotoHoursTextBox.Text, out var chastemp) ? chastemp : 0;
                var mili = int.TryParse(MileageTextBox.Text, out var militemp) ? militemp : 0;
                var itogchas = chastemp == 0 ? "" : "Мото-часы: " + chastemp + "\r\n";
                var itogmili = militemp == 0 ? "" : "Пробег: " + militemp + "\r\n";
                otvet = otvet.Substring(0, otvet.Length - 4);


                anti = anti.Contains("__") || string.IsNullOrWhiteSpace(anti) ? "" : "Антифриз: " + anti + " " + (int.TryParse(AntifreezeLitersTextBox.Text, out var tempanti) ? tempanti : 0) + "л; " + "\r\n";
                filters = (filters == "" || string.IsNullOrWhiteSpace(filters)) ? "" : "Фильтры: " + filters + "\r\n";
                grease = grease.Contains("__") || string.IsNullOrWhiteSpace(grease) ? "" : "Смазка: " + grease + " " + (int.TryParse(GreaseAmountTextBox.Text, out var tempgrease) ? tempgrease : 0) + "л; " + "\r\n";
                var motors = "";
                motors = MotorTypeComboBox.Text.Contains("__") || string.IsNullOrWhiteSpace(MotorTypeComboBox.Text) ? "" : "Мотор(ы) " + MotorTypeComboBox.Text + "; \r\n";
                // MessageBox.Show("Маслов " + maslo + "|" + "Анти " + anti + "|" + "фильтры" + filters + "|" + "Смазка " + grease);
                string message = $"*{(material.НомерЯчейки.Split("_")[1].Length == 1 ? "0" + material.НомерЯчейки.Split("_")[1] : material.НомерЯчейки.Split("_")[1])}.{GetMonth(HomePage.CurrentMounth)}.{DateTime.Now.Year}* на машине {Pattern.GetWordAndInteger(material.НомерЯчейки.Split("_")[0])} произведено *техническое обслуживание*. Было использовано: \r\n" +
                   subvalue +
                   oilmess +
                   anti +
                   filters + "\r\n" +
                   grease +
                   motors +
                   itogchas +
                   itogmili +
                    "Ответственный(е): " + otvet;
                material.message = message;
                await App.dBcontext.Materials.AddAsync(material);
                await App.dBcontext.SaveChangesAsync();
                
                
                //SendWhatsapp(material.НомерЯчейки, message);
                
                SendWhatsapp(material.НомерЯчейки.Split("_")[0], message);
                //try
                //{"An error occurred while saving the entity changes. See the inner exception for detail
                //    await SignalRClient.SendNotificationAsync($"0x01|{material.НомерЯчейки} - Добавлено|Добавление");
                //}
                //catch (MySqlException ex)
                //{
                //    MessageBox.Show(ex.ToString());
                //    await SignalRClient.SendNotificationAsync($"0x02|{material.НомерЯчейки} - Добавить не получилось, идет откат изменений|Ошибка");
                //}
                //catch(HubException he)
                //{
                //    MessageBox.Show(he.ToString());
                //}
                Close();

            }
            // }
            // catch (Exception ex)
            // {
            //     MessageBox.Show(ex.Message);
            // }
        }
        
        public string GetMonth(string month)
        {
            if (month != null)
            {
                Dictionary<string, string> list = new Dictionary<string, string>
                {
                    ["Январь"] = "01",
                    ["Февраль"] = "02",
                    ["Март"] = "03",
                    ["Апрель"] = "04",
                    ["Май"] = "05",
                    ["Июнь"] = "06",
                    ["Июль"] = "07",
                    ["Август"] = "08",
                    ["Сентябрь"] = "09",
                    ["Октябрь"] = "10",
                    ["Ноябрь"] = "11",
                    ["Декабрь"] = "12",


                };
                return list[month];
            }
            else
            {
                return DateTime.Now.Month.ToString().Length == 1 ? "0" + DateTime.Now.Month : DateTime.Now.Month.ToString();
            }

        }
        public async void SendWhatsapp(string group, string message)
        {
            string messageEncoded = Uri.EscapeDataString(message);
            string url = $"whatsapp://send?text={messageEncoded}";
            var machine = group;
            Process.Start(new ProcessStartInfo
            {
                FileName = url,              
                UseShellExecute = true // обязательно true для запуска через PATH
            });
            machine = Pattern.GetValue(machine);         // WhatsApp Web
            Clipboard.SetText(machine);
            Thread.Sleep(8000);
            WhatsAppAutomation.CTRLV();
            WhatsAppAutomation.BOTTOM();
            WhatsAppAutomation.Enter();
            Clipboard.SetText("Т.О техники компании");
            WhatsAppAutomation.CTRLV();
            WhatsAppAutomation.BOTTOM();
            WhatsAppAutomation.BOTTOM();
            WhatsAppAutomation.Enter();
            WhatsAppAutomation.BOTTOM();
            WhatsAppAutomation.Enter();
            WhatsAppAutomation.Enter();
            Thread.Sleep(2000);
            var mainWindow = Application.Current.Windows
        .OfType<MainWindow>()
        .FirstOrDefault();

            // Поднимаем его
            mainWindow?.BringToFront();





        }
        public async Task<string> FindItemContentAsync(ListView listView, string contentToFind)
        {
            return await Task.Run(() =>
            {
                string? result = "";

                // Для доступа к элементам ListView используем Dispatcher
                listView.Dispatcher.Invoke(() =>
                {
                    foreach (var item in listView.Items)
                    {
                        // Преобразуем item к ListViewItem, если это возможно
                        ListViewItem listViewItem = listView.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;
                        if (listViewItem != null && listViewItem.Content != null)
                        {
                            // Используем IndexOf для поиска строки в контенте
                            if (listViewItem.Content.ToString().IndexOf(contentToFind, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                result = listViewItem.Content.ToString();  // Возвращаем найденный контент
                                return;  // Выход из цикла после нахождения первого совпадения
                            }
                        }
                    }
                });

                return result; // Если элемент не найден, result будет null
            });
        }

        private void AddPersonBtn_Click(object sender, RoutedEventArgs e)
        {
            StackPanel panel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(5),

            };
            TextBlock text = new TextBlock()
            {
                Margin = new Thickness(20, 0, 40, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Text = "Ответственный"

            };
            ComboBox combo = new ComboBox()
            {
                Width = 430,
                ItemsSource = GetPersone(),
                Background = Brushes.LightGray,
            };
            panel.Children.Add(text);
            panel.Children.Add(combo);
            stackResponseble.Children.Add(panel);
        }

        private void AddOilButton_Click(object sender, RoutedEventArgs e)
        {
            var oilWindow = new FormAddOil();
            if (oilWindow.ShowDialog() == true)
            {
                OliControl control = new OliControl()
                {
                    Margin = new Thickness(0, 3, 0, 3),
                    brendTitle = oilWindow.SelectedOil.brend,
                    vilocityTitle = oilWindow.SelectedOil.vilocity,
                    valueTitle = oilWindow.SelectedOil.value,
                    
                };
                StackPanel stack = new StackPanel();
                List<material.Subunit> subs = new List<material.Subunit>();
                foreach(var item in oilWindow.SelectedOil.subUnits)
                {
                    stack = new StackPanel() { Margin = new Thickness(3) };
                    stack.Children.Add(new TextBlock() { Text = item.Name });
                    stack.Children.Add(new TextBlock() { Text = item.Count.ToString() });
                    control.SubPanel.Add(stack);
                    subs.Add(new material.Subunit() { Name = item.Name, Count = item.Count });
                    
                }

                material.Oil oil = new material.Oil() { brend = oilWindow.SelectedOil.brend, type = oilWindow.SelectedOil.type, vilocity = oilWindow.SelectedOil.vilocity, value = oilWindow.SelectedOil.value, subUnits = subs};
                
                Button button = new Button() { Content = "❌", Margin = new Thickness(0, 3, 0, 3) };
                button.Click += (sender, e) => DeleteOil_Click(sender, e);
                listOils.Add(oil);
                panelOil.Children.Add(control);
                panelDelOil.Children.Add(button);

            }
        }
        private int GetIndex(object sender)
        {
            int index = 0;
            foreach (var item in panelDelOil.Children)
            {
                if (sender == item)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }
        private void DeleteOil_Click(object sender, EventArgs e)
        {
            int index = GetIndex(sender);
            panelDelOil.Children.RemoveAt(index);
            panelOil.Children.RemoveAt(index);
            listOils.RemoveAt(index);
        }
    }
}

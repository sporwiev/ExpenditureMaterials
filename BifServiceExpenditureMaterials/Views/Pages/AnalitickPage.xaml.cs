using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BifServiceExpenditureMaterials.AutoPiter;
using BifServiceExpenditureMaterials.Controls;
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.Helpers;
using Newtonsoft.Json.Linq;
using Other = BifServiceExpenditureMaterials.Helpers.Other;

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для AnalitickPage.xaml
    /// </summary>
    public partial class AnalitickPage : UserControl
    {
        public AnalitickPage()
        {
            InitializeComponent();
            Loaded += AnalitickPage_Loaded;
        }

        

        private void TypeProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var o = Other.GetMouthNumber(DateTime.Now.Month);
            var l = Other.GetMouthNumber(o) - 1;
            var oldmonth = Other.GetMouthNumber(l);
            var oldoldmonth = Other.GetMouthNumber(l - 1);
            try
            {
                var product = ((ComboBox)sender).SelectedIndex;
                List<int?> material = new List<int?>();
                var selected = TypeProductComboBox.SelectedIndex - 1 < 0 ? 0 : TypeProductComboBox.SelectedIndex - 1;
                if (TypeProductComboBox.Items.CurrentPosition != -1)
                {
                    typeproduct.Text = ((ComboBoxItem)(TypeProductComboBox.Items[selected])).Content.ToString();
                }
                

                {
                    List<int?> materialold = new List<int?>();
                    List<int?> materialnew = new List<int?>();
                    switch (product)
                    {
                        case 0:
                            //typeproduct.Text = "Масло";
                            //TypeProductComboBox.SelectedItem = "Масло";

                            if (ComboBoxMonth.Text == "")
                            {
                                material.Clear();
                                var countmaterial = App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Select(s => s.CountMaterials).ToList();
                                //List<string> dates = new List<string>();
                                List<int?, string?, string?, string?> values = new List<int?, string?, string?, string?>();
                                int i = 0;
                                foreach (var item in App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).ToList())
                                {
                                    var day = item.Ячейка.Split(":").Last();
                                    var date = (day.Length == 1 ? "0" + day : day) + "." + (Other.GetMouthNumber(item.Месяц).ToString().Length == 1 ? "0" + Other.GetMouthNumber(item.Месяц).ToString() : Other.GetMouthNumber(item.Месяц).ToString()) + "." + ComboBoxYear.Text;
                                    //dates.Add(date);
                                    //material.Add(countmaterial[i].count_oil);
                                    //values.Add(countmaterial[i].count_oil, item.НомерЯчейки.Split("_")[0], date, item.Ячейка);
                                    i++;
                                }

                                MyChart.ValuesOldMonth = values;
                                MyChart.DrawChart();
                                //MyChart.Values = new SubList<int?, string?, string?> (App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Select(s => s.Oil.Name).ToList(),dates,App.dBcontext.machine.Select(s => s.Code).ToList());


                                //MyChart.Dates = App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Select(s => s.Oil.Name).ToList();
                            }
                            else
                            {
                                //MyChart.Values = material = App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == ComboBoxMonth.Text).Select(s => s.CountMaterials.count_oil).ToList();
                            }
                            break;
                        case 1:
                            material.Clear();
                            //typeproduct.Text = "Антифриз";
                            //TypeProductComboBox.SelectedItem = "Антифриз";


                            if (ComboBoxMonth.Text == "")
                            {
                                var countmaterial = App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Select(s => s.CountMaterials).ToList();
                                //List<string> dates = new List<string>();
                                List<int?, string?, string?, string?> values = new List<int?, string?, string?, string?>();
                                int i = 0;
                                foreach (var item in App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).ToList())
                                {
                                    var day = item.Ячейка.Split(":").Last();
                                    var date = (day.Length == 1 ? "0" + day : day) + "." + (Other.GetMouthNumber(item.Месяц).ToString().Length == 1 ? "0" + Other.GetMouthNumber(item.Месяц).ToString() : Other.GetMouthNumber(item.Месяц).ToString()) + "." + ComboBoxYear.Text;
                                    //dates.Add(date);
                                    material.Add(countmaterial[i].count_antifreeze);
                                    values.Add(countmaterial[i].count_antifreeze, item.НомерЯчейки.Split("_")[0], date, item.Ячейка);
                                    i++;
                                }

                                MyChart.ValuesOldMonth = values;
                                MyChart.DrawChart();
                                //MyChart.Values = new SubList<int?, string?, string?> (App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Select(s => s.Oil.Name).ToList(),dates,App.dBcontext.machine.Select(s => s.Code).ToList());


                                //MyChart.Dates = App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Select(s => s.Oil.Name).ToList();
                            }
                            else
                            {
                                //MyChart.Values = material = App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == ComboBoxMonth.Text).Select(s => s.CountMaterials.count_oil).ToList();
                            }

                            break;
                        case 2:
                            material.Clear();
                            //typeproduct.Text = "Смазка";
                            //TypeProductComboBox.SelectedItem = "Смазка";
                            if (ComboBoxMonth.Text == "")
                            {
                                var countmaterial = App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Select(s => s.CountMaterials).ToList();
                                //List<string> dates = new List<string>();
                                List<int?, string?, string?, string?> values = new List<int?, string?, string?, string?>();
                                int i = 0;
                                foreach (var item in App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).ToList())
                                {
                                    var day = item.Ячейка.Split(":")[1];
                                    var date = (day.Length == 1 ? "0" + day : day) + "." + (Other.GetMouthNumber(item.Месяц).ToString().Length == 1 ? "0" + Other.GetMouthNumber(item.Месяц).ToString() : Other.GetMouthNumber(item.Месяц).ToString()) + "." + ComboBoxYear.Text;
                                    //dates.Add(date);
                                    material.Add(countmaterial[i].count_grease);
                                    values.Add(countmaterial[i].count_grease, item.НомерЯчейки.Split("_")[0], date, item.Ячейка);
                                    i++;
                                }

                                MyChart.ValuesOldMonth = values;
                                MyChart.DrawChart();
                                //MyChart.Values = new SubList<int?, string?, string?> (App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Select(s => s.Oil.Name).ToList(),dates,App.dBcontext.machine.Select(s => s.Code).ToList());


                                //MyChart.Dates = App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Select(s => s.Oil.Name).ToList();
                            }
                            else
                            {
                                //MyChart.Values = material = App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == ComboBoxMonth.Text).Select(s => s.CountMaterials.count_oil).ToList();
                            }
                            break;
                        case 3:
                            material.Clear();

                            //TypeProductComboBox.SelectedItem = "Фильтры";
                            if (ComboBoxMonth.Text == "")
                            {
                                var countmaterial = App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Select(s => s.CountMaterials).ToList();
                                //List<string> dates = new List<string>();
                                List<int?, string?, string?, string?> values = new List<int?, string?, string?, string?>();
                                int i = 0;
                                foreach (var item in App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).ToList())
                                {
                                    var day = item.Ячейка.Split(":")[1];
                                    var date = (day.Length == 1 ? "0" + day : day) + "." + (Other.GetMouthNumber(item.Месяц).ToString().Length == 1 ? "0" + Other.GetMouthNumber(item.Месяц).ToString() : Other.GetMouthNumber(item.Месяц).ToString()) + "." + ComboBoxYear.Text;
                                    //dates.Add(date);
                                    //material.Add(countmaterial[i].count_filter.Split(":");
                                    //values.Add(countmaterial[i].count_oil, item.НомерЯчейки.Split("_")[0], date);
                                    i++;
                                }

                                //MyChart.Values = values;
                                //MyChart.Values = new SubList<int?, string?, string?> (App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Select(s => s.Oil.Name).ToList(),dates,App.dBcontext.machine.Select(s => s.Code).ToList());


                                //MyChart.Dates = App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Select(s => s.Oil.Name).ToList();
                            }
                            else
                            {
                                //MyChart.Values = material = App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == ComboBoxMonth.Text).Select(s => s.CountMaterials.count_oil).ToList();
                            }
                            break;
                    }
                    //MyChart.DrawChart();
                    // = (System.Collections.IEnumerable)productdb;
                    sum.Text = $"Всего использовано за {oldoldmonth} : " + materialold.Sum().ToString() + "\r\n" +
                     $"Всего использовано за {oldmonth} : " + materialnew.Sum().ToString();
                }
                
            }
            catch
            {

            }
        }
        

        private void AnalitickPage_Loaded(object sender, RoutedEventArgs e)
        {
            var o = Other.GetMouthNumber(DateTime.Now.Month);
            var l = Other.GetMouthNumber(o) - 1;
            var oldmonth = Other.GetMouthNumber(l);
            var oldoldmonth = Other.GetMouthNumber(l - 1);
            ComboBoxYear.ItemsSource = new List<string>() { "2023", "2024", "2025", "2026" };

            ComboBoxMonth.ItemsSource = Other.GetAllMoths();
            
            ComboBoxMonth.SelectedItem = "";
            ComboBoxYear.SelectedItem = DateTime.Now.Year.ToString();
            ComboBoxMonth.SelectionChanged += ComboBoxMonth_SelectionChanged;
            ComboBoxYear.SelectionChanged += ComboBoxYear_SelectionChanged;
            
            {
                List<int?> materialold = new List<int?>();
                List<int?> materialnew = new List<int?>();
                if (ComboBoxMonth.Text == "")
                {
                    
                    //List<string> dates = new List<string>();
                    List<int?, string?, string?, string?> valuesold = new List<int?, string?, string?, string?>();
                    List<int?, string?, string?, string?> valuesnew = new List<int?, string?, string?, string?>();
                    int i = 0;

                    var countmaterialold = App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == oldoldmonth).Select(s => s.CountMaterials).ToList();
                    var countmaterialnew = App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == oldmonth).Select(s => s.CountMaterials).ToList();
                    foreach (var item in App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == oldoldmonth).ToList())
                    {
                        var day = item.Ячейка.Split(":")[1];
                        
                        var date = (day.Length == 1 ? "0" + day : day) + "." + (Other.GetMouthNumber(item.Месяц) - 1.ToString().Length == 1 ? "0" + Other.GetMouthNumber(item.Месяц).ToString() : Other.GetMouthNumber(item.Месяц).ToString()) + "." + ComboBoxYear.Text;
                        //dates.Add(date);
                       // materialold.Add(countmaterialold[i].count_oil);
                        //valuesold.Add(countmaterialold[i].count_oil, item.НомерЯчейки.Split("_").First(), date, item.Ячейка);
                        i++;
                    }
                    i = 0;
                    foreach (var item in App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == oldmonth).ToList())
                    {
                        if (i == 31) break;
                        var day = item.Ячейка.Split(":")[1];
                        var date = (day.Length == 1 ? "0" + day : day) + "." + (Other.GetMouthNumber(item.Месяц).ToString().Length == 1 ? "0" + Other.GetMouthNumber(item.Месяц).ToString() : Other.GetMouthNumber(item.Месяц).ToString()) + "." + ComboBoxYear.Text;
                        //dates.Add(date);
                        //materialnew.Add(countmaterialnew[i].count_oil);
                       // valuesnew.Add(countmaterialnew[i].count_oil, item.НомерЯчейки.Split("_")[0], date, item.Ячейка);
                        i++;
                    }
                    MyChart.ValuesOldMonth = valuesold;
                    MyChart.ValuesNewMonth = valuesnew;
                    MyChart.DrawChart();
                    //MyChart.Values = new SubList<int?, string?, string?> (App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Select(s => s.Oil.Name).ToList(),dates,App.dBcontext.machine.Select(s => s.Code).ToList());


                    //MyChart.Dates = App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Select(s => s.Oil.Name).ToList();
                }
                else
                {
                    //MyChart.Values = material = App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == ComboBoxMonth.Text).Select(s => s.CountMaterials.count_oil).ToList();
                }
                //    List<int> values = new List<int>();
                //foreach(var item in material)
                //{
                //    values.Add((int)item);
                //}
                sum.Text = $"Всего использовано за {oldoldmonth} : " + materialold.Sum().ToString() + "\r\n" +
                     $"Всего использовано за {oldmonth} : " + materialnew.Sum().ToString();



            }
            //MyChart.DrawChart();
        }

        private void ComboBoxYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var o = Other.GetMouthNumber(DateTime.Now.Month);
                var l = Other.GetMouthNumber(o) - 1;
                var oldmonth = Other.GetMouthNumber(l);
                var oldoldmonth = Other.GetMouthNumber(l - 1);
                
                {
                    var type = TypeProductComboBox;
                    List<int?, string?, string?, string?> valuesold = new List<int?, string?, string?, string?>();
                    List<int?, string?, string?, string?> valuesnew = new List<int?, string?, string?, string?>();


                    var countmaterialold = App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == oldoldmonth).Select(s => s.CountMaterials).ToList();
                    var countmaterialnew = App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == o).Select(s => s.CountMaterials).ToList();
                    int i = 0;

                    switch (TypeProductComboBox.SelectedIndex)
                    {
                        case 0:

                        case 1:
                            valuesold.Clear();
                            valuesnew.Clear();
                            foreach (var item in App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == oldoldmonth).ToList())
                            {
                                var day = item.Ячейка.Split(":")[1];
                                var date = (day.Length == 1 ? "0" + day : day) + "." + (Other.GetMouthNumber(item.Месяц).ToString().Length == 1 ? "0" + Other.GetMouthNumber(item.Месяц).ToString() : Other.GetMouthNumber(item.Месяц).ToString()) + "." + ComboBoxYear.Text;
                                //dates.Add(date);

                                //valuesold.Add(countmaterialold[i].count_oil, item.НомерЯчейки.Split("_")[0], date, item.Ячейка);
                                i++;
                            }
                            i = 0;
                            foreach (var item in App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == o).ToList())
                            {
                                var day = item.Ячейка.Split(":")[1];
                                var date = (day.Length == 1 ? "0" + day : day) + "." + (Other.GetMouthNumber(item.Месяц).ToString().Length == 1 ? "0" + Other.GetMouthNumber(item.Месяц).ToString() : Other.GetMouthNumber(item.Месяц).ToString()) + "." + ComboBoxYear.Text;
                                //dates.Add(date);

                                //valuesnew.Add(countmaterialnew[i].count_oil, item.НомерЯчейки.Split("_")[0], date, item.Ячейка);
                                i++;
                            }
                            MyChart.ValuesOldMonth = valuesold;
                            MyChart.ValuesNewMonth = valuesnew;
                            MyChart.DrawChart();
                            // FilterProduct<Oil>(tabs.SelectedItem as TabItem, new Oil(), App.dBcontext.Oil.Where(s => s.Name == product.Text).Select(s => s.Name).First());

                            break;
                        case 2:
                            valuesold.Clear();
                            valuesnew.Clear();
                            foreach (var item in App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == oldoldmonth).ToList())
                            {
                                var day = item.Ячейка.Split(":")[1];
                                var date = (day.Length == 1 ? "0" + day : day) + "." + (Other.GetMouthNumber(item.Месяц).ToString().Length == 1 ? "0" + Other.GetMouthNumber(item.Месяц).ToString() : Other.GetMouthNumber(item.Месяц).ToString()) + "." + ComboBoxYear.Text;
                                //dates.Add(date);

                                valuesold.Add(countmaterialold[i].count_antifreeze, item.НомерЯчейки.Split("_")[0], date, item.Ячейка);
                                i++;
                            }
                            i = 0;
                            foreach (var item in App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == o).ToList())
                            {
                                var day = item.Ячейка.Split(":")[1];
                                var date = (day.Length == 1 ? "0" + day : day) + "." + (Other.GetMouthNumber(item.Месяц).ToString().Length == 1 ? "0" + Other.GetMouthNumber(item.Месяц).ToString() : Other.GetMouthNumber(item.Месяц).ToString()) + "." + ComboBoxYear.Text;
                                //dates.Add(date);

                                valuesnew.Add(countmaterialnew[i].count_antifreeze, item.НомерЯчейки.Split("_")[0], date, item.Ячейка);
                                i++;
                            }

                            MyChart.ValuesOldMonth = valuesold;
                            MyChart.ValuesNewMonth = valuesnew;
                            MyChart.DrawChart();
                            //FilterProduct<Antifreeze>(tabs.SelectedItem as TabItem, new Antifreeze(), App.dBcontext.Antifreeze.Where(s => s.Name == product.Text).Select(s => s.Name).First());

                            break;
                        case 3:
                            valuesold.Clear();
                            valuesnew.Clear();
                            foreach (var item in App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == oldmonth).ToList())
                            {
                                var day = item.Ячейка.Split(":")[1];
                                var date = (day.Length == 1 ? "0" + day : day) + "." + (Other.GetMouthNumber(item.Месяц).ToString().Length == 1 ? "0" + Other.GetMouthNumber(item.Месяц).ToString() : Other.GetMouthNumber(item.Месяц).ToString()) + "." + ComboBoxYear.Text;
                                //dates.Add(date);

                                valuesold.Add(countmaterialold[i].count_grease, item.НомерЯчейки.Split("_")[0], date, item.Ячейка);
                                i++;
                            }
                            i = 0;
                            foreach (var item in App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == o).ToList())
                            {
                                var day = item.Ячейка.Split(":")[1];
                                var date = (day.Length == 1 ? "0" + day : day) + "." + (Other.GetMouthNumber(item.Месяц).ToString().Length == 1 ? "0" + Other.GetMouthNumber(item.Месяц).ToString() : Other.GetMouthNumber(item.Месяц).ToString()) + "." + ComboBoxYear.Text;
                                //dates.Add(date);

                                valuesnew.Add(countmaterialnew[i].count_grease, item.НомерЯчейки.Split("_")[0], date, item.Ячейка);
                                i++;
                            }

                            MyChart.ValuesOldMonth = valuesold;
                            MyChart.ValuesNewMonth = valuesnew;
                            MyChart.DrawChart();
                            //FilterProduct<Grease>(tabs.SelectedItem as TabItem, new Grease(), App.dBcontext.Grease.Where(s => s.Name == product.Text).Select(s => s.Name).First());

                            break;
                        case 4:

                            break;
                    }

                }
            }catch
            {

            }
            //MyChart.DrawChart();
        }

        private void ComboBoxMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try {
                var o = Other.GetMouthNumber(DateTime.Now.Month);
                var l = Other.GetMouthNumber(o) - 1;
                var oldmonth = Other.GetMouthNumber(l);
                var oldoldmonth = Other.GetMouthNumber(l - 1);
                
                {
                    var type = TypeProductComboBox;
                    List<int?, string?, string?, string?> valuesold = new List<int?, string?, string?, string?>();
                    List<int?, string?, string?, string?> valuesnew = new List<int?, string?, string?, string?>();
                    int i = 0;

                    var countmaterialold = App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == oldoldmonth).Select(s => s.CountMaterials).ToList();
                    var countmaterialnew = App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == o).Select(s => s.CountMaterials).ToList();


                    switch (TypeProductComboBox.SelectedIndex)
                    {
                        case 0:

                        case 1:
                            foreach (var item in App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == oldoldmonth).ToList())
                            {
                                var day = item.Ячейка.Split(":")[1];
                                var date = (day.Length == 1 ? "0" + day : day) + "." + (Other.GetMouthNumber(item.Месяц).ToString().Length == 1 ? "0" + Other.GetMouthNumber(item.Месяц).ToString() : Other.GetMouthNumber(item.Месяц).ToString()) + "." + ComboBoxYear.Text;

                                //valuesold.Add(countmaterialold[i].count_oil, item.НомерЯчейки.Split("_")[0], date, item.Ячейка);
                                i++;
                            }
                            i = 0;
                            foreach (var item in App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == oldmonth).ToList())
                            {
                                var day = item.Ячейка.Split(":")[1];
                                var date = (day.Length == 1 ? "0" + day : day) + "." + (Other.GetMouthNumber(item.Месяц).ToString().Length == 1 ? "0" + Other.GetMouthNumber(item.Месяц).ToString() : Other.GetMouthNumber(item.Месяц).ToString()) + "." + ComboBoxYear.Text;

                                //valuesold.Add(countmaterialold[i].count_oil, item.НомерЯчейки.Split("_")[0], date, item.Ячейка);
                                i++;
                            }
                            MyChart.ValuesOldMonth = valuesold;
                            MyChart.ValuesNewMonth = valuesnew;
                            break;
                        case 2:
                            foreach (var item in App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == oldoldmonth).ToList())
                            {
                                var day = item.Ячейка.Split(":")[1];
                                var date = (day.Length == 1 ? "0" + day : day) + "." + (Other.GetMouthNumber(item.Месяц).ToString().Length == 1 ? "0" + Other.GetMouthNumber(item.Месяц).ToString() : Other.GetMouthNumber(item.Месяц).ToString()) + "." + ComboBoxYear.Text;

                                valuesold.Add(countmaterialold[i].count_antifreeze, item.НомерЯчейки.Split("_")[0], date, item.Ячейка);
                                i++;
                            }
                            i = 0;
                            foreach (var item in App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == oldoldmonth).ToList())
                            {
                                var day = item.Ячейка.Split(":")[1];
                                var date = (day.Length == 1 ? "0" + day : day) + "." + (Other.GetMouthNumber(item.Месяц).ToString().Length == 1 ? "0" + Other.GetMouthNumber(item.Месяц).ToString() : Other.GetMouthNumber(item.Месяц).ToString()) + "." + ComboBoxYear.Text;

                                valuesold.Add(countmaterialold[i].count_antifreeze, item.НомерЯчейки.Split("_")[0], date, item.Ячейка);
                                i++;
                            }
                            MyChart.ValuesOldMonth = valuesold;
                            MyChart.ValuesNewMonth = valuesnew;
                            break;
                        case 3:
                            foreach (var item in App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == oldoldmonth).ToList())
                            {
                                var day = item.Ячейка.Split(":")[1];
                                var date = (day.Length == 1 ? "0" + day : day) + "." + (Other.GetMouthNumber(item.Месяц).ToString().Length == 1 ? "0" + Other.GetMouthNumber(item.Месяц).ToString() : Other.GetMouthNumber(item.Месяц).ToString()) + "." + ComboBoxYear.Text;

                                valuesold.Add(countmaterialold[i].count_grease, item.НомерЯчейки.Split("_")[0], date, item.Ячейка);
                                i++;
                            }
                            i = 0;
                            foreach (var item in App.dBcontext.Materials.Where(s => s.Год == Convert.ToInt32(ComboBoxYear.Text)).Where(s => s.Месяц == oldoldmonth).ToList())
                            {
                                var day = item.Ячейка.Split(":")[1];
                                var date = (day.Length == 1 ? "0" + day : day) + "." + (Other.GetMouthNumber(item.Месяц).ToString().Length == 1 ? "0" + Other.GetMouthNumber(item.Месяц).ToString() : Other.GetMouthNumber(item.Месяц).ToString()) + "." + ComboBoxYear.Text;

                                valuesold.Add(countmaterialold[i].count_grease, item.НомерЯчейки.Split("_")[0], date, item.Ячейка);
                                i++;
                            }
                            MyChart.ValuesOldMonth = valuesold;
                            MyChart.ValuesNewMonth = valuesnew;
                            break;
                        case 4:

                            break;
                    }
                    MyChart.DrawChart();

                }
            }
            catch
            {

            }
        }
    }
}

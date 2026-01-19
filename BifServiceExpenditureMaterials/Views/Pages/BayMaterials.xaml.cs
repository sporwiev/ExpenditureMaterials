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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.Helpers;
using BifServiceExpenditureMaterials.Models;

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для AnalitickPage.xaml
    /// </summary>
    public partial class BayMaterialsPage : UserControl
    {
        public BayMaterialsPage()
        {
            InitializeComponent();
        }
        public string CountItogProduct = "";
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

                switch ((sender as ComboBox).SelectedIndex)
                {
                    case 0:
                        ComboBoxProduct.ItemsSource = App.dBcontext.Oil.Select(s => s.Name).ToList();
                        break;
                    case 1:
                        ComboBoxProduct.ItemsSource = App.dBcontext.Antifreeze.Select(s => s.Name).ToList();
                        break;
                    case 2:
                        ComboBoxProduct.ItemsSource = App.dBcontext.Grease.Select(s => s.Name).ToList();
                        break;
                    case 3:
                        ComboBoxProduct.ItemsSource = App.dBcontext.Filter.Select(s => s.Name).ToList();
                        break;
                }
            
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            {
                switch (TypeProductComboBox.SelectedIndex)
                {
                    case 0:
                        var oilid = 0;
                        if (App.dBcontext.oilcount.Count() != 0)
                        {
                            if(App.dBcontext.Oil.Where(s => s.Name == ComboBoxProduct.Text).Count() != 0)
                            {
                                
                                //var idoil = App.dBcontext.Oil.Where(s => s.Name == ComboBoxProduct.Text).First().Id;
                                //if (App.dBcontext.oilcount.Where(s => s.id_oil == idoil).Any())
                                //{
                                //    if (App.dBcontext.oilcount.Where(s => s.id_oil == idoil).First().count <= 10)
                                //    {
                                //        App.dBcontext.oilcount.Where(s => s.id_oil == idoil).First().count += Convert.ToInt32(TextBoxCountProduct.Text);
                                //        await App.dBcontext.SaveChangesAsync();
                                //        CountItogProduct = App.dBcontext.oilcount.Where(s => s.id_oil == idoil).First().count.ToString();
                                //    }
                                //    else
                                //    {
                                //        MessageBox.Show("Для пополнения используйте максимум этого масла");
                                //        return;
                                //    }
                                //}
                                //else
                                //{
                                    
                                //    await App.dBcontext.oilcount.AddAsync(new OilCount() { count = Convert.ToInt32(TextBoxCountProduct.Text), id_oil = idoil, is_editing = 1, date = DateTime.Now.ToString() });
                                //    await App.dBcontext.SaveChangesAsync();
                                //    CountItogProduct = App.dBcontext.oilcount.Where(s => s.id_oil == idoil).First().count.ToString();
                                //}
                                //oilid = idoil;

                            }
                        }
                        else
                        {
                            var idoil = App.dBcontext.Oil.Where(s => s.Name == ComboBoxProduct.Text).First().Id;
                            //await App.dBcontext.oilcount.AddAsync(new OilCount() { count = Convert.ToInt32(TextBoxCountProduct.Text), id_oil = idoil, date = DateTime.Now.ToString(), is_editing = 1 });
                            await App.dBcontext.SaveChangesAsync();
                           // CountItogProduct = App.dBcontext.oilcount.Where(s => s.id_oil == idoil).First().count.ToString();
                           // oilid = idoil;
                        }
                        App.dBcontext.historycountoil.Add(new HistoryCountOil() { count = Convert.ToInt32(TextBoxCountProduct.Text), oil_id = oilid, date = DateTime.Now.ToString(), is_current = 1});
                        break;
                    case 1:
                        var antiid = 0;
                        if (App.dBcontext.antifreezecount.Count() != 0)
                        {
                            if (App.dBcontext.Antifreeze.Where(s => s.Name == ComboBoxProduct.Text).Count() != 0)
                            {

                                var idanti = App.dBcontext.Antifreeze.Where(s => s.Name == ComboBoxProduct.Text).First().Id;
                                if (App.dBcontext.antifreezecount.Where(s => s.id_antifreeze == idanti).Any())
                                {
                                    if (App.dBcontext.antifreezecount.Where(s => s.id_antifreeze == idanti).First().count <= 10)
                                    {
                                        App.dBcontext.antifreezecount.Where(s => s.id_antifreeze == idanti).First().count += Convert.ToInt32(TextBoxCountProduct.Text);
                                        await App.dBcontext.SaveChangesAsync();
                                        CountItogProduct = App.dBcontext.antifreezecount.Where(s => s.id_antifreeze == idanti).First().count.ToString();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Для пополнения используйте максимум этого антифриза");
                                        return;
                                    }
                                }
                                else
                                {
                                    await App.dBcontext.antifreezecount.AddAsync(new AntifreezeCount() { count = Convert.ToInt32(TextBoxCountProduct.Text), id_antifreeze = idanti, date = DateTime.Now.ToString(), is_editing = 1 });
                                    await App.dBcontext.SaveChangesAsync();
                                    CountItogProduct = App.dBcontext.antifreezecount.Where(s => s.id_antifreeze == idanti).First().count.ToString();
                                }
                                antiid = idanti;
                            }
                           

                        }
                        else
                        {
                            var idanti = App.dBcontext.Antifreeze.Where(s => s.Name == ComboBoxProduct.Text).First().Id;
                            await App.dBcontext.antifreezecount.AddAsync(new AntifreezeCount() { count = Convert.ToInt32(TextBoxCountProduct.Text), id_antifreeze = idanti, date = DateTime.Now.ToString(), is_editing = 1 });
                            await App.dBcontext.SaveChangesAsync();
                            CountItogProduct = App.dBcontext.antifreezecount.Where(s => s.id_antifreeze == idanti).First().count.ToString();
                            antiid = idanti;
                        }
                        App.dBcontext.historycountantifreeze.Add(new HistoryCountAntifreeze() { count = Convert.ToInt32(TextBoxCountProduct.Text), antifreeze_id = antiid, date = DateTime.Now.ToString(), is_current = 1 });
                        break;
                    case 2:
                        var greaseid = 0;
                        if (App.dBcontext.greasecount.Count() != 0)
                        {
                            if (App.dBcontext.Grease.Where(s => s.Name == ComboBoxProduct.Text).Count() != 0)
                            {

                                var idgrease = App.dBcontext.Grease.Where(s => s.Name == ComboBoxProduct.Text).First().Id;
                                if(App.dBcontext.greasecount.Where(s => s.id_grease == idgrease).Any())
                                {
                                    if (App.dBcontext.greasecount.Where(s => s.id_grease == idgrease).First().count <= 10)
                                    {
                                        App.dBcontext.greasecount.Where(s => s.id_grease == idgrease).First().count += Convert.ToInt32(TextBoxCountProduct.Text);
                                        await App.dBcontext.SaveChangesAsync();
                                        CountItogProduct = App.dBcontext.greasecount.Where(s => s.id_grease == idgrease).First().count.ToString();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Для пополнения используйте максимум этой смазки");
                                        return;
                                    }
                                }
                                else
                                {
                                    await App.dBcontext.greasecount.AddAsync(new GreaseCount() { count = Convert.ToInt32(TextBoxCountProduct.Text), id_grease = idgrease, date = DateTime.Now.ToString(), is_editing = 1 });
                                    await App.dBcontext.SaveChangesAsync();
                                    CountItogProduct = App.dBcontext.greasecount.Where(s => s.id_grease == idgrease).First().count.ToString();
                                }
                                    
                                greaseid = idgrease;
                            }
                            else
                            {
                                var idgrease = App.dBcontext.Oil.Where(s => s.Name == ComboBoxProduct.Text).First().Id;
                                //await App.dBcontext.greasecount.AddAsync(new GreaseCount() { count = Convert.ToInt32(TextBoxCountProduct.Text), date = DateTime.Now.ToString(), id_grease = idgrease, is_editing = 1 });
                                await App.dBcontext.SaveChangesAsync();
                                //CountItogProduct = App.dBcontext.greasecount.Where(s => s.id_grease == idgrease).First().count.ToString();
                                //greaseid = idgrease;

                            }

                        }
                        else
                        {
                            var idgrease = App.dBcontext.Grease.Where(s => s.Name == ComboBoxProduct.Text).First().Id;
                            //MessageBox.Show(idgrease.ToString());
                            await App.dBcontext.greasecount.AddAsync(new GreaseCount() { count = Convert.ToInt32(TextBoxCountProduct.Text), date = DateTime.Now.ToString(), id_grease = idgrease, is_editing = 1 });
                            await App.dBcontext.SaveChangesAsync();
                            CountItogProduct = App.dBcontext.greasecount.Where(s => s.id_grease == idgrease).First().count.ToString();
                            greaseid = idgrease;

                        }
                        App.dBcontext.historycountgrease.Add(new HistoryCountGrease() { count = Convert.ToInt32(TextBoxCountProduct.Text), grease_id = greaseid, date = DateTime.Now.ToString(), is_current = 1 });
                        break;
                    case 3:
                        var filterid = 0;
                        if (App.dBcontext.filterscount.Count() != 0)
                        {
                            if (App.dBcontext.Filter.Where(s => s.Name == ComboBoxProduct.Text).Count() != 0)
                            {

                                var idfilter = App.dBcontext.Filter.Where(s => s.Name == ComboBoxProduct.Text).First().id;
                                if(App.dBcontext.filterscount.Where(s => s.id_filter == idfilter).Any())
                                {
                                    if (App.dBcontext.filterscount.Where(s => s.id_filter == idfilter).First().count <= 10) {
                                        App.dBcontext.filterscount.Where(s => s.id_filter == idfilter).First().count += Convert.ToInt32(TextBoxCountProduct.Text);
                                        await App.dBcontext.SaveChangesAsync();
                                        CountItogProduct = App.dBcontext.filterscount.Where(s => s.id_filter == idfilter).First().count.ToString();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Для пополнения используйте максимум этого фильтра");
                                        return;
                                    }
                                }
                                else
                                {
                                    await App.dBcontext.filterscount.AddAsync(new FiltersCount() { count = Convert.ToInt32(TextBoxCountProduct.Text), id_filter = idfilter, date = DateTime.Now.ToString(), is_editing = 1 });
                                    await App.dBcontext.SaveChangesAsync();
                                    CountItogProduct = App.dBcontext.filterscount.Where(s => s.id_filter == idfilter).First().count.ToString();
                                }

                                filterid = idfilter;

                            }
                            else
                            {
                                var idfilter = App.dBcontext.Filter.Where(s => s.Name == ComboBoxProduct.Text).First().id;
                                await App.dBcontext.filterscount.AddAsync(new FiltersCount() { count = Convert.ToInt32(TextBoxCountProduct.Text), id_filter = idfilter, date = DateTime.Now.ToString(), is_editing = 1 });
                                await App.dBcontext.SaveChangesAsync();
                                CountItogProduct = App.dBcontext.filterscount.Where(s => s.id_filter == idfilter).First().count.ToString();
                                filterid = idfilter;
                            }

                        }
                        else
                        {
                            var idfilter = App.dBcontext.Filter.Where(s => s.Name == ComboBoxProduct.Text).First().id;
                            MessageBox.Show(idfilter.ToString());
                            await App.dBcontext.filterscount.AddAsync(new FiltersCount() { count = Convert.ToInt32(TextBoxCountProduct.Text),id_filter = idfilter, date = DateTime.Now.ToString(), is_editing = 1 });
                            await App.dBcontext.SaveChangesAsync();
                            CountItogProduct = App.dBcontext.filterscount.Where(s => s.id_filter == idfilter).First().count.ToString();
                            filterid = idfilter;
                        }
                        App.dBcontext.historycountfilters.Add(new HistoryCountFilters() { count = Convert.ToInt32(TextBoxCountProduct.Text), filter_id = filterid, date = DateTime.Now.ToString(), is_current = 1 });
                        await App.dBcontext.SaveChangesAsync();
                        break;
                }
                try
                {
                    await SignalRClient.SendNotificationAsync($"0x05|Пополнено на {Convert.ToInt32(TextBoxCountProduct.Text)}, текущий остаток {CountItogProduct} |{ComboBoxProduct.Text}");
                }
                catch (Exception ex)
                {

                }
                await App.dBcontext.SaveChangesAsync();
            }
        }

        private void ComboBoxProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboProduct = ComboBoxProduct;
            if (comboProduct.SelectedItem != null)
            {

                {
                    switch (TypeProductComboBox.SelectedIndex)
                    {
                        case 0:
                           // var idoil = App.dBcontext.Oil.Where(s => s.Name == comboProduct.SelectedItem as string).First().Id;
                           //// var OilProduct = App.dBcontext.oilcount.Where(s => s.id_oil == idoil);
                           // if (OilProduct.Count() == 0)
                           // {
                           //     CountTextBox.Text = "По данному маслу отстатка нет";
                           // }
                           // else
                           // {
                           //     CountTextBox.Text = "Текущий остаток: " + OilProduct.First().count;
                           // }
                            break;
                        case 1:

                            var idanti = App.dBcontext.Antifreeze.Where(s => s.Name == comboProduct.SelectedItem as string).First().Id;
                            var AntiProduct = App.dBcontext.antifreezecount.Where(s => s.id_antifreeze == idanti);
                            if (AntiProduct.Count() == 0)
                            {
                                CountTextBox.Text = "По данному антифризу отстатка нет";
                            }
                            else
                            {
                                CountTextBox.Text = "Текущий остаток: " + AntiProduct.First().count;
                            }
                            break;
                        case 2:
                            var idgrease = App.dBcontext.Grease.Where(s => s.Name == comboProduct.SelectedItem as string).First().Id;
                            var GreaseProduct = App.dBcontext.greasecount.Where(s => s.id_grease == idgrease);
                            if (GreaseProduct.Count() == 0)
                            {
                                CountTextBox.Text = "По данной смазке отстатка нет";
                            }
                            else
                            {
                                CountTextBox.Text = "Текущий остаток: " + GreaseProduct.First().count;
                            }
                            break;
                        case 3:
                            var idfilter = App.dBcontext.Filter.Where(s => s.Name == comboProduct.SelectedItem as string).First().id;
                            var FilterProduct = App.dBcontext.filterscount.Where(s => s.id_filter == idfilter);
                            if (FilterProduct.Count() == 0)
                            {
                                CountTextBox.Text = "По данному фильтру отстатка нет";
                            }
                            else
                            {
                                CountTextBox.Text = "Текущий остаток: " + FilterProduct.First().count;
                            }
                            break;
                    }
                }
            }
        }
    }
}

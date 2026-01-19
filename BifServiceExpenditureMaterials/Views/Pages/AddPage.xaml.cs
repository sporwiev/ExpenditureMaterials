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
using BifServiceExpenditureMaterials.Models;

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddPage.xaml
    /// </summary>
    public partial class AddPage : UserControl
    {
        ComboBox suzh1Box;
        ComboBox suzh2Box;
        ComboBox suzh3Box;
        public AddPage()
        {
            InitializeComponent();
            combo.SelectionChanged += Combo_SelectionChanged;
            
            suzh1.MouseRightButtonDown += Suzh_MouseDown;
            suzh2.MouseRightButtonDown += Suzh_MouseDown;
            suzh3.MouseRightButtonDown += Suzh_MouseDown;
            combo.SelectedIndex = 0;
        }

        private void Suzh_MouseDown(object sender, MouseButtonEventArgs e)
        {

                if (e.ChangedButton == MouseButton.Right)
                {
                    if (combo.SelectedValue != null)

                        switch (combo.SelectedValue.ToString())
                        {
                            case "Масло":

                                switch (Grid.GetRow(sender as TextBox))
                                {
                                    case 2:
                                        suzh1Box = new ComboBox() { ItemsSource = App.dBcontext.oiltypefields.Select(s => s.name).ToList() };
                                        Grid.SetRow(suzh1Box, 2);
                                        Grid.SetColumn(suzh1Box, 1);
                                        grid.Children.Add(suzh1Box);
                                        break;
                                    case 3:
                                        suzh2Box = new ComboBox() { ItemsSource = App.dBcontext.oilbrandfields.Select(s => s.name).ToList() };
                                        Grid.SetRow(suzh2Box, 3);
                                        Grid.SetColumn(suzh2Box, 1);
                                        grid.Children.Add(suzh2Box);
                                        break;
                                    case 4:
                                        suzh2Box = new ComboBox() { ItemsSource = App.dBcontext.oilvilocityfields.Select(s => s.name).ToList() };
                                        Grid.SetRow(suzh2Box, 4);
                                        Grid.SetColumn(suzh2Box, 1);
                                        grid.Children.Add(suzh2Box);
                                        break;

                                }
                                grid.Children.Remove((TextBox)sender);
                                break;
                            case "Антифриз":
                                switch (Grid.GetRow(sender as TextBox))
                                {
                                    case 2:
                                        suzh1Box = new ComboBox() { ItemsSource = App.dBcontext.antifreezecolorfields.Select(s => s.name).ToList() };
                                        Grid.SetRow(suzh1Box, 2);
                                        Grid.SetColumn(suzh1Box, 1);
                                        grid.Children.Add(suzh1Box);
                                        break;
                                    case 3:
                                        suzh2Box = new ComboBox() { ItemsSource = App.dBcontext.antifreezebrandfields.Select(s => s.name).ToList() };
                                        Grid.SetRow(suzh2Box, 3);
                                        Grid.SetColumn(suzh2Box, 1);
                                        grid.Children.Add(suzh2Box);
                                        break;
                                    case 4:
                                        suzh2Box = new ComboBox() { ItemsSource = App.dBcontext.antifreezetypefields.Select(s => s.name).ToList() };
                                        Grid.SetRow(suzh2Box, 4);
                                        Grid.SetColumn(suzh2Box, 1);
                                        grid.Children.Add(suzh2Box);
                                        break;

                                }
                                grid.Children.Remove((TextBox)sender);
                                break;
                            case "Смазка":
                                switch (Grid.GetRow(sender as TextBox))
                                {
                                    case 2:
                                        suzh1Box = new ComboBox() { ItemsSource = App.dBcontext.greasebrandfields.Select(s => s.name).ToList() };
                                        Grid.SetRow(suzh1Box, 2);
                                        Grid.SetColumn(suzh1Box, 1);
                                        grid.Children.Add(suzh1Box);
                                        break;
                                    case 3:
                                        suzh2Box = new ComboBox() { ItemsSource = App.dBcontext.greasetypefields.Select(s => s.name).ToList() };
                                        Grid.SetRow(suzh2Box, 3);
                                        Grid.SetColumn(suzh2Box, 1);
                                        grid.Children.Add(suzh2Box);
                                        break;
                                    case 4:
                                        suzh2Box = new ComboBox() { ItemsSource = App.dBcontext.greasevilocityfields.Select(s => s.name).ToList() };
                                        Grid.SetRow(suzh2Box, 4);
                                        Grid.SetColumn(suzh2Box, 1);
                                        grid.Children.Add(suzh2Box);
                                        break;

                                }
                                grid.Children.Remove((TextBox)sender);
                                break;
                        }
                }
            
        }
        

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            suzhcombobox1.IsEnabled = true;
            suzhcombobox2.IsEnabled = true;
            suzhcombobox3.IsEnabled = true;
            suzhtextbox1.IsEnabled = true;
            suzhtextbox2.IsEnabled = true;
            suzhtextbox3.IsEnabled = true;
            but1.IsEnabled = true; 
            but2.IsEnabled = true;
            but3.IsEnabled = true;

                switch (combo.SelectedIndex)
                {
                    case 0:
                        //await App.dBcontext.Oil.AddAsync(new Oil() { Name = namepropduct.Text });
                        suzh1.Text = "Тип (Пример: 'Гидравлическое')";
                        suzh2.Text = "Марка (Пример: 'XCMG_Г')";
                        suzh3.Text = "Вязкость (Пример: 'HVLP_46')";
                        suzhcombobox1.ItemsSource = App.dBcontext.oiltypefields.Select(s => s.name).ToList();
                        suzhcombobox2.ItemsSource = App.dBcontext.oilbrandfields.Select(s => s.name).ToList();
                        suzhcombobox3.ItemsSource = App.dBcontext.oilvilocityfields.Select(s => s.name).ToList();
                        filterTextBox.IsEnabled = false;

                        break;
                    case 1:
                        // App.dBcontext.Antifreeze.AddAsync(new Antifreeze() { Name = namepropduct.Text });
                        suzh1.Text = "Цвет (Пример: 'Красный')";
                        suzh2.Text = "Марка (Пример: 'Лукойл')";
                        suzh3.Text = "Тип (Пример: 'G_12')";
                        suzhcombobox1.ItemsSource = App.dBcontext.antifreezecolorfields.Select(s => s.name).ToList();
                        suzhcombobox2.ItemsSource = App.dBcontext.antifreezebrandfields.Select(s => s.name).ToList();
                        suzhcombobox3.ItemsSource = App.dBcontext.antifreezetypefields.Select(s => s.name).ToList();
                        filterTextBox.IsEnabled = false;
                        break;
                    case 2:
                        //await App.dBcontext.Grease.AddAsync(new Grease() { Name = namepropduct.Text });
                        suzh1.Text = "Марка (Пример: 'TESMA')";
                        suzh2.Text = "Тип (Пример: 'Смазка_стрелы_T')";
                        suzh3.Text = "Вязкость (Пример: 'MC_4217_2p')";
                        suzhcombobox1.ItemsSource = App.dBcontext.greasebrandfields.Select(s => s.name).ToList();
                        suzhcombobox2.ItemsSource = App.dBcontext.greasetypefields.Select(s => s.name).ToList();
                        suzhcombobox3.ItemsSource = App.dBcontext.greasevilocityfields.Select(s => s.name).ToList();
                        filterTextBox.IsEnabled = false;
                        break;
                    case 3:
                        //await App.dBcontext.Filter.AddAsync(new Filter() { Name = namepropduct.Text });
                        suzh1.IsEnabled = false;
                        suzh2.IsEnabled = false;
                        suzh3.IsEnabled = false;
                        suzhcombobox1.IsEnabled = false;
                        suzhcombobox2.IsEnabled = false;
                        suzhcombobox3.IsEnabled = false;
                        suzhtextbox1.IsEnabled = false;
                        suzhtextbox2.IsEnabled = false;
                        suzhtextbox3.IsEnabled = false;
                        but1.IsEnabled = false;
                        but2.IsEnabled = false;
                        but3.IsEnabled = false;
                        filterTextBox.IsEnabled = true;
                        break;

                }
            
        }
        public string GetText(UIElement el)
        {
            if(el is TextBox text)
            {
                return text.Text;
            }
            if(el is ComboBox combo)
            {
                return combo.Text;
            }
            return "";
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            {
                var suzhitog1 = GetText(suzhtextbox1) == "" ? GetText(suzhcombobox1) : GetText(suzhtextbox1);
                var suzhitog2 = GetText(suzhtextbox2) == "" ? GetText(suzhcombobox2) : GetText(suzhtextbox2);
                var suzhitog3 = GetText(suzhtextbox3) == "" ? GetText(suzhcombobox3) : GetText(suzhtextbox3);

                switch (combo.SelectedIndex)
                {
                    case 0:
                        var typeoil = App.dBcontext.oiltypefields.Where(s => s.name == suzhitog1);
                        var brandoil = App.dBcontext.oilbrandfields.Where(s => s.name == suzhitog2);
                        var viscooil = App.dBcontext.oilvilocityfields.Where(s => s.name == suzhitog3);
                        int idtypeoil = 0;
                        int idbrandoil = 0;
                        int idviscooil = 0;
                        if (typeoil.Count() != 0)
                        {
                            idtypeoil = typeoil.First().Id;
                        }
                        else
                        {
                            await App.dBcontext.oiltypefields.AddAsync(new Oiltypefields() { name = suzhitog1, code = 0 });
                            await App.dBcontext.SaveChangesAsync();
                            App.dBcontext.oiltypefields.Where(s => s.name == suzhitog1).First().code = App.dBcontext.oiltypefields.Where(s => s.name == suzhitog1).First().Id;
                            idtypeoil = App.dBcontext.oiltypefields.Where(s => s.name == suzhitog1).First().Id;
                            await App.dBcontext.SaveChangesAsync();
                        }
                        if (brandoil.Count() != 0)
                        {
                            idbrandoil = brandoil.First().Id;
                        }
                        else
                        {
                            await App.dBcontext.oilbrandfields.AddAsync(new Oilbrandfields() { name = suzhitog2, code = idtypeoil });
                            await App.dBcontext.SaveChangesAsync();
                            //App.dBcontext.oilbrandfields.Where(s => s.name == suzhitog2).First().code = idtypeoil;
                            idbrandoil = App.dBcontext.oilbrandfields.Where(s => s.name == suzhitog2).First().Id;
                            await App.dBcontext.SaveChangesAsync();
                        }
                        if (viscooil.Count() != 0)
                        {
                            idviscooil = viscooil.First().Id;
                        }
                        else
                        {
                            await App.dBcontext.oilvilocityfields.AddAsync(new Oilvilocityfields() { name = suzhitog3, code = idbrandoil });
                            await App.dBcontext.SaveChangesAsync();
                            //App.dBcontext.oilvilocityfields.Where(s => s.name == suzhitog3).First().code = idbrandoil;
                            idviscooil = App.dBcontext.oilvilocityfields.Where(s => s.name == suzhitog3).First().Id;
                            await App.dBcontext.SaveChangesAsync();
                        }
                        App.dBcontext.Oil.Add(new Oil() { Name = suzhitog1 + "_" + suzhitog2 + "_" + suzhitog3 });
                        await App.dBcontext.SaveChangesAsync();
                        try
                        {
                            //await SignalRClient.SendNotificationAsync($"0x05|Добавлено новое масло {suzhitog1 + "_" + suzhitog2 + "_" + suzhitog3}|Новое");
                        }
                        catch (Exception ex)
                        {

                        }
                        break;
                    case 1:
                        var coloranti = App.dBcontext.antifreezecolorfields.Where(s => s.name == suzhitog1);
                        var brandanti = App.dBcontext.antifreezebrandfields.Where(s => s.name == suzhitog2);
                        var typeanti = App.dBcontext.antifreezetypefields.Where(s => s.name == suzhitog3);
                        int idtypeanti = 0;
                        int idbrandanti = 0;
                        int idcoloranti = 0;
                        if (coloranti.Count() != 0)
                        {
                            idcoloranti = coloranti.First().Id;
                        }
                        else
                        {
                            await App.dBcontext.antifreezecolorfields.AddAsync(new Antifreezecolorfields() { name = suzhitog1, code = 0 });
                            await App.dBcontext.SaveChangesAsync();
                            App.dBcontext.antifreezecolorfields.Where(s => s.name == suzhitog1).First().code = App.dBcontext.antifreezecolorfields.Where(s => s.name == suzhitog1).First().Id;
                            idcoloranti = App.dBcontext.antifreezecolorfields.Where(s => s.name == suzhitog1).First().Id;
                            await App.dBcontext.SaveChangesAsync();
                        }
                        if (brandanti.Count() != 0)
                        {
                            idbrandanti = brandanti.First().Id;
                        }
                        else
                        {
                            await App.dBcontext.antifreezebrandfields.AddAsync(new Antifreezebrandfields() { name = suzhitog2, code = idcoloranti });
                            await App.dBcontext.SaveChangesAsync();
                            //App.dBcontext.antifreezebrandfields.Where(s => s.name == suzhitog2).First().code = idcoloranti;
                            idbrandoil = App.dBcontext.oilbrandfields.Where(s => s.name == suzhitog2).First().Id;
                            await App.dBcontext.SaveChangesAsync();
                        }
                        if (typeanti.Count() != 0)
                        {
                            idtypeanti = typeanti.First().Id;
                        }
                        else
                        {
                            await App.dBcontext.antifreezetypefields.AddAsync(new Antifreezetypefields() { name = suzhitog3, code =  idbrandanti });
                            await App.dBcontext.SaveChangesAsync();
                            //App.dBcontext.antifreezetypefields.Where(s => s.name == suzhitog3).First().code = idbrandanti;
                            idtypeanti = App.dBcontext.antifreezetypefields.Where(s => s.name == suzhitog3).First().Id;
                            await App.dBcontext.SaveChangesAsync();
                        }
                        App.dBcontext.Antifreeze.Add(new Antifreeze() { Name = suzhitog1 + "_" + suzhitog2 + "_" + suzhitog3 });
                        await App.dBcontext.SaveChangesAsync();
                        try
                        {
                            //await SignalRClient.SendNotificationAsync($"0x05|Добавлен новый антифриз {suzhitog1 + "_" + suzhitog2 + "_" + suzhitog3}|Новое");
                        }
                        catch (Exception ex)
                        {

                        }
                        break;
                    case 2:
                        var typegrease = App.dBcontext.oiltypefields.Where(s => s.name == suzhitog1);
                        var brandgrease = App.dBcontext.oilbrandfields.Where(s => s.name == suzhitog2);
                        var viscogrease = App.dBcontext.oilvilocityfields.Where(s => s.name == suzhitog3);
                        int idtypegrease = 0;
                        int idbrandgrease = 0;
                        int idviscogrease = 0;
                        if (typegrease.Count() != 0)
                        {
                            idtypegrease = typegrease.First().Id;
                        }
                        else
                        {
                            await App.dBcontext.oiltypefields.AddAsync(new Oiltypefields() { name = suzhitog1, code = 0 });
                            await App.dBcontext.SaveChangesAsync();
                            App.dBcontext.oiltypefields.Where(s => s.name == suzhitog1).First().code = App.dBcontext.oiltypefields.Where(s => s.name == suzhitog1).First().Id;
                            idtypegrease = App.dBcontext.oiltypefields.Where(s => s.name == suzhitog1).First().Id;
                            await App.dBcontext.SaveChangesAsync();
                        }
                        if (brandgrease.Count() != 0)
                        {
                            idbrandgrease = brandgrease.First().Id;
                        }
                        else
                        {
                            await App.dBcontext.oilbrandfields.AddAsync(new Oilbrandfields() { name = suzhitog2, code = idtypegrease });
                            await App.dBcontext.SaveChangesAsync();
                            //App.dBcontext.oilbrandfields.Where(s => s.name == suzhitog2).First().code = idtypegrease;
                            idbrandgrease = App.dBcontext.oilbrandfields.Where(s => s.name == suzhitog2).First().Id;
                            await App.dBcontext.SaveChangesAsync();
                        }
                        if (viscogrease.Count() != 0)
                        {
                            idviscogrease = viscogrease.First().Id;
                        }
                        else
                        {
                            await App.dBcontext.oilvilocityfields.AddAsync(new Oilvilocityfields() { name = suzhitog3, code = idbrandgrease });
                            await App.dBcontext.SaveChangesAsync();
                            //App.dBcontext.oilvilocityfields.Where(s => s.name == suzhitog3).First().code = idbrandgrease;
                            idviscogrease = App.dBcontext.oilvilocityfields.Where(s => s.name == suzhitog3).First().Id;
                            await App.dBcontext.SaveChangesAsync();
                        }
                        App.dBcontext.Grease.Add(new Grease() { Name = suzhitog1 + "_" + suzhitog2 + "_" + suzhitog3 });
                        await App.dBcontext.SaveChangesAsync();
                        try
                        {
                            //await SignalRClient.SendNotificationAsync($"0x05|Добавлена новая смазка {suzhitog1 + "_" + suzhitog2 + "_" + suzhitog3}|Новое");
                        }
                        catch (Exception ex)
                        {

                        }
                        break;
                    case 3:
                        if (filterTextBox.IsEnabled)
                        {
                            App.dBcontext.Filter.Add(new Filter() { Name = filterTextBox.Text });
                        }
                        await App.dBcontext.SaveChangesAsync();
                        try
                        {
                            //await SignalRClient.SendNotificationAsync($"0x05|Добавлен новый фильтр {filterTextBox.Text}|Новое");
                        }
                        catch (Exception ex)
                        {

                        }
                        break;

                }
                
                //if (App.dBcontext.)
                
                //await App.dBcontext.SaveChangesAsync();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var row = Grid.GetRow(sender as Button);
            //var column = Grid.GetColumn(sender as Button);
            switch (row)
            {
                case 2:

                    if (suzhcombobox1.IsEnabled)
                    {

                        suzhtextbox1.IsEnabled = true;
                        suzhcombobox1.IsEnabled = false;
                        Grid.SetZIndex(suzhtextbox1, 1);
                        Grid.SetZIndex(suzhcombobox1, 0);
                    }
                    else
                    {
                        suzhtextbox1.IsEnabled = false;
                        suzhcombobox1.IsEnabled = true;
                        Grid.SetZIndex(suzhtextbox1, 0);
                        Grid.SetZIndex(suzhcombobox1, 1);
                    }
                    break;
                case 3:
                    if (suzhcombobox2.IsEnabled)
                    {

                        suzhtextbox2.IsEnabled = true;
                        suzhcombobox2.IsEnabled = false;
                        Grid.SetZIndex(suzhtextbox2, 1);
                        Grid.SetZIndex(suzhcombobox2, 0);
                    }
                    else
                    {
                        suzhtextbox2.IsEnabled = false;
                        suzhcombobox2.IsEnabled = true;
                        Grid.SetZIndex(suzhtextbox2, 0);
                        Grid.SetZIndex(suzhcombobox2, 1);
                    }
                    break;
                case 4:
                    if (suzhcombobox3.IsEnabled)
                    {

                        suzhtextbox3.IsEnabled = true;
                        suzhcombobox3.IsEnabled = false;
                        Grid.SetZIndex(suzhtextbox3, 1);
                        Grid.SetZIndex(suzhcombobox3, 0);
                    }
                    else
                    {
                        suzhtextbox3.IsEnabled = false;
                        suzhcombobox3.IsEnabled = true;
                        Grid.SetZIndex(suzhtextbox3, 0);
                        Grid.SetZIndex(suzhcombobox3, 1);
                    }
                    break;
            }

        }
    }

}

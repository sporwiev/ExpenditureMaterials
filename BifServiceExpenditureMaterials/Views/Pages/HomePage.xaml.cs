using BifServiceExpenditureMaterials.Controls;
using BifServiceExpenditureMaterials.Forms;
using BifServiceExpenditureMaterials.Helpers;
using Microsoft.AspNetCore.SignalR.Client;
using System.Data;
using System.Windows.Controls;
using System.Management;
using System.Windows.Media;
using System.Windows.Media.Animation;
using BifServiceExpenditureMaterials.Models;
using Wpf.Ui.Controls;
using BifServiceExpenditureMaterials.Views.Windows;
using System.Linq;
using BifServiceExpenditureMaterials.ViewModels.Pages;

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для HomePage.xaml
    /// </summary>
    public partial class HomePage : UserControl
    {

        public static string? CurrentMounth { get; set; }
        public static string? activeMonth { get; private set; }

        public static int CurrentYear;
        public static HomePage _home;
        public static bool isOpenFiltes = false;
        public static TabControl tab;
        public static TabItem? ActiveTabItem;
        private CancellationTokenSource? _cts;
        public static bool isUpdate = false;
        public HomeViewModel ViewModel { get; set; }

        
        public HomePage()
        {
            InitializeComponent();
            _home = this;
            //SearchTextBox.Text = GetSystemUUID();
            Loaded += HomePage_Loaded;
            MainWindow.SizeChangedEvent.AddOwner(typeof(HomePage));
            
            SizeChanged += HomePage_SizeChanged;
            //215D5922-38C0-4840-AE4F-88A4C2841B36
        }
        public HomePage(HomeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = this;
            this.ViewModel = viewModel;
        }

        private void HomePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            montableGl.Height = this.Height / 3;
        }


        //private void InitHub()
        //{
        //    _connection = new HubConnectionBuilder()
        //    .WithUrl("http://192.168.1.126:5000/hub") // IP сервера
        //    .WithAutomaticReconnect()
        //    .Build();
        //}
        public static string GetSystemUUID()
        {
            string uuid = "";
            using (var mc = new ManagementClass("Win32_ComputerSystemProduct"))
            {
                foreach (var o in mc.GetInstances())
                {
                    var mo = (ManagementObject)o;
                    uuid = mo["UUID"].ToString();
                    break;
                }
            }
            return uuid;
        }
        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            //await SignalRClient.InitializeAsync();
            //await SignalRClient.Connection.StartAsync();
            var mounthtext = DateTime.Now.ToString("MMMM");
            mounthtext = mounthtext.Replace(mounthtext[0], mounthtext[0].ToString().ToUpper()[0]);
            activeMonth = mounthtext;
            //if (SignalRClient.Connection.State == HubConnectionState.Disconnected)
            //{

            //    await SignalRClient.Connection.StartAsync();
            //}
            if (!isUpdate)
            {
                //await SignalRClient.SendNotificationAsync("0x01|Вход в систему: " + PatternSystemIndeficators.GetUser(GetSystemUUID()) + "|Вход");
                //if (App.filetabs != null)
                //{
                var year = 2024;
                for (; ; )
                {
                    if (year == 2030)
                        break;
                    YearComboBox.Items.Add(year++);

                }

                SearchTextBox.TextChanged += SearchTextBox_TextChanged;
                //}

                YearComboBox.SelectionChanged -= YearComboBox_SelectionChanged;
                YearComboBox.Text = DateTime.Now.Year.ToString();


                YearComboBox.SelectionChanged += YearComboBox_SelectionChanged;

                //SignalRClient.Connection.On<string>("ReceiveUpdate", message =>
                //{
                //    // Например, лог или вызов действия
                //    //MessageBox.Show($"Получено уведомление: {message}");
                //    Dispatcher.BeginInvoke(async () =>
                //    {
                //        DoubleAnimation animopacitystart = new() { To = 1, Duration = new TimeSpan(0, 0, 1) };

                //        var nomer = message.Split('|')[0];
                //        var mess = message.Split('|')[1];
                //        var title = message.Split('|')[2];
                //        infobar.BeginAnimation(InfoBar.OpacityProperty, animopacitystart);
                //        infobar.Message = mess;
                //        infobar.Title = title;
                //        switch (nomer)
                //        {
                //            case "0x01" or "0x03":
                //                infobar.Severity = InfoBarSeverity.Success;
                //                UpdateData(activeMonth, true);
                //                break;
                //            case "0x02":
                //                infobar.Severity = InfoBarSeverity.Error;
                //                UpdateData(activeMonth, true);
                //                break;
                //            case "0x04":
                //                infobar.Severity = InfoBarSeverity.Warning;
                //                UpdateData(activeMonth, true);
                //                break;
                //            case "0x05":
                //                infobar.Severity = InfoBarSeverity.Success;

                //                break;
                //            case "0x06":
                //                infobar.Severity = InfoBarSeverity.Error;
                //                break;
                //            case "0x07":
                //                infobar.Severity = InfoBarSeverity.Warning;
                //                break;


                //        }

                //        await Task.Delay(6000);
                //        animopacitystart.To = 0;
                //        infobar.BeginAnimation(InfoBar.OpacityProperty, animopacitystart);


                //    });

                //});
                //if(SignalRClient.Connection.State == HubConnectionState.Disconnected)
                //{
                    //System.Windows.MessageBox.Show(SignalRClient.Connection.State.ToString());
                    UpdateData(mounthtext, true);
                //}
                
            }
            else
            {
                var year = 2024;
                for (; ; )
                {
                    if (year == 2030)
                        break;
                    YearComboBox.Items.Add(year++);

                }

                SearchTextBox.TextChanged += SearchTextBox_TextChanged;
                //}

                //YearComboBox.SelectionChanged -= YearComboBox_SelectionChanged;
                

                UpdateData(mounthtext, true);
                YearComboBox.Text = DateTime.Now.Year.ToString();
                YearComboBox.SelectionChanged += YearComboBox_SelectionChanged;
                
                
                montableGl.Datagrid.UnselectAllCells();
            }
            foreach (var child in monthPanel.Children)
            {
                var button = child as System.Windows.Controls.Button;
                if (button.Content.ToString() == mounthtext)
                {
                    button.Background = Brushes.BlueViolet;
                   

                }
            }
            //await _connection.StartAsync();
            //}
            //else
            //{
            //    SaveProject.OnDownload(App.filetabs);
            //}

            //}
        }
        //public async Task Update()
        //{
        //    await Task.Run(async () =>
        //    {
        //        while (true)
        //        {
        //            _connection.On<string>("ReceiveUpdate", async (message) =>
        //            {
        //                // Здесь ты можешь перезагрузить данные
        //                if (message != "")
        //                {
        //                    await Dispatcher.InvokeAsync(() => UpdateData(GetTabItemByHeader(tabs,CurrentMounth)));
        //                }
        //            });
        //        }
        //    });
        //}
       
        
        
        
        public static void RefreshDataGrid()
        {
            _home.UpdateData(activeMonth,true);
        }
        public static MonthlyTable GetTable()
        {
            return null;
        }
        public async Task UpdateData(string month, bool is_update)
        {
            List<machine> machines = App.dBcontext.machine.ToList();
            List<Material> materials = App.dBcontext.Materials.ToList();
            montableGl.Datagrid.ItemsSource = null;
            montableGl.BuildTable();


            int i = 0;
            machines.ForEach(a => {

                    montableGl?.SetCellValue(montableGl?.Datagrid, i, 0, a.Code);
                    montableGl?.EnabledCell(montableGl?.Datagrid, i, 0, false);
                i++;

            });

            materials.ForEach(async material =>
            {


                int.TryParse(YearComboBox.Text as string, out var result);

                if (material.Год == result && activeMonth == material.Месяц)
                {

                    var row = Convert.ToInt32(material.Ячейка.Split(":")[0]);
                    var col = Convert.ToInt32(material.Ячейка.Split(":")[1]);

                    var gt = materials.OrderBy(e => e.Id).Where(e => e.НомерЯчейки == material.НомерЯчейки).ToList()[0].ТипТраты;

                        montableGl.SetCellBackground(montableGl.Datagrid, row, col, material.НомерЯчейки);

                        montableGl.SetCellValue(montableGl?.Datagrid, row, col, material.НомерЯчейки);

                }
            });

                montableGl.SearchCell(montableGl.Datagrid, 1, DateTime.Now.Day);

        }
        public static TabItem GetTabItemByHeader(TabControl tabControl, string header)
        {
            // Проходим по всем вкладкам в TabControl
            return tabControl.Items.OfType<TabItem>().FirstOrDefault(a => a.Header.ToString().IndexOf(header) != -1);
            //foreach (var item in tabControl.Items)
            //{
            //    if (item is TabItem tabItem && tabItem.Header.ToString().IndexOf(header) != -1)
            //    {
            //        return tabItem;  // Возвращаем найденный TabItem
            //    }
            //}
            //return null;  // Если не нашли TabItem с таким Header
        }
        public static string GetHeaderByTabItem(TabItem item)
        {

            return item.Header.ToString();

        }
        public static TabItem GetTabItemBySelectionIndex(TabControl tabControl, int index)
        {
            // Проверяем, что индекс допустим (не выходит за пределы списка вкладок)
            if (index >= 0 && index < tabControl.Items.Count)
            {
                return tabControl.Items[index] as TabItem;  // Возвращаем соответствующий TabItem
            }
            return null;  // Если индекс невалиден
        }

        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            var bt = ((ComboBox)sender).Text;
            
            CurrentYear = int.TryParse(bt, out var year) ? year : DateTime.Now.Year;

            UpdateData(activeMonth, true);
        }
        
        private void SearchTextBox_TextChanged(Wpf.Ui.Controls.AutoSuggestBox sender, Wpf.Ui.Controls.AutoSuggestBoxTextChangedEventArgs args)
        {
            _cts?.Cancel(); // отменяем предыдущий поиск
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            string search = SearchTextBox.Text.Trim().ToLower();

            montableGl.Datagrid.SelectedCells.Clear();
            Task.Run(async () =>
            {
                try
                {
                    // Задержка 300 мс
                    await Task.Delay(3000, token);
                }
                catch (TaskCanceledException)
                {
                    return;
                }

                if (token.IsCancellationRequested)
                    return;

                object foundItem = null;

                // Поиск и UI-обновление на Dispatcher
                await Dispatcher.InvokeAsync(() =>
                {
                    int rowCount = montableGl.Datagrid.Items.Count;
                    
                    for (int i = 0; i < rowCount; i++)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        DataGridCell cell = montableGl.NewGetCell(montableGl.Datagrid, i, 0);
                        if (cell?.Content is System.Windows.Controls.TextBlock ts)
                        {
                            if (ts.Text.ToLower().IndexOf(search) != -1)
                            {
                                var column = montableGl.Datagrid.Columns[0];
                                var cel = new DataGridCellInfo(montableGl.Datagrid.Items[i], column);
                                montableGl.Datagrid.SelectedCells.Add(cel);

                                montableGl.Datagrid.ScrollIntoView(montableGl.Datagrid.Items[i], column);
                                
                                break;
                            }
                        }
                    }
                });

            }, token);
        }
        private void AddMachineBtn_Click(object sender, RoutedEventArgs e)
        {
            new FormAddMachine().Show();
            
        }
        
        private void UpdateMachineBtn_Click(object sender, RoutedEventArgs e)
        {
            new FormUpdateMachine().Show();
        }

        private void SearchFilterButton_Click(object sender, RoutedEventArgs e)
        {
            //FormSearchFilters().Show();
        }
        private void UpdateFillTable_Click(object sender, RoutedEventArgs e)
        {
            UpdateData(activeMonth, true);
        }
        private void TypeProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var product = ((ComboBox)sender).SelectedIndex;

                switch (product)
                {
                    case 0:
                        ProductComboBox.ItemsSource = App.dBcontext.Oil?.Select(s => s.Name).ToList();
                        break;
                    case 1:

                        ProductComboBox.ItemsSource = App.dBcontext.Antifreeze?.Select(s => s.Name).ToList();
                        break;
                    case 2:
                        ProductComboBox.ItemsSource = App.dBcontext.Grease?.Select(s => s.Name).ToList();
                        break;
                    case 3:
                        ProductComboBox.ItemsSource = App.dBcontext.Filter?.Select(s => s.Name).ToList();
                        break;
                }
                ProductComboBox.SelectedIndex = 0;
                // = (System.Collections.IEnumerable)productdb;
            
        }
        public static void ViewOnPoint(int x,int y)
        {
            MonthlyTable.ViewCellOnPoints(x,y);
        }
        public void FilterProduct<T>(string month,T t,string value)
        {
            
            var name = "";
            MonthlyTable montableGl = null;
            var nametab = month;
            var table = montableGl;
            montableGl = table;
            montableGl.Datagrid.ItemsSource = null;
            montableGl.BuildTable();
            //RefreshDataGrid(montableGl.Datagrid);
            name = nametab;
            int i = 0;
            var oil = "";
            var antifreeze = new int();
            var grease = new int();
            var filter = new int();
            var listfilterPoints = new List<string>();
            List<machine> machines;
            List<Material> materials = new List<Material>();
            SolidColorBrush brush = new SolidColorBrush()
            { 
                Color = Color.FromRgb(
                    (byte)new Random().Next(1,255),
                    (byte)new Random().Next(1, 255),
                    (byte)new Random().Next(1, 255)
                ) 
            };

            {


                int.TryParse(YearComboBox.Text as string, out var resultmachineyear);
                machines = App.dBcontext.machine
                        .Where(m => m.Год == resultmachineyear)
                        .ToList();
                
                if (t is Oil)
                {
                    oil = App.dBcontext.Oil.Where(s => s.Name == value).First().Id.ToString();
                    materials = App.dBcontext.Materials.Where(s => s.oilCode == oil && s.Год == resultmachineyear).ToList();

                }
                if (t is Antifreeze)
                {
                    antifreeze = App.dBcontext.Antifreeze.Where(s => s.Name == value).First().Id;
                    materials = App.dBcontext.Materials.Where(s => s.antifreeze_id == antifreeze && s.Год == resultmachineyear).ToList();
                }
                if (t is Grease)
                {
                    grease = App.dBcontext.Grease.Where(s => s.Name == value).First().Id;
                    materials = App.dBcontext.Materials.Where(s => s.grease_id == grease && s.Год == resultmachineyear).ToList();
                }
                if (t is Filter)
                {
                    filter = App.dBcontext.Filter.Where(s => s.Name == value).First().id;
                    var countmaterial = App.dBcontext.CountMaterials.Where(s => s.count_filterids.IndexOf(filter.ToString()) != -1).Select(s => s.Id).ToList();
                    foreach(var mat in countmaterial)
                    {
                        var material = App.dBcontext.Materials.Where(s => s.countmaterial_id == mat && s.Год == resultmachineyear).First();
                        var row = Convert.ToInt32(material.Ячейка.Split(":")[0]);
                        var col = Convert.ToInt32(material.Ячейка.Split(":")[1]);
                        montableGl.SetCellValue(montableGl?.Datagrid,row,col, material.НомерЯчейки);
                        montableGl.SetCellBackground(montableGl?.Datagrid, row, col, brush);
                    }
                }
               //materials = App.dBcontext.Materials.ToList();
                //await App.dBcontext.DisposeAsync();
            }
            foreach (var machine in machines)
            {

                montableGl.SetCellValue(montableGl?.Datagrid, i, 0, machine?.Code);
                montableGl.EnabledCell(montableGl?.Datagrid, i, 0, false);

                i++;
            }
            foreach (var material in materials)
            {
                if (activeMonth == material.Месяц)
                {
                    var row = Convert.ToInt32(material.Ячейка.Split(":")[0]);
                    var col = Convert.ToInt32(material.Ячейка.Split(":")[1]);
                    var gt = materials.OrderBy(e => e.Id).Where(e => e.НомерЯчейки == material.НомерЯчейки).First().ТипТраты;
                    montableGl.SetCellValue(montableGl?.Datagrid, row, col, material.НомерЯчейки);

                    montableGl.SetCellBackground(montableGl.Datagrid, row, col, brush);

                    //montableGl.EnabledCell(montableGl?.Datagrid, Convert.ToInt32(material.Ячейка.Split(":")[0]), Convert.ToInt32(material.Ячейка.Split(":")[1]), false);
                }
            }
            montableGl.SearchCell(montableGl.Datagrid, 1, DateTime.Now.Day);
        }
        private void FilterSaveButton_Click(object sender,EventArgs e)
        {
            var but = sender as System.Windows.Controls.Button;
            if (but.Content.ToString() == "Применить")
            {
                var type = TypeProductComboBox;
                var product = ProductComboBox;

                {

                    switch (type.SelectedIndex)
                    {
                        case 0:
                            if(App.dBcontext.Oil.Where(s => s.Name == product.Text).Select(s => s.Name).Any())
                                FilterProduct<Oil>(activeMonth, new Oil(), App.dBcontext.Oil.Where(s => s.Name == product.Text).Select(s => s.Name).First());
                            break;
                        case 1:
                            if(App.dBcontext.Antifreeze.Where(s => s.Name == product.Text).Select(s => s.Name).Any())
                                FilterProduct<Antifreeze>(activeMonth, new Antifreeze(), App.dBcontext.Antifreeze.Where(s => s.Name == product.Text).Select(s => s.Name).First());

                            break;
                        case 2:
                            if(App.dBcontext.Grease.Where(s => s.Name == product.Text).Select(s => s.Name).Any())
                                FilterProduct<Grease>(activeMonth, new Grease(), App.dBcontext.Grease.Where(s => s.Name == product.Text).Select(s => s.Name).First());
                            break;
                        case 3:
                            if(App.dBcontext.Filter.Where(s => s.Name == product.Text).Select(s => s.Name).Any())
                                FilterProduct<Filter>(activeMonth, new Filter(), App.dBcontext.Filter.Where(s => s.Name == product.Text).Select(s => s.Name).First());
                            break;
                    }
                }
                but.Content = "Отменить";
            }
            else
            {
                but.Content = "Применить";
                UpdateData(activeMonth,true);
            }
        }

        private void ProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var product = (ComboBox)sender;
           
                
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var selectButton = ((System.Windows.Controls.Button)sender);
            await Task.Run(async () =>
            {
                await Dispatcher.InvokeAsync(async () =>
                 {
                     MainWindow.ActiveAnimate();

                     foreach (var child in monthPanel.Children)
                     {
                         var button = child as System.Windows.Controls.Button;
                         button.Background = Brushes.Transparent;

                     }

                     
                 });
                
                
                await Task.Delay(500);
                await Dispatcher.InvokeAsync(async () =>
                {
                    activeMonth = selectButton.Content.ToString();
                    await UpdateData(activeMonth, true);
                });
                await Task.Delay(500);
                await Dispatcher.InvokeAsync(async () =>
                {

                    

                    MainWindow.DisableAnimate();
                });
            });
            selectButton.Background = Brushes.BlueViolet;
        }

        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
           // var selectButton = ((System.Windows.Controls.Button)sender);
           // selectButton.Background = Brushes.Transparent;
        }
    }
    
}


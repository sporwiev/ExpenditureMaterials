using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
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
using BifServiceExpenditureMaterials.Autopattern;
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.Helpers;
using BifServiceExpenditureMaterials.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Wpf.Ui.Controls;

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddPatternMachine.xaml
    /// </summary>
    public partial class AddPatternMachine : UserControl
    {
        public AddPatternMachine()
        {
            InitializeComponent();
            Loaded += AddPatternMachine_Loaded;
            GetNotification();

        }

        private void AddPatternMachine_Loaded(object sender, RoutedEventArgs e)
        {
            BuildPatternMachine();
            BuildNameMachine();
        }

        private void BuildNameMachine()
        {
            
            {
                listmachine.ItemsSource = App.dBcontext.machine.Select(s => s.Code).ToList();
            }
        }

        private async void BuildPatternMachine()
        {
            listnamemachine.Items.Clear();
            listpatternmachine.Items.Clear();
            listbuttonsubmit.Items.Clear();
            listbuttondelete.Items.Clear();
            try
            {
                
                {
                    int index = 0;
                    foreach(var line in App.dBcontext.patternMachines.ToList()) {
                        int i = index;
                        var submit = new Wpf.Ui.Controls.Button { Content = "Изменить", VerticalAlignment = VerticalAlignment.Center, Height = 40 };
                        var delete = new Wpf.Ui.Controls.Button { Content = "❌", VerticalAlignment = VerticalAlignment.Center, Height = 40 };
                        submit.Click += (sender, e) => EditPattern_Click(sender, e, line.Id,i);
                        delete.Click += (sender, e) => DeletePattern_Click(sender, e, line.Id);
                        listnamemachine.Items.Add(new Wpf.Ui.Controls.TextBox() { Text = line.name ,VerticalAlignment = VerticalAlignment.Center, Height = 40,IsEnabled = false});
                        listpatternmachine.Items.Add(new Wpf.Ui.Controls.TextBox() { Text =  line.pattern, VerticalAlignment = VerticalAlignment.Center, Height = 40 });
                        listbuttonsubmit.Items.Add(submit);
                        listbuttondelete.Items.Add(delete);
                        index++;
                    }
                }
                
            }
            catch(Exception ex)
            {
                //await SignalRClient.SendNotificationAsync("0x02322|" + ex.Message + "|Ошибка");
            }
        }

        private async void DeletePattern_Click(object sender, RoutedEventArgs e, int i)
        {
            //try
            //{
                
                {

                    var pattern = App.dBcontext.patternMachines.Where(s => s.Id == i).First();
                    App.dBcontext.patternMachines.Remove(pattern);
                    App.dBcontext.SaveChanges();
                    //await SignalRClient.SendNotificationAsync("0x02321|Удаление правила для машины: " + pattern.name + "|Удаление");
                   // await App.dBcontext.RecreateTableWithDataAsync("patternMachines",true);
                }
            //}
            //catch (Exception ex)
            //{
            //    await SignalRClient.SendNotificationAsync("0x02322|" + ex.Message + "|Ошибка");
            //}
            BuildPatternMachine();
            
        }

        private async void GetNotification()
        {
            //if (SignalRClient.Connection.State == HubConnectionState.Disconnected)
            //{
            //    await SignalRClient.Connection.StartAsync();
            //}
            //SignalRClient.Connection.On<string>("ReceiveUpdate", message =>
            //{
                // Например, лог или вызов действия
                //MessageBox.Show($"Получено уведомление: {message}");
                //Dispatcher.BeginInvoke(async () =>
                //{
                //    DoubleAnimation animopacitystart = new() { To = 1, Duration = new TimeSpan(0, 0, 1) };

                //    var nomer = message.Split('|')[0];
                //    var mess = message.Split('|')[1];
                //    var title = message.Split('|')[2];
                //    infobar.BeginAnimation(InfoBar.OpacityProperty, animopacitystart);
                //    infobar.Message = mess;
                //    infobar.Title = title;
                //    switch (nomer)
                //    {
                //        case "0x02321":
                //            infobar.Severity = InfoBarSeverity.Success;
                //            break;
                //        case "0x02322": 
                //            infobar.Severity = InfoBarSeverity.Error;
                //            break;


                //    }

                //    await Task.Delay(6000);
                //    animopacitystart.To = 0;
                //    infobar.BeginAnimation(InfoBar.OpacityProperty, animopacitystart);


                //});

            //});
        }
        private async void EditPattern_Click(object sender, RoutedEventArgs e, int indexpattern,int index)
        {
            try
            {
                
                {
                    
                    var pattern = App.dBcontext.patternMachines.Where(s => s.Id == indexpattern).First();
                    var newpattern = (Wpf.Ui.Controls.TextBox)listpatternmachine.Items[index];
                    pattern.pattern = newpattern.Text;
                    App.dBcontext.SaveChanges();
                   // await SignalRClient.SendNotificationAsync("0x02321|Изменено правило для машины: " + pattern.name + "|Изменение");
                    //await App.dBcontext.RecreateTableWithDataAsync("patternMachines",true);
                }
            }
            catch (Exception ex)
            {
                //await SignalRClient.SendNotificationAsync("0x02322|" + ex.Message + "|Ошибка");
            }
            BuildPatternMachine();

        }

        private async void AddNewPattern_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                {
                    App.dBcontext.patternMachines.Add(new PatternMachine() { name = listmachine.Text, pattern = machineeditorpattern.Text });
                    App.dBcontext.SaveChanges();
                    //await SignalRClient.SendNotificationAsync("0x02321|Добавлено новое правило для машины: " + listmachine.Text + "|Добавлено");
                }
            }
            catch (Exception ex)
            {
                //await SignalRClient.SendNotificationAsync("0x02322|" + ex.Message + "|Ошибка");
            }
            
            BuildPatternMachine();
        }

    }
}

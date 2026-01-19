using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.Helpers;
using BifServiceExpenditureMaterials.Models;
using BifServiceExpenditureMaterials.Views.Pages;
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
using System.Windows.Shapes;

namespace BifServiceExpenditureMaterials.Forms
{
    /// <summary>
    /// Логика взаимодействия для FormAddMachine.xaml
    /// </summary>
    public partial class FormAddMachine : Window
    {
        public FormAddMachine()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var list = new List<string>()
            {
                "2025",
                "2024",
                "2026",
                "2027",
                "2028",
                "2029",
            };
            YearComboBox.ItemsSource = list;

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            
            {
                machine machine = new machine();
                machine.Год = int.TryParse(YearComboBox.Text, out var year) ? year : DateTime.Now.Year;
                machine.Code = MachineTextBox.Text;
                await App.dBcontext.machine.AddAsync(machine);
                await App.dBcontext.SaveChangesAsync();
                //await App.dBcontext.DisposeAsync();
            }
            await SignalRClient.SendNotificationAsync($"0x03|{MachineTextBox.Text} Машина добавлена|Добавление");
            //HomePage._home.UpdateData(activeTab);
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

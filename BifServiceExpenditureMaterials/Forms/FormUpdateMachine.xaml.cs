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
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.Helpers;
using BifServiceExpenditureMaterials.Views.Pages;

namespace BifServiceExpenditureMaterials.Forms
{
    /// <summary>
    /// Логика взаимодействия для FormUpdateMachine.xaml
    /// </summary>
    public partial class FormUpdateMachine : Window
    {
        public FormUpdateMachine()
        {
            InitializeComponent();
            MachineComboBox.ItemsSource = App.dBcontext.machine.Select(s => s.Code).ToList();
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            
            {
                
                var machine = App.dBcontext.machine.OrderBy(s => s.Id).Where(s => s.Code == MachineComboBox.Text).ToList()[0];
                machine.Code = NewMachineTextBox.Text;
                await App.dBcontext.SaveChangesAsync();
                //await App.dBcontext.DisposeAsync();
            }
            await SignalRClient.SendNotificationAsync($"0x04|Машина - {MachineComboBox.Text} Изменена на {NewMachineTextBox.Text}|Изменение");
            //HomePage._home.UpdateData(activeTab);
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

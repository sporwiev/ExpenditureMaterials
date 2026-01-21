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
using BifServiceExpenditureMaterials.ViewModels.Windows;

namespace BifServiceExpenditureMaterials.Forms
{
    /// <summary>
    /// Логика взаимодействия для FormPrint.xaml
    /// </summary>
    public partial class FormPrint : Window
    {

        public FormPrintViewModel? ViewModel;

        public FormPrint(FormPrintViewModel ViewModel)
        {
            InitializeComponent();
            DataContext = this;
            this.ViewModel = ViewModel;
            MessageBox.Show(ViewModel.DayComboBox.Count.ToString());
        }
        public FormPrint()
        {
            InitializeComponent();
        }
        

    }
}

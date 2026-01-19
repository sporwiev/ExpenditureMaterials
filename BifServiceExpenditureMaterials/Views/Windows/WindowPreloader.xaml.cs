using Accessibility;
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.Forms;
using BifServiceExpenditureMaterials.Helpers;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using System.Windows.Shapes;

namespace BifServiceExpenditureMaterials.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowPreloader.xaml
    /// </summary>
    public partial class WindowPreloader : Window
    {
        private int i = 0;
        public static WindowPreloader window;
        public static bool isConnected; 
        IHost host { get; set; }
        
        public WindowPreloader(IHost host)
        {
            InitializeComponent();
            window = this;
            this.host = host;
            Loaded += WindowPreloader_Loaded;
            curvers.Text = App.CurrentVersion;
            
        }
        
        private async void WindowPreloader_Loaded(object sender, RoutedEventArgs e)
        {
            
           
            if (LoadingAnim()) 
                await Dispatcher.Invoke(async () => await host.StartAsync());
        }

        public bool GetProccess(string name)
        {
            Process[] processes = Process.GetProcesses();
            return processes.OrderBy(p => p.ProcessName).Select(p => p.ProcessName).Where(p => p.ToUpper() == name.ToUpper()).Any();
        }
        public bool LoadingAnim()
        {

            
            
            return true;
            
        }
    }
}

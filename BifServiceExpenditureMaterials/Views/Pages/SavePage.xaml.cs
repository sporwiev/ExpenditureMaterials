using BifServiceExpenditureMaterials.Database;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Data.Common;
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
using Microsoft.EntityFrameworkCore;
using BifServiceExpenditureMaterials.Helpers;
using Microsoft.Win32;

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для SavePage.xaml
    /// </summary>
    public partial class SavePage : UserControl
    {
        public SavePage()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
            //{
            //    var files = App.dBcontext.files.OrderBy(e => e.id);
            //    foreach (var item in files)
            //    {
            //        var menuContext = new MenuItem()
            //        {
            //            Header = "Открыть"
            //        };
            //        menuContext.Click += (sender, e) => MenuContext_Click(sender, e, item.fullname);
            //        var list = new ListViewItem()
            //        {
            //            Content = item.name,
            //            ContextMenu = new ContextMenu() { ItemsSource = new[] { menu } }
            //        };
            //        listfiles.Items.Add(list);
            //    }
            //}
        }

        private void MenuContext_Click(object sender, RoutedEventArgs e,string fullname)
        {
            File.Open(fullname,FileMode.Open);
            Environment.Exit(0);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var time = DateTime.Now.ToString().Replace(":", "_");
            SaveProject.OnSave(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\BifService","Снимок_" + time + ".bif");
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            var a = "Файлы учета компании (*.bif)|*.bif";
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = a;
            if (ofd.ShowDialog() == true) {
                SaveProject.OnDownload(ofd.FileName);
            }
        }
    }
}


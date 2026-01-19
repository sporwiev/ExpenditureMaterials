using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Win32;
using Microsoft.AspNetCore.SignalR.Client;
using BifServiceExpenditureMaterials.Helpers;
using BifServiceExpenditureMaterials.UpdateApp;
using System.Management;

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для UpdateApplication.xaml
    /// </summary>
    public partial class UpdateApplicationPage : UserControl
    {
        public UpdateApplicationPage()
        {
            InitializeComponent();
            Loaded += UpdateApplicationPage_Loaded;
            
            
        }

        private async void UpdateApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentVersion.Text = "Текущая версия: " + App.CurrentVersion;
            HttpClient client = new HttpClient();
            var JsonResponse = await client.GetAsync("http://" + Networks.GetIp() + ":5000/files");
            var result = await JsonSerializer.DeserializeAsync<List<string>>(await JsonResponse.Content.ReadAsStreamAsync());
            var currentversion = result.Count;
            ListBoxVersionProject.Visibility = Visibility.Visible;
            ListBoxVersionProject.ItemsSource = result;
            if (GetSystemUUID() == "215D5922-38C0-4840-AE4F-88A4C2841B36")
            {

                ButtonInstallProject.Content = "Загрузить новую версию";
                ButtonInstall.Visibility = Visibility.Visible;
            }
            else
            {
                ButtonInstallProject.Content = "Установить актуальную версию";
                ButtonInstall.Visibility = Visibility.Collapsed;
            }
            ListBoxVersionProject.SelectedIndex = currentversion - 1;
        }

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
        private async void ButtonInstallProject_Click(object sender, RoutedEventArgs e)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("http://" + Networks.GetIp() + ":5000/PreviewVersion");
            var JsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<string>(JsonResult);
            var itog = new FileInfo(result).Name.Split('_')[1];
            var len = itog.Length - 4;
            itog = itog.Substring(0, len);

            if (GetSystemUUID() == "215D5922-38C0-4840-AE4F-88A4C2841B36")
            {
                OpenFileDialog ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == true)
                {
                    await UpdateNewVersionAsync(ofd.FileName);
                    SignalRClient.NotifyUpdateAvailable(itog);
                }
            }else
            {

                UpdateManager.InstallUpdate(itog);
            }

        }
        private async void ButtonInstall_Click(object sender, RoutedEventArgs e)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync("http://" + Networks.GetIp() + ":5000/PreviewVersion");
            var JsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<string>(JsonResult);
            var itog = new FileInfo(result).Name.Split('_')[1];
            var len = itog.Length - 4;
            itog = itog.Substring(0, len);

            UpdateManager.InstallUpdate(itog);
            

        }
        public async Task UpdateNewVersionAsync(string zipPath)
        {
            using var client = new HttpClient();
            using var form = new MultipartFormDataContent();
            using var filestream = File.OpenRead(zipPath);
            var fileContent = new StreamContent(filestream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
            form.Add(fileContent, "file", Path.GetFileName(zipPath));
            var response = await client.PostAsync("http://" + Networks.GetIp() + ":5000/upload", form);
            var result = await response.Content.ReadAsStringAsync();
        }

        
    }
    
}

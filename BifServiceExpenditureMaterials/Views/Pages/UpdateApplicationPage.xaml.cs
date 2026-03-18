using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Windows.Controls;
using BifServiceExpenditureMaterials.Helpers;
using BifServiceExpenditureMaterials.UpdateApp;
using Microsoft.Win32;

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Страница обновления приложения. Показывает список версий на сервере
    /// и позволяет установить актуальную версию или (для администратора) загрузить новую.
    /// </summary>
    public partial class UpdateApplicationPage : UserControl
    {
        private static readonly string AdminUuid = "215D5922-38C0-4840-AE4F-88A4C2841B36";

        public UpdateApplicationPage()
        {
            InitializeComponent();
            Loaded += UpdateApplicationPage_Loaded;
        }

        // ─── Загрузка страницы ─────────────────────────────────────────────────

        private async void UpdateApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentVersion.Text = $"Текущая версия: {App.CurrentVersion}";

            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                var response = await client.GetAsync($"http://{Networks.GetIp()}:5000/files");
                response.EnsureSuccessStatusCode();

                var versions = await JsonSerializer.DeserializeAsync<List<string>>(
                    await response.Content.ReadAsStreamAsync());

                if (versions == null || versions.Count == 0)
                {
                    StatusText.Text = "Список версий пуст";
                    return;
                }

                ListBoxVersionProject.ItemsSource = versions;
                ListBoxVersionProject.SelectedIndex = versions.Count - 1;

                bool isAdmin = GetSystemUUID() == AdminUuid;
                ButtonInstallProject.Visibility = isAdmin ? Visibility.Collapsed : Visibility.Visible;
                ButtonInstall.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;

                StatusText.Text = $"Доступно версий: {versions.Count}";
            }
            catch (Exception ex)
            {
                StatusText.Text = "Не удалось подключиться к серверу обновлений";
                Debug.WriteLine($"[UpdateApplicationPage] Ошибка загрузки версий: {ex.Message}");
            }
        }

        // ─── UUID системы ──────────────────────────────────────────────────────

        private static string GetSystemUUID()
        {
            try
            {
                using var mc = new ManagementClass("Win32_ComputerSystemProduct");
                foreach (ManagementObject mo in mc.GetInstances())
                    return mo["UUID"]?.ToString() ?? "";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdateApplicationPage] GetSystemUUID error: {ex.Message}");
            }
            return "";
        }

        // ─── Установить версию (пользователь) ─────────────────────────────────

        private async void ButtonInstallProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var version = await FetchLatestVersionNameAsync();
                if (version == null) return;
                UpdateManager.InstallUpdate(version);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdateApplicationPage] Ошибка установки: {ex.Message}");
                MessageBox.Show($"Ошибка установки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ─── Загрузить новую версию на сервер (администратор) ─────────────────

        private async void ButtonInstall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ofd = new OpenFileDialog
                {
                    Filter = "ZIP-архив (*.zip)|*.zip",
                    Title = "Выберите архив с новой версией"
                };

                if (ofd.ShowDialog() != true) return;

                var version = await FetchLatestVersionNameAsync();
                if (version == null) return;

                await UploadNewVersionAsync(ofd.FileName);
                SignalRClient.NotifyUpdateAvailable(version);

                MessageBox.Show("Версия успешно загружена на сервер.", "Готово",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdateApplicationPage] Ошибка загрузки на сервер: {ex.Message}");
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ─── Вспомогательные методы ────────────────────────────────────────────

        private static async Task<string?> FetchLatestVersionNameAsync()
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            var response = await client.GetAsync($"http://{Networks.GetIp()}:5000/PreviewVersion");
            response.EnsureSuccessStatusCode();

            var jsonResult = await response.Content.ReadAsStringAsync();
            var path = JsonSerializer.Deserialize<string>(jsonResult);

            if (string.IsNullOrEmpty(path))
                return null;

            var fileName = new FileInfo(path).Name;
            var parts = fileName.Split('_');
            if (parts.Length < 2) return fileName;

            var versionWithExt = parts[1];
            return versionWithExt.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                ? versionWithExt[..^4]
                : versionWithExt;
        }

        private static async Task UploadNewVersionAsync(string zipPath)
        {
            using var client = new HttpClient();
            using var form = new MultipartFormDataContent();
            using var fileStream = File.OpenRead(zipPath);
            var content = new StreamContent(fileStream);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
            form.Add(content, "file", Path.GetFileName(zipPath));
            var response = await client.PostAsync($"http://{Networks.GetIp()}:5000/upload", form);
            response.EnsureSuccessStatusCode();
        }
    }
}

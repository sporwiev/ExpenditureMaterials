using System.Diagnostics;
using System.IO;
using System.Windows.Controls;
using BifServiceExpenditureMaterials.Helpers;
using Microsoft.Win32;

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Страница сохранения/загрузки снимков базы данных и отправки файлов другим пользователям.
    /// </summary>
    public partial class SavePage : UserControl
    {
        private static readonly string SaveDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BifService");

        public SavePage()
        {
            InitializeComponent();
        }

        // ─── Загрузка списка снимков ────────────────────────────────────────────

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshFileList();
        }

        private void RefreshFileList()
        {
            try
            {
                listfiles.Items.Clear();

                if (!Directory.Exists(SaveDir))
                {
                    Directory.CreateDirectory(SaveDir);
                    return;
                }

                var files = Directory.GetFiles(SaveDir, "*.bif")
                    .OrderByDescending(f => new FileInfo(f).LastWriteTime)
                    .ToList();

                foreach (var path in files)
                {
                    var info = new FileInfo(path);
                    var item = new ListBoxItem
                    {
                        Content = $"{info.Name}  ({info.LastWriteTime:dd.MM.yyyy HH:mm})",
                        Tag = path,
                        Padding = new Thickness(6, 4, 6, 4)
                    };
                    item.MouseDoubleClick += (s, e) => OpenSnapshot((string)((ListBoxItem)s).Tag);
                    listfiles.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SavePage] Ошибка обновления списка файлов: {ex.Message}");
            }
        }

        // ─── Открытие снимка двойным кликом ───────────────────────────────────

        private void OpenSnapshot(string fullPath)
        {
            try
            {
                if (!File.Exists(fullPath))
                {
                    MessageBox.Show("Файл не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var answer = MessageBox.Show(
                    $"Загрузить снимок?\n{Path.GetFileName(fullPath)}",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (answer == MessageBoxResult.Yes)
                    SaveProject.OnDownload(fullPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SavePage] Ошибка открытия снимка: {ex.Message}");
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ─── Кнопка «Сохранить» ────────────────────────────────────────────────

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                SaveProject.OnSave(SaveDir, $"Снимок_{timestamp}.bif");

                MessageBox.Show("Снимок успешно сохранён.", "Сохранено",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                RefreshFileList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SavePage] Ошибка сохранения: {ex.Message}");
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ─── Кнопка «Загрузить» ────────────────────────────────────────────────

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ofd = new OpenFileDialog
                {
                    Filter = "Файлы учёта (*.bif)|*.bif",
                    Title = "Выберите снимок базы данных"
                };

                if (ofd.ShowDialog() == true)
                {
                    SaveProject.OnDownload(ofd.FileName);
                    MessageBox.Show("База данных загружена.", "Готово",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    RefreshFileList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SavePage] Ошибка загрузки: {ex.Message}");
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ─── Кнопка «Отправить» ────────────────────────────────────────────────

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            // Функционал отправки по SignalR — заготовка
            MessageBox.Show("Функция отправки файла пока не реализована.", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

using System.Diagnostics;
using System.IO;
using System.Windows.Threading;
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.Services;
using BifServiceExpenditureMaterials.Services.Interfaces;
using BifServiceExpenditureMaterials.ViewModels.Pages;
using BifServiceExpenditureMaterials.ViewModels.Windows;
using BifServiceExpenditureMaterials.Views.Pages;
using BifServiceExpenditureMaterials.Views.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;
using Wpf.Ui.DependencyInjection;

namespace BifServiceExpenditureMaterials
{
    /// <summary>
    /// Точка входа приложения. Настройка DI-контейнера, запуск хоста и обработка глобальных ошибок.
    /// </summary>
    public partial class App
    {
        private TaskbarIcon? _trayIcon;

        public static string? filetabs;

        /// <summary>Текущая версия приложения.</summary>
        public static string CurrentVersion = "0.1.35";

        /// <summary>Глобальный контекст базы данных.</summary>
        public static AppDbContext dBcontext { get; set; } = null!;

        // ─── Конфигурация хоста и DI ────────────────────────────────────────────

        private static readonly IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(c =>
            {
                c?.SetBasePath(Path.GetDirectoryName(AppContext.BaseDirectory));
            })
            .ConfigureServices((context, services) =>
            {
                services.AddNavigationViewPageProvider();

                services.AddHostedService<ApplicationHostService>();

                // ─── Сервисы приложения (MVVM) ──────────────────────────
                services.AddSingleton<AppDbContext>();
                services.AddSingleton<IMaterialService, MaterialService>();
                services.AddSingleton<IMachineService, MachineService>();
                services.AddSingleton<IDialogService, DialogService>();

                // ─── WPF-UI сервисы ─────────────────────────────────────
                services.AddSingleton<IThemeService, ThemeService>();
                services.AddSingleton<ITaskBarService, TaskBarService>();
                services.AddSingleton<INavigationService, NavigationService>();

                // ─── Главное окно ───────────────────────────────────────
                services.AddSingleton<INavigationWindow, MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

                // ─── Страницы и их ViewModels ───────────────────────────
                services.AddSingleton<HomePage>();
                services.AddSingleton<HomeViewModel>();

                services.AddSingleton<SavePage>();
                services.AddSingleton<SaveViewModel>();

                services.AddSingleton<ViewMachinePage>();
                services.AddSingleton<ViewMachneViewModel>();

                services.AddSingleton<AddPage>();
                services.AddSingleton<AddViewModel>();

                services.AddSingleton<BayMaterialsPage>();
                services.AddSingleton<BayMaterialsViewModel>();

                services.AddSingleton<UpdateApplicationPage>();
                services.AddSingleton<UpdateApplicationViewModel>();

                services.AddSingleton<AnalitickPage>();
                services.AddSingleton<AnalitickViewModel>();

                services.AddSingleton<VisualizeExpenditure>();
                services.AddSingleton<AddPatternMachine>();
                services.AddSingleton<DynamicViewMaterialElement>();

            }).Build();

        /// <summary>
        /// Контейнер DI-сервисов.
        /// </summary>
        public static IServiceProvider Services => _host.Services;

        // ─── Жизненный цикл приложения ──────────────────────────────────────────

        /// <summary>
        /// Запуск приложения: инициализация БД, трей-иконки и окна загрузки.
        /// </summary>
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            try
            {
                var trayResource = FindResource("TrayIcon");
                _trayIcon = trayResource as TaskbarIcon;

                await Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        dBcontext = new AppDbContext();
                        var loader = new WindowPreloader(_host);
                        loader.Show();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[App] Ошибка инициализации БД: {ex.Message}");
                        MessageBox.Show(
                            $"Не удалось подключиться к базе данных:\n{ex.Message}",
                            "Критическая ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[App] Ошибка запуска приложения: {ex.Message}");
                MessageBox.Show(
                    $"Ошибка при запуске приложения:\n{ex.Message}",
                    "Критическая ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Завершение приложения: освобождение ресурсов.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            try
            {
                _trayIcon?.Dispose();
                await _host.StopAsync();

                if (dBcontext != null)
                    await dBcontext.DisposeAsync();

                _host.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[App] Ошибка при завершении: {ex.Message}");
            }
        }

        /// <summary>
        /// Глобальный обработчик необработанных исключений UI-потока.
        /// Логирует ошибку и показывает сообщение пользователю.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Debug.WriteLine($"[App] Необработанное исключение: {e.Exception}");

            try
            {
                // Записываем ошибку в файл лога
                var logPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "BifService", "error.log");

                var logDir = Path.GetDirectoryName(logPath);
                if (!string.IsNullOrEmpty(logDir) && !Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);

                var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {e.Exception}\n\n";
                File.AppendAllText(logPath, logMessage);
            }
            catch
            {
                // Не пробрасываем ошибку логирования
            }

            MessageBox.Show(
                $"Произошла непредвиденная ошибка:\n{e.Exception.Message}\n\n" +
                "Приложение попытается продолжить работу.",
                "Ошибка приложения",
                MessageBoxButton.OK, MessageBoxImage.Warning);

            // Помечаем исключение как обработанное — приложение не закрывается
            e.Handled = true;
        }

        // ─── Трей-меню ──────────────────────────────────────────────────────────

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            Current.MainWindow?.Show();
            Current.MainWindow?.Activate();
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            _trayIcon?.Dispose();
            Current.Shutdown();
        }
    }
}

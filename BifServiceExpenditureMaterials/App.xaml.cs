using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Threading;
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.Helpers;
using BifServiceExpenditureMaterials.Services;
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
    /// Interaction logic for App.xaml
    /// </summary>
    
    public partial class App
    {
        private TaskbarIcon _trayIcon;

        public static string? filetabs;

        public static string CurrentVersion = "0.1.35";

        public static AppDbContext dBcontext { get; set; }
        
        // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static readonly IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration(c => { c?.SetBasePath(Path.GetDirectoryName(AppContext.BaseDirectory)); })
            .ConfigureServices((context, services) =>
            {
                services.AddNavigationViewPageProvider();

                services.AddHostedService<ApplicationHostService>();


                // Theme manipulation
                services.AddSingleton<IThemeService, ThemeService>();

                // TaskBar manipulation
                services.AddSingleton<ITaskBarService, TaskBarService>();

                // Service containing navigation, same as INavigationWindow... but without window
                services.AddSingleton<INavigationService, NavigationService>();

                // Main window with navigation
                services.AddSingleton<INavigationWindow, MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

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
        /// Gets services.
        /// </summary>
        public static IServiceProvider Services
        {
            get { return _host.Services; }
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            //filetabs = e.Args.Length > 0 ? e.Args[0] : null;
            _trayIcon = (TaskbarIcon)FindResource("TrayIcon");

            await Task.Run(async () =>
            {
                await Dispatcher.BeginInvoke(() =>
                {
                    dBcontext = new AppDbContext();
                    WindowPreloader loader = new WindowPreloader(_host);
                    loader.Show();
                });
            });
            
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            _trayIcon?.Dispose();
            await _host.StopAsync();
            await dBcontext.DisposeAsync();
            _host.Dispose();
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
        }

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {
            Current.MainWindow?.Show();
            Current.MainWindow?.Activate();
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            _trayIcon.Dispose(); // Важно!
            Current.Shutdown();
        }
    }
}

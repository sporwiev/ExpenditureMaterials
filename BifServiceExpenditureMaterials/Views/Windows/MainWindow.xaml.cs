using BifServiceExpenditureMaterials.Helpers;
using BifServiceExpenditureMaterials.UpdateApp;
using BifServiceExpenditureMaterials.ViewModels.Windows;
using BifServiceExpenditureMaterials.Views.Pages;
using Microsoft.AspNetCore.SignalR.Client;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using Wpf.Ui;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BifServiceExpenditureMaterials.Views.Windows
{
    public partial class MainWindow : INavigationWindow
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;

        public static MainWindow? window;
        public MainWindowViewModel ViewModel { get; }

        private string version = ""; 
        public MainWindow(
            
            MainWindowViewModel viewModel,
            INavigationViewPageProvider navigationViewPageProvider,
            INavigationService navigationService
        )
        {

            ViewModel = viewModel;
            DataContext = this;
            window = this;
            
            SystemThemeWatcher.Watch(this);

            InitializeComponent();
            
            SetPageService(navigationViewPageProvider);

            navigationService.SetNavigationControl(RootNavigation);
            
            //EqualsVersion();
            RootNavigation.FrameMargin = new Thickness(1, 0, 0, 0);

            //System.Windows.MessageBox.Show(IPAddress.IPv6Any.ToString());

        }

        #region INavigationWindow methods

        public INavigationView GetNavigation() => RootNavigation;

        public bool Navigate(Type pageType) => RootNavigation.Navigate(typeof(HomePage));

        public void SetPageService(INavigationViewPageProvider navigationViewPageProvider) => RootNavigation.SetPageProviderService(navigationViewPageProvider);

        public void ShowWindow() => Show();


        public static void ActiveAnimate()
        {
            var win = window;
            win.boxOpac.Visibility = Visibility.Visible;
            win.messageOpac.Visibility=Visibility.Visible;
        }
        public static void DisableAnimate()
        {
            var win = window;
            win.boxOpac.Visibility = Visibility.Collapsed;
            win.messageOpac.Visibility = Visibility.Collapsed;
        }
        public void BringToFront()
        {
            var hWnd = new WindowInteropHelper(this).Handle;

            // Разворачиваем, если окно свернуто
            ShowWindow(hWnd, SW_RESTORE);

            // Пытаемся сделать его активным
            SetForegroundWindow(hWnd);
            SetActiveWindow(hWnd);

            // Временно делаем Topmost, чтобы пробиться поверх других
            this.Topmost = true;
            this.Topmost = false;
        }

        public void CloseWindow() => Close();

        #endregion INavigationWindow methods

        /// <summary>
        /// Raises the closed event.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Make sure that closing this window will begin the process of closing the application.
            Application.Current.Shutdown();
        }

        INavigationView INavigationWindow.GetNavigation()
        {
            throw new NotImplementedException();
            
        }

        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            throw new NotImplementedException();
        }
        
            
            
            
        //}

        private async void FluentWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowPreloader.window.Close();
                await SignalRClient.InitializeAsync();
                SignalRClient.Connection.On<string>("ReceiveUpdateNotification", async (version) => 
                {
                    var result = System.Windows.MessageBox.Show(
                        $"Доступна новая версия: {version}\nУстановить сейчас?",
                        "Обновление", System.Windows.MessageBoxButton.YesNo);

                    if (result == System.Windows.MessageBoxResult.Yes)
                    {
                        UpdateManager.InstallUpdate(version);
                    }
                });
                
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message + "\r\n" + "Попросите разработчика перезапустить сервер");
            }
        }

        private void SnackbarPresenter_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //System.Windows.MessageBox.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UpdateManager.InstallUpdate(version);
        }
    }
}

using BifServiceExpenditureMaterials.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui;

namespace BifServiceExpenditureMaterials.Services
{
    /// <summary>
    /// Хостируемый сервис приложения.
    /// Управляет жизненным циклом: при старте открывает главное окно,
    /// при завершении — корректно останавливает хост.
    /// </summary>
    public class ApplicationHostService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private INavigationWindow? _navigationWindow;

        public ApplicationHostService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Запускается, когда хост готов к работе — открывает главное окно навигации.
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await HandleActivationAsync();
        }

        /// <summary>
        /// Вызывается при завершении приложения. Выполняет корректное завершение работы.
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        /// <summary>
        /// Создаёт и показывает главное окно, если оно ещё не открыто.
        /// После открытия навигирует на HomePage.
        /// </summary>
        private async Task HandleActivationAsync()
        {
            if (!Application.Current.Windows.OfType<MainWindow>().Any())
            {
                _navigationWindow = (_serviceProvider.GetService(typeof(INavigationWindow)) as INavigationWindow)!;
                _navigationWindow!.ShowWindow();
                _navigationWindow.Navigate(typeof(Views.Pages.HomePage));
            }

            await Task.CompletedTask;
        }
    }
}

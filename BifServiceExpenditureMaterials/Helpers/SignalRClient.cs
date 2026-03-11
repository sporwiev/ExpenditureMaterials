using System;
using System.Threading.Tasks;
using BifServiceExpenditureMaterials.UpdateApp;
using Microsoft.AspNetCore.SignalR.Client;

namespace BifServiceExpenditureMaterials.Helpers
{
    /// <summary>
    /// Клиент SignalR для получения уведомлений об обновлениях и отправки сообщений на сервер.
    /// Адрес хаба: http://&lt;локальный IP&gt;:5000/hub
    /// </summary>
    public static class SignalRClient
    {
        /// <summary>Активное подключение к SignalR-хабу.</summary>
        public static HubConnection? Connection { get; private set; }

        /// <summary>
        /// Инициализирует и запускает подключение к SignalR-хабу.
        /// Если соединение уже установлено — повторная инициализация не выполняется.
        /// </summary>
        public static async Task InitializeAsync()
        {
            // Если соединение уже активно — ничего не делаем
            if (Connection?.State == HubConnectionState.Connected)
                return;

            // Определяем адрес сервера по локальному IP-адресу машины
            var localIp = Networks.GetIp();
            if (string.IsNullOrEmpty(localIp))
                return;

            Connection = new HubConnectionBuilder()
                .WithUrl($"http://{localIp}:5000/hub")
                .WithAutomaticReconnect()  // автоматическое переподключение при разрыве
                .Build();

            // Обработчик уведомления о новой версии приложения от сервера
            Connection.On<string>("ReceiveUpdateNotification", (version) =>
            {
                var result = MessageBox.Show(
                    $"Доступна новая версия: {version}\nУстановить сейчас?",
                    "Обновление приложения",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                    UpdateManager.InstallUpdate(version);
            });

            // Запускаем соединение (ранее вызов StartAsync отсутствовал — соединение не устанавливалось)
            try
            {
                await Connection.StartAsync();
            }
            catch (Exception ex)
            {
                // Логируем ошибку подключения, не пробрасываем исключение выше
                System.Diagnostics.Debug.WriteLine($"[SignalR] Ошибка подключения: {ex.Message}");
            }
        }

        /// <summary>
        /// Отправляет уведомление об изменении данных на сервер.
        /// </summary>
        /// <param name="message">Текст сообщения.</param>
        public static async Task SendNotificationAsync(string message)
        {
            if (Connection?.State == HubConnectionState.Connected)
                await Connection.InvokeAsync("NotifyChange", message);
        }

        /// <summary>
        /// Уведомляет сервер о доступности новой версии приложения.
        /// </summary>
        /// <param name="version">Строка версии, например «0.1.36».</param>
        public static async Task NotifyUpdateAvailable(string version)
        {
            if (Connection?.State == HubConnectionState.Connected)
                await Connection.InvokeAsync("ReceiveUpdateNotification", version);
        }
    }
}

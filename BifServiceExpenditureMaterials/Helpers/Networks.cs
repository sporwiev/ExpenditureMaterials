using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace BifServiceExpenditureMaterials.Helpers
{
    /// <summary>
    /// Вспомогательный класс для работы с сетью:
    /// получение IP-адреса, автоматическое переподключение к Wi-Fi.
    /// </summary>
    internal static class Networks
    {
        /// <summary>Название Wi-Fi профиля для переподключения.</summary>
        private const string WifiProfileName = "BIF_5G";

        /// <summary>Флаг — выполняется ли сейчас процесс переподключения.</summary>
        private static bool _reconnecting = false;

        /// <summary>Флаг — выполняется ли мониторинг состояния сети.</summary>
        private static bool _checking = false;

        /// <summary>
        /// Возвращает локальный путь до папки приложения (без имени директории BifServiceExpenditureMaterials).
        /// </summary>
        public static string GetLocalPath()
        {
            var path = "";
            foreach (var directory in Environment.CurrentDirectory.Split('\\'))
            {
                if (directory == "BifServiceExpenditureMaterials") break;
                path += directory + '\\';
            }
            // Убираем финальный слеш
            return path.TrimEnd('\\');
        }

        /// <summary>
        /// Возвращает первый IPv4-адрес текущей машины.
        /// Возвращает <c>null</c>, если адрес не найден.
        /// </summary>
        public static string? GetIp()
        {
            return Dns
                .GetHostEntry(Dns.GetHostName())
                .AddressList
                .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork)
                ?.ToString();
        }

        /// <summary>
        /// Запускает мониторинг сетевого соединения.
        /// При потере сети — начинает попытки переподключения к Wi-Fi-профилю.
        /// </summary>
        public static void ConnectNetwork()
        {
            NetworkChange.NetworkAvailabilityChanged += OnNetworkAvailabilityChanged;

            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                // Сеть недоступна — начинаем переподключение
                if (!_reconnecting)
                {
                    _reconnecting = true;
                    _checking = false;
                    Task.Run(ReconnectWifi);
                }
            }
            else
            {
                // Сеть доступна — запускаем мониторинг
                _reconnecting = false;
                _checking = true;
                Task.Run(MonitorWifi);
            }
        }

        /// <summary>
        /// Обработчик изменения доступности сети.
        /// Вызывается системой при подключении/отключении сетевых интерфейсов.
        /// </summary>
        private static void OnNetworkAvailabilityChanged(object? sender, NetworkAvailabilityEventArgs e)
        {
            if (e.IsAvailable)
            {
                // Сеть восстановлена — останавливаем переподключение
                _reconnecting = false;
                _checking = true;
                Task.Run(MonitorWifi);
            }
            else
            {
                // Сеть пропала — начинаем переподключение
                _checking = false;
                if (!_reconnecting)
                {
                    _reconnecting = true;
                    Task.Run(ReconnectWifi);
                }
            }
        }

        /// <summary>
        /// Мониторинг состояния Wi-Fi-соединения.
        /// При обнаружении разрыва переключается на режим переподключения.
        /// </summary>
        private static void MonitorWifi()
        {
            while (_checking)
            {
                // Пауза между проверками — не занимаем CPU бесполезно
                Thread.Sleep(3000);

                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    // Сеть пропала — переходим к переподключению
                    _checking = false;
                    _reconnecting = true;
                    ReconnectWifi();
                }
            }
        }

        /// <summary>
        /// Попытки переподключения к Wi-Fi-профилю через команду netsh.
        /// Повторяет каждые 5 секунд до успешного подключения.
        /// </summary>
        private static void ReconnectWifi()
        {
            while (_reconnecting)
            {
                try
                {
                    // Подключаемся к сохранённому Wi-Fi-профилю
                    var psi = new ProcessStartInfo("netsh", $"wlan connect name=\"{WifiProfileName}\"")
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    using var process = Process.Start(psi);
                    if (process != null)
                    {
                        process.WaitForExit(10_000);
                        var output = process.StandardOutput.ReadToEnd();
                        var error = process.StandardError.ReadToEnd();

                        if (!string.IsNullOrEmpty(error))
                            Debug.WriteLine($"[Networks] netsh ошибка: {error}");
                    }

                    // Проверяем, появилась ли сеть после команды
                    if (NetworkInterface.GetIsNetworkAvailable())
                    {
                        Debug.WriteLine("[Networks] Сеть восстановлена.");
                        _reconnecting = false;
                        _checking = true;
                        Task.Run(MonitorWifi);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[Networks] Ошибка переподключения: {ex.Message}");
                }

                // Ожидаем перед следующей попыткой
                Thread.Sleep(5000);
            }
        }
    }
}

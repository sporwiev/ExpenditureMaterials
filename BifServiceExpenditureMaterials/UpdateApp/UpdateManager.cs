using System;
using System.Diagnostics;
using System.IO;

namespace BifServiceExpenditureMaterials.UpdateApp
{
    /// <summary>
    /// Менеджер обновления приложения.
    /// Запускает внешнее приложение-установщик и завершает текущий процесс.
    /// </summary>
    public static class UpdateManager
    {
        /// <summary>
        /// Путь к папке данных приложения в %AppData% (Roaming).
        /// Используется как базовая директория для поиска установщика обновлений.
        /// </summary>
        public static string GetAppDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }

        /// <summary>
        /// Устанавливает обновление приложения:
        /// запускает UpdateBIFApplication.exe и завершает текущую программу.
        /// </summary>
        /// <param name="version">Версия обновления (для информации, не используется в пути).</param>
        public static void InstallUpdate(string version)
        {
            var updaterPath = Path.Combine(
                GetAppDirectory(),
                "BifService", "UpdateBif", "UpdateBIFApplication",
                "bin", "Debug", "net8.0-windows", "UpdateBIFApplication.exe");

            if (!File.Exists(updaterPath))
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateManager] Установщик не найден: {updaterPath}");
                return;
            }

            Process.Start(updaterPath);
            Environment.Exit(0);
        }
    }
}

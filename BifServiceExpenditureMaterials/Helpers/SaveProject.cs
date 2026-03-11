using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using BifServiceExpenditureMaterials.Controls;
using BifServiceExpenditureMaterials.Views.Pages;
using Newtonsoft.Json;

namespace BifServiceExpenditureMaterials.Helpers
{
    /// <summary>
    /// Сохранение и загрузка проекта (состояния таблиц) в файл JSON.
    /// Проект — это набор вкладок (TabItem) с месячными таблицами (MonthlyTable).
    /// </summary>
    internal static class SaveProject
    {
        /// <summary>
        /// Сохраняет данные всех вкладок в JSON-файл.
        /// </summary>
        /// <param name="pathDirectory">Директория для сохранения файла.</param>
        /// <param name="nameFile">Имя файла (с расширением .json или без).</param>
        public static void OnSave(string pathDirectory, string nameFile)
        {
            // Собираем данные всех вкладок: ключ — заголовок вкладки, значение — JSON таблицы
            var allTabsData = new Dictionary<string, string>();

            foreach (TabItem tab in HomePage.tab.Items)
            {
                if (tab.Content is MonthlyTable table)
                    allTabsData[(string)tab.Header] = table.SerializeTableData();
            }

            var json = JsonConvert.SerializeObject(allTabsData, Formatting.Indented);
            File.WriteAllText(Path.Combine(pathDirectory, nameFile), json);
        }

        /// <summary>
        /// Загружает данные вкладок из JSON-файла и восстанавливает таблицы.
        /// Если файл не существует — ничего не делает.
        /// </summary>
        /// <param name="filePath">Полный путь к файлу проекта.</param>
        public static void OnDownload(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            var json = File.ReadAllText(filePath);
            var allTabsData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            if (allTabsData == null) return;

            // Восстанавливаем данные в соответствующих вкладках
            foreach (TabItem tab in HomePage.tab.Items)
            {
                if (tab.Content is MonthlyTable table &&
                    allTabsData.TryGetValue((string)tab.Header, out var tabDataJson))
                {
                    table.DeserializeTableData(tabDataJson);
                }
            }
        }
    }
}

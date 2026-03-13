using System;
using System.Collections.Generic;
using System.IO;

namespace BifServiceExpenditureMaterials.Helpers
{
    /// <summary>
    /// Вспомогательный статический класс с утилитами для работы с месяцами, путями и ячейками.
    /// </summary>
    public static class Other
    {
        // ─── Статические словари (создаются один раз, переиспользуются) ───────────

        /// <summary>
        /// Словарь «название месяца → номер» для быстрого преобразования.
        /// </summary>
        private static readonly Dictionary<string, int> MonthNameToNumber = new()
        {
            ["Январь"]   = 1,
            ["Февраль"]  = 2,
            ["Март"]     = 3,
            ["Апрель"]   = 4,
            ["Май"]      = 5,
            ["Июнь"]     = 6,
            ["Июль"]     = 7,
            ["Август"]   = 8,
            ["Сентябрь"] = 9,
            ["Октябрь"]  = 10,
            ["Ноябрь"]   = 11,
            ["Декабрь"]  = 12,
        };

        /// <summary>
        /// Словарь «номер месяца → название» для быстрого преобразования.
        /// </summary>
        private static readonly Dictionary<int, string> MonthNumberToName = new()
        {
            [1]  = "Январь",
            [2]  = "Февраль",
            [3]  = "Март",
            [4]  = "Апрель",
            [5]  = "Май",
            [6]  = "Июнь",
            [7]  = "Июль",
            [8]  = "Август",
            [9]  = "Сентябрь",
            [10] = "Октябрь",
            [11] = "Ноябрь",
            [12] = "Декабрь",
        };

        // ─── Публичные методы ─────────────────────────────────────────────────────

        /// <summary>
        /// Преобразует название месяца в его порядковый номер (1–12).
        /// </summary>
        /// <param name="monthName">Название месяца на русском языке.</param>
        /// <returns>Номер месяца (1–12).</returns>
        public static int GetMouthNumber(string monthName)
        {
            if (string.IsNullOrEmpty(monthName))
            {
                System.Diagnostics.Debug.WriteLine("[Other] GetMouthNumber: передано пустое название месяца");
                return 1;
            }

            if (MonthNameToNumber.TryGetValue(monthName, out var number))
                return number;

            System.Diagnostics.Debug.WriteLine($"[Other] GetMouthNumber: неизвестный месяц '{monthName}'");
            return 1;
        }

        /// <summary>
        /// Преобразует порядковый номер месяца в его название на русском языке.
        /// Если номер вне диапазона 1–12, возвращает "Январь" и логирует ошибку.
        /// </summary>
        /// <param name="number">Номер месяца (1–12).</param>
        /// <returns>Название месяца на русском языке.</returns>
        public static string GetMouthNumber(int number)
        {
            if (MonthNumberToName.TryGetValue(number, out var name))
                return name;

            System.Diagnostics.Debug.WriteLine($"[Other] GetMouthNumber: номер месяца вне диапазона: {number}");
            return "Январь";
        }

        /// <summary>
        /// Возвращает полный список названий всех месяцев (первый элемент — пустая строка-заглушка).
        /// </summary>
        public static List<string> GetAllMonths()
        {
            return new List<string>
            {
                "",
                "Январь",
                "Февраль",
                "Март",
                "Апрель",
                "Май",
                "Июнь",
                "Июль",
                "Август",
                "Сентябрь",
                "Октябрь",
                "Ноябрь",
                "Декабрь",
            };
        }

        /// <summary>
        /// Устаревший псевдоним для <see cref="GetAllMonths"/>.
        /// Оставлен для обратной совместимости.
        /// </summary>
        [Obsolete("Используйте GetAllMonths(). Этот метод будет удалён в следующей версии.")]
        public static List<string> GetAllMoths() => GetAllMonths();

        /// <summary>
        /// Возвращает список дней месяца (1–31) в виде строк для ComboBox.
        /// </summary>
        public static List<string> GetDayList()
        {
            var list = new List<string>(31);
            for (int i = 1; i <= 31; i++)
                list.Add(i.ToString());
            return list;
        }

        /// <summary>
        /// Возвращает список годов от 2024 до 2029 в виде строк для ComboBox.
        /// </summary>
        public static List<string> GetYearList()
        {
            var list = new List<string>();
            for (int i = 2024; i <= 2029; i++)
                list.Add(i.ToString());
            return list;
        }

        /// <summary>
        /// Строит путь к файлу внутри директории приложения.
        /// Находит корневую папку «BifServiceExpenditureMaterials» и добавляет к ней имя файла.
        /// </summary>
        /// <param name="name">Имя файла или относительный путь внутри папки проекта.</param>
        /// <returns>Полный путь к файлу.</returns>
        public static string GetPathProject(string name)
        {
            var parts = Environment.CurrentDirectory.Split(Path.DirectorySeparatorChar);
            var result = "";

            foreach (var part in parts)
            {
                result = Path.Combine(result, part);
                if (string.Equals(part, "BifServiceExpenditureMaterials", StringComparison.OrdinalIgnoreCase))
                    return Path.Combine(result, name);
            }

            // Если папка не найдена — вернуть путь относительно текущей директории
            return Path.Combine(Environment.CurrentDirectory, name);
        }

        /// <summary>
        /// Извлекает числовое значение из строки ячейки формата «Префикс:Число».
        /// Например, «МАШ-001:15» → 15.
        /// </summary>
        /// <param name="cellValue">Значение ячейки в формате «...Значение:Число».</param>
        /// <returns>Числовое значение после последнего двоеточия.</returns>
        public static int GetValueInYacheyka(string cellValue)
        {
            if (string.IsNullOrEmpty(cellValue))
                return 0;

            var parts = cellValue.Split(':');
            return int.TryParse(parts[^1], out var result) ? result : 0;
        }
    }
}

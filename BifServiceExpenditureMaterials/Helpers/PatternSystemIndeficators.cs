using System.Collections.Generic;

namespace BifServiceExpenditureMaterials.Helpers
{
    /// <summary>
    /// Вспомогательный класс для сопоставления системных идентификаторов (GUID) с именами пользователей.
    /// Используется для определения текущего пользователя по уникальному идентификатору системы.
    /// </summary>
    internal static class PatternSystemIndicators
    {
        /// <summary>
        /// Статический словарь: GUID системы → имя пользователя.
        /// Создаётся один раз для избежания повторных аллокаций.
        /// </summary>
        private static readonly Dictionary<string, string> UserKeyMap = new()
        {
            ["0FC0AA46-82C5-11E9-A784-98FA9B2496C2"] = "Андрей Пятых",
            ["215D5922-38C0-4840-AE4F-88A4C2841B36"] = "Администратор",
            [""]                                      = "Ольга Растворова",
        };

        /// <summary>
        /// Возвращает имя пользователя по GUID системы.
        /// </summary>
        /// <param name="key">GUID системы пользователя.</param>
        /// <returns>Имя пользователя, или <c>null</c> если ключ не найден.</returns>
        public static string? GetUser(string key)
        {
            return UserKeyMap.TryGetValue(key, out var name) ? name : null;
        }
    }
}

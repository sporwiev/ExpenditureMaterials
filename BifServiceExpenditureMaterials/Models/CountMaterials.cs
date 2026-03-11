namespace BifServiceExpenditureMaterials.Models
{
    /// <summary>
    /// Количество материалов, израсходованных в рамках одной записи Material.
    /// Хранит объёмы/количества по каждому типу материала.
    /// </summary>
    public class CountMaterials
    {
        /// <summary>Уникальный идентификатор (генерируется автоматически БД).</summary>
        public int Id { get; set; }

        /// <summary>Количество/объём масла (в литрах или кг — хранится как строка для гибкости).</summary>
        public string? count_oil { get; set; }

        /// <summary>Количество антифриза (в литрах).</summary>
        public int? count_antifreeze { get; set; }

        /// <summary>Описание фильтров (артикулы или количество, через разделитель).</summary>
        public string? count_filter { get; set; }

        /// <summary>Количество смазки (в граммах).</summary>
        public int? count_grease { get; set; }

        /// <summary>Моточасы (для учёта наработки двигателя).</summary>
        public int? count_other_clock { get; set; }

        /// <summary>Пробег (в километрах) на момент замены.</summary>
        public int? count_other_milesage { get; set; }

        /// <summary>Описание работ по двигателю.</summary>
        public string? count_motors { get; set; }

        /// <summary>Описание работ по подразделению.</summary>
        public string? count_subunit { get; set; }

        /// <summary>Идентификаторы фильтров через разделитель (для нескольких позиций).</summary>
        public string? count_filterids { get; set; }

        /// <summary>Наименования фильтров через разделитель (для отображения).</summary>
        public string? count_filtername { get; set; }
    }
}

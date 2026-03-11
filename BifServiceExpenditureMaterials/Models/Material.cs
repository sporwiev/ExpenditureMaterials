namespace BifServiceExpenditureMaterials.Models
{
    /// <summary>
    /// Запись о расходе материалов на конкретную машину за определённый день.
    /// Является основной транзакционной записью в системе.
    /// </summary>
    public class Material
    {
        /// <summary>Уникальный идентификатор записи.</summary>
        public int Id { get; set; }

        /// <inheritdoc cref="Id"/>
        public int GetId() => Id;

        /// <summary>
        /// Ключ ячейки таблицы в формате «КодМашины_ДеньМесяца»
        /// (например, «КМ-001_15»).
        /// </summary>
        public string? Ячейка { get; set; }

        /// <summary>
        /// Тип расхода: «ТО» — техническое обслуживание, «Доливка» — доливка материала.
        /// </summary>
        public string? ТипТраты { get; set; }

        /// <summary>Составной номер ячейки (строка:столбец), используется для поиска.</summary>
        public string? НомерЯчейки { get; set; }

        /// <summary>ФИО ответственного за проведение работ.</summary>
        public string? Ответственный { get; set; }

        /// <summary>Название месяца на русском языке.</summary>
        public string? Месяц { get; set; }

        /// <summary>Год проведения работ.</summary>
        public int? Год { get; set; }

        /// <summary>Код марки масла (текстовое поле, дублирует связь через oil_id).</summary>
        public string? oilCode { get; set; }

        // ─── Внешние ключи и навигационные свойства ──────────────────────────────

        public int?   oil_id       { get; set; }
        public Oil?   Oil          { get; set; }

        public int?    filter_id   { get; set; }
        public Filter? Filter      { get; set; }

        public int?    motor_id    { get; set; }
        public Motors? Motor       { get; set; }

        public int?    grease_id   { get; set; }
        public Grease? Grease      { get; set; }

        public int?    other_id    { get; set; }
        public Other?  Other       { get; set; }

        public int?        antifreeze_id { get; set; }
        public Antifreeze? Antifreeze    { get; set; }

        public int?           countmaterial_id { get; set; }
        public CountMaterials? CountMaterials   { get; set; }

        /// <summary>Идентификатор подразделения (хранится как строка).</summary>
        public string? subunit_id { get; set; }

        /// <summary>Дополнительный комментарий к записи.</summary>
        public string? message { get; set; }
    }
}

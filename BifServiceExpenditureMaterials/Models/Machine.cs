namespace BifServiceExpenditureMaterials.Models
{
    /// <summary>
    /// Машина (единица техники), по которой ведётся учёт расхода материалов.
    /// Примечание: класс намеренно назван строчными (machine) для совместимости с БД.
    /// </summary>
    public class machine
    {
        /// <summary>Уникальный идентификатор машины.</summary>
        public int Id { get; set; }

        /// <inheritdoc cref="Id"/>
        public int GetId() => Id;

        /// <summary>Код (бортовой номер) машины, например «КМ-001».</summary>
        public string? Code { get; set; }

        /// <summary>Год выпуска или год постановки на учёт.</summary>
        public int? Год { get; set; }
    }
}

namespace BifServiceExpenditureMaterials.Models
{
    /// <summary>
    /// Прочие данные к записи о ТО или доливке:
    /// фиксирует показания счётчика моточасов и пробега на момент обслуживания.
    /// </summary>
    public class Other
    {
        /// <summary>Уникальный идентификатор.</summary>
        public int Id { get; set; }

        /// <inheritdoc cref="Id"/>
        public int GetId() => Id;

        /// <summary>Показание счётчика моточасов на момент обслуживания.</summary>
        public int? Clock { get; set; }

        /// <summary>Показание одометра (пробег, км) на момент обслуживания.</summary>
        public int? Mileage { get; set; }
    }
}

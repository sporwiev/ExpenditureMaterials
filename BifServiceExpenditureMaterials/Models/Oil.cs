namespace BifServiceExpenditureMaterials.Models
{
    /// <summary>
    /// Справочник масел. Содержит перечень марок/наименований моторных масел.
    /// </summary>
    public class Oil
    {
        /// <summary>Уникальный идентификатор.</summary>
        public int Id { get; set; }

        /// <inheritdoc cref="Id"/>
        public int GetId() => Id;

        /// <summary>Наименование масла (марка, вязкость, например «Shell Helix 5W-40»).</summary>
        public string? Name { get; set; }
    }
}

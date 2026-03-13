using BifServiceExpenditureMaterials.Models;

namespace BifServiceExpenditureMaterials.Services.Interfaces
{
    /// <summary>
    /// Сервис для работы с материалами (масла, антифризы, смазки, фильтры).
    /// Централизует доступ к базе данных для операций с материалами.
    /// </summary>
    public interface IMaterialService
    {
        /// <summary>
        /// Получает все материалы из базы данных.
        /// </summary>
        List<Material> GetAllMaterials();

        /// <summary>
        /// Получает материалы для указанного месяца и года.
        /// </summary>
        List<Material> GetMaterialsByMonthAndYear(string month, int year);

        /// <summary>
        /// Получает материал по номеру ячейки.
        /// </summary>
        Material? GetMaterialByCell(string cellNumber);

        /// <summary>
        /// Получает первый материал по номеру ячейки (отсортированный по Id).
        /// </summary>
        Material? GetFirstMaterialByCell(string cellNumber);

        /// <summary>
        /// Получает данные подсчёта материалов по идентификатору.
        /// </summary>
        CountMaterials? GetCountMaterialsById(int id);

        /// <summary>
        /// Сохраняет новый материал в базу данных.
        /// </summary>
        Task<bool> SaveMaterialAsync(Material material);

        /// <summary>
        /// Удаляет материал из базы данных.
        /// </summary>
        Task<bool> DeleteMaterialAsync(Material material);

        /// <summary>
        /// Получает список названий масел.
        /// </summary>
        List<string> GetOilNames();

        /// <summary>
        /// Получает список названий антифризов.
        /// </summary>
        List<string> GetAntifreezeNames();

        /// <summary>
        /// Получает список названий смазок.
        /// </summary>
        List<string> GetGreaseNames();

        /// <summary>
        /// Получает список названий фильтров.
        /// </summary>
        List<string> GetFilterNames();

        /// <summary>
        /// Получает тип траты по номеру ячейки.
        /// </summary>
        string? GetExpenseType(string cellNumber);

        /// <summary>
        /// Фильтрует материалы по типу продукта и значению.
        /// </summary>
        List<Material> FilterByProduct<T>(string productName, int year) where T : class;
    }
}

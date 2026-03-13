using System.Diagnostics;
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.Models;
using BifServiceExpenditureMaterials.Services.Interfaces;

namespace BifServiceExpenditureMaterials.Services
{
    /// <summary>
    /// Реализация сервиса для работы с материалами.
    /// Обеспечивает безопасный доступ к БД с обработкой ошибок.
    /// </summary>
    public class MaterialService : IMaterialService
    {
        private readonly AppDbContext _context;

        public MaterialService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc/>
        public List<Material> GetAllMaterials()
        {
            try
            {
                return _context.Materials.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MaterialService] Ошибка получения материалов: {ex.Message}");
                return new List<Material>();
            }
        }

        /// <inheritdoc/>
        public List<Material> GetMaterialsByMonthAndYear(string month, int year)
        {
            try
            {
                return _context.Materials
                    .Where(m => m.Месяц == month && m.Год == year)
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MaterialService] Ошибка фильтрации материалов: {ex.Message}");
                return new List<Material>();
            }
        }

        /// <inheritdoc/>
        public Material? GetMaterialByCell(string cellNumber)
        {
            try
            {
                return _context.Materials
                    .FirstOrDefault(m => m.НомерЯчейки == cellNumber);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MaterialService] Ошибка получения материала по ячейке '{cellNumber}': {ex.Message}");
                return null;
            }
        }

        /// <inheritdoc/>
        public Material? GetFirstMaterialByCell(string cellNumber)
        {
            try
            {
                return _context.Materials
                    .OrderBy(m => m.Id)
                    .FirstOrDefault(m => m.НомерЯчейки == cellNumber);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MaterialService] Ошибка получения первого материала по ячейке '{cellNumber}': {ex.Message}");
                return null;
            }
        }

        /// <inheritdoc/>
        public CountMaterials? GetCountMaterialsById(int id)
        {
            try
            {
                return _context.CountMaterials.FirstOrDefault(c => c.Id == id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MaterialService] Ошибка получения CountMaterials id={id}: {ex.Message}");
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> SaveMaterialAsync(Material material)
        {
            try
            {
                _context.Materials.Add(material);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MaterialService] Ошибка сохранения материала: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteMaterialAsync(Material material)
        {
            try
            {
                _context.Materials.Remove(material);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MaterialService] Ошибка удаления материала: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc/>
        public List<string> GetOilNames()
        {
            try
            {
                return _context.Oil?.Select(o => o.Name).ToList() ?? new List<string>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MaterialService] Ошибка получения списка масел: {ex.Message}");
                return new List<string>();
            }
        }

        /// <inheritdoc/>
        public List<string> GetAntifreezeNames()
        {
            try
            {
                return _context.Antifreeze?.Select(a => a.Name).ToList() ?? new List<string>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MaterialService] Ошибка получения списка антифризов: {ex.Message}");
                return new List<string>();
            }
        }

        /// <inheritdoc/>
        public List<string> GetGreaseNames()
        {
            try
            {
                return _context.Grease?.Select(g => g.Name).ToList() ?? new List<string>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MaterialService] Ошибка получения списка смазок: {ex.Message}");
                return new List<string>();
            }
        }

        /// <inheritdoc/>
        public List<string> GetFilterNames()
        {
            try
            {
                return _context.Filter?.Select(f => f.Name).ToList() ?? new List<string>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MaterialService] Ошибка получения списка фильтров: {ex.Message}");
                return new List<string>();
            }
        }

        /// <inheritdoc/>
        public string? GetExpenseType(string cellNumber)
        {
            try
            {
                return _context.Materials
                    .OrderBy(m => m.Id)
                    .FirstOrDefault(m => m.НомерЯчейки == cellNumber)
                    ?.ТипТраты;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MaterialService] Ошибка получения типа траты для ячейки '{cellNumber}': {ex.Message}");
                return null;
            }
        }

        /// <inheritdoc/>
        public List<Material> FilterByProduct<T>(string productName, int year) where T : class
        {
            try
            {
                if (typeof(T) == typeof(Oil))
                {
                    var oil = _context.Oil?.FirstOrDefault(o => o.Name == productName);
                    if (oil == null) return new List<Material>();
                    return _context.Materials
                        .Where(m => m.oilCode == oil.Id.ToString() && m.Год == year)
                        .ToList();
                }

                if (typeof(T) == typeof(Antifreeze))
                {
                    var antifreeze = _context.Antifreeze?.FirstOrDefault(a => a.Name == productName);
                    if (antifreeze == null) return new List<Material>();
                    return _context.Materials
                        .Where(m => m.antifreeze_id == antifreeze.Id && m.Год == year)
                        .ToList();
                }

                if (typeof(T) == typeof(Grease))
                {
                    var grease = _context.Grease?.FirstOrDefault(g => g.Name == productName);
                    if (grease == null) return new List<Material>();
                    return _context.Materials
                        .Where(m => m.grease_id == grease.Id && m.Год == year)
                        .ToList();
                }

                return new List<Material>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MaterialService] Ошибка фильтрации по продукту '{productName}': {ex.Message}");
                return new List<Material>();
            }
        }
    }
}

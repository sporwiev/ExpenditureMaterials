using System.Diagnostics;
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.Models;
using BifServiceExpenditureMaterials.Services.Interfaces;

namespace BifServiceExpenditureMaterials.Services
{
    /// <summary>
    /// Реализация сервиса для работы с машинами.
    /// Обеспечивает безопасный доступ к БД с обработкой ошибок.
    /// </summary>
    public class MachineService : IMachineService
    {
        private readonly AppDbContext _context;

        public MachineService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc/>
        public List<machine> GetAllMachines()
        {
            try
            {
                return _context.machine.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MachineService] Ошибка получения машин: {ex.Message}");
                return new List<machine>();
            }
        }

        /// <inheritdoc/>
        public List<machine> GetMachinesByYear(int year)
        {
            try
            {
                return _context.machine
                    .Where(m => m.Год == year)
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MachineService] Ошибка получения машин за {year} год: {ex.Message}");
                return new List<machine>();
            }
        }

        /// <inheritdoc/>
        public async Task<bool> AddMachineAsync(machine newMachine)
        {
            try
            {
                if (newMachine == null)
                {
                    Debug.WriteLine("[MachineService] Попытка добавить null-машину");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(newMachine.Code))
                {
                    Debug.WriteLine("[MachineService] Код машины пустой");
                    return false;
                }

                await _context.machine.AddAsync(newMachine);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MachineService] Ошибка добавления машины: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateMachineCodeAsync(int machineId, string newCode)
        {
            try
            {
                var existingMachine = _context.machine.FirstOrDefault(m => m.Id == machineId);
                if (existingMachine == null)
                {
                    Debug.WriteLine($"[MachineService] Машина с id={machineId} не найдена");
                    return false;
                }

                existingMachine.Code = newCode;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MachineService] Ошибка обновления машины id={machineId}: {ex.Message}");
                return false;
            }
        }

        /// <inheritdoc/>
        public int GetMachineCount()
        {
            try
            {
                return _context.machine.Count();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MachineService] Ошибка подсчёта машин: {ex.Message}");
                return 0;
            }
        }
    }
}

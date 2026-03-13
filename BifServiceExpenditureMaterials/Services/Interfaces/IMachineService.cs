using BifServiceExpenditureMaterials.Models;

namespace BifServiceExpenditureMaterials.Services.Interfaces
{
    /// <summary>
    /// Сервис для работы с машинами.
    /// Централизует доступ к базе данных для операций с машинами.
    /// </summary>
    public interface IMachineService
    {
        /// <summary>
        /// Получает все машины из базы данных.
        /// </summary>
        List<machine> GetAllMachines();

        /// <summary>
        /// Получает машины для указанного года.
        /// </summary>
        List<machine> GetMachinesByYear(int year);

        /// <summary>
        /// Добавляет новую машину в базу данных.
        /// </summary>
        Task<bool> AddMachineAsync(machine newMachine);

        /// <summary>
        /// Обновляет код машины.
        /// </summary>
        Task<bool> UpdateMachineCodeAsync(int machineId, string newCode);

        /// <summary>
        /// Получает количество машин.
        /// </summary>
        int GetMachineCount();
    }
}

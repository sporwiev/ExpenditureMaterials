using System.Collections.ObjectModel;
using System.Diagnostics;
using BifServiceExpenditureMaterials.Helpers;
using BifServiceExpenditureMaterials.Models;
using BifServiceExpenditureMaterials.Services;
using BifServiceExpenditureMaterials.Services.Interfaces;
using CommunityToolkit.Mvvm.Input;

namespace BifServiceExpenditureMaterials.ViewModels.Windows
{
    /// <summary>
    /// ViewModel формы изменения кода машины.
    /// </summary>
    public partial class FormUpdateMachineViewModel : ObservableObject
    {
        private readonly IMachineService _machineService;
        private readonly IDialogService _dialogService;

        /// <summary>Список кодов машин для ComboBox.</summary>
        [ObservableProperty]
        private ObservableCollection<string> _machineCodes = new();

        /// <summary>Выбранный код машины (текущий).</summary>
        [ObservableProperty]
        private string? _selectedMachineCode;

        /// <summary>Новый код машины.</summary>
        [ObservableProperty]
        private string? _newMachineCode;

        /// <summary>Событие: форму нужно закрыть.</summary>
        public event Action? CloseRequested;

        public FormUpdateMachineViewModel()
        {
            _machineService = new MachineService(App.dBcontext);
            _dialogService = new DialogService();
            LoadMachineCodes();
        }

        public FormUpdateMachineViewModel(IMachineService machineService, IDialogService dialogService)
        {
            _machineService = machineService ?? throw new ArgumentNullException(nameof(machineService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            LoadMachineCodes();
        }

        /// <summary>
        /// Загружает список кодов машин из БД.
        /// </summary>
        private void LoadMachineCodes()
        {
            try
            {
                MachineCodes.Clear();
                var machines = _machineService.GetAllMachines();
                foreach (var m in machines)
                {
                    if (!string.IsNullOrEmpty(m.Code))
                        MachineCodes.Add(m.Code);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormUpdateMachineVM] Ошибка загрузки машин: {ex.Message}");
            }
        }

        /// <summary>
        /// Сохраняет изменённый код машины.
        /// </summary>
        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedMachineCode))
                {
                    _dialogService.ShowWarning("Выберите машину для изменения.");
                    return;
                }

                var newCode = NewMachineCode?.Trim();
                if (string.IsNullOrEmpty(newCode))
                {
                    _dialogService.ShowWarning("Введите новый код машины.");
                    return;
                }

                // Находим машину по старому коду
                var machines = _machineService.GetAllMachines();
                var machine = machines.OrderBy(m => m.Id).FirstOrDefault(m => m.Code == SelectedMachineCode);
                if (machine == null)
                {
                    _dialogService.ShowError("Машина не найдена в базе данных.");
                    return;
                }

                var success = await _machineService.UpdateMachineCodeAsync(machine.Id, newCode);
                if (success)
                {
                    try
                    {
                        await SignalRClient.SendNotificationAsync(
                            $"0x04|Машина - {SelectedMachineCode} Изменена на {newCode}|Изменение");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[FormUpdateMachineVM] Ошибка SignalR: {ex.Message}");
                    }
                    CloseRequested?.Invoke();
                }
                else
                {
                    _dialogService.ShowError("Не удалось обновить код машины.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormUpdateMachineVM] Ошибка сохранения: {ex.Message}");
                _dialogService.ShowError($"Ошибка при сохранении: {ex.Message}");
            }
        }

        /// <summary>
        /// Закрывает форму без сохранения.
        /// </summary>
        [RelayCommand]
        private void Cancel()
        {
            CloseRequested?.Invoke();
        }
    }
}

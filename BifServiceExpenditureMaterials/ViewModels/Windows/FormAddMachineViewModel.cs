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
    /// ViewModel формы добавления новой машины.
    /// </summary>
    public partial class FormAddMachineViewModel : ObservableObject
    {
        private readonly IMachineService _machineService;
        private readonly IDialogService _dialogService;

        /// <summary>Код новой машины.</summary>
        [ObservableProperty]
        private string? _machineCode;

        /// <summary>Выбранный год.</summary>
        [ObservableProperty]
        private string? _selectedYear;

        /// <summary>Список годов для ComboBox.</summary>
        [ObservableProperty]
        private ObservableCollection<string> _years = new();

        /// <summary>Событие: форму нужно закрыть.</summary>
        public event Action? CloseRequested;

        public FormAddMachineViewModel()
        {
            _machineService = new MachineService(App.dBcontext);
            _dialogService = new DialogService();
            InitializeYears();
        }

        public FormAddMachineViewModel(IMachineService machineService, IDialogService dialogService)
        {
            _machineService = machineService ?? throw new ArgumentNullException(nameof(machineService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            InitializeYears();
        }

        /// <summary>
        /// Заполняет список годов (от 2 лет назад до 5 лет вперёд).
        /// </summary>
        private void InitializeYears()
        {
            var currentYear = DateTime.Now.Year;
            for (int year = currentYear - 2; year <= currentYear + 5; year++)
                Years.Add(year.ToString());
            SelectedYear = currentYear.ToString();
        }

        /// <summary>
        /// Сохраняет новую машину в БД и отправляет уведомление через SignalR.
        /// </summary>
        [RelayCommand]
        private async Task SaveAsync()
        {
            try
            {
                var code = MachineCode?.Trim();
                if (string.IsNullOrEmpty(code))
                {
                    _dialogService.ShowWarning("Введите код машины.");
                    return;
                }

                var year = int.TryParse(SelectedYear, out var y) ? y : DateTime.Now.Year;
                var newMachine = new machine { Code = code, Год = year };

                var success = await _machineService.AddMachineAsync(newMachine);
                if (success)
                {
                    try
                    {
                        await SignalRClient.SendNotificationAsync($"0x03|{code} Машина добавлена|Добавление");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[FormAddMachineVM] Ошибка SignalR: {ex.Message}");
                    }
                    CloseRequested?.Invoke();
                }
                else
                {
                    _dialogService.ShowError("Не удалось сохранить машину в базу данных.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormAddMachineVM] Ошибка сохранения: {ex.Message}");
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

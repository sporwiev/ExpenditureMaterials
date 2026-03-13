using System.Collections.ObjectModel;
using System.Diagnostics;
using BifServiceExpenditureMaterials.Forms;
using BifServiceExpenditureMaterials.Models;
using BifServiceExpenditureMaterials.Services.Interfaces;
using CommunityToolkit.Mvvm.Input;

namespace BifServiceExpenditureMaterials.ViewModels.Pages
{
    /// <summary>
    /// ViewModel главной страницы (HomePage).
    /// Содержит бизнес-логику: загрузка данных, фильтрация, поиск, выбор месяца/года.
    /// </summary>
    public partial class HomeViewModel : ObservableObject
    {
        private readonly IMaterialService _materialService;
        private readonly IMachineService _machineService;
        private readonly IDialogService _dialogService;

        // ─── Свойства состояния ─────────────────────────────────────────────────

        /// <summary>Текущий выбранный месяц.</summary>
        [ObservableProperty]
        private string? _activeMonth;

        /// <summary>Текущий выбранный год.</summary>
        [ObservableProperty]
        private int _currentYear;

        /// <summary>Текст поисковой строки.</summary>
        [ObservableProperty]
        private string? _searchText;

        /// <summary>Флаг: данные обновляются.</summary>
        [ObservableProperty]
        private bool _isLoading;

        /// <summary>Флаг: открыта панель фильтров.</summary>
        [ObservableProperty]
        private bool _isFilterOpen;

        /// <summary>Список годов для ComboBox.</summary>
        [ObservableProperty]
        private ObservableCollection<int> _years = new();

        /// <summary>Индекс выбранного типа продукта для фильтрации.</summary>
        [ObservableProperty]
        private int _selectedProductTypeIndex;

        /// <summary>Список продуктов для текущего типа.</summary>
        [ObservableProperty]
        private ObservableCollection<string> _productNames = new();

        /// <summary>Выбранный продукт для фильтрации.</summary>
        [ObservableProperty]
        private string? _selectedProductName;

        /// <summary>Текст кнопки фильтра (Применить/Отменить).</summary>
        [ObservableProperty]
        private string _filterButtonText = "Применить";

        /// <summary>Флаг: фильтр активен.</summary>
        [ObservableProperty]
        private bool _isFilterActive;

        // ─── Данные для таблицы ─────────────────────────────────────────────────

        /// <summary>Список машин для отображения в таблице.</summary>
        public List<machine> Machines { get; private set; } = new();

        /// <summary>Список материалов для текущего месяца/года.</summary>
        public List<Material> CurrentMaterials { get; private set; } = new();

        // ─── События для уведомления View ───────────────────────────────────────

        /// <summary>Событие: нужно обновить таблицу.</summary>
        public event Action? TableUpdateRequested;

        /// <summary>Событие: нужно обновить подсветку месяца.</summary>
        public event Action<string>? MonthHighlightRequested;

        // ─── Конструкторы ───────────────────────────────────────────────────────

        public HomeViewModel(
            IMaterialService materialService,
            IMachineService machineService,
            IDialogService dialogService)
        {
            _materialService = materialService ?? throw new ArgumentNullException(nameof(materialService));
            _machineService = machineService ?? throw new ArgumentNullException(nameof(machineService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            InitializeYears();
        }

        /// <summary>
        /// Конструктор без параметров (для совместимости с DI и XAML).
        /// Использует глобальный контекст БД из App.dBcontext.
        /// </summary>
        public HomeViewModel()
        {
            var context = App.dBcontext;
            _materialService = new Services.MaterialService(context);
            _machineService = new Services.MachineService(context);
            _dialogService = new Services.DialogService();

            InitializeYears();
        }

        // ─── Инициализация ──────────────────────────────────────────────────────

        /// <summary>
        /// Заполняет список годов (2024–2029) и устанавливает текущий год.
        /// </summary>
        private void InitializeYears()
        {
            for (int year = 2024; year <= 2029; year++)
                Years.Add(year);

            CurrentYear = DateTime.Now.Year;
        }

        /// <summary>
        /// Инициализирует ViewModel при загрузке страницы.
        /// Определяет текущий месяц и запрашивает данные.
        /// </summary>
        public void Initialize()
        {
            try
            {
                var monthText = DateTime.Now.ToString("MMMM");
                monthText = char.ToUpper(monthText[0]) + monthText[1..];
                ActiveMonth = monthText;
                LoadData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HomeViewModel] Ошибка инициализации: {ex.Message}");
                _dialogService.ShowError($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        // ─── Загрузка данных ────────────────────────────────────────────────────

        /// <summary>
        /// Загружает список машин и материалов для текущего месяца/года.
        /// После загрузки генерирует событие <see cref="TableUpdateRequested"/>.
        /// </summary>
        public void LoadData()
        {
            try
            {
                IsLoading = true;
                Machines = _machineService.GetAllMachines() ?? new List<machine>();
                CurrentMaterials = _materialService.GetAllMaterials() ?? new List<Material>();
                TableUpdateRequested?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HomeViewModel] Ошибка загрузки данных: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Возвращает материалы для текущего месяца и года для отображения в таблице.
        /// </summary>
        public List<Material> GetFilteredMaterials()
        {
            if (string.IsNullOrEmpty(ActiveMonth))
                return new List<Material>();

            return CurrentMaterials
                .Where(m => m.Год == CurrentYear && m.Месяц == ActiveMonth)
                .ToList();
        }

        // ─── Команды ────────────────────────────────────────────────────────────

        /// <summary>
        /// Открывает форму печати документа.
        /// </summary>
        [RelayCommand]
        private void PrintButton()
        {
            try
            {
                new FormPrint().Show();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HomeViewModel] Ошибка открытия формы печати: {ex.Message}");
                _dialogService.ShowError("Не удалось открыть форму печати.");
            }
        }

        /// <summary>
        /// Открывает форму добавления машины.
        /// </summary>
        [RelayCommand]
        private void AddMachine()
        {
            try
            {
                new FormAddMachine().Show();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HomeViewModel] Ошибка открытия формы добавления машины: {ex.Message}");
                _dialogService.ShowError("Не удалось открыть форму добавления.");
            }
        }

        /// <summary>
        /// Открывает форму изменения машины.
        /// </summary>
        [RelayCommand]
        private void UpdateMachine()
        {
            try
            {
                new FormUpdateMachine().Show();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HomeViewModel] Ошибка открытия формы изменения машины: {ex.Message}");
                _dialogService.ShowError("Не удалось открыть форму изменения.");
            }
        }

        /// <summary>
        /// Обновляет данные таблицы.
        /// </summary>
        [RelayCommand]
        private void RefreshData()
        {
            LoadData();
        }

        /// <summary>
        /// Обработчик выбора месяца.
        /// </summary>
        [RelayCommand]
        private void SelectMonth(string month)
        {
            if (string.IsNullOrEmpty(month)) return;

            ActiveMonth = month;
            LoadData();
            MonthHighlightRequested?.Invoke(month);
        }

        /// <summary>
        /// Обработчик изменения года.
        /// </summary>
        partial void OnCurrentYearChanged(int value)
        {
            if (value > 0)
                LoadData();
        }

        /// <summary>
        /// Обработчик изменения типа продукта — загружает список продуктов.
        /// </summary>
        partial void OnSelectedProductTypeIndexChanged(int value)
        {
            LoadProductNames(value);
        }

        /// <summary>
        /// Загружает список продуктов для выбранного типа.
        /// </summary>
        private void LoadProductNames(int typeIndex)
        {
            try
            {
                ProductNames.Clear();
                var names = typeIndex switch
                {
                    0 => _materialService.GetOilNames(),
                    1 => _materialService.GetAntifreezeNames(),
                    2 => _materialService.GetGreaseNames(),
                    3 => _materialService.GetFilterNames(),
                    _ => new List<string>()
                };

                foreach (var name in names)
                    ProductNames.Add(name);

                if (ProductNames.Count > 0)
                    SelectedProductName = ProductNames[0];
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HomeViewModel] Ошибка загрузки списка продуктов: {ex.Message}");
            }
        }

        /// <summary>
        /// Применяет или отменяет фильтр по продукту.
        /// </summary>
        [RelayCommand]
        private void ToggleFilter()
        {
            if (!IsFilterActive)
            {
                // Применить фильтр
                if (string.IsNullOrEmpty(SelectedProductName)) return;

                try
                {
                    var filteredMaterials = SelectedProductTypeIndex switch
                    {
                        0 => _materialService.FilterByProduct<Oil>(SelectedProductName, CurrentYear),
                        1 => _materialService.FilterByProduct<Antifreeze>(SelectedProductName, CurrentYear),
                        2 => _materialService.FilterByProduct<Grease>(SelectedProductName, CurrentYear),
                        _ => new List<Material>()
                    };

                    CurrentMaterials = filteredMaterials;
                    IsFilterActive = true;
                    FilterButtonText = "Отменить";
                    TableUpdateRequested?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[HomeViewModel] Ошибка применения фильтра: {ex.Message}");
                    _dialogService.ShowError("Ошибка при применении фильтра.");
                }
            }
            else
            {
                // Отменить фильтр
                IsFilterActive = false;
                FilterButtonText = "Применить";
                LoadData();
            }
        }
    }
}

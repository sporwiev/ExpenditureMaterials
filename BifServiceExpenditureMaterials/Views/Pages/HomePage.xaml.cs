using BifServiceExpenditureMaterials.Controls;
using BifServiceExpenditureMaterials.ViewModels.Pages;
using BifServiceExpenditureMaterials.Views.Windows;
using System.Diagnostics;
using System.Management;
using System.Windows.Controls;
using System.Windows.Media;

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Главная страница приложения.
    /// Содержит только UI-логику: анимации, обновление ячеек DataGrid,
    /// подсветка месяцев. Бизнес-логика — в <see cref="HomeViewModel"/>.
    /// </summary>
    public partial class HomePage : UserControl
    {
        // ─── Статические ссылки (необходимы для доступа из других частей приложения) ──

        /// <summary>Текущий выбранный месяц (статический доступ для Forms).</summary>
        public static string? CurrentMounth { get; set; }

        /// <summary>Активный месяц (статический доступ для MonthlyTable).</summary>
        public static string? activeMonth { get; private set; }

        /// <summary>Текущий год (статический доступ для Forms).</summary>
        public static int CurrentYear;

        /// <summary>Глобальная ссылка на экземпляр HomePage.</summary>
        public static HomePage _home;

        /// <summary>Флаг: открыта ли панель фильтров.</summary>
        public static bool isOpenFiltes = false;

        /// <summary>Ссылка на TabControl (используется в SaveProject).</summary>
        public static TabControl tab;

        /// <summary>Активная вкладка.</summary>
        public static TabItem? ActiveTabItem;

        /// <summary>Токен отмены поиска.</summary>
        private CancellationTokenSource? _cts;

        /// <summary>Флаг: было ли обновление данных.</summary>
        public static bool isUpdate = false;

        /// <summary>ViewModel главной страницы.</summary>
        public HomeViewModel ViewModel { get; set; }

        // ─── Конструкторы ───────────────────────────────────────────────────────

        public HomePage()
        {
            InitializeComponent();
            ViewModel = new HomeViewModel();
            DataContext = ViewModel;
            _home = this;
            Loaded += HomePage_Loaded;

            // Подписка на события ViewModel
            ViewModel.TableUpdateRequested += OnTableUpdateRequested;
            ViewModel.MonthHighlightRequested += OnMonthHighlightRequested;
        }

        public HomePage(HomeViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel ?? new HomeViewModel();
            DataContext = ViewModel;
            _home = this;
            Loaded += HomePage_Loaded;

            ViewModel.TableUpdateRequested += OnTableUpdateRequested;
            ViewModel.MonthHighlightRequested += OnMonthHighlightRequested;
        }

        // ─── Получение UUID системы ─────────────────────────────────────────────

        /// <summary>
        /// Возвращает UUID системы через WMI.
        /// </summary>
        public static string GetSystemUUID()
        {
            try
            {
                using var mc = new ManagementClass("Win32_ComputerSystemProduct");
                foreach (var o in mc.GetInstances())
                {
                    var mo = (ManagementObject)o;
                    return mo["UUID"]?.ToString() ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HomePage] Ошибка получения UUID: {ex.Message}");
            }
            return string.Empty;
        }

        // ─── Загрузка страницы ──────────────────────────────────────────────────

        private void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.Initialize();
                activeMonth = ViewModel.ActiveMonth;

                // Настройка ComboBox года
                YearComboBox.SelectionChanged -= YearComboBox_SelectionChanged;
                YearComboBox.Text = DateTime.Now.Year.ToString();
                YearComboBox.SelectionChanged += YearComboBox_SelectionChanged;

                // Настройка поиска
                SearchTextBox.TextChanged += SearchTextBox_TextChanged;

                // Подсветка текущего месяца
                HighlightMonth(ViewModel.ActiveMonth);

                if (isUpdate)
                {
                    montableGl?.Datagrid?.UnselectAllCells();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HomePage] Ошибка при загрузке страницы: {ex.Message}");
            }
        }

        // ─── Обработчики событий ViewModel ──────────────────────────────────────

        /// <summary>
        /// Обновляет таблицу при получении данных из ViewModel.
        /// </summary>
        private void OnTableUpdateRequested()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    if (montableGl?.Datagrid == null) return;

                    montableGl.Datagrid.ItemsSource = null;
                    montableGl.BuildTable();

                    // Заполняем первый столбец — коды машин
                    int i = 0;
                    foreach (var machine in ViewModel.Machines)
                    {
                        montableGl.SetCellValue(montableGl.Datagrid, i, 0, machine.Code ?? "");
                        montableGl.EnabledCell(montableGl.Datagrid, i, 0, false);
                        i++;
                    }

                    // Заполняем ячейки материалами
                    var filteredMaterials = ViewModel.GetFilteredMaterials();
                    foreach (var material in filteredMaterials)
                    {
                        if (string.IsNullOrEmpty(material.Ячейка)) continue;

                        var parts = material.Ячейка.Split(':');
                        if (parts.Length < 2) continue;

                        if (!int.TryParse(parts[0], out var row) || !int.TryParse(parts[1], out var col))
                            continue;

                        if (!string.IsNullOrEmpty(material.НомерЯчейки))
                        {
                            montableGl.SetCellBackground(montableGl.Datagrid, row, col, material.НомерЯчейки);
                            montableGl.SetCellValue(montableGl.Datagrid, row, col, material.НомерЯчейки);
                        }
                    }

                    // Прокручиваем к текущему дню
                    montableGl.SearchCell(montableGl.Datagrid, 1, DateTime.Now.Day);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HomePage] Ошибка обновления таблицы: {ex.Message}");
            }
        }

        /// <summary>
        /// Подсвечивает кнопку выбранного месяца.
        /// </summary>
        private void OnMonthHighlightRequested(string month)
        {
            try
            {
                Dispatcher.Invoke(() => HighlightMonth(month));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HomePage] Ошибка подсветки месяца: {ex.Message}");
            }
        }

        /// <summary>
        /// Сбрасывает все кнопки месяцев и подсвечивает указанный.
        /// </summary>
        private void HighlightMonth(string? month)
        {
            if (monthPanel == null || string.IsNullOrEmpty(month)) return;

            foreach (var child in monthPanel.Children)
            {
                if (child is System.Windows.Controls.Button button)
                {
                    button.Background = Brushes.Transparent;
                    if (button.Content?.ToString() == month)
                        button.Background = Brushes.BlueViolet;
                }
            }
        }

        // ─── Статические методы (обратная совместимость) ─────────────────────────

        /// <summary>
        /// Обновляет DataGrid (вызывается из внешнего кода).
        /// </summary>
        public static void RefreshDataGrid()
        {
            try
            {
                _home?.ViewModel?.LoadData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HomePage] Ошибка RefreshDataGrid: {ex.Message}");
            }
        }

        public static MonthlyTable GetTable() => null;

        public static void ViewOnPoint(int x, int y)
        {
            try
            {
                MonthlyTable.ViewCellOnPoints(x, y);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HomePage] Ошибка ViewOnPoint: {ex.Message}");
            }
        }

        // ─── TabItem утилиты (обратная совместимость) ────────────────────────────

        public static TabItem? GetTabItemByHeader(TabControl tabControl, string header)
        {
            try
            {
                return tabControl?.Items.OfType<TabItem>()
                    .FirstOrDefault(a => a.Header?.ToString()?.IndexOf(header) != -1);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HomePage] Ошибка GetTabItemByHeader: {ex.Message}");
                return null;
            }
        }

        public static string GetHeaderByTabItem(TabItem item) => item?.Header?.ToString() ?? "";

        public static TabItem? GetTabItemBySelectionIndex(TabControl tabControl, int index)
        {
            if (tabControl == null || index < 0 || index >= tabControl.Items.Count)
                return null;
            return tabControl.Items[index] as TabItem;
        }

        // ─── UI обработчики событий ─────────────────────────────────────────────

        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var bt = ((ComboBox)sender).Text;
                CurrentYear = int.TryParse(bt, out var year) ? year : DateTime.Now.Year;
                ViewModel.CurrentYear = CurrentYear;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HomePage] Ошибка смены года: {ex.Message}");
            }
        }

        private void SearchTextBox_TextChanged(Wpf.Ui.Controls.AutoSuggestBox sender,
            Wpf.Ui.Controls.AutoSuggestBoxTextChangedEventArgs args)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;
            string search = SearchTextBox.Text?.Trim().ToLower() ?? "";

            if (string.IsNullOrEmpty(search))
            {
                montableGl?.Datagrid?.SelectedCells.Clear();
                return;
            }

            montableGl?.Datagrid?.SelectedCells.Clear();
            Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(300, token);
                    if (token.IsCancellationRequested) return;

                    await Dispatcher.InvokeAsync(() =>
                    {
                        if (montableGl?.Datagrid == null) return;

                        int rowCount = montableGl.Datagrid.Items.Count;
                        for (int i = 0; i < rowCount; i++)
                        {
                            if (token.IsCancellationRequested) return;

                            var cell = montableGl.NewGetCell(montableGl.Datagrid, i, 0);
                            if (cell?.Content is System.Windows.Controls.TextBlock ts &&
                                ts.Text.ToLower().Contains(search))
                            {
                                var column = montableGl.Datagrid.Columns[0];
                                var cel = new DataGridCellInfo(montableGl.Datagrid.Items[i], column);
                                montableGl.Datagrid.SelectedCells.Add(cel);
                                montableGl.Datagrid.ScrollIntoView(montableGl.Datagrid.Items[i], column);
                                break;
                            }
                        }
                    });
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[HomePage] Ошибка поиска: {ex.Message}");
                }
            }, token);
        }

        private void AddMachineBtn_Click(object sender, RoutedEventArgs e)
            => ViewModel.AddMachineCommand.Execute(null);

        private void UpdateMachineBtn_Click(object sender, RoutedEventArgs e)
            => ViewModel.UpdateMachineCommand.Execute(null);

        private void SearchFilterButton_Click(object sender, RoutedEventArgs e) { }

        private void UpdateFillTable_Click(object sender, RoutedEventArgs e)
            => ViewModel.RefreshDataCommand.Execute(null);

        private void TypeProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ViewModel.SelectedProductTypeIndex = ((ComboBox)sender).SelectedIndex;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HomePage] Ошибка смены типа продукта: {ex.Message}");
            }
        }

        private void FilterSaveButton_Click(object sender, EventArgs e)
            => ViewModel.ToggleFilterCommand.Execute(null);

        private void ProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectButton = (System.Windows.Controls.Button)sender;
                var month = selectButton.Content?.ToString();
                if (string.IsNullOrEmpty(month)) return;

                MainWindow.ActiveAnimate();

                foreach (var child in monthPanel.Children)
                {
                    if (child is System.Windows.Controls.Button button)
                        button.Background = Brushes.Transparent;
                }

                await Task.Delay(500);

                activeMonth = month;
                ViewModel.SelectMonthCommand.Execute(month);

                await Task.Delay(500);
                MainWindow.DisableAnimate();

                selectButton.Background = Brushes.BlueViolet;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[HomePage] Ошибка переключения месяца: {ex.Message}");
            }
        }

        private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) { }

        /// <summary>
        /// Метод фильтрации продуктов — используется из UI.
        /// Оставлен для совместимости, но логика перенесена в ViewModel.
        /// </summary>
        public void FilterProduct<T>(string month, T t, string value)
        {
            // Логика перенесена в ViewModel.ToggleFilter()
            // Этот метод сохранён для обратной совместимости
            ViewModel.ToggleFilterCommand.Execute(null);
        }
    }
}

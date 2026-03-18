using System.Diagnostics;
using System.Windows.Controls;
using BifServiceExpenditureMaterials.Controls;
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.Helpers;
using BifServiceExpenditureMaterials.Models;
using Microsoft.EntityFrameworkCore;
using Other = BifServiceExpenditureMaterials.Helpers.Other;

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Аналитика расходников: графики потребления масла, антифриза, смазки, фильтров по году/месяцу.
    /// Фильтры (год, месяц) показывают только те периоды, где реально есть данные для выбранного типа.
    /// </summary>
    public partial class AnalitickPage : UserControl
    {
        // Флаг — предотвращает рекурсивные вызовы при программном изменении ComboBox
        private bool _refreshing = false;

        // Допустимые названия месяцев в БД (для фильтрации мусорных значений)
        private static readonly HashSet<string> ValidMonthNames = new()
        {
            "Январь","Февраль","Март","Апрель","Май","Июнь",
            "Июль","Август","Сентябрь","Октябрь","Ноябрь","Декабрь"
        };

        public AnalitickPage()
        {
            InitializeComponent();
            Loaded += AnalitickPage_Loaded;
        }

        // ─── Инициализация ──────────────────────────────────────────────────────

        private void AnalitickPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Сначала показываем все годы из БД (тип ещё не выбран)
                RefreshYears();
                RefreshMonths();

                TypeProductComboBox.SelectionChanged += TypeProductComboBox_SelectionChanged;
                ComboBoxYear.SelectionChanged += ComboBoxYear_SelectionChanged;
                ComboBoxMonth.SelectionChanged += ComboBoxMonth_SelectionChanged;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AnalitickPage] Ошибка загрузки: {ex.Message}");
            }
        }

        // ─── Обработчики фильтров ──────────────────────────────────────────────

        private void TypeProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_refreshing) return;
            // При смене типа обновляем доступные годы и месяцы
            RefreshYears();
            RefreshMonths();
            RefreshChart();
        }

        private void ComboBoxYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_refreshing) return;
            // При смене года обновляем доступные месяцы
            RefreshMonths();
            RefreshChart();
        }

        private void ComboBoxMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_refreshing) return;
            RefreshChart();
        }

        // ─── Обновление списка годов ───────────────────────────────────────────

        private void RefreshYears()
        {
            _refreshing = true;
            try
            {
                var prevYear = ComboBoxYear.SelectedItem as string;
                var typeIndex = TypeProductComboBox.SelectedIndex; // 0=пусто,1..4

                List<string> years;

                if (typeIndex == 0)
                {
                    // Тип не выбран — все годы из БД
                    years = App.dBcontext?.Materials?
                        .Select(m => m.Год)
                        .Distinct()
                        .OrderBy(y => y)
                        .AsEnumerable()
                        .Select(y => y.ToString())
                        .ToList() ?? new List<string>();
                }
                else
                {
                    // Только годы, где есть ненулевые данные по выбранному типу
                    years = GetYearsWithData(typeIndex);
                }

                if (years.Count == 0)
                    years.Add(DateTime.Now.Year.ToString());

                ComboBoxYear.ItemsSource = years;

                // Восстанавливаем выбранный год или берём текущий
                if (prevYear != null && years.Contains(prevYear))
                    ComboBoxYear.SelectedItem = prevYear;
                else if (years.Contains(DateTime.Now.Year.ToString()))
                    ComboBoxYear.SelectedItem = DateTime.Now.Year.ToString();
                else
                    ComboBoxYear.SelectedIndex = years.Count - 1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AnalitickPage] RefreshYears error: {ex.Message}");
            }
            finally
            {
                _refreshing = false;
            }
        }

        // ─── Обновление списка месяцев ─────────────────────────────────────────

        private void RefreshMonths()
        {
            _refreshing = true;
            try
            {
                var prevMonth = ComboBoxMonth.SelectedItem as string;
                var typeIndex = TypeProductComboBox.SelectedIndex;

                if (!int.TryParse(ComboBoxYear.Text, out int year))
                    year = DateTime.Now.Year;

                List<string> months;

                if (typeIndex == 0)
                {
                    // Тип не выбран — все месяцы из БД для выбранного года
                    var monthNames = App.dBcontext?.Materials?
                        .Where(m => m.Год == year)
                        .Select(m => m.Месяц)
                        .Distinct()
                        .ToList() ?? new List<string?>();

                    months = OrderedMonths(monthNames);
                }
                else
                {
                    months = GetMonthsWithData(typeIndex, year);
                }

                // Первый элемент — пустая строка "все месяцы"
                months.Insert(0, "");

                ComboBoxMonth.ItemsSource = months;

                // Восстанавливаем выбранный месяц или ставим "все"
                if (prevMonth != null && months.Contains(prevMonth))
                    ComboBoxMonth.SelectedItem = prevMonth;
                else
                    ComboBoxMonth.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AnalitickPage] RefreshMonths error: {ex.Message}");
            }
            finally
            {
                _refreshing = false;
            }
        }

        // ─── Обновление графика ────────────────────────────────────────────────

        private void RefreshChart()
        {
            try
            {
                if (!int.TryParse(ComboBoxYear.Text, out int year)) return;

                var selectedMonth = ComboBoxMonth.SelectedItem as string ?? "";
                var typeIndex = TypeProductComboBox.SelectedIndex;

                typeproduct.Text = typeIndex switch
                {
                    1 => "🛢 Масло (л)",
                    2 => "❄ Антифриз (л)",
                    3 => "🔧 Смазка (кг)",
                    4 => "🔩 Фильтры (шт)",
                    _ => ""
                };

                List<int?, string?, string?, string?> oldValues;
                List<int?, string?, string?, string?> newValues;

                if (!string.IsNullOrEmpty(selectedMonth))
                {
                    // Выбран конкретный месяц — показываем только его
                    newValues = BuildValues(typeIndex, year, selectedMonth);
                    oldValues = new List<int?, string?, string?, string?>();
                }
                else
                {
                    // "Все месяцы" — сравниваем два предыдущих месяца
                    var prevMonthName  = GetMonthOffset(-1);
                    var prev2MonthName = GetMonthOffset(-2);
                    oldValues = BuildValues(typeIndex, year, prev2MonthName);
                    newValues = BuildValues(typeIndex, year, prevMonthName);
                }

                MyChart.ValuesOldMonth = oldValues;
                MyChart.ValuesNewMonth = newValues;
                MyChart.DrawChart();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AnalitickPage] Ошибка обновления графика: {ex.Message}");
            }
        }

        // ─── Запросы к БД: что имеет данные ───────────────────────────────────

        /// <summary>
        /// Возвращает список годов, в которых есть ненулевые данные для указанного типа расходника.
        /// </summary>
        private static List<string> GetYearsWithData(int typeIndex)
        {
            var allMaterials = App.dBcontext?.Materials?
                .Include(m => m.CountMaterials)
                .ToList() ?? new List<Material>();

            return allMaterials
                .Where(m => HasDataForType(m.CountMaterials, typeIndex))
                .Select(m => m.Год)
                .Distinct()
                .OrderBy(y => y)
                .Select(y => y.ToString())
                .ToList();
        }

        /// <summary>
        /// Возвращает список месяцев в заданном году, где есть данные для типа расходника.
        /// Порядок — календарный.
        /// </summary>
        private static List<string> GetMonthsWithData(int typeIndex, int year)
        {
            var materials = App.dBcontext?.Materials?
                .Include(m => m.CountMaterials)
                .Where(m => m.Год == year)
                .ToList() ?? new List<Material>();

            var monthNames = materials
                .Where(m => HasDataForType(m.CountMaterials, typeIndex))
                .Select(m => m.Месяц)
                .Distinct()
                .ToList();

            return OrderedMonths(monthNames);
        }

        /// <summary>
        /// Возвращает true если запись CountMaterials содержит данные для указанного типа.
        /// </summary>
        private static bool HasDataForType(CountMaterials? cm, int typeIndex)
        {
            if (cm == null) return false;
            return typeIndex switch
            {
                1 => !string.IsNullOrEmpty(cm.count_oil) && cm.count_oil != "0",
                2 => (cm.count_antifreeze ?? 0) > 0,
                3 => (cm.count_grease ?? 0) > 0,
                4 => !string.IsNullOrEmpty(cm.count_filter) && cm.count_filter != "0",
                _ => true  // тип не выбран — считаем что есть
            };
        }

        /// <summary>
        /// Сортирует список названий месяцев в календарном порядке.
        /// Отсеивает нестандартные значения (например "12 Марта") — только чистые названия месяцев.
        /// </summary>
        private static List<string> OrderedMonths(IEnumerable<string?> rawMonths)
        {
            return rawMonths
                .Where(m => !string.IsNullOrEmpty(m) && ValidMonthNames.Contains(m!))
                .Select(m => m!)
                .Distinct()
                .OrderBy(m => Other.GetMouthNumber(m))
                .ToList();
        }

        // ─── Вспомогательные методы ────────────────────────────────────────────

        private static string GetMonthOffset(int offset)
        {
            var month = DateTime.Now.Month + offset;
            if (month < 1) month += 12;
            if (month > 12) month -= 12;
            return Other.GetMouthNumber(month);
        }

        private static long SumByMonth(int typeIndex, int year, string monthName)
        {
            try
            {
                var items = (App.dBcontext?.Materials?
                    .Where(m => m.Год == year && m.Месяц == monthName)
                    .Select(m => m.CountMaterials)
                    .ToList()) ?? new List<CountMaterials?>();

                return typeIndex switch
                {
                    1 => items.Sum(c => ParseInt(c?.count_oil)),
                    2 => items.Sum(c => (long)(c?.count_antifreeze ?? 0)),
                    3 => items.Sum(c => (long)(c?.count_grease ?? 0)),
                    4 => items.Sum(c => ParseInt(c?.count_filter)),
                    _ => 0
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AnalitickPage] SumByMonth error: {ex.Message}");
                return 0;
            }
        }

        private static List<int?, string?, string?, string?> BuildValues(int typeIndex, int year, string monthName)
        {
            var result = new List<int?, string?, string?, string?>();
            if (typeIndex == 0 || string.IsNullOrEmpty(monthName)) return result;

            try
            {
                var monthNum = Other.GetMouthNumber(monthName);
                var materials = (App.dBcontext?.Materials?
                    .Include(m => m.CountMaterials)
                    .Where(m => m.Год == year && m.Месяц == monthName)
                    .OrderBy(m => m.Id)
                    .ToList()) ?? new List<Material>();

                foreach (var item in materials)
                {
                    var cm = item.CountMaterials;
                    var parts = item.Ячейка?.Split(':');
                    var dayStr = parts?.Length >= 2 ? parts[1] : "0";
                    if (!int.TryParse(dayStr, out int day)) day = 1;

                    var date = $"{day:00}.{monthNum:00}.{year}";
                    var machine = item.НомерЯчейки?.Split('_').FirstOrDefault() ?? "?";

                    int? value = typeIndex switch
                    {
                        1 => ParseIntNullable(cm?.count_oil),
                        2 => cm?.count_antifreeze,
                        3 => cm?.count_grease,
                        4 => ParseIntNullable(cm?.count_filter),
                        _ => null
                    };

                    result.Add(value, machine, date, item.Ячейка);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AnalitickPage] BuildValues error: {ex.Message}");
            }

            return result;
        }

        private static long ParseInt(object? value)
        {
            if (value == null) return 0;
            return long.TryParse(value.ToString(), out long n) ? n : 0;
        }

        private static int? ParseIntNullable(object? value)
        {
            if (value == null) return null;
            return int.TryParse(value.ToString(), out int n) ? n : null;
        }
    }
}

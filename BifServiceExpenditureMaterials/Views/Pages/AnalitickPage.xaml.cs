using System.Diagnostics;
using System.Windows.Controls;
using BifServiceExpenditureMaterials.Controls;
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.Helpers;
using BifServiceExpenditureMaterials.Models;
using Other = BifServiceExpenditureMaterials.Helpers.Other;

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Аналитика расходников: графики потребления масла, антифриза, смазки, фильтров по году/месяцу.
    /// </summary>
    public partial class AnalitickPage : UserControl
    {
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
                ComboBoxYear.ItemsSource = new List<string> { "2023", "2024", "2025", "2026" };
                ComboBoxMonth.ItemsSource = Other.GetAllMonths();

                ComboBoxYear.SelectedItem = DateTime.Now.Year.ToString();
                ComboBoxMonth.SelectedIndex = 0; // пустая строка = все месяцы

                ComboBoxYear.SelectionChanged += (s, e2) => RefreshChart();
                ComboBoxMonth.SelectionChanged += (s, e2) => RefreshChart();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AnalitickPage] Ошибка загрузки: {ex.Message}");
            }
        }

        // ─── Смена типа расходника ─────────────────────────────────────────────

        private void TypeProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshChart();
        }

        // ─── Обновление графика ────────────────────────────────────────────────

        private void RefreshChart()
        {
            try
            {
                if (!int.TryParse(ComboBoxYear.Text, out int year)) return;

                var selectedMonth = ComboBoxMonth.Text;
                var typeIndex = TypeProductComboBox.SelectedIndex; // 0=пусто,1=Масло,2=Антифриз,3=Смазка,4=Фильтры

                // Предыдущие два месяца для отображения в статусе
                var prevMonthName  = GetMonthOffset(-1);
                var prev2MonthName = GetMonthOffset(-2);

                // Данные двух предыдущих месяцев (для статус-строки)
                long sumPrev  = SumByMonth(typeIndex, year, prevMonthName);
                long sumPrev2 = SumByMonth(typeIndex, year, prev2MonthName);

                sum.Text = $"Потреблено за {prev2MonthName}: {sumPrev2}  |  за {prevMonthName}: {sumPrev}";

                // Название типа для метки
                typeproduct.Text = typeIndex switch
                {
                    1 => "🛢 Масло (л)",
                    2 => "❄ Антифриз (л)",
                    3 => "🔧 Смазка (кг)",
                    4 => "🔩 Фильтры (шт)",
                    _ => "—"
                };

                // Строим данные графика
                var oldValues = BuildValues(typeIndex, year, prev2MonthName);
                var newValues = BuildValues(typeIndex, year, prevMonthName);

                // Если выбран конкретный месяц — показываем только его
                if (!string.IsNullOrEmpty(selectedMonth))
                {
                    newValues = BuildValues(typeIndex, year, selectedMonth);
                    oldValues = new List<int?, string?, string?, string?>();
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

        // ─── Вспомогательные методы ────────────────────────────────────────────

        /// <summary>
        /// Возвращает название месяца смещённого на <paramref name="offset"/> от текущего.
        /// </summary>
        private static string GetMonthOffset(int offset)
        {
            var month = DateTime.Now.Month + offset;
            if (month < 1) month += 12;
            if (month > 12) month -= 12;
            return Other.GetMouthNumber(month);
        }

        /// <summary>
        /// Считает суммарный расход расходника за месяц/год.
        /// </summary>
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

        /// <summary>
        /// Строит список точек для графика: (значение, машина, дата, ячейка).
        /// </summary>
        private static List<int?, string?, string?, string?> BuildValues(int typeIndex, int year, string monthName)
        {
            var result = new List<int?, string?, string?, string?>();
            if (typeIndex == 0 || string.IsNullOrEmpty(monthName)) return result;

            try
            {
                var monthNum = Other.GetMouthNumber(monthName);
                var materials = (App.dBcontext?.Materials?
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

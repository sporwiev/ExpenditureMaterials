using System.Diagnostics;
using System.Windows.Controls;
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.Helpers;
using Microsoft.EntityFrameworkCore;

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Просмотр истории расходников по машине, месяцу и году.
    /// </summary>
    public partial class ViewMachinePage : UserControl
    {
        private static readonly Dictionary<string, int> MonthNumbers = new()
        {
            ["Январь"] = 1, ["Февраль"] = 2, ["Март"] = 3, ["Апрель"] = 4,
            ["Май"] = 5, ["Июнь"] = 6, ["Июль"] = 7, ["Август"] = 8,
            ["Сентябрь"] = 9, ["Октябрь"] = 10, ["Ноябрь"] = 11, ["Декабрь"] = 12
        };

        public ViewMachinePage()
        {
            InitializeComponent();
            Loaded += ViewMachinePage_Loaded;
        }

        // ─── Загрузка страницы ─────────────────────────────────────────────────

        private async void ViewMachinePage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!App.dBcontext.machine.Any()) return;

                // Список машин из записей материалов
                var machines = App.dBcontext.Materials
                    .OrderBy(m => m.Id)
                    .Select(m => m.НомерЯчейки)
                    .AsEnumerable()
                    .Select(s => s?.Split('_').FirstOrDefault() ?? "")
                    .Distinct()
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
                MachineComboBox.ItemsSource = machines;

                MounthComboBox.ItemsSource = App.dBcontext.Materials
                    .OrderBy(m => m.Id)
                    .Select(m => m.Месяц)
                    .Distinct()
                    .ToList();

                YearComboBox.ItemsSource = App.dBcontext.Materials
                    .OrderBy(m => m.Id)
                    .Select(m => m.Год)
                    .Distinct()
                    .ToList();

                // Установить текущий месяц и год по умолчанию
                var currentYear = DateTime.Now.Year;
                var currentMonth = Other.GetMouthNumber(DateTime.Now.Month); // возвращает название месяца
                if (YearComboBox.Items.Contains(currentYear))
                    YearComboBox.SelectedItem = currentYear;
                else if (YearComboBox.Items.Count > 0)
                    YearComboBox.SelectedIndex = 0;

                if (MounthComboBox.Items.Contains(currentMonth))
                    MounthComboBox.SelectedItem = currentMonth;
                else if (MounthComboBox.Items.Count > 0)
                    MounthComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ViewMachinePage] Ошибка загрузки: {ex.Message}");
            }
        }

        // ─── Смена фильтра (машина / месяц / год) ─────────────────────────────

        private async void MachineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var machine = MachineComboBox.Text;
                var month   = MounthComboBox.Text;
                var yearText = YearComboBox.Text;

                if (string.IsNullOrEmpty(machine)) return;

                if (!int.TryParse(yearText, out int year))
                    year = DateTime.Now.Year;

                var materials = await App.dBcontext.Materials
                    .Include(m => m.CountMaterials)
                    .Include(m => m.Oil)
                    .Include(m => m.Grease)
                    .Include(m => m.Antifreeze)
                    .Where(m => m.НомерЯчейки.StartsWith(machine + "_"))
                    .Where(m => string.IsNullOrEmpty(month) || m.Месяц == month)
                    .Where(m => m.Год == year)
                    .OrderBy(m => m.Id)
                    .ToListAsync();

                var list = materials.Select(m => new
                {
                    Дата                  = GetDay(m.Ячейка, m.Месяц, m.Год.ToString()),
                    Машина                = m.НомерЯчейки ?? "—",
                    Вид                   = m.ТипТраты ?? "—",
                    Масло                 = m.Oil?.Name ?? "—",
                    Литры_масла           = m.CountMaterials?.count_oil ?? "—",
                    Фильтры               = m.CountMaterials?.count_filtername ?? "—",
                    Кол_фильтров          = ParseInt(m.CountMaterials?.count_filter),
                    Антифриз              = m.Antifreeze?.Name ?? "—",
                    Литры_антифриза       = m.CountMaterials?.count_antifreeze ?? 0,
                    Смазка                = m.Grease?.Name ?? "—",
                    Кг_смазки             = m.CountMaterials?.count_grease ?? 0,
                    Моточасы              = m.CountMaterials?.count_other_clock ?? 0,
                    Пробег_км             = m.CountMaterials?.count_other_milesage ?? 0,
                    Ответственный         = m.Ответственный ?? "—"
                }).ToList();

                if (list.Count == 0)
                {
                    datagrid.ItemsSource = null;
                    return;
                }

                // Строка ИТОГО
                list.Add(new
                {
                    Дата              = "─── ИТОГО ───",
                    Машина            = list[0].Машина,
                    Вид               = "ТО + Доливка",
                    Масло             = "—",
                    Литры_масла       = "—",
                    Фильтры           = "—",
                    Кол_фильтров      = list.Sum(x => x.Кол_фильтров),
                    Антифриз          = "—",
                    Литры_антифриза   = list.Sum(x => x.Литры_антифриза),
                    Смазка            = "—",
                    Кг_смазки         = list.Sum(x => x.Кг_смазки),
                    Моточасы          = list.Sum(x => x.Моточасы),
                    Пробег_км         = list.Sum(x => x.Пробег_км),
                    Ответственный     = "—"
                });

                datagrid.ItemsSource = list;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ViewMachinePage] Ошибка загрузки данных: {ex.Message}");
            }
        }

        // ─── Вспомогательные методы ────────────────────────────────────────────

        private static int ParseInt(object? value)
        {
            if (value == null) return 0;
            return int.TryParse(value.ToString(), out int n) ? n : 0;
        }

        private string GetDay(string? ячейка, string? месяц, string? год)
        {
            try
            {
                if (string.IsNullOrEmpty(ячейка) || string.IsNullOrEmpty(месяц) || string.IsNullOrEmpty(год))
                    return "—";

                var parts = ячейка.Split(':');
                if (parts.Length < 2 || !int.TryParse(parts[1], out int day))
                    return "—";

                if (!MonthNumbers.TryGetValue(месяц, out int monthNum))
                    return "—";

                if (!int.TryParse(год, out int yearNum))
                    return "—";

                // Вычисляем реальный день недели
                var date = new DateTime(yearNum, monthNum, Math.Min(day, DateTime.DaysInMonth(yearNum, monthNum)));
                var dayOfWeek = date.DayOfWeek switch
                {
                    DayOfWeek.Monday    => "Понедельник",
                    DayOfWeek.Tuesday   => "Вторник",
                    DayOfWeek.Wednesday => "Среда",
                    DayOfWeek.Thursday  => "Четверг",
                    DayOfWeek.Friday    => "Пятница",
                    DayOfWeek.Saturday  => "Суббота",
                    DayOfWeek.Sunday    => "Воскресенье",
                    _ => "—"
                };

                return $"{месяц} {dayOfWeek} ({day:00}.{monthNum:00}.{год})";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ViewMachinePage] Ошибка GetDay: {ex.Message}");
                return "—";
            }
        }
    }
}

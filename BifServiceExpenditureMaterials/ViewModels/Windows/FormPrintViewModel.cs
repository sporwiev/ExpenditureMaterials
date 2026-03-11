using System.Collections.Generic;
using BifServiceExpenditureMaterials.Helpers;
using Material = BifServiceExpenditureMaterials.Models.Material;

namespace BifServiceExpenditureMaterials.ViewModels.Windows
{
    /// <summary>
    /// ViewModel формы печати документов.
    /// Управляет фильтрами периода (дата от/до) и формирует список записей для печати.
    /// </summary>
    public partial class FormPrintViewModel : ObservableObject
    {
        // ─── Статические вспомогательные методы ──────────────────────────────────

        /// <summary>
        /// Возвращает список дней (1–31) в виде строк для ComboBox.
        /// </summary>
        private static List<string> GetDayList()
        {
            var list = new List<string>(31);
            for (int i = 1; i <= 31; i++)
                list.Add(i.ToString());
            return list;
        }

        /// <summary>
        /// Возвращает список годов (2024–2029) для ComboBox.
        /// </summary>
        private static List<string> GetYearList()
        {
            var list = new List<string>();
            for (int i = 2024; i <= 2029; i++)
                list.Add(i.ToString());
            return list;
        }

        // ─── Обозреваемые свойства (инициализация источников данных) ─────────────

        /// <summary>Список названий месяцев для ComboBox.</summary>
        [ObservableProperty]
        private List<string>? _monthComboBox = new List<string>
        {
            "Январь", "Февраль", "Март", "Апрель", "Май", "Июнь",
            "Июль", "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"
        };

        /// <summary>Список дней (1–31) для ComboBox.</summary>
        [ObservableProperty]
        private List<string>? _dayComboBox = GetDayList();

        /// <summary>Список годов для ComboBox.</summary>
        [ObservableProperty]
        private List<string>? _yearComboBox = GetYearList();

        // ─── Фильтры периода «От» ─────────────────────────────────────────────────

        /// <summary>Выбранный месяц начала периода.</summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EnterListBox))]
        private string? _selectedMonthBeforeComboBox;

        /// <summary>Выбранный день начала периода.</summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EnterListBox))]
        private string? _selectedDayBeforeComboBox;

        /// <summary>Выбранный год начала периода.</summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EnterListBox))]
        private string? _selectedYearBeforeComboBox;

        // ─── Фильтры периода «До» ─────────────────────────────────────────────────

        /// <summary>Выбранный месяц конца периода.</summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EnterListBox))]
        private string? _selectedMonthAfterComboBox;

        /// <summary>Выбранный день конца периода.</summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EnterListBox))]
        private string? _selectedDayAfterComboBox;

        /// <summary>Выбранный год конца периода.</summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EnterListBox))]
        private string? _selectedYearAfterComboBox;

        // ─── Результирующий список ────────────────────────────────────────────────

        /// <summary>
        /// Список материалов, отфильтрованных по выбранному периоду.
        /// Вычисляется динамически при изменении любого фильтра.
        /// Возвращает пустой список, если какой-либо фильтр не выбран.
        /// </summary>
        public List<string>? EnterListBox
        {
            get
            {
                // Проверяем, что все фильтры выбраны
                if (string.IsNullOrEmpty(SelectedMonthBeforeComboBox) ||
                    string.IsNullOrEmpty(SelectedMonthAfterComboBox) ||
                    string.IsNullOrEmpty(SelectedYearBeforeComboBox) ||
                    string.IsNullOrEmpty(SelectedYearAfterComboBox) ||
                    string.IsNullOrEmpty(SelectedDayBeforeComboBox) ||
                    string.IsNullOrEmpty(SelectedDayAfterComboBox))
                {
                    return new List<string>();
                }

                // Парсим числовые значения фильтров
                int monthBefore = Other.GetMouthNumber(SelectedMonthBeforeComboBox);
                int monthAfter  = Other.GetMouthNumber(SelectedMonthAfterComboBox);
                int yearBefore  = Convert.ToInt32(SelectedYearBeforeComboBox);
                int yearAfter   = Convert.ToInt32(SelectedYearAfterComboBox);
                int dayBefore   = Convert.ToInt32(SelectedDayBeforeComboBox);
                int dayAfter    = Convert.ToInt32(SelectedDayAfterComboBox);

                // Фильтруем материалы по периоду и возвращаем строковое представление
                return App.dBcontext.Materials
                    .Where(a =>
                        Other.GetMouthNumber(a.Месяц) >= monthBefore &&
                        Other.GetMouthNumber(a.Месяц) <= monthAfter  &&
                        a.Год >= yearBefore &&
                        a.Год <= yearAfter  &&
                        Other.GetValueInYacheyka(a.Ячейка) >= dayBefore &&
                        Other.GetValueInYacheyka(a.Ячейка) <= dayAfter)
                    .Select(a => a.ToString())
                    .ToList();
            }
            set { /* сеттер требуется для привязки WPF, реальное значение вычисляется в геттере */ }
        }
    }
}

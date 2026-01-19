using BifServiceExpenditureMaterials.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для ViewMachinePage.xaml
    /// </summary>
    public partial class ViewMachinePage : UserControl
    {
        public ViewMachinePage()
        {
            InitializeComponent();
            
            {
                if(App.dBcontext.machine.Count() == 0)
                {
                    return;
                }
            }
                Loaded += ViewMachinePage_Loaded;
        }

        private async void ViewMachinePage_Loaded(object sender, RoutedEventArgs e)
        {
            
            {
                List<string> newmaterial = new List<string>();
                foreach(var material in App.dBcontext.Materials.OrderBy(e => e.Id).Select(e => e.НомерЯчейки).Distinct().ToList())
                {
                    newmaterial.Add(material.Split("_")[0]);
                }
                MachineComboBox.ItemsSource = newmaterial.Distinct();
                //MachineComboBox.ItemsSource = App.dBcontext.machine.OrderBy(e => e.Id).Select(e => e.Code).ToList();

                MounthComboBox.ItemsSource = App.dBcontext.Materials.OrderBy(e => e.Id).Select(e => e.Месяц).Distinct().ToList();
                YearComboBox.ItemsSource = App.dBcontext.Materials.OrderBy(e => e.Id).Select(e => e.Год).Distinct().ToList();
                Task.Delay(1000);
                var machine = MachineComboBox.Text;
                var materials = await App.dBcontext.Materials
                    .Include(e => e.CountMaterials)
                    .Include(e => e.Oil)
                    .Include(e => e.Grease)
                    .Include(e => e.Antifreeze)
                    .Where(e => e.НомерЯчейки.StartsWith(machine + "_"))
                    .Where(e => e.Месяц == MounthComboBox.Text)
                    .Where(e => e.Год == Convert.ToInt32(YearComboBox.Text == "" ? DateTime.Now.Year : YearComboBox.Text))
                    .OrderBy(e => e.Месяц)
                    .ToListAsync(); // Запрос завершён, дальше в памяти
                datagrid.ItemsSource = materials.Select(e => new
                {
                    Дата = GetDay(e.Ячейка, e.Месяц, e.Год.ToString()),
                    Машина = e.НомерЯчейки,
                    Вид_Монтажа = e.ТипТраты,
                    Масло = e.Oil?.Name,
                    Количество_Литров_Масла = e.CountMaterials?.count_oil,
                    Фильтры = e.CountMaterials?.count_filtername,
                    Количество_Фильтров = e.CountMaterials?.count_filter,
                    Мотор_ы = e.CountMaterials?.count_motors,
                    Антифриз = e.Antifreeze?.Name,
                    Количество_Антифриза = e.CountMaterials?.count_antifreeze,
                    Смазка = e.Grease?.Name,
                    Количество_Смазки = e.CountMaterials?.count_grease,
                    Мото_Часы = e.CountMaterials?.count_other_clock,
                    Пробег = e.CountMaterials?.count_other_milesage,
                    e.Ответственный
                }).ToList();
                //await App.dBcontext.DisposeAsync();
            }
        }
        private async void MachineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            await Task.Delay(1000);
            var machine = MachineComboBox.Text;
            var month = MounthComboBox.Text;
            var yearText = YearComboBox.Text;
            if (!int.TryParse(yearText, out int year))
            {
                MessageBox.Show("Некорректный год: " + yearText);
                return;
            }

            
            {
                var materials = await App.dBcontext.Materials
                    .Include(e => e.CountMaterials)
                    .Include(e => e.Oil)
                    .Include(e => e.Grease)
                    .Include(e => e.Antifreeze)
                    .Where(e => e.НомерЯчейки.StartsWith(machine + "_"))
                    .Where(e => e.Месяц == MounthComboBox.Text)
                    .Where(e => e.Год == Convert.ToInt32(YearComboBox.Text))
                    .OrderBy(e => e.Месяц)
                    .ToListAsync(); // Запрос завершён, дальше в памяти
                var list = materials.Select(e => new
                {
                    Дата = GetDay(e.Ячейка ?? "", e.Месяц ?? "", e.Год.ToString() ?? "_______"),
                    Машина = e.НомерЯчейки,
                    Вид_Монтажа = e.ТипТраты,
                    Масло = e.Oil?.Name ?? "_______",
                    //Количество_Литров_Масла = e.CountMaterials?.count_oil ?? 0,
                    Фильтры = e.CountMaterials?.count_filtername ?? "_______",
                    Количество_Фильтров = (e.CountMaterials?.count_filter ?? "_______").ToString() ?? "_______",
                    Мотор_ы = (e.CountMaterials?.count_motors ?? "_______").ToString() ?? "_______",
                    Антифриз = e.Antifreeze?.Name ?? "_______",
                    Количество = e.CountMaterials?.count_antifreeze ?? 0,
                    Смазка = e.Grease?.Name ?? "_______",
                    Количество_Смазки = e.CountMaterials?.count_grease ?? 0,
                    Мото_Часы = e.CountMaterials?.count_other_clock ?? 0,
                    Пробег = e.CountMaterials?.count_other_milesage ?? 0,
                    Ответственный = e.Ответственный ?? "_______"
                }).ToList();

                int countfilter = list.Sum(x => int.TryParse(x.Количество_Фильтров, out var n) ? n : 0);
                //int countoils = list.Sum(x => x.Количество_Литров_Масла);
                list.Add(new
                {
                    Дата = "ИТОГО",
                    Машина = list.Count != 0 ? list[0].Машина : "_______",
                    Вид_Монтажа = "ТО/Доливка",
                    Масло = "_______",
                    //Количество_Литров_Масла = countoils,
                    Фильтры = "_______",
                    Количество_Фильтров = countfilter.ToString(),
                    Мотор_ы = "_______", // можешь тоже просуммировать, как с фильтрами
                    Антифриз = "_______",
                    Количество = list.Sum(x => x.Количество),
                    Смазка = "_______",
                    Количество_Смазки = list.Sum(x => x.Количество_Смазки),
                    Мото_Часы = list.Sum(x => x.Мото_Часы),
                    Пробег = list.Sum(x => x.Пробег),
                    Ответственный = "_______"
                });
                datagrid.ItemsSource = list;
                //await App.dBcontext.DisposeAsync();
            }
        }
        public string GetDay(string Ячейка,string Месяц,string Год)
        {
            var dateInteger = new Dictionary<string, int>
            {
                ["Январь"] = 01,
                ["Февраль"] = 02,
                ["Март"] = 03,
                ["Апрель"] = 04,
                ["Май"] = 05,
                ["Июнь"] = 06,
                ["Июль"] = 07,
                ["Август"] = 08,
                ["Сентябрь"] = 09,
                ["Октябрь"] = 10,
                ["Ноябрь"] = 11,
                ["Декабрь"] = 12
            };
            //var dateString = new Dictionary<int, string> {
            //    [01] = $"{Месяц} Понедельник (01.{dateInteger[Месяц]}.{Год})",
            //    [02] = $"{Месяц} Вторник",
            //    [03] = $"{Месяц} Среда",
            //    [04] = $"{Месяц} Четверг",
            //    [05] = $"{Месяц} Пятница",
            //    [06] = $"{Месяц} Суббота",
            //    [07] = $"{Месяц} Воскресенье",
            //    [08] = $"{Месяц} Понедельник ()",
            //    [09] = $"{Месяц} Вторник",
            //    [10] = $"{Месяц} Среда",
            //    [11] = $"{Месяц} Четверг",
            //    [12] = $"{Месяц} Пятница",
            //    [13] = $"{Месяц} Суббота",
            //    [14] = $"{Месяц} Воскресенье",
            //    [15] = $"{Месяц} Понедельник ()",
            //    [16] = $"{Месяц} Вторник",
            //    [17] = $"{Месяц} Среда",
            //    [18] = $"{Месяц} Четверг",
            //    [19] = $"{Месяц} Пятница",
            //    [20] = $"{Месяц} Суббота",
            //    [21] = $"{Месяц} Воскресенье",
            //    [22] = $"{Месяц} Понедельник ()",
            //    [23] = $"{Месяц} Вторник",
            //    [24] = $"{Месяц} Среда",
            //    [25] = $"{Месяц} Четверг",
            //    [26] = $"{Месяц} Пятница",
            //    [27] = $"{Месяц} Суббота",
            //    [28] = $"{Месяц} Воскресенье",
            //    [29] = $"{Месяц} Понедельник ()",
            //    [30] = $"{Месяц} Вторник",
            //    [31] = $"{Месяц} Среда",
            //};
            var daysOfWeek = new[] {
    "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Воскресенье"
};

            var result = new Dictionary<int, string>();

            for (int day = 1; day <= 31; day++)
            {
                int dayOfWeekIndex = (day - 1) % 7; // индекс в массиве дней недели от 0 до 6
                string dayName = daysOfWeek[dayOfWeekIndex];
                string dateFormatted = $"{day:00}.{dateInteger[Месяц]:00}.{Год}";
                result[day] = $"{Месяц} {dayName} ({dateFormatted})";
            }
            return result[Convert.ToInt32(Ячейка.Split(':')[1])];
        }
    }
}

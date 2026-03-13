using BifServiceExpenditureMaterials.Autopattern;
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.Helpers;
using BifServiceExpenditureMaterials.Views.Pages;
using BifServiceExpenditureMaterials.Views.Windows;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BifServiceExpenditureMaterials.Controls;
using BifServiceExpenditureMaterials.material;
using BifServiceExpenditureMaterials.Models;
using Subunit = BifServiceExpenditureMaterials.Models.Subunit;

namespace BifServiceExpenditureMaterials.Forms
{
    /// <summary>
    /// Объединённая форма для ТО и Доливки.
    /// Заменяет FormTO и FormDol — вся логика идентична, отличается только тип траты.
    /// При ошибке "Sequence contains no elements" расходник автоматически добавляется в БД.
    /// </summary>
    public partial class FormMaterial : Window
    {
        System.Timers.Timer timer;

        /// <summary>Номер ячейки в таблице (например "КамАЗ_5").</summary>
        public string nomer { get; set; }

        /// <summary>Координата ячейки (строка:столбец).</summary>
        public string yache { get; set; }

        /// <summary>Год записи.</summary>
        public int year { get; set; }

        /// <summary>Тип траты: "ТО" или "Доливка".</summary>
        public string ТипТраты { get; set; }

        [DllImport("user32.dll")]
        private static extern IntPtr SetActiveWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);
        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_RESTORE = 9;

        private List<material.Oil> listOils = new List<material.Oil>();
        private List<string> listSubs = new List<string>();
        private int oilindexpanel = 1;

        /// <summary>
        /// Конструктор для создания новой записи или просмотра существующей.
        /// </summary>
        /// <param name="номерЯчейки">Номер ячейки.</param>
        /// <param name="ячейка">Координата ячейки (строка:столбец).</param>
        /// <param name="isWrite">true — новая запись, false — просмотр/редактирование.</param>
        /// <param name="year">Год записи.</param>
        /// <param name="типТраты">"ТО" или "Доливка".</param>
        public FormMaterial(string номерЯчейки, string ячейка, bool isWrite, int year, string типТраты = "ТО")
        {
            Closing += FormMaterial_Closing;
            InitializeComponent();
            this.year = year;
            this.ТипТраты = типТраты;
            UpdateTitle();

            if (isWrite)
            {
                nomer = номерЯчейки;
                yache = ячейка;
            }
            else
            {
                GetData(номерЯчейки);
            }
        }

        /// <summary>
        /// Конструктор для просмотра существующих данных материала.
        /// </summary>
        public FormMaterial(Material material, CountMaterials countmaterial, string nomer, string yache, string типТраты = "ТО")
        {
            InitializeComponent();
            this.ТипТраты = типТраты;
            UpdateTitle();
            SetData(material, countmaterial);
            this.nomer = nomer;
            this.yache = yache;
        }

        /// <summary>Обновляет заголовок окна и GroupBox в зависимости от типа траты.</summary>
        private void UpdateTitle()
        {
            var label = ТипТраты == "ТО" ? "Форма ТО" : "Форма Доливки";
            TitleBar.Title = $"Биф сервисы - {label}";
            MainGroupBox.Header = ТипТраты == "ТО" ? "ТО" : "Доливка";
        }

        private void FormMaterial_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            HomePage.RefreshDataGrid();
        }

        #region Автодобавление расходников в БД

        /// <summary>
        /// Находит масло по Id. Если не найдено — автоматически создаёт запись в БД.
        /// </summary>
        private Models.Oil EnsureOilExists(string oilId)
        {
            var oil = App.dBcontext.Oil.FirstOrDefault(s => s.Id.ToString() == oilId);
            if (oil == null)
            {
                oil = new Models.Oil { Name = $"Масло_{oilId}" };
                App.dBcontext.Oil.Add(oil);
                App.dBcontext.SaveChanges();
                Debug.WriteLine($"[FormMaterial] Масло с Id={oilId} не найдено — создано автоматически (Id={oil.Id})");
            }
            return oil;
        }

        /// <summary>
        /// Находит масло по имени. Если не найдено — создаёт запись.
        /// </summary>
        private Models.Oil EnsureOilExistsByName(string oilName)
        {
            var oil = App.dBcontext.Oil.FirstOrDefault(e => e.Name == oilName);
            if (oil == null)
            {
                oil = new Models.Oil { Name = oilName };
                App.dBcontext.Oil.Add(oil);
                App.dBcontext.SaveChanges();
                Debug.WriteLine($"[FormMaterial] Масло '{oilName}' не найдено — создано автоматически (Id={oil.Id})");
            }
            return oil;
        }

        /// <summary>
        /// Находит смазку по имени. Если не найдена — создаёт запись.
        /// </summary>
        private Grease EnsureGreaseExists(string greaseName)
        {
            var grease = App.dBcontext.Grease.FirstOrDefault(e => e.Name == greaseName);
            if (grease == null)
            {
                grease = new Grease { Name = greaseName };
                App.dBcontext.Grease.Add(grease);
                App.dBcontext.SaveChanges();
                Debug.WriteLine($"[FormMaterial] Смазка '{greaseName}' не найдена — создана автоматически (Id={grease.Id})");
            }
            return grease;
        }

        /// <summary>
        /// Находит антифриз по имени. Если не найден — создаёт запись.
        /// </summary>
        private Antifreeze EnsureAntifreezeExists(string antifreezeName)
        {
            var antifreeze = App.dBcontext.Antifreeze.FirstOrDefault(e => e.Name == antifreezeName);
            if (antifreeze == null)
            {
                antifreeze = new Antifreeze { Name = antifreezeName };
                App.dBcontext.Antifreeze.Add(antifreeze);
                App.dBcontext.SaveChanges();
                Debug.WriteLine($"[FormMaterial] Антифриз '{antifreezeName}' не найден — создан автоматически (Id={antifreeze.Id})");
            }
            return antifreeze;
        }

        /// <summary>
        /// Находит мотор по имени. Если не найден — создаёт запись.
        /// </summary>
        private Motors EnsureMotorExists(string motorName)
        {
            var motor = App.dBcontext.Motors.FirstOrDefault(e => e.Name == motorName);
            if (motor == null)
            {
                motor = new Motors { Name = motorName };
                App.dBcontext.Motors.Add(motor);
                App.dBcontext.SaveChanges();
                Debug.WriteLine($"[FormMaterial] Мотор '{motorName}' не найден — создан автоматически (id={motor.id})");
            }
            return motor;
        }

        /// <summary>
        /// Находит фильтр по имени. Если не найден — создаёт запись.
        /// </summary>
        private Filter EnsureFilterExists(string filterName)
        {
            var filter = App.dBcontext.Filter.FirstOrDefault(e => e.Name == filterName);
            if (filter == null)
            {
                filter = new Filter { Name = filterName };
                App.dBcontext.Filter.Add(filter);
                App.dBcontext.SaveChanges();
                Debug.WriteLine($"[FormMaterial] Фильтр '{filterName}' не найден — создан автоматически (id={filter.id})");
            }
            return filter;
        }

        /// <summary>
        /// Находит подагрегат по Id. Если не найден — создаёт заглушку.
        /// </summary>
        private Subunit EnsureSubunitExists(int subunitId)
        {
            var sub = App.dBcontext.subunit.FirstOrDefault(s => s.Id == subunitId);
            if (sub == null)
            {
                sub = new Subunit { Name = $"Подагрегат_{subunitId}" };
                App.dBcontext.subunit.Add(sub);
                App.dBcontext.SaveChanges();
                Debug.WriteLine($"[FormMaterial] Подагрегат Id={subunitId} не найден — создан автоматически");
            }
            return sub;
        }

        #endregion

        #region Загрузка и отображение данных

        public async void SetData(Material? material, CountMaterials countmaterial)
        {
            try
            {
                // Инициализация выпадающих списков
                AntifreezeColorComboBox.ItemsSource = App.dBcontext.antifreezecolorfields.Select(s => s.name).ToList();
                AntifreezeColorComboBox.SelectionChanged += AntifreezeColorComboBox_SelectionChanged;
                AntifreezeBrandComboBox.SelectionChanged += AntifreezeBrandComboBox_SelectionChanged;

                GreaseBrandComboBox.ItemsSource = App.dBcontext.greasebrandfields.Select(s => s.name).ToList();
                GreaseBrandComboBox.SelectionChanged += GreaseBrandComboBox_SelectionChanged;
                GreaseTypeComboBox.SelectionChanged += GreaseTypeComboBox_SelectionChanged;

                MotorTypeComboBox.ItemsSource = new List<string>() { "Низ", "Верх", "Низ-верх" };
                ResponsibleComboBox.ItemsSource = GetPersone();

                // Загрузка антифриза
                string[] anti;
                if (material?.antifreeze_id != null && material.antifreeze_id != 0)
                {
                    var antifreezeObj = App.dBcontext.Antifreeze
                        .FirstOrDefault(s => s.Id.ToString() == material.antifreeze_id.ToString());
                    anti = antifreezeObj?.Name?.Split("_") ?? ["", "", ""];
                }
                else
                {
                    anti = ["", "", ""];
                }

                // Загрузка смазки
                string[] grease;
                if (material?.grease_id != null && material.grease_id != 0)
                {
                    var greaseObj = App.dBcontext.Grease
                        .FirstOrDefault(s => s.Id.ToString() == material.grease_id.ToString());
                    grease = greaseObj?.Name?.Split("_") ?? ["", "", ""];
                }
                else
                {
                    grease = ["", "", ""];
                }

                #region Antifreeze
                for (int i = 0; i < AntifreezeColorComboBox.Items.Count; i++)
                {
                    if (AntifreezeColorComboBox.Items[i].ToString() == anti[0])
                    {
                        AntifreezeColorComboBox.SelectedIndex = i;
                        for (int j = 0; j < AntifreezeBrandComboBox.Items.Count; j++)
                        {
                            if (AntifreezeBrandComboBox.Items[j].ToString() == anti[1])
                            {
                                AntifreezeBrandComboBox.SelectedIndex = j;
                                for (int t = 0; t < AntifreezeTypeComboBox.Items.Count; t++)
                                {
                                    if (AntifreezeTypeComboBox.Items[t].ToString() == anti[2])
                                    {
                                        AntifreezeTypeComboBox.SelectedIndex = t;
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
                AntifreezeLitersTextBox.Text = countmaterial.count_antifreeze.ToString();
                #endregion

                #region Grease
                for (int i = 0; i < GreaseBrandComboBox.Items.Count; i++)
                {
                    if (GreaseBrandComboBox.Items[i].ToString() == grease[0])
                    {
                        GreaseBrandComboBox.SelectedIndex = i;
                        for (int j = 0; j < GreaseTypeComboBox.Items.Count; j++)
                        {
                            if (GreaseTypeComboBox.Items[j].ToString() == grease[1])
                            {
                                GreaseTypeComboBox.SelectedIndex = j;
                                for (int t = 0; t < GreaseViscosityComboBox.Items.Count; t++)
                                {
                                    if (GreaseViscosityComboBox.Items[t].ToString() == grease[2])
                                    {
                                        GreaseViscosityComboBox.SelectedIndex = t;
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
                GreaseAmountTextBox.Text = countmaterial.count_grease.ToString();
                #endregion

                #region Filters
                var filterscount = countmaterial?.count_filter?.Split(":").ToList();
                var filtersname = countmaterial?.count_filtername?.Split(":").ToList();
                foreach (var filter in filterscount ?? new List<string>() { "" })
                    FilterListViewCount.Items.Add(filter);
                foreach (var filter in filtersname ?? new List<string>() { "" })
                    FilterListViewName.Items.Add(filter);
                #endregion

                #region Other
                MotoHoursTextBox.Text = countmaterial?.count_other_clock?.ToString();
                MileageTextBox.Text = countmaterial?.count_other_milesage?.ToString();
                for (int i = 0; i < MotorTypeComboBox.Items.Count - 1; i++)
                {
                    if (MotorTypeComboBox.Items[i].ToString() == countmaterial?.count_motors)
                        MotorTypeComboBox.SelectedIndex = i;
                }
                #endregion

                #region Responsible
                stackResponseble.Children.Clear();
                if (material?.Ответственный != null)
                {
                    var parts = material.Ответственный.Split("|");
                    for (int i = 0; i < parts.Length - 1; i++)
                    {
                        var otvets = parts[i];
                        StackPanel panel = new() { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
                        TextBlock text = new() { Margin = new Thickness(20, 0, 40, 0), VerticalAlignment = VerticalAlignment.Center, Text = "Ответственный" };
                        ComboBox combo = new() { ItemsSource = GetPersone(), Background = Brushes.LightGray, Width = 430 };
                        for (int j = 0; j < combo.Items.Count; j++)
                        {
                            if (otvets == combo.Items[j].ToString())
                            {
                                combo.SelectedIndex = j;
                                break;
                            }
                        }
                        panel.Children.Add(text);
                        panel.Children.Add(combo);
                        stackResponseble.Children.Add(panel);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormMaterial] Ошибка в SetData: {ex.Message}");
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        /// <summary>Добавляет карточки масел на панель.</summary>
        public void addOilCard(string names, Material material)
        {
            try
            {
                int index = 0;
                int i = 1;
                int j = 1;
                names = names.Substring(0, names.Length - 1);

                foreach (var item in names.Split("*"))
                {
                    var countMat = App.dBcontext.CountMaterials
                        .FirstOrDefault(s => s.Id == material.countmaterial_id);
                    var countOilParts = countMat?.count_oil?.Split(",");

                    OliControl control = new OliControl()
                    {
                        Margin = new Thickness(0, 3, 0, 3),
                        brendTitle = item.Split("_").Length > 1 ? item.Split("_")[1] : "",
                        vilocityTitle = item.Split("_").Length > 2 ? item.Split("_")[2] : "",
                        valueTitle = countOilParts != null && index < countOilParts.Length ? countOilParts[index] : "0",
                    };
                    index++;

                    if (material.subunit_id != null)
                    {
                        var subParts = material.subunit_id.Split("*");
                        foreach (var item2 in subParts)
                        {
                            if (i == subParts.Length) break;

                            StackPanel stack = new StackPanel();
                            var subItems = item2.Split(",");
                            foreach (var item3 in subItems)
                            {
                                if (j == subItems.Length) break;

                                var subunit = EnsureSubunitExists(Convert.ToInt32(item3));
                                stack = new StackPanel() { Margin = new Thickness(3) };
                                stack.Children.Add(new TextBlock() { Text = subunit.Name });

                                var subCountParts = material.CountMaterials?.count_subunit?.Split("*");
                                var subCountValue = "0";
                                if (subCountParts != null && i < subCountParts.Length)
                                {
                                    var innerParts = subCountParts[i].Split(",");
                                    if (j < innerParts.Length)
                                        subCountValue = innerParts[j];
                                }
                                stack.Children.Add(new TextBlock() { Text = subCountValue });
                                control.SubPanel.Add(stack);
                                j++;
                            }
                            i++;
                        }
                    }
                    control.VisiblePanel.Visibility = Visibility.Visible;
                    panelOil.Children.Add(control);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormMaterial] Ошибка в addOilCard: {ex.Message}");
            }
        }

        /// <summary>Загружает данные существующей записи по номеру ячейки.</summary>
        public async void GetData(string НомерЯчейки)
        {
            Loaded -= Window_Loaded;

            try
            {
                var materials = App.dBcontext?.Materials?.ToList()
                    .Where(e => e.НомерЯчейки == НомерЯчейки).ToList();
                if (materials == null || materials.Count == 0)
                {
                    MessageBox.Show("Запись не найдена");
                    return;
                }
                var material = materials[0];
                MessageButton.ToolTip = material.message ?? "";
                string names = "";

                // Загрузка масел с автодобавлением
                if (!string.IsNullOrEmpty(material.oilCode))
                {
                    if (material.oilCode.IndexOf(",") != -1)
                    {
                        foreach (var item in material.oilCode.Split(","))
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                var oil = EnsureOilExists(item);
                                names += oil.Name + "*";
                            }
                        }
                        addOilCard(names, material);
                    }
                    else
                    {
                        var oil = EnsureOilExists(material.oilCode);
                        names = oil.Name;
                        var countMat = App.dBcontext.CountMaterials
                            .FirstOrDefault(s => s.Id.ToString() == material.countmaterial_id.ToString());
                        OliControl control = new OliControl()
                        {
                            brendTitle = names.Split("_").Length > 1 ? names.Split("_")[1] : "",
                            vilocityTitle = names.Split("_").Length > 2 ? names.Split("_")[2] : "",
                            valueTitle = countMat?.count_oil ?? "0"
                        };
                        panelOil.Children.Add(control);
                    }
                }
                else if (material.oil_id != null && material.oil_id.ToString() != "")
                {
                    var oil = EnsureOilExists(material.oil_id.ToString());
                    names = oil.Name + "*";
                    var countMat = App.dBcontext.CountMaterials
                        .FirstOrDefault(s => s.Id.ToString() == material.countmaterial_id.ToString());
                    OliControl control = new OliControl()
                    {
                        brendTitle = names.Split("_").Length > 1 ? names.Split("_")[1] : "",
                        vilocityTitle = names.Split("_").Length > 2 ? names.Split("_")[2] : "",
                        valueTitle = countMat?.count_oil ?? "0"
                    };
                    panelOil.Children.Add(control);
                }

                // Загрузка антифриза с автодобавлением
                var anti = (material.antifreeze_id == null || material.antifreeze_id == 0)
                    ? ["", "", "", "", "", "", ""]
                    : (App.dBcontext.Antifreeze.FirstOrDefault(e => e.Id.ToString() == material.antifreeze_id.ToString())?.Name?.Split('_')
                       ?? ["", "", "", "", "", "", ""]);

                // Загрузка смазки с автодобавлением
                var grease = (material.grease_id == null || material.grease_id == 0)
                    ? ["", "", "", "", "", "", ""]
                    : (App.dBcontext.Grease.FirstOrDefault(e => e.Id.ToString() == material.grease_id.ToString())?.Name?.Split('_')
                       ?? ["", "", "", "", "", "", ""]);

                // Загрузка других данных
                var countMaterial = App.dBcontext.CountMaterials
                    .FirstOrDefault(e => e.Id.ToString() == material.countmaterial_id.ToString());
                if (countMaterial == null) return;

                var clock = countMaterial.count_other_clock;
                var mileage = countMaterial.count_other_milesage;
                var motors = countMaterial.count_motors;
                var respone = material?.Ответственный ?? "";

                if (!string.IsNullOrEmpty(respone))
                {
                    var text = "";
                    foreach (var tr in respone.Split("|"))
                        text += tr + "\r\n";

                    stackResponseble.Children.Clear();
                    stackResponseble.Children.Add(new TextBlock() { Text = text, Margin = new Thickness(20, 0, 0, 0) });
                }

                // Заполнение ComboBox-ов
                AntifreezeColorComboBox.Items.Add(anti[0]);
                AntifreezeColorComboBox.SelectedIndex = 0;
                AntifreezeBrandComboBox.Items.Add(anti.Length > 1 ? anti[1] : "");
                AntifreezeBrandComboBox.SelectedIndex = 0;
                AntifreezeTypeComboBox.Items.Add(anti.Length > 2 ? anti[2] : "");
                AntifreezeTypeComboBox.SelectedIndex = 0;
                AntifreezeLitersTextBox.Text = material?.CountMaterials?.count_antifreeze?.ToString();

                GreaseBrandComboBox.Items.Add(grease[0]);
                GreaseBrandComboBox.SelectedIndex = 0;
                GreaseTypeComboBox.Items.Add(grease.Length > 1 ? grease[1] : "");
                GreaseTypeComboBox.SelectedIndex = 0;
                GreaseViscosityComboBox.Items.Add(grease.Length > 2 ? grease[2] : "");
                GreaseViscosityComboBox.SelectedIndex = 0;
                GreaseAmountTextBox.Text = material?.CountMaterials?.count_grease?.ToString();

                MotoHoursTextBox.Text = clock.ToString();
                MileageTextBox.Text = mileage.ToString();
                MotorTypeComboBox.Items.Add(motors);
                MotorTypeComboBox.SelectedIndex = 0;
                ResponsibleComboBox.Items.Add(respone);
                ResponsibleComboBox.SelectedIndex = 0;

                // Фильтры
                var filters = App.dBcontext.CountMaterials
                    .Where(e => e.Id.ToString() == material.countmaterial_id.ToString()).ToList();
                if (filters.Count != 0)
                {
                    foreach (var filter in filters)
                    {
                        if (string.IsNullOrEmpty(filter.count_filterids) || filter.count_filterids.Length == 1)
                        {
                            FilterListViewCount.Items.Add(filter.count_filter);
                            FilterListViewName.Items.Add(filter.count_filtername);
                        }
                        else
                        {
                            var idParts = filter.count_filterids.Split(":");
                            var filterParts = filter.count_filter?.Split(":") ?? Array.Empty<string>();
                            var nameParts = filter.count_filtername?.Split(":") ?? Array.Empty<string>();
                            for (int i = 0; i < idParts.Length; i++)
                            {
                                FilterListViewCount.Items.Add(i < filterParts.Length ? filterParts[i] : "");
                                FilterListViewName.Items.Add(i < nameParts.Length ? nameParts[i] : "");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormMaterial] Ошибка в GetData: {ex.Message}");
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        #endregion

        #region Список персонала

        public List<string> GetPersone()
        {
            return new List<string>()
            {
                "Пятых Андрей Анатольевич",
                "Джонни",
                "Илья",
                "Артем",
                "Антон",
                "Аксас Абдельхафид",
                "Муравкин Юрий",
                "Павлов Игорь",
                "Плотников Иван",
                "Резников Степан",
                "Сергеев Денис",
                "Степанов Максим",
                "Цыба Валентин",
            };
        }

        #endregion

        #region Обработчики событий UI

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                AntifreezeColorComboBox.ItemsSource = App.dBcontext.antifreezecolorfields.Select(s => s.name).ToList();
                AntifreezeColorComboBox.SelectionChanged += AntifreezeColorComboBox_SelectionChanged;
                AntifreezeBrandComboBox.SelectionChanged += AntifreezeBrandComboBox_SelectionChanged;

                GreaseBrandComboBox.ItemsSource = App.dBcontext.greasebrandfields.Select(s => s.name).ToList();
                GreaseBrandComboBox.SelectionChanged += GreaseBrandComboBox_SelectionChanged;
                GreaseTypeComboBox.SelectionChanged += GreaseTypeComboBox_SelectionChanged;

                MotorTypeComboBox.ItemsSource = new List<string>() { "Низ", "Верх", "Низ-верх" };
                ResponsibleComboBox.ItemsSource = GetPersone();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormMaterial] Ошибка в Window_Loaded: {ex.Message}");
            }
        }

        private void GreaseTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (GreaseTypeComboBox.SelectedValue == null) return;

                if (GreaseTypeComboBox.SelectedValue.ToString() == "Смазка_стрелы_Т")
                {
                    GreaseViscosityComboBox.ItemsSource = new List<string> { "NULL" };
                    GreaseViscosityComboBox.SelectedIndex = 0;
                    return;
                }

                var typeField = App.dBcontext.greasetypefields
                    .FirstOrDefault(s => s.name == GreaseTypeComboBox.SelectedValue.ToString());
                if (typeField != null)
                {
                    GreaseViscosityComboBox.ItemsSource = App.dBcontext.greasevilocityfields
                        .OrderBy(s => s.Id)
                        .Where(s => s.code.ToString() == typeField.Id.ToString())
                        .Select(s => s.name).ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormMaterial] Ошибка в GreaseTypeComboBox_SelectionChanged: {ex.Message}");
            }
        }

        private void GreaseBrandComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (GreaseBrandComboBox.SelectedValue == null) return;

                var brandField = App.dBcontext.greasebrandfields
                    .FirstOrDefault(s => s.name == GreaseBrandComboBox.SelectedValue.ToString());
                if (brandField != null)
                {
                    GreaseTypeComboBox.ItemsSource = App.dBcontext.greasetypefields
                        .OrderBy(s => s.Id)
                        .Where(s => s.code.ToString() == brandField.Id.ToString())
                        .Select(s => s.name).ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormMaterial] Ошибка в GreaseBrandComboBox_SelectionChanged: {ex.Message}");
            }
        }

        private void AntifreezeBrandComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (AntifreezeBrandComboBox.SelectedValue == null) return;

                var brandField = App.dBcontext.antifreezebrandfields
                    .FirstOrDefault(s => s.name == AntifreezeBrandComboBox.SelectedValue.ToString());
                if (brandField != null)
                {
                    AntifreezeTypeComboBox.ItemsSource = App.dBcontext.antifreezetypefields
                        .OrderBy(s => s.Id)
                        .Where(s => s.code.ToString() == brandField.Id.ToString())
                        .Select(s => s.name).ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormMaterial] Ошибка в AntifreezeBrandComboBox_SelectionChanged: {ex.Message}");
            }
        }

        private void AntifreezeColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (AntifreezeColorComboBox.SelectedValue == null) return;

                var colorField = App.dBcontext.antifreezecolorfields
                    .FirstOrDefault(s => s.name == AntifreezeColorComboBox.SelectedValue.ToString());
                if (colorField != null)
                {
                    AntifreezeBrandComboBox.ItemsSource = App.dBcontext.antifreezebrandfields
                        .OrderBy(s => s.Id)
                        .Where(s => s.code.ToString() == colorField.Id.ToString())
                        .Select(s => s.name).ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormMaterial] Ошибка в AntifreezeColorComboBox_SelectionChanged: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(MotoHoursTextBox.Text) || string.IsNullOrEmpty(MileageTextBox.Text))
            {
                MessageBox.Show("Заполните поля: Мото-часы и Пробег");
                return;
            }
            AddData();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MessageButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Wpf.Ui.Controls.Button)sender;
            Clipboard.SetText(button.ToolTip?.ToString() ?? "");
            flyout.IsOpen = true;
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
            timer.Start();
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            flyout.Dispatcher.Invoke(() => { flyout.IsOpen = false; });
            timer.Stop();
            timer.Dispose();
        }

        private void SelectFilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterListViewName.Items.Clear();
            var filterWindow = new FormAddFilters();
            if (filterWindow.ShowDialog() == true)
            {
                foreach (var filter in filterWindow.SelectedFiltersName)
                    FilterListViewName.Items.Add(filter);
                foreach (var filter in filterWindow.SelectedFiltersCount)
                    FilterListViewCount.Items.Add(filter);
            }
        }

        private void AddPersonBtn_Click(object sender, RoutedEventArgs e)
        {
            StackPanel panel = new StackPanel() { Orientation = Orientation.Horizontal, Margin = new Thickness(5) };
            TextBlock text = new TextBlock() { Margin = new Thickness(20, 0, 40, 0), VerticalAlignment = VerticalAlignment.Center, Text = "Ответственный" };
            ComboBox combo = new ComboBox() { Width = 430, ItemsSource = GetPersone(), Background = Brushes.LightGray };
            panel.Children.Add(text);
            panel.Children.Add(combo);
            stackResponseble.Children.Add(panel);
        }

        private void AddOilButton_Click(object sender, RoutedEventArgs e)
        {
            var oilWindow = new FormAddOil();
            if (oilWindow.ShowDialog() == true)
            {
                OliControl control = new OliControl()
                {
                    Margin = new Thickness(0, 3, 0, 3),
                    brendTitle = oilWindow.SelectedOil.brend,
                    vilocityTitle = oilWindow.SelectedOil.vilocity,
                    valueTitle = oilWindow.SelectedOil.value,
                };
                StackPanel stack = new StackPanel();
                List<material.Subunit> subs = new List<material.Subunit>();
                foreach (var item in oilWindow.SelectedOil.subUnits)
                {
                    stack = new StackPanel() { Margin = new Thickness(3) };
                    stack.Children.Add(new TextBlock() { Text = item.Name });
                    stack.Children.Add(new TextBlock() { Text = item.Count.ToString() });
                    control.SubPanel.Add(stack);
                    subs.Add(new material.Subunit() { Name = item.Name, Count = item.Count });
                }

                material.Oil oil = new material.Oil()
                {
                    brend = oilWindow.SelectedOil.brend,
                    type = oilWindow.SelectedOil.type,
                    vilocity = oilWindow.SelectedOil.vilocity,
                    value = oilWindow.SelectedOil.value,
                    subUnits = subs
                };

                Button button = new Button() { Content = "\u274c", Margin = new Thickness(0, 3, 0, 3) };
                button.Click += (sender, e) => DeleteOil_Click(sender, e);
                listOils.Add(oil);
                panelOil.Children.Add(control);
                panelDelOil.Children.Add(button);
            }
        }

        private int GetIndex(object sender)
        {
            int index = 0;
            foreach (var item in panelDelOil.Children)
            {
                if (sender == item) return index;
                index++;
            }
            return -1;
        }

        private void DeleteOil_Click(object sender, EventArgs e)
        {
            int index = GetIndex(sender);
            if (index < 0) return;
            panelDelOil.Children.RemoveAt(index);
            panelOil.Children.RemoveAt(index);
            listOils.RemoveAt(index);
        }

        #endregion

        #region Сохранение данных (AddData) с автодобавлением расходников

        /// <summary>Собирает все данные из формы и сохраняет в БД.</summary>
        private async void AddData()
        {
            try
            {
                // Сборка данных смазки
                var greaseBrand = GreaseBrandComboBox.Text ?? "";
                var greaseType = GreaseTypeComboBox.Text ?? "";
                var greaseViscosity = GreaseViscosityComboBox.Text ?? "";
                var greaseFullName = $"{greaseBrand}_{greaseType}_{greaseViscosity}";
                if (greaseFullName == "__") greaseFullName = "";

                // Сборка данных антифриза
                var anticolor = AntifreezeColorComboBox.Text ?? "";
                var antibrand = AntifreezeBrandComboBox.Text ?? "";
                var antitype = AntifreezeTypeComboBox.Text ?? "";
                var antiFullName = $"{anticolor}_{antibrand}_{antitype}";
                if (antiFullName == "__") antiFullName = "";

                // Сборка ответственных
                var otvets = "";
                foreach (var item in stackResponseble.Children)
                {
                    if (item is StackPanel stak)
                    {
                        var text = ((ComboBox)stak.Children[1]).Text;
                        otvets += text + "|";
                    }
                }

                var motor = MotorTypeComboBox.Text;

                // Сборка фильтров с автодобавлением
                var filtersName = "";
                var filtersCount = "";
                var filtersIds = "";
                foreach (var item in FilterListViewName.Items)
                {
                    if (item.ToString() != "")
                    {
                        filtersName += item + ":";
                        var filter = EnsureFilterExists(item.ToString());
                        filtersIds += filter.id + ":";
                    }
                }
                foreach (var item in FilterListViewCount.Items)
                {
                    if (item.ToString() != "")
                        filtersCount += item + ":";
                }

                // Сборка масел с автодобавлением
                string oil_ids = "";
                string valueOil = "";
                var oilmess = "";
                var sub_ids = "";
                var sub_ids_count = "";
                var subvalue = "";

                foreach (var item in listOils)
                {
                    var oilObj = EnsureOilExistsByName(item.ToString());
                    if (listOils.Count == 1)
                        oil_ids += oilObj.Id;
                    else
                        oil_ids += oilObj.Id + ",";

                    valueOil += item.value + ",";
                    oilmess += item.ToString()?.Replace("_", " ") + ": " + item.value + ", ";
                }

                // Обрезка лишних разделителей
                if (oilmess.Length >= 2)
                    oilmess = oilmess.Substring(0, oilmess.Length - 2);
                if (valueOil.Length >= 1)
                    valueOil = valueOil.Substring(0, valueOil.Length - 1);
                filtersName = filtersName.Length > 0 ? filtersName.Substring(0, filtersName.Length - 1) : "";
                filtersCount = filtersCount.Length > 0 ? filtersCount.Substring(0, filtersCount.Length - 1) : "";
                filtersIds = filtersIds.Length > 0 ? filtersIds.Substring(0, filtersIds.Length - 1) : "";

                object oilid = "";
                if (listOils.Count != 1 && oil_ids.Length > 0)
                    oilid = oil_ids.Substring(0, oil_ids.Length - 1);
                else
                    oilid = oil_ids;

                // Получение Id смазки, антифриза, мотора с автодобавлением
                object greaseid = string.IsNullOrEmpty(greaseFullName) || greaseFullName == "__"
                    ? "NULL"
                    : (object)EnsureGreaseExists(greaseFullName).Id;

                object antifreezeid = string.IsNullOrEmpty(antiFullName) || antiFullName == "__"
                    ? "NULL"
                    : (object)EnsureAntifreezeExists(antiFullName).Id;

                object motorid = string.IsNullOrEmpty(motor)
                    ? "NULL"
                    : (object)EnsureMotorExists(motor).id;

                // Создание объекта Material
                Material material = new Material
                {
                    Ячейка = (string?)yache,
                    ТипТраты = ТипТраты,
                    НомерЯчейки = nomer,
                    Ответственный = otvets,
                    Месяц = HomePage.CurrentMounth ?? DateTime.Now.ToString("M"),
                    Год = year == 0 ? DateTime.Now.Year : year,
                    oilCode = oil_ids.Length == 1 ? null : oil_ids,
                    oil_id = oil_ids.Length == 1 ? Convert.ToInt32(oil_ids) : null,
                    subunit_id = sub_ids,
                    motor_id = motorid.ToString() == "NULL" ? null : Convert.ToInt32(motorid),
                    grease_id = greaseid.ToString() == "NULL" ? null : Convert.ToInt32(greaseid),
                    antifreeze_id = antifreezeid.ToString() == "NULL" ? null : Convert.ToInt32(antifreezeid),
                    CountMaterials = new CountMaterials()
                    {
                        count_other_clock = int.TryParse(MotoHoursTextBox.Text, out var temp5) ? temp5 : 0,
                        count_other_milesage = int.TryParse(MileageTextBox.Text, out var temp6) ? temp6 : 0,
                        count_antifreeze = int.TryParse(AntifreezeLitersTextBox.Text, out var temp2) ? temp2 : 0,
                        count_filter = filtersCount,
                        count_subunit = sub_ids_count,
                        count_filterids = filtersIds,
                        count_filtername = filtersName,
                        count_grease = int.TryParse(GreaseAmountTextBox.Text, out var temp3) ? temp3 : 0,
                        count_oil = valueOil,
                        count_motors = MotorTypeComboBox.Text
                    }
                };

                // Формирование сообщения для WhatsApp
                var filters = "";
                if (!string.IsNullOrEmpty(filtersName))
                {
                    var fNames = filtersName.Split(":");
                    var fCounts = filtersCount.Split(":");
                    for (int i = 0; i < fNames.Length; i++)
                    {
                        if (fNames[i] != "")
                            filters += fNames[i] + " " + (i < fCounts.Length ? fCounts[i] : "0") + "шт; ";
                    }
                }

                var otvet = "";
                foreach (var item in otvets.Split("|"))
                    otvet += item + ", ";

                var chastemp = int.TryParse(MotoHoursTextBox.Text, out var ct) ? ct : 0;
                var militemp = int.TryParse(MileageTextBox.Text, out var mt) ? mt : 0;
                var itogchas = chastemp == 0 ? "" : "Мото-часы: " + chastemp + "\r\n";
                var itogmili = militemp == 0 ? "" : "Пробег: " + militemp + "\r\n";
                if (otvet.Length >= 4)
                    otvet = otvet.Substring(0, otvet.Length - 4);

                var antiMsg = antiFullName.Contains("__") || string.IsNullOrWhiteSpace(antiFullName)
                    ? "" : "Антифриз: " + antiFullName + " " + (int.TryParse(AntifreezeLitersTextBox.Text, out var tempanti) ? tempanti : 0) + "л; \r\n";
                filters = string.IsNullOrWhiteSpace(filters) ? "" : "Фильтры: " + filters + "\r\n";
                var greaseMsg = greaseFullName.Contains("__") || string.IsNullOrWhiteSpace(greaseFullName)
                    ? "" : "Смазка: " + greaseFullName + " " + (int.TryParse(GreaseAmountTextBox.Text, out var tempgrease) ? tempgrease : 0) + "л; \r\n";
                var motors = string.IsNullOrWhiteSpace(MotorTypeComboBox.Text) || MotorTypeComboBox.Text.Contains("__")
                    ? "" : "Мотор(ы) " + MotorTypeComboBox.Text + "; \r\n";

                var действие = ТипТраты == "ТО" ? "техническое обслуживание" : "Доливка";
                var номерДня = material.НомерЯчейки.Split("_")[1];
                номерДня = номерДня.Length == 1 ? "0" + номерДня : номерДня;

                string message = $"*{номерДня}.{GetMonth(HomePage.CurrentMounth)}.{DateTime.Now.Year}* " +
                    $"на машине {Pattern.GetWordAndInteger(material.НомерЯчейки.Split("_")[0])} произведена *{действие}*. Было использовано: \r\n" +
                    subvalue + oilmess + antiMsg + filters + "\r\n" + greaseMsg + motors + itogchas + itogmili +
                    "Ответственный(е): " + otvet;

                material.message = message;
                await App.dBcontext.Materials.AddAsync(material);
                await App.dBcontext.SaveChangesAsync();

                SendWhatsapp(material.НомерЯчейки.Split("_")[0], message);
                Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormMaterial] Ошибка в AddData: {ex.Message}");
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        #endregion

        #region Вспомогательные методы

        /// <summary>Преобразует название месяца в двузначный номер.</summary>
        private static readonly Dictionary<string, string> MonthToNumber = new()
        {
            ["Январь"] = "01", ["Февраль"] = "02", ["Март"] = "03",
            ["Апрель"] = "04", ["Май"] = "05", ["Июнь"] = "06",
            ["Июль"] = "07", ["Август"] = "08", ["Сентябрь"] = "09",
            ["Октябрь"] = "10", ["Ноябрь"] = "11", ["Декабрь"] = "12",
        };

        public string GetMonth(string month)
        {
            if (month != null && MonthToNumber.TryGetValue(month, out var num))
                return num;
            return DateTime.Now.Month.ToString("D2");
        }

        /// <summary>Открывает WhatsApp и отправляет сообщение.</summary>
        public async void SendWhatsapp(string group, string message)
        {
            try
            {
                string messageEncoded = Uri.EscapeDataString(message);
                string url = $"whatsapp://send?text={messageEncoded}";
                var machine = group;
                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
                machine = Pattern.GetValue(machine);
                Clipboard.SetText(machine);
                Thread.Sleep(8000);
                WhatsAppAutomation.CTRLV();
                WhatsAppAutomation.BOTTOM();
                WhatsAppAutomation.Enter();
                Clipboard.SetText("Т.О техники компании");
                WhatsAppAutomation.CTRLV();
                WhatsAppAutomation.BOTTOM();
                WhatsAppAutomation.BOTTOM();
                WhatsAppAutomation.Enter();
                WhatsAppAutomation.BOTTOM();
                WhatsAppAutomation.Enter();
                WhatsAppAutomation.Enter();
                Thread.Sleep(2000);
                var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                mainWindow?.BringToFront();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormMaterial] Ошибка отправки WhatsApp: {ex.Message}");
            }
        }

        public async Task<string> FindItemContentAsync(ListView listView, string contentToFind)
        {
            return await Task.Run(() =>
            {
                string? result = "";
                listView.Dispatcher.Invoke(() =>
                {
                    foreach (var item in listView.Items)
                    {
                        ListViewItem listViewItem = listView.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;
                        if (listViewItem?.Content != null &&
                            listViewItem.Content.ToString().IndexOf(contentToFind, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            result = listViewItem.Content.ToString();
                            return;
                        }
                    }
                });
                return result;
            });
        }

        #endregion
    }
}

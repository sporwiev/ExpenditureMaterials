using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace BifServiceExpenditureMaterials.Forms
{
    /// <summary>
    /// Форма выбора фильтров с переключателями (ToggleSwitch) и количеством.
    /// Динамически создаёт UI-элементы на основе списка фильтров из БД.
    /// </summary>
    public partial class FormAddFilters : Window
    {
        private readonly List<string> activeFiltersName = new();
        private readonly List<string> activeFiltersCount = new();

        public int MaxHeightWindow = 0;

        /// <summary>Список выбранных фильтров (названия).</summary>
        public List<string> SelectedFiltersName => activeFiltersName;

        /// <summary>Список количеств для выбранных фильтров.</summary>
        public List<string> SelectedFiltersCount => activeFiltersCount;

        public FormAddFilters()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
        }

        /// <summary>
        /// Собирает активные фильтры из визуального дерева.
        /// </summary>
        private void CollectActiveFilters()
        {
            activeFiltersName.Clear();
            activeFiltersCount.Clear();

            void FindToggleSwitches(DependencyObject parent)
            {
                try
                {
                    int count = VisualTreeHelper.GetChildrenCount(parent);
                    for (int i = 0; i < count; i++)
                    {
                        var child = VisualTreeHelper.GetChild(parent, i);

                        if (child is StackPanel panel)
                        {
                            ToggleSwitch? toggle = null;
                            ComboBox? combo = null;

                            foreach (var element in panel.Children)
                            {
                                if (element is ToggleSwitch ts) toggle = ts;
                                if (element is ComboBox cb) combo = cb;
                            }

                            if (toggle?.IsChecked == true)
                            {
                                string toggleLabel = toggle.OffContent?.ToString() ?? "Без названия";
                                string comboText = combo?.Text ?? "";
                                activeFiltersName.Add(toggleLabel);
                                activeFiltersCount.Add(comboText);
                            }
                        }

                        FindToggleSwitches(child);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[FormAddFilters] Ошибка обхода визуального дерева: {ex.Message}");
                }
            }

            FindToggleSwitches(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                FillWindowsOnFields();
                FillAllComboBoxes();
                Height += 50;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormAddFilters] Ошибка загрузки формы: {ex.Message}");
                System.Windows.MessageBox.Show("Не удалось загрузить список фильтров.", "Ошибка",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Создаёт UI-элементы для каждого фильтра из БД.
        /// </summary>
        private void FillWindowsOnFields()
        {
            try
            {
                var filters = App.dBcontext.Filter?.Select(x => x.Name).ToList() ?? new List<string>();
                foreach (var filter in filters)
                {
                    if (string.IsNullOrEmpty(filter)) continue;
                    glpan.Children.Add(CreateField(filter));
                    Height += 40;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormAddFilters] Ошибка загрузки фильтров из БД: {ex.Message}");
            }
        }

        /// <summary>
        /// Создаёт пару ToggleSwitch + ComboBox для одного фильтра.
        /// </summary>
        private StackPanel CreateField(string text)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(5)
            };

            var toggleSwitch = new ToggleSwitch
            {
                OffContent = text,
                OnContent = text,
                Width = 220,
                Margin = new Thickness(0, 0, 30, 0)
            };

            var comboBox = new ComboBox
            {
                Width = 120,
                Background = Brushes.Gray,
                Foreground = Brushes.White
            };

            toggleSwitch.Click += (s, e) => Switches_Click(s, e, comboBox);
            panel.Children.Add(toggleSwitch);
            panel.Children.Add(comboBox);
            return panel;
        }

        private void Switches_Click(object sender, RoutedEventArgs e, ComboBox combo)
        {
            try
            {
                if (sender is ToggleSwitch toggle)
                    combo.SelectedIndex = toggle.IsChecked == true ? 0 : -1;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormAddFilters] Ошибка переключения: {ex.Message}");
            }
        }

        /// <summary>
        /// Заполняет все ComboBox числами 1–100.
        /// </summary>
        private void FillAllComboBoxes()
        {
            var items = Enumerable.Range(1, 100).ToList();

            void Fill(DependencyObject parent)
            {
                try
                {
                    int count = VisualTreeHelper.GetChildrenCount(parent);
                    for (int i = 0; i < count; i++)
                    {
                        var child = VisualTreeHelper.GetChild(parent, i);
                        if (child is ComboBox comboBox)
                            comboBox.ItemsSource = items;
                        Fill(child);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[FormAddFilters] Ошибка заполнения ComboBox: {ex.Message}");
                }
            }

            Fill(this);
        }

        /// <summary>
        /// Кнопка «Сохранить».
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                activeFiltersName.Clear();
                activeFiltersCount.Clear();
                CollectActiveFilters();
                DialogResult = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormAddFilters] Ошибка сохранения: {ex.Message}");
            }
        }

        /// <summary>
        /// Кнопка «Закрыть».
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

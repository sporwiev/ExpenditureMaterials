using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using BifServiceExpenditureMaterials.Database;

namespace BifServiceExpenditureMaterials.Forms
{
    /// <summary>
    /// Форма добавления масла с выбором типа, бренда, вязкости и подразделений.
    /// </summary>
    public partial class FormAddOil : Window
    {
        private material.Oil? oil;

        /// <summary>Выбранное масло с заполненными данными.</summary>
        public material.Oil? SelectedOil => oil;

        public int index = 0;

        public FormAddOil()
        {
            InitializeComponent();
            Loaded += FormAddOil_Loaded;
        }

        private void FormAddOil_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                OilTypeComboBox.ItemsSource = App.dBcontext.oiltypefields?.Select(s => s.name).ToList();
                OilTypeComboBox.SelectionChanged += OilTypeComboBox_SelectionChanged;
                OilBrandComboBox.SelectionChanged += OilBrandComboBox_SelectionChanged;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormAddOil] Ошибка загрузки типов масел: {ex.Message}");
                MessageBox.Show("Не удалось загрузить данные масел.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OilTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (OilTypeComboBox.SelectedValue == null) return;

                var typeName = OilTypeComboBox.SelectedValue.ToString();
                var typeField = App.dBcontext.oiltypefields?.FirstOrDefault(s => s.name == typeName);
                if (typeField == null) return;

                var brands = App.dBcontext.oilbrandfields?
                    .Where(a => a.code.ToString() == typeField.Id.ToString())
                    .Select(a => a.name)
                    .ToList();
                OilBrandComboBox.ItemsSource = brands;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormAddOil] Ошибка при выборе типа масла: {ex.Message}");
            }
        }

        private void OilBrandComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (OilBrandComboBox.SelectedValue == null) return;

                var brandName = OilBrandComboBox.SelectedValue.ToString();
                var brandField = App.dBcontext.oilbrandfields?.FirstOrDefault(s => s.name == brandName);
                if (brandField == null) return;

                var viscosities = App.dBcontext.oilvilocityfields?
                    .OrderBy(s => s.Id)
                    .Where(s => s.code.ToString() == brandField.Id.ToString())
                    .Select(s => s.name)
                    .ToList();
                OilViscosityComboBox.ItemsSource = viscosities;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormAddOil] Ошибка при выборе бренда: {ex.Message}");
            }
        }

        /// <summary>
        /// Кнопка «Отмена» — закрывает форму.
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Кнопка «Сохранить» — собирает данные и возвращает DialogResult=true.
        /// </summary>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                oil = new material.Oil();
                int i = 0;
                foreach (var item in panelSubunit.Children)
                {
                    if (i >= 1 && item is StackPanel stack)
                    {
                        var nameCombo = stack.Children[1] as ComboBox;
                        var valueBox = stack.Children[2] as TextBox;
                        var name = nameCombo?.Text ?? "";
                        var valueText = valueBox?.Text ?? "0";
                        if (!int.TryParse(valueText, out int value)) value = 0;

                        oil.subUnits.Add(new material.Subunit { Count = value, Name = name });
                    }
                    i++;
                }

                oil.type = OilTypeComboBox.Text;
                oil.brend = OilBrandComboBox.Text;
                oil.vilocity = OilViscosityComboBox.Text;
                oil.value = OilLitersTextBox.Text;
                DialogResult = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormAddOil] Ошибка сохранения: {ex.Message}");
                MessageBox.Show("Ошибка при сохранении данных.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Создаёт UI-элемент для подразделения (без предзаданных значений).
        /// </summary>
        public StackPanel CreateUiElement()
        {
            try
            {
                return CreateSubunitPanel(0, "0");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormAddOil] Ошибка создания элемента: {ex.Message}");
                return new StackPanel();
            }
        }

        /// <summary>
        /// Создаёт UI-элемент для подразделения с предзаданным индексом и значением.
        /// </summary>
        public StackPanel CreateUiElement(int selectedIndex, int countValue)
        {
            try
            {
                return CreateSubunitPanel(selectedIndex, countValue.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormAddOil] Ошибка создания элемента: {ex.Message}");
                return new StackPanel();
            }
        }

        /// <summary>
        /// Общий метод создания панели подразделения.
        /// </summary>
        private StackPanel CreateSubunitPanel(int selectedIndex, string initialValue)
        {
            var stackpanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(5)
            };

            List<string> subunitNames;
            try
            {
                subunitNames = App.dBcontext.subunit?.Select(s => s.Name).ToList() ?? new List<string>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormAddOil] Ошибка загрузки подразделений: {ex.Message}");
                subunitNames = new List<string>();
            }

            var label = new TextBlock
            {
                Width = 90,
                Text = "Часть",
                VerticalAlignment = VerticalAlignment.Center
            };

            var combo = new ComboBox
            {
                ItemsSource = subunitNames,
                Background = Brushes.LightGray,
                Width = 140,
                SelectedIndex = selectedIndex
            };

            var input = new TextBox
            {
                Margin = new Thickness(10, 0, 0, 0),
                Width = 60,
                Text = initialValue
            };
            input.TextChanged += Input_TextChanged;

            var button = new Button
            {
                Margin = new Thickness(10, 0, 0, 0),
                Content = "❌",
                VerticalAlignment = VerticalAlignment.Center
            };
            button.Click += DeleteItem_Click;

            stackpanel.Children.Add(label);
            stackpanel.Children.Add(combo);
            stackpanel.Children.Add(input);
            stackpanel.Children.Add(button);

            return stackpanel;
        }

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int result = 0;
                int i = 0;
                foreach (var item in panelSubunit.Children)
                {
                    if (i >= 1 && item is StackPanel stack)
                    {
                        var textBox = stack.Children[2] as TextBox;
                        var text = textBox?.Text ?? "0";
                        if (int.TryParse(text, out int value))
                            result += value;
                    }
                    i++;
                }
                OilLitersTextBox.Text = result.ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormAddOil] Ошибка пересчёта литров: {ex.Message}");
            }
        }

        private int GetIndex(object sender)
        {
            int index = 0;
            foreach (var item in panelSubunit.Children)
            {
                if (item is StackPanel stack &&
                    stack.Children.Count > 3 &&
                    stack.Children[3] is Button btn &&
                    btn == (Button)sender)
                {
                    return index;
                }
                index++;
            }
            return 0;
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var idx = GetIndex(sender);
                if (idx >= 0 && idx < panelSubunit.Children.Count)
                    panelSubunit.Children.RemoveAt(idx);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormAddOil] Ошибка удаления элемента: {ex.Message}");
            }
        }

        /// <summary>
        /// Кнопка «Добавить часть» — создаёт новый элемент подразделения.
        /// </summary>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            panelSubunit.Children.Add(CreateUiElement());
        }

        private void OilLitersTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(OilLitersTextBox.Text)) return;
                if (!int.TryParse(OilLitersTextBox.Text, out int totalLiters) || totalLiters <= 0) return;

                int count = App.dBcontext.subunit?.Count() ?? 0;
                if (count <= 0) return;

                int subValue = totalLiters / count;
                for (int i = 0; i < count - 1; i++)
                {
                    panelSubunit.Children.Add(CreateUiElement(i, subValue));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormAddOil] Ошибка авто-распределения литров: {ex.Message}");
            }
        }
    }
}

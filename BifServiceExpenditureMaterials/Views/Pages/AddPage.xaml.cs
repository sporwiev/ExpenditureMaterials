using System.Diagnostics;
using System.Windows.Controls;
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.Models;

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Страница добавления новых расходников (масло, антифриз, смазка, фильтр) в справочники.
    /// </summary>
    public partial class AddPage : UserControl
    {
        public AddPage()
        {
            InitializeComponent();
            combo.SelectionChanged += Combo_SelectionChanged;
            combo.SelectedIndex = 0;
        }

        // ─── Переключение типа расходника ───────────────────────────────────────

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Сбрасываем все в активное состояние
                bool isFilter = combo.SelectedIndex == 3;
                suzh1.IsEnabled = !isFilter;
                suzh2.IsEnabled = !isFilter;
                suzh3.IsEnabled = !isFilter;
                suzhcombobox1.IsEnabled = !isFilter;
                suzhcombobox2.IsEnabled = !isFilter;
                suzhcombobox3.IsEnabled = !isFilter;
                suzhtextbox1.IsEnabled = !isFilter;
                suzhtextbox2.IsEnabled = !isFilter;
                suzhtextbox3.IsEnabled = !isFilter;
                but1.IsEnabled = !isFilter;
                but2.IsEnabled = !isFilter;
                but3.IsEnabled = !isFilter;
                filterTextBox.IsEnabled = isFilter;

                switch (combo.SelectedIndex)
                {
                    case 0: // Масло
                        suzh1.Text = "Тип (Пример: 'Гидравлическое')";
                        suzh2.Text = "Марка (Пример: 'XCMG_Г')";
                        suzh3.Text = "Вязкость (Пример: 'HVLP_46')";
                        suzhcombobox1.ItemsSource = App.dBcontext.oiltypefields?.Select(s => s.name).ToList();
                        suzhcombobox2.ItemsSource = App.dBcontext.oilbrandfields?.Select(s => s.name).ToList();
                        suzhcombobox3.ItemsSource = App.dBcontext.oilvilocityfields?.Select(s => s.name).ToList();
                        break;
                    case 1: // Антифриз
                        suzh1.Text = "Цвет (Пример: 'Красный')";
                        suzh2.Text = "Марка (Пример: 'Лукойл')";
                        suzh3.Text = "Тип (Пример: 'G_12')";
                        suzhcombobox1.ItemsSource = App.dBcontext.antifreezecolorfields?.Select(s => s.name).ToList();
                        suzhcombobox2.ItemsSource = App.dBcontext.antifreezebrandfields?.Select(s => s.name).ToList();
                        suzhcombobox3.ItemsSource = App.dBcontext.antifreezetypefields?.Select(s => s.name).ToList();
                        break;
                    case 2: // Смазка
                        suzh1.Text = "Марка (Пример: 'TESMA')";
                        suzh2.Text = "Тип (Пример: 'Смазка_стрелы_T')";
                        suzh3.Text = "Вязкость (Пример: 'MC_4217_2p')";
                        suzhcombobox1.ItemsSource = App.dBcontext.greasebrandfields?.Select(s => s.name).ToList();
                        suzhcombobox2.ItemsSource = App.dBcontext.greasetypefields?.Select(s => s.name).ToList();
                        suzhcombobox3.ItemsSource = App.dBcontext.greasevilocityfields?.Select(s => s.name).ToList();
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AddPage] Ошибка при переключении типа: {ex.Message}");
            }
        }

        // ─── Переключение TextBox ↔ ComboBox по кнопке ────────────────────────

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var row = Grid.GetRow(sender as Button);
            ToggleInputMode(row);
        }

        private void ToggleInputMode(int row)
        {
            (ComboBox combo, TextBox text) = row switch
            {
                2 => (suzhcombobox1, suzhtextbox1),
                3 => (suzhcombobox2, suzhtextbox2),
                4 => (suzhcombobox3, suzhtextbox3),
                _ => (null, null)
            };
            if (combo == null) return;

            bool useCombo = combo.IsEnabled;
            combo.IsEnabled = !useCombo;
            text.IsEnabled = useCombo;
            Grid.SetZIndex(combo, useCombo ? 0 : 1);
            Grid.SetZIndex(text, useCombo ? 1 : 0);
        }

        // ─── Вспомогательный метод получения текста из поля ───────────────────

        private string GetText(UIElement el) => el switch
        {
            TextBox tb => tb.Text,
            ComboBox cb => cb.Text,
            _ => ""
        };

        // ─── Кнопка «Добавить» ─────────────────────────────────────────────────

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var s1 = GetText(suzhtextbox1.IsEnabled ? (UIElement)suzhtextbox1 : suzhcombobox1);
                var s2 = GetText(suzhtextbox2.IsEnabled ? (UIElement)suzhtextbox2 : suzhcombobox2);
                var s3 = GetText(suzhtextbox3.IsEnabled ? (UIElement)suzhtextbox3 : suzhcombobox3);

                switch (combo.SelectedIndex)
                {
                    case 0: await AddOilAsync(s1, s2, s3); break;
                    case 1: await AddAntifreezeAsync(s1, s2, s3); break;
                    case 2: await AddGreaseAsync(s1, s2, s3); break;
                    case 3: await AddFilterAsync(); break;
                }

                MessageBox.Show("Расходник успешно добавлен!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AddPage] Ошибка добавления: {ex.Message}");
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ─── Масло ─────────────────────────────────────────────────────────────

        private async Task AddOilAsync(string typeName, string brandName, string viscoName)
        {
            if (string.IsNullOrWhiteSpace(typeName) || string.IsNullOrWhiteSpace(brandName))
                throw new InvalidOperationException("Укажите тип и марку масла.");

            var typeField = App.dBcontext.oiltypefields.FirstOrDefault(s => s.name == typeName);
            if (typeField == null)
            {
                typeField = new Oiltypefields { name = typeName, code = 0 };
                await App.dBcontext.oiltypefields.AddAsync(typeField);
                await App.dBcontext.SaveChangesAsync();
                typeField.code = typeField.Id;
                await App.dBcontext.SaveChangesAsync();
            }

            var brandField = App.dBcontext.oilbrandfields.FirstOrDefault(s => s.name == brandName);
            if (brandField == null)
            {
                brandField = new Oilbrandfields { name = brandName, code = typeField.Id };
                await App.dBcontext.oilbrandfields.AddAsync(brandField);
                await App.dBcontext.SaveChangesAsync();
            }

            if (!string.IsNullOrWhiteSpace(viscoName))
            {
                var viscoField = App.dBcontext.oilvilocityfields.FirstOrDefault(s => s.name == viscoName);
                if (viscoField == null)
                {
                    await App.dBcontext.oilvilocityfields.AddAsync(
                        new Oilvilocityfields { name = viscoName, code = brandField.Id });
                    await App.dBcontext.SaveChangesAsync();
                }
            }

            var oilName = typeName + "_" + brandName + (string.IsNullOrWhiteSpace(viscoName) ? "" : "_" + viscoName);
            App.dBcontext.Oil.Add(new Oil { Name = oilName });
            await App.dBcontext.SaveChangesAsync();
        }

        // ─── Антифриз ──────────────────────────────────────────────────────────

        private async Task AddAntifreezeAsync(string colorName, string brandName, string typeName)
        {
            if (string.IsNullOrWhiteSpace(colorName) || string.IsNullOrWhiteSpace(brandName))
                throw new InvalidOperationException("Укажите цвет и марку антифриза.");

            var colorField = App.dBcontext.antifreezecolorfields.FirstOrDefault(s => s.name == colorName);
            if (colorField == null)
            {
                colorField = new Antifreezecolorfields { name = colorName, code = 0 };
                await App.dBcontext.antifreezecolorfields.AddAsync(colorField);
                await App.dBcontext.SaveChangesAsync();
                colorField.code = colorField.Id;
                await App.dBcontext.SaveChangesAsync();
            }

            var brandField = App.dBcontext.antifreezebrandfields.FirstOrDefault(s => s.name == brandName);
            if (brandField == null)
            {
                brandField = new Antifreezebrandfields { name = brandName, code = colorField.Id };
                await App.dBcontext.antifreezebrandfields.AddAsync(brandField);
                await App.dBcontext.SaveChangesAsync();
            }

            if (!string.IsNullOrWhiteSpace(typeName))
            {
                var typeField = App.dBcontext.antifreezetypefields.FirstOrDefault(s => s.name == typeName);
                if (typeField == null)
                {
                    await App.dBcontext.antifreezetypefields.AddAsync(
                        new Antifreezetypefields { name = typeName, code = brandField.Id });
                    await App.dBcontext.SaveChangesAsync();
                }
            }

            var antiName = colorName + "_" + brandName + (string.IsNullOrWhiteSpace(typeName) ? "" : "_" + typeName);
            App.dBcontext.Antifreeze.Add(new Antifreeze { Name = antiName });
            await App.dBcontext.SaveChangesAsync();
        }

        // ─── Смазка ────────────────────────────────────────────────────────────

        private async Task AddGreaseAsync(string brandName, string typeName, string viscoName)
        {
            if (string.IsNullOrWhiteSpace(brandName))
                throw new InvalidOperationException("Укажите марку смазки.");

            var brandField = App.dBcontext.greasebrandfields.FirstOrDefault(s => s.name == brandName);
            if (brandField == null)
            {
                brandField = new Greasebrandfields { name = brandName, code = 0 };
                await App.dBcontext.greasebrandfields.AddAsync(brandField);
                await App.dBcontext.SaveChangesAsync();
                brandField.code = brandField.Id;
                await App.dBcontext.SaveChangesAsync();
            }

            var typeField = App.dBcontext.greasetypefields.FirstOrDefault(s => s.name == typeName);
            if (typeField == null && !string.IsNullOrWhiteSpace(typeName))
            {
                typeField = new Greasetypefields { name = typeName, code = brandField.Id };
                await App.dBcontext.greasetypefields.AddAsync(typeField);
                await App.dBcontext.SaveChangesAsync();
            }

            if (!string.IsNullOrWhiteSpace(viscoName))
            {
                var viscoField = App.dBcontext.greasevilocityfields.FirstOrDefault(s => s.name == viscoName);
                if (viscoField == null)
                {
                    await App.dBcontext.greasevilocityfields.AddAsync(
                        new Greasevilocityfields { name = viscoName, code = typeField?.Id ?? brandField.Id });
                    await App.dBcontext.SaveChangesAsync();
                }
            }

            var greaseName = brandName
                + (string.IsNullOrWhiteSpace(typeName) ? "" : "_" + typeName)
                + (string.IsNullOrWhiteSpace(viscoName) ? "" : "_" + viscoName);
            App.dBcontext.Grease.Add(new Grease { Name = greaseName });
            await App.dBcontext.SaveChangesAsync();
        }

        // ─── Фильтр ────────────────────────────────────────────────────────────

        private async Task AddFilterAsync()
        {
            var name = filterTextBox.Text?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Введите название фильтра.");

            App.dBcontext.Filter.Add(new Filter { Name = name });
            await App.dBcontext.SaveChangesAsync();
        }
    }
}

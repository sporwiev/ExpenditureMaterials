using System.Diagnostics;
using System.Windows.Controls;
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.Models;
using BifServiceExpenditureMaterials.Helpers;

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Страница пополнения запасов расходников.
    /// </summary>
    public partial class BayMaterialsPage : UserControl
    {
        public BayMaterialsPage()
        {
            InitializeComponent();
        }

        // ─── Тип расходника ────────────────────────────────────────────────────

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                CountTextBox.Text = "";
                switch ((sender as ComboBox).SelectedIndex)
                {
                    case 0: ComboBoxProduct.ItemsSource = App.dBcontext.Oil?.Select(s => s.Name).ToList(); break;
                    case 1: ComboBoxProduct.ItemsSource = App.dBcontext.Antifreeze?.Select(s => s.Name).ToList(); break;
                    case 2: ComboBoxProduct.ItemsSource = App.dBcontext.Grease?.Select(s => s.Name).ToList(); break;
                    case 3: ComboBoxProduct.ItemsSource = App.dBcontext.Filter?.Select(s => s.Name).ToList(); break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BayMaterials] Ошибка загрузки расходников: {ex.Message}");
            }
        }

        // ─── Выбор конкретного расходника — показать остаток ──────────────────

        private void ComboBoxProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ComboBoxProduct.SelectedItem == null) return;
                var name = ComboBoxProduct.SelectedItem as string;

                switch (TypeProductComboBox.SelectedIndex)
                {
                    case 0: ShowOilCount(name); break;
                    case 1: ShowAntifreezeCount(name); break;
                    case 2: ShowGreaseCount(name); break;
                    case 3: ShowFilterCount(name); break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BayMaterials] Ошибка отображения остатка: {ex.Message}");
                CountTextBox.Text = "Ошибка загрузки данных";
            }
        }

        private void ShowOilCount(string name)
        {
            var oil = App.dBcontext.Oil.FirstOrDefault(s => s.Name == name);
            if (oil == null) { CountTextBox.Text = "Масло не найдено"; return; }
            var rec = App.dBcontext.oilcount?.FirstOrDefault(s => s.id_oil == oil.Id);
            CountTextBox.Text = rec == null ? "Остатка нет" : $"Текущий остаток: {rec.count} л";
        }

        private void ShowAntifreezeCount(string name)
        {
            var anti = App.dBcontext.Antifreeze.FirstOrDefault(s => s.Name == name);
            if (anti == null) { CountTextBox.Text = "Антифриз не найден"; return; }
            var rec = App.dBcontext.antifreezecount?.FirstOrDefault(s => s.id_antifreeze == anti.Id);
            CountTextBox.Text = rec == null ? "Остатка нет" : $"Текущий остаток: {rec.count} л";
        }

        private void ShowGreaseCount(string name)
        {
            var grease = App.dBcontext.Grease.FirstOrDefault(s => s.Name == name);
            if (grease == null) { CountTextBox.Text = "Смазка не найдена"; return; }
            var rec = App.dBcontext.greasecount?.FirstOrDefault(s => s.id_grease == grease.Id);
            CountTextBox.Text = rec == null ? "Остатка нет" : $"Текущий остаток: {rec.count} кг";
        }

        private void ShowFilterCount(string name)
        {
            var filter = App.dBcontext.Filter.FirstOrDefault(s => s.Name == name);
            if (filter == null) { CountTextBox.Text = "Фильтр не найден"; return; }
            var rec = App.dBcontext.filterscount?.FirstOrDefault(s => s.id_filter == filter.id);
            CountTextBox.Text = rec == null ? "Остатка нет" : $"Текущий остаток: {rec.count} шт";
        }

        // ─── Кнопка «Пополнить» ────────────────────────────────────────────────

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(TextBoxCountProduct.Text, out int addCount) || addCount <= 0)
                {
                    MessageBox.Show("Введите корректное количество.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (ComboBoxProduct.SelectedItem == null)
                {
                    MessageBox.Show("Выберите расходник.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string currentCount = "";

                switch (TypeProductComboBox.SelectedIndex)
                {
                    case 0: currentCount = await ReplenishOilAsync(addCount); break;
                    case 1: currentCount = await ReplenishAntifreezeAsync(addCount); break;
                    case 2: currentCount = await ReplenishGreaseAsync(addCount); break;
                    case 3: currentCount = await ReplenishFilterAsync(addCount); break;
                }

                await App.dBcontext.SaveChangesAsync();

                // Уведомление по SignalR
                try
                {
                    var productName = ComboBoxProduct.SelectedItem as string;
                    await SignalRClient.SendNotificationAsync(
                        $"0x05|Пополнено на {addCount}, текущий остаток {currentCount}|{productName}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[BayMaterials] SignalR ошибка: {ex.Message}");
                }

                // Обновить отображение остатка
                ComboBoxProduct_SelectionChanged(null, null);

                MessageBox.Show($"Пополнение выполнено. Текущий остаток: {currentCount}", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[BayMaterials] Ошибка пополнения: {ex.Message}");
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ─── Пополнение по типам ───────────────────────────────────────────────

        private async Task<string> ReplenishOilAsync(int addCount)
        {
            var name = ComboBoxProduct.SelectedItem as string;
            var oil = App.dBcontext.Oil.FirstOrDefault(s => s.Name == name)
                      ?? throw new InvalidOperationException("Масло не найдено в базе данных.");

            var rec = App.dBcontext.oilcount?.FirstOrDefault(s => s.id_oil == oil.Id);
            if (rec != null)
            {
                if (rec.count > 10)
                    throw new InvalidOperationException("Достигнут максимальный остаток масла.");
                rec.count += addCount;
            }
            else
            {
                await App.dBcontext.oilcount.AddAsync(
                    new OilCount { count = addCount, id_oil = oil.Id, date = DateTime.Now.ToString(), is_editing = 1 });
            }

            App.dBcontext.historycountoil.Add(
                new HistoryCountOil { count = addCount, oil_id = oil.Id, date = DateTime.Now.ToString(), is_current = 1 });

            await App.dBcontext.SaveChangesAsync();
            return App.dBcontext.oilcount.FirstOrDefault(s => s.id_oil == oil.Id)?.count.ToString() ?? "0";
        }

        private async Task<string> ReplenishAntifreezeAsync(int addCount)
        {
            var name = ComboBoxProduct.SelectedItem as string;
            var anti = App.dBcontext.Antifreeze.FirstOrDefault(s => s.Name == name)
                       ?? throw new InvalidOperationException("Антифриз не найден в базе данных.");

            var rec = App.dBcontext.antifreezecount?.FirstOrDefault(s => s.id_antifreeze == anti.Id);
            if (rec != null)
            {
                if (rec.count > 10)
                    throw new InvalidOperationException("Достигнут максимальный остаток антифриза.");
                rec.count += addCount;
            }
            else
            {
                await App.dBcontext.antifreezecount.AddAsync(
                    new AntifreezeCount { count = addCount, id_antifreeze = anti.Id, date = DateTime.Now.ToString(), is_editing = 1 });
            }

            App.dBcontext.historycountantifreeze.Add(
                new HistoryCountAntifreeze { count = addCount, antifreeze_id = anti.Id, date = DateTime.Now.ToString(), is_current = 1 });

            await App.dBcontext.SaveChangesAsync();
            return App.dBcontext.antifreezecount.FirstOrDefault(s => s.id_antifreeze == anti.Id)?.count.ToString() ?? "0";
        }

        private async Task<string> ReplenishGreaseAsync(int addCount)
        {
            var name = ComboBoxProduct.SelectedItem as string;
            // ИСПРАВЛЕНО: использовалась Oil-таблица вместо Grease
            var grease = App.dBcontext.Grease.FirstOrDefault(s => s.Name == name)
                         ?? throw new InvalidOperationException("Смазка не найдена в базе данных.");

            var rec = App.dBcontext.greasecount?.FirstOrDefault(s => s.id_grease == grease.Id);
            if (rec != null)
            {
                if (rec.count > 10)
                    throw new InvalidOperationException("Достигнут максимальный остаток смазки.");
                rec.count += addCount;
            }
            else
            {
                await App.dBcontext.greasecount.AddAsync(
                    new GreaseCount { count = addCount, id_grease = grease.Id, date = DateTime.Now.ToString(), is_editing = 1 });
            }

            App.dBcontext.historycountgrease.Add(
                new HistoryCountGrease { count = addCount, grease_id = grease.Id, date = DateTime.Now.ToString(), is_current = 1 });

            await App.dBcontext.SaveChangesAsync();
            return App.dBcontext.greasecount.FirstOrDefault(s => s.id_grease == grease.Id)?.count.ToString() ?? "0";
        }

        private async Task<string> ReplenishFilterAsync(int addCount)
        {
            var name = ComboBoxProduct.SelectedItem as string;
            var filter = App.dBcontext.Filter.FirstOrDefault(s => s.Name == name)
                         ?? throw new InvalidOperationException("Фильтр не найден в базе данных.");

            var rec = App.dBcontext.filterscount?.FirstOrDefault(s => s.id_filter == filter.id);
            if (rec != null)
            {
                if (rec.count > 10)
                    throw new InvalidOperationException("Достигнут максимальный остаток фильтра.");
                rec.count += addCount;
            }
            else
            {
                await App.dBcontext.filterscount.AddAsync(
                    new FiltersCount { count = addCount, id_filter = filter.id, date = DateTime.Now.ToString(), is_editing = 1 });
            }

            App.dBcontext.historycountfilters.Add(
                new HistoryCountFilters { count = addCount, filter_id = filter.id, date = DateTime.Now.ToString(), is_current = 1 });

            await App.dBcontext.SaveChangesAsync();
            return App.dBcontext.filterscount.FirstOrDefault(s => s.id_filter == filter.id)?.count.ToString() ?? "0";
        }
    }
}

using System.Diagnostics;
using System.Windows.Controls;
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.Models;

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Страница управления правилами автораспознавания машин.
    /// Каждое правило связывает имя машины с шаблоном номерного знака.
    /// </summary>
    public partial class AddPatternMachine : UserControl
    {
        public AddPatternMachine()
        {
            InitializeComponent();
            Loaded += AddPatternMachine_Loaded;
        }

        // ─── Загрузка ──────────────────────────────────────────────────────────

        private void AddPatternMachine_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMachineList();
            LoadPatterns();
        }

        private void LoadMachineList()
        {
            try
            {
                listmachine.ItemsSource = App.dBcontext?.machine?.Select(s => s.Code).ToList()
                                         ?? new List<string?>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AddPatternMachine] Ошибка загрузки машин: {ex.Message}");
            }
        }

        private void LoadPatterns()
        {
            try
            {
                var items = App.dBcontext?.patternMachines?.ToList()
                            ?? new List<PatternMachine>();

                // ViewModel для DataGrid
                patternsGrid.ItemsSource = items.Select(p => new PatternRow
                {
                    Id = p.Id,
                    Name = p.name,
                    Pattern = p.pattern
                }).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AddPatternMachine] Ошибка загрузки правил: {ex.Message}");
            }
        }

        // ─── Добавить новое правило ────────────────────────────────────────────

        private void AddNewPattern_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var machineName = listmachine.Text?.Trim();
                var pattern = machineeditorpattern.Text?.Trim();

                if (string.IsNullOrEmpty(machineName))
                {
                    MessageBox.Show("Выберите машину.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(pattern))
                {
                    MessageBox.Show("Введите шаблон.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                App.dBcontext.patternMachines.Add(new PatternMachine { name = machineName, pattern = pattern });
                App.dBcontext.SaveChanges();

                machineeditorpattern.Text = "";
                LoadPatterns();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AddPatternMachine] Ошибка добавления: {ex.Message}");
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ─── Сохранить изменённый шаблон ──────────────────────────────────────

        private void SavePattern_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is not FrameworkElement el || el.Tag is not int id) return;

                var row = (patternsGrid.ItemsSource as IEnumerable<PatternRow>)
                          ?.FirstOrDefault(r => r.Id == id);
                if (row == null) return;

                var entity = App.dBcontext.patternMachines.FirstOrDefault(p => p.Id == id);
                if (entity == null)
                {
                    MessageBox.Show("Правило не найдено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                entity.pattern = row.Pattern;
                App.dBcontext.SaveChanges();

                MessageBox.Show("Правило обновлено.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AddPatternMachine] Ошибка сохранения: {ex.Message}");
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ─── Удалить правило ──────────────────────────────────────────────────

        private void DeletePattern_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is not FrameworkElement el || el.Tag is not int id) return;

                var entity = App.dBcontext.patternMachines.FirstOrDefault(p => p.Id == id);
                if (entity == null) return;

                var confirm = MessageBox.Show(
                    $"Удалить правило для машины «{entity.name}»?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (confirm != MessageBoxResult.Yes) return;

                App.dBcontext.patternMachines.Remove(entity);
                App.dBcontext.SaveChanges();
                LoadPatterns();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AddPatternMachine] Ошибка удаления: {ex.Message}");
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ─── Вспомогательный класс строки таблицы ─────────────────────────────

        private class PatternRow
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Pattern { get; set; }
        }
    }
}

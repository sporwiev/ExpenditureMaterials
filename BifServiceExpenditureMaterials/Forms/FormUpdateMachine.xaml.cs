using System.Diagnostics;
using BifServiceExpenditureMaterials.ViewModels.Windows;

namespace BifServiceExpenditureMaterials.Forms
{
    /// <summary>
    /// Форма изменения кода машины.
    /// Логика — в <see cref="FormUpdateMachineViewModel"/>.
    /// </summary>
    public partial class FormUpdateMachine : Window
    {
        public FormUpdateMachineViewModel ViewModel { get; }

        public FormUpdateMachine()
        {
            InitializeComponent();
            ViewModel = new FormUpdateMachineViewModel();
            DataContext = ViewModel;
            ViewModel.CloseRequested += () => Close();

            try
            {
                MachineComboBox.ItemsSource = ViewModel.MachineCodes;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormUpdateMachine] Ошибка инициализации: {ex.Message}");
            }
        }

        /// <summary>
        /// Кнопка «Сохранить».
        /// </summary>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedMachineCode = MachineComboBox.Text?.Trim() ?? "";
            ViewModel.NewMachineCode = NewMachineTextBox.Text?.Trim() ?? "";
            await ViewModel.SaveCommand.ExecuteAsync(null);
        }

        /// <summary>
        /// Кнопка «Отмена».
        /// </summary>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ViewModel.CancelCommand.Execute(null);
        }
    }
}

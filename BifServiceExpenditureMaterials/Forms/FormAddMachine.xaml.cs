using System.Diagnostics;
using BifServiceExpenditureMaterials.ViewModels.Windows;

namespace BifServiceExpenditureMaterials.Forms
{
    /// <summary>
    /// Форма добавления новой машины в базу данных.
    /// Логика — в <see cref="FormAddMachineViewModel"/>.
    /// </summary>
    public partial class FormAddMachine : Wpf.Ui.Controls.FluentWindow
    {
        public FormAddMachineViewModel ViewModel { get; }

        public FormAddMachine()
        {
            InitializeComponent();
            ViewModel = new FormAddMachineViewModel();
            DataContext = ViewModel;
            ViewModel.CloseRequested += () => Close();
        }

        /// <summary>
        /// Заполняет ComboBox годов при загрузке формы.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                YearComboBox.ItemsSource = ViewModel.Years;
                YearComboBox.SelectedItem = ViewModel.SelectedYear;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FormAddMachine] Ошибка загрузки: {ex.Message}");
            }
        }

        /// <summary>
        /// Кнопка «Сохранить».
        /// </summary>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.MachineCode = MachineTextBox.Text?.Trim() ?? "";
            ViewModel.SelectedYear = string.IsNullOrEmpty(YearComboBox.Text)
                ? DateTime.Now.Year.ToString()
                : YearComboBox.Text;
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

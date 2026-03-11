using System;
using System.Collections.Generic;
using System.Windows;
using BifServiceExpenditureMaterials.Helpers;
using BifServiceExpenditureMaterials.Models;

namespace BifServiceExpenditureMaterials.Forms
{
    /// <summary>
    /// Форма добавления новой машины в базу данных.
    /// </summary>
    public partial class FormAddMachine : Window
    {
        public FormAddMachine()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Заполняет ComboBox годов при загрузке формы.
        /// Диапазон: от 2 лет назад до 5 лет вперёд относительно текущего года.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var currentYear = DateTime.Now.Year;
            var years = new List<string>();

            // Формируем список годов динамически
            for (int year = currentYear - 2; year <= currentYear + 5; year++)
                years.Add(year.ToString());

            YearComboBox.ItemsSource = years;
            // По умолчанию выбираем текущий год
            YearComboBox.SelectedItem = currentYear.ToString();
        }

        /// <summary>
        /// Сохраняет новую машину в базу данных и закрывает форму.
        /// Отправляет уведомление через SignalR о добавлении.
        /// </summary>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var machineCode = MachineTextBox.Text?.Trim();

            // Проверяем, что код машины не пустой
            if (string.IsNullOrEmpty(machineCode))
            {
                MessageBox.Show("Введите код машины.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newMachine = new machine
            {
                Code = machineCode,
                Год = int.TryParse(YearComboBox.Text, out var year) ? year : DateTime.Now.Year
            };

            await App.dBcontext.machine.AddAsync(newMachine);
            await App.dBcontext.SaveChangesAsync();

            // Уведомляем других клиентов через SignalR
            await SignalRClient.SendNotificationAsync($"0x03|{machineCode} Машина добавлена|Добавление");

            Close();
        }

        /// <summary>
        /// Закрывает форму без сохранения.
        /// </summary>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

using System.Windows;
using BifServiceExpenditureMaterials.Services.Interfaces;

namespace BifServiceExpenditureMaterials.Services
{
    /// <summary>
    /// Реализация сервиса диалогов через стандартные MessageBox.
    /// </summary>
    public class DialogService : IDialogService
    {
        /// <inheritdoc/>
        public void ShowMessage(string message, string title = "Информация")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <inheritdoc/>
        public void ShowError(string message, string title = "Ошибка")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <inheritdoc/>
        public void ShowWarning(string message, string title = "Предупреждение")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <inheritdoc/>
        public bool Confirm(string message, string title = "Подтверждение")
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
    }
}

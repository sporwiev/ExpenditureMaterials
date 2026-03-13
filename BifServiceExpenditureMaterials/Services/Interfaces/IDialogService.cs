namespace BifServiceExpenditureMaterials.Services.Interfaces
{
    /// <summary>
    /// Сервис для отображения диалогов и уведомлений.
    /// Абстрагирует UI-взаимодействие от ViewModels.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Показывает информационное сообщение пользователю.
        /// </summary>
        void ShowMessage(string message, string title = "Информация");

        /// <summary>
        /// Показывает сообщение об ошибке.
        /// </summary>
        void ShowError(string message, string title = "Ошибка");

        /// <summary>
        /// Показывает предупреждение.
        /// </summary>
        void ShowWarning(string message, string title = "Предупреждение");

        /// <summary>
        /// Показывает диалог подтверждения (Да/Нет).
        /// </summary>
        bool Confirm(string message, string title = "Подтверждение");
    }
}

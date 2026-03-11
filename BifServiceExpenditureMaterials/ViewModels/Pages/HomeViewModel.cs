using BifServiceExpenditureMaterials.Forms;
using CommunityToolkit.Mvvm.Input;

namespace BifServiceExpenditureMaterials.ViewModels.Pages
{
    /// <summary>
    /// ViewModel главной страницы (HomePage).
    /// Отвечает за открытие формы печати документов.
    /// </summary>
    public partial class HomeViewModel : ObservableObject
    {
        /// <summary>
        /// Открывает форму печати документа.
        /// Форма создаётся с ViewModel по умолчанию (период — текущий месяц).
        /// </summary>
        [RelayCommand]
        public void PrintButton_Click()
        {
            new FormPrint().Show();
        }
    }
}

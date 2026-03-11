using System.Windows;
using BifServiceExpenditureMaterials.ViewModels.Windows;

namespace BifServiceExpenditureMaterials.Forms
{
    /// <summary>
    /// Форма печати документа.
    /// Позволяет выбрать период (дата от/до) и вывести список записей на печать.
    /// </summary>
    public partial class FormPrint : Window
    {
        /// <summary>ViewModel формы с логикой фильтрации и данными для печати.</summary>
        public FormPrintViewModel? ViewModel { get; private set; }

        /// <summary>
        /// Конструктор с явной передачей ViewModel.
        /// Используется при открытии формы с предустановленными параметрами.
        /// </summary>
        /// <param name="viewModel">Готовая ViewModel с заполненными фильтрами.</param>
        public FormPrint(FormPrintViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = ViewModel;
        }

        /// <summary>
        /// Конструктор без параметров — ViewModel создаётся автоматически с настройками по умолчанию.
        /// </summary>
        public FormPrint()
        {
            InitializeComponent();
            ViewModel = new FormPrintViewModel();
            DataContext = ViewModel;
        }
    }
}

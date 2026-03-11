using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace BifServiceExpenditureMaterials.ViewModels.Windows
{
    /// <summary>
    /// ViewModel главного окна приложения (MainWindow).
    /// Управляет заголовком окна и элементами меню системного трея.
    /// </summary>
    public partial class MainWindowViewModel : ObservableObject
    {
        /// <summary>Заголовок главного окна.</summary>
        [ObservableProperty]
        private string _applicationTitle = "Биф Сервисы — Расход Материалов";

        /// <summary>
        /// Элементы контекстного меню иконки в системном трее.
        /// «Home» — открыть главное окно.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Главная", Tag = "tray_home" }
        };
    }
}

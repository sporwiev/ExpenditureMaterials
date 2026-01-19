using System.Collections.ObjectModel;
using BifServiceExpenditureMaterials.Views.Pages;
using Wpf.Ui.Controls;

namespace BifServiceExpenditureMaterials.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _applicationTitle = "WPF UI - BifServiceExpenditureMaterials";

        

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new()
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };

        

        
    }
}

using BifServiceExpenditureMaterials.Views.Windows;
using CommunityToolkit.Mvvm.Input;

namespace BifServiceExpenditureMaterials.ViewModels.Pages
{
    public partial class HomeViewModel : ObservableObject
    {
        [RelayCommand]
        public async Task PrintButton_Click()
        {
            MainWindow.window.Navigate(typeof());
        }
    }
}

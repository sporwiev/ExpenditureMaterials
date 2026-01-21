using BifServiceExpenditureMaterials.Forms;
using BifServiceExpenditureMaterials.Helpers;
using BifServiceExpenditureMaterials.ViewModels.Windows;
using BifServiceExpenditureMaterials.Views.Windows;
using CommunityToolkit.Mvvm.Input;

namespace BifServiceExpenditureMaterials.ViewModels.Pages
{
    public partial class HomeViewModel : ObservableObject
    {
        [RelayCommand]
        public async Task PrintButton_Click()
        {

            new FormPrint().Show();
        }
        public FormPrintViewModel InitializeViewModel()
        {
            FormPrintViewModel viewModel = new()
            {
                SelectedDayAfterComboBox = (DateTime.Now.Day - 5).ToString(),
                SelectedDayBeforeComboBox = (DateTime.Now.Day + 5).ToString(),
                SelectedMonthAfterComboBox = DateTime.Now.ToString("MMMM"),
                SelectedMonthBeforeComboBox = DateTime.Now.ToString("MMMM"),
                SelectedYearAfterComboBox = DateTime.Now.ToString("yyyy"),
                SelectedYearBeforeComboBox = DateTime.Now.ToString("yyyy"),

            };
            viewModel.EnterListBox = App.dBcontext.Materials.Where(a =>
        Other.GetMouthNumber(a.Месяц) >= Other.GetMouthNumber(viewModel.SelectedMonthBeforeComboBox) &&
        Other.GetMouthNumber(a.Месяц) <= Other.GetMouthNumber(viewModel.SelectedMonthAfterComboBox) &&
        a.Год >= Convert.ToInt32(viewModel.SelectedYearAfterComboBox) &&
        a.Год <= Convert.ToInt32(viewModel.SelectedYearBeforeComboBox) &&
        Other.GetValueInYacheyka(a.Ячейка) >= Convert.ToInt32(viewModel.SelectedDayBeforeComboBox) && Convert.ToInt32(Other.GetValueInYacheyka(a.Ячейка)) <= Convert.ToInt32(viewModel.SelectedDayAfterComboBox)).Select(a => a.ToString()).ToList();

            return null;
        }
    }
}

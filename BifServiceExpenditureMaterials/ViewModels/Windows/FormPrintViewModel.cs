using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Accessibility;
using BifServiceExpenditureMaterials.Models;

namespace BifServiceExpenditureMaterials.ViewModels.Windows
{
    public partial class FormPrintViewModel : ObservableObject
    {
        [ObservableProperty]
        List<string>? _monthComboBox = new List<string> { "Январь","Февраль","Март","Апрель","Май","Июнь","Июль","Август","Сентябрь","Октябрь","Ноябрь","Декабрь" };

        [ObservableProperty]
        List<string>? _dayComboBox = getDayList();

        [ObservableProperty]
        List<string>? _yearComboBox = getYearList();

        
        public List<string>? EnterListBox { get => App.dBcontext.Materials.Where(a => 
        GetIntMonth(a.Месяц) >= GetIntMonth(SelectedMonthBeforeComboBox) && 
        GetIntMonth(a.Месяц) <= GetIntMonth(SelectedMonthAfterComboBox) && 
        a.Год >= Convert.ToInt32(SelectedYearAfterComboBox) && 
        a.Год <= Convert.ToInt32(SelectedYearBeforeComboBox) &&
        GetValueInYacheyka(a.Ячейка) >= Convert.ToInt32(SelectedDayBeforeComboBox) && Convert.ToInt32(GetValueInYacheyka(a.Ячейка)) <= Convert.ToInt32(SelectedDayAfterComboBox)).Select(a => a.ToString()).ToList();
            set; }


        private int GetValueInYacheyka(string yacheyka)
        {
            return Convert.ToInt32(yacheyka.Split(":").ToList().Last());
        }
        private int GetIntMonth(string month)
        {
            if (month != null)
            {
                Dictionary<string, int> months = new Dictionary<string, int>
                {
                    ["Январь"] = 1,
                    ["Февраль"] = 2,
                    ["Март"] = 3,
                    ["Апрель"] = 4,
                    ["Май"] = 5,
                    ["Июнь"] = 6,
                    ["Июль"] = 7,
                    ["Август"] = 8,
                    ["Сентябрь"] = 9,
                    ["Октябрь"] = 10,
                    ["Ноябрь"] = 11,
                    ["Декабрь"] = 12,
                };

                return months[month];
            }
            return 0;
        }
        public string? SelectedMonthBeforeComboBox {  
            get => SelectedMonthBeforeComboBox; 
            set {
                SelectedMonthBeforeComboBox = value;
                OnPropertyChanged();
            } }

        public string? SelectedDayBeforeComboBox
        {
            get => SelectedDayBeforeComboBox;
            set
            {
                SelectedDayBeforeComboBox = value;
                OnPropertyChanged();
            }
        }

        public string? SelectedYearBeforeComboBox
        {
            get => SelectedYearBeforeComboBox;
            set
            {
                SelectedYearBeforeComboBox = value;
                OnPropertyChanged();
            }
        }

        public string? SelectedMonthAfterComboBox
        {
            get => SelectedMonthAfterComboBox;
            set
            {
                SelectedMonthAfterComboBox = value;
                OnPropertyChanged();
            }
        }

        public string? SelectedDayAfterComboBox
        {
            get => SelectedDayAfterComboBox;
            set
            {
                SelectedDayAfterComboBox = value;
                OnPropertyChanged();
            }
        }

        public string? SelectedYearAfterComboBox
        {
            get => SelectedYearAfterComboBox;
            set
            {
                SelectedYearAfterComboBox = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        private static List<string> getDayList()
        {
            List<string> list = new List<string>();
            for (int i = 1; i < 32; i++)
                list.Add(i.ToString());
            return list;
        }
        private static List<string> getYearList()
        {
            List<string> list = new List<string>();
            for (int i = 2024; i < 2030; i++)
                list.Add(i.ToString());
            return list;
        }
    }


}

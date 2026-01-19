using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BifServiceExpenditureMaterials.Helpers
{
    public static class Other
    {
        public static int GetMouthNumber(string mouth)
        {
            Dictionary<string, int> mouths = new Dictionary<string, int>()
            {
                ["Янаврь"] =   01,
                ["Февраль"] =  02,
                ["Март"] =     03,
                ["Апрель"] =   04,
                ["Май"] =      05,
                ["Июнь"] =     06,
                ["Июль"] =     07,
                ["Август"] =   08,
                ["Сентябрь"] = 09,
                ["Октябрь"] =  10,
                ["Ноябрь"] =   11,
                ["Декабрь"] =  12
            };
            return mouths[mouth];
        }
        public static string GetMouthNumber(int number)
        {
            Dictionary<int, string> mouths = new Dictionary<int, string>()
            {
                [01] = "Янаврь",
                [02] = "Февраль",
                [03] = "Март",
                [04] = "Апрель",
                [05] = "Май",
                [06] = "Июнь",
                [07] = "Июль",
                [08] = "Август",
                [09] = "Сентябрь",
                [10] = "Октябрь" ,
                [11] = "Ноябрь",
                [12] = "Декабрь"
            };
            return mouths[number];
        }
        public static List<string> GetAllMoths()
        {
            return new List<string>() {
                "",
                "Янаврь",
                "Февраль",
                "Март",
                "Апрель",
                "Май",
                "Июнь",
                "Июль",
                "Август",
                "Сентябрь",
                "Октябрь" ,
                "Ноябрь",
                "Декабрь"
            };
        }
        public static string GetPathProject(string name)
        {
            string path = "";
            foreach (var item in Environment.CurrentDirectory.Split("\\"))
            {
                path += item + "\\";
                if(item.ToUpper() == "BifServiceExpenditureMaterials")
                {
                    return path + "\\" + name;
                }

            }
            return path;
        }
    }
}

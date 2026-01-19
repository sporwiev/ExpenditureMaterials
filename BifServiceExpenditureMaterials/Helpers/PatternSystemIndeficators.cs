using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BifServiceExpenditureMaterials.Helpers
{
    class PatternSystemIndeficators
    {
        public static string GetUser(string key)
        {
            Dictionary<string, string> userkey = new Dictionary<string, string>()
            {
                ["0FC0AA46-82C5-11E9-A784-98FA9B2496C2"] = "Андрей Пятых",
                ["215D5922-38C0-4840-AE4F-88A4C2841B36"] = "Администратор",
                [""] = "Ольга Растворова",
                

            };
            return userkey[key];
        }
    }
}

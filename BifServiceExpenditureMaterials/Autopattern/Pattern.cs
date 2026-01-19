using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BifServiceExpenditureMaterials.Database;

namespace BifServiceExpenditureMaterials.Autopattern
{
    /// <summary>
    /// Паттерны для нахождения группы в whatsapp по номеру машины
    /// </summary>
    public class Pattern
    {
        /// <summary>
        /// Метод возврата названия группы
        /// </summary>
        /// <param name="key">Параметр названия номера машины</param>
        /// <returns></returns>
        public static string GetValue(string key)
        {

            
            {
                if(App.dBcontext.patternMachines.Where(s => s.name == key).Any())
                {
                    var pattern = App.dBcontext.patternMachines.Where(s => s.name == key).First();
                    return pattern.pattern;
                }
                else
                {
                    return key;
                }
            }

            //switch (key) 
            //{
            //    case "С825МР198":
            //        return "XCMG 100 c825mp198";
            //    case "В988СТ178":
            //        return "*ВВВ988СТ178*";
            //    case "В986СТ178":
            //        return "*ВВВ986СТ178*";
            //    default:
            //        return key;
            //}
        }
        /// <summary>
        /// Создание пробелов между цифрами и буквами
        /// </summary>
        /// <param name="input">Исходный номер машины</param>
        /// <returns></returns>
        public static string GetWordAndInteger(string input)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                sb.Append(input[i]);
                // Проверяем, есть ли следующий символ и является ли он другой категорией
                if (i < input.Length - 1)
                {
                    bool currentIsLetter = char.IsLetter(input[i]);
                    bool currentIsDigit = char.IsDigit(input[i]);
                    bool nextIsLetter = char.IsLetter(input[i + 1]);
                    bool nextIsDigit = char.IsDigit(input[i + 1]);

                    if ((currentIsLetter && nextIsDigit) || (currentIsDigit && nextIsLetter))
                    {
                        sb.Append(' ');
                    }
                }
            }
            string newitem = ""; 
            for (int i = 0; i < input.Length-1; i++)
            {
                var isInteger = isConvertedToInteger(input[i]);
                var isWords = isConvertedToInteger(input[i + 1]);
                if(isInteger || isWords)
                {
                    newitem += input[i];
                }
            }
                return sb.ToString();
        }
        public static bool isConvertedToInteger(char i)
        {
            try
            {
                return int.TryParse(i.ToString(), out int b);
                
            }
            catch (Exception ex) 
            {
                return false;
            }
        }
    }
}

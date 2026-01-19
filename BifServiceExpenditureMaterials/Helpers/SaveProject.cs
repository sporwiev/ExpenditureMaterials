using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BifServiceExpenditureMaterials.Views.Pages;
using System.IO;
using System.Windows.Controls;
using BifServiceExpenditureMaterials.Controls;

namespace BifServiceExpenditureMaterials.Helpers
{
    class SaveProject
    {
        public static void OnSave(string pathDirectory,string nameFile)
        {

            var allTabsData = new Dictionary<string, string>();

            foreach (TabItem tab in HomePage.tab.Items)
            {
                if (tab.Content is MonthlyTable table)
                {
                    allTabsData[(string)tab.Header] = table.SerializeTableData();
                }
            }
            var json = JsonConvert.SerializeObject(allTabsData, Formatting.Indented);

            File.WriteAllText(pathDirectory + "\\" + nameFile, json);
        }
        public static void OnDownload(string filePath)
        {
            if (!File.Exists(filePath))
                return;
            var json = File.ReadAllText(filePath);
            var allTabsData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            foreach (TabItem tab in HomePage.tab.Items)
            {
                if (tab.Content is MonthlyTable table && allTabsData.TryGetValue((string)tab.Header, out var tabDataJson))
                {
                    table.DeserializeTableData(tabDataJson);
                }
            }
        }
        //public static TabControl OnDownload(string filePath)
        //{
        //    var json = File.ReadAllText(filePath);
        //    TabControl tabs = new TabControl();
        //    List<MonthlyTable> tables = JsonConvert.DeserializeObject<List<MonthlyTable>>(json);
        //    foreach(var item in tables)
        //    {
        //        tabs.Items.Add(item);
        //    }
        //    return tabs;

        //}
    }
}

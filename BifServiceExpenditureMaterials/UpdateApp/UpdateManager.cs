using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BifServiceExpenditureMaterials.UpdateApp
{
    public static class UpdateManager
    {
        public static string GetAppDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
        public static void InstallUpdate(string version)
        {
            Process.Start(GetAppDirectory() + "\\BifService\\UpdateBif\\UpdateBIFApplication\\bin\\Debug\\net8.0-windows\\UpdateBIFApplication.exe");
            Environment.Exit(0);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using BifServiceExpenditureMaterials.Views.Pages;
using BifServiceExpenditureMaterials.Views.Windows;

namespace BifServiceExpenditureMaterials.Helpers
{
    class Networks
    {
        private static string wifiProfileName = "BIF_5G";
        private static bool reconnecting = false;
        private static bool checking = false;

        public static string GetLocalPath()
        {
            var path = "";
            foreach(var directory in Environment.CurrentDirectory.Split('\\'))
            {
                if (directory == "BifServiceExpenditureMaterials") break;
               path += directory + '\\';
            }
            path = path.Substring(0, path.Length - 1);
            return path;
        }
        public static string GetIp()
        {
            return Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString() ?? null;
        }

        public static void ConnectNetwork()
        {
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                //Console.WriteLine("Сеть отключена. Попытка переподключения...");
                if (!reconnecting)
                {
                    reconnecting = true;
                    checking = false;
                    Task.Run(() => ReconnectWifi());
                }
            }
            else
            {
                //Console.WriteLine("Сеть доступна.");
                reconnecting = false;
                checking = true;
                Task.Run(() => ChechingWifi());

            }

        }

        private static void NetworkChange_NetworkAvailabilityChanged(object? sender, NetworkAvailabilityEventArgs e)
        {
            MessageBox.Show("");
        }


        private static void ChechingWifi() 
        {
             while (checking)
             {
                 if (NetworkInterface.GetIsNetworkAvailable())
                 {

                     reconnecting = true;
                     ReconnectWifi();
                     checking = false;
                 }
                
             }
        }
        private static void ReconnectWifi()
        {

                // Пример с netsh - переподключение по профилю Wi-Fi
                while (reconnecting)
                {
                    try
                    {
                        //Console.WriteLine("Выполняется подключение к Wi-Fi...");
                        var psi = new ProcessStartInfo("netsh", $"wlan connect name=\"{wifiProfileName}\"")
                        {
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        };

                        using (var process = Process.Start(psi))
                        {
                            process.WaitForExit(10000);
                            string output = process.StandardOutput.ReadToEnd();
                            string error = process.StandardError.ReadToEnd();
                            //Console.WriteLine(output);
                            //if (!string.IsNullOrEmpty(error))
                            //Console.WriteLine("Ошибка: " + error);
                        }

                        // Проверить состояние сети
                        if (NetworkInterface.GetIsNetworkAvailable())
                        {
                            //Console.WriteLine("Сеть восстановлена.");
                            reconnecting = false;
                            checking = true;
                            Task.Run(() => ChechingWifi());

                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine("Ошибка переподключения: " + ex.Message);
                    }

                    Thread.Sleep(5000); // Подождать перед следующей попыткой
                }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using BifServiceExpenditureMaterials.Database;
using BifServiceExpenditureMaterials.UpdateApp;
using BifServiceExpenditureMaterials.Views.Pages;
using Microsoft.AspNetCore.SignalR.Client;

namespace BifServiceExpenditureMaterials.Helpers
{
    public static class SignalRClient
    {
        public static HubConnection Connection { get; set; }

        public static async Task InitializeAsync()
        {
            if (Connection != null && Connection.State == HubConnectionState.Connected)
                return;
            //IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
            var a = Networks.GetIp();
            Connection = new HubConnectionBuilder()
                .WithUrl($"http://" + a + ":5000/hub")
                .WithAutomaticReconnect()
                .Build();

            Connection.On<string>("ReceiveUpdateNotification", async (version) =>
            {
                var result = MessageBox.Show(
                    $"Доступна новая версия: {version}\nУстановить сейчас?",
                    "Обновление", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    UpdateManager.InstallUpdate(version);
                }
            });

        }
        public static async Task SendNotificationAsync(string message)
        {
            if (Connection?.State == HubConnectionState.Connected)
            {
                await Connection.InvokeAsync("NotifyChange", message);
            }
        }
        public static async Task NotifyUpdateAvailable(string version)
        {
            if (Connection?.State == HubConnectionState.Connected)
            {
                await Connection.InvokeAsync("ReceiveUpdateNotification", version);
            }
        }
    }
}

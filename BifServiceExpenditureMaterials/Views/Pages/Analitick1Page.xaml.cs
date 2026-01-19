using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BifServiceExpenditureMaterials.AutoPiter;
using BifServiceExpenditureMaterials.Controls;
using Newtonsoft.Json.Linq;

namespace BifServiceExpenditureMaterials.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для Analitick1Page.xaml
    /// </summary>
    public partial class Analitick1Page : UserControl
    {
        public Analitick1Page() { 
        
            InitializeComponent();
        }
        public string Cookie = "AuthCoocies=01026617F79FA3B8DD08FE66C75F0D32D1DD08010636003700310037003000300000012F00FF";
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            CardPanel.Children.Clear();
            AppAutoPiterApp context = new();
            while (!await context.IsAuthorization(Cookie))
            {
                Cookie = await context.Autorization();
            }
            List<JToken> jsons = await context.GetInvoiceOrderByDateTime(Cookie, startdate.Text, enddate.Text);
            int i = 0;
            foreach (JToken json in jsons)
            {
                if (json.Count() != 0)
                {
                    foreach (var t in json)
                    {
                        try
                        {
                            var number = context.GetData(t.ToString(), "OrderNumber");
                            var data = await context.GetFullInvoiceOrder(Cookie, number);
                            //await Task.Delay(1000);
                            if (/*App.dBcontext.GetData(App.dBcontext.GetData(data, "Status"), "Name") == "Выдано" && */context.GetData(data, "Name").IndexOf("Фил") != -1)
                            {
                                Card card = new Card()
                                {

                                    Count = context.GetData(data, "OrderCount"),
                                    TitleCard = context.GetData(data, "Name"),
                                    Date = context.GetData(data, "OrderDate"),
                                    Status = context.GetData(context.GetData(data, "Status"), "Name"),
                                    Margin = new Thickness(5)
                                };
                                CardPanel.Children.Add(card);

                            }


                            //MessageBox.Show(
                            //"Код детали: " + App.dBcontext.GetData(data, "DetailUid") + "\r\n" +
                            //"Номер заказа: " + App.dBcontext.GetData(data, "Number") + "\r\n" +
                            //"Номер заменён на: " + App.dBcontext.GetData(data, "NumberChange") + "\r\n" +
                            //"Дата оформления: " + App.dBcontext.GetData(data, "OrderDate") + "\r\n" +
                            //"Каталог: " + App.dBcontext.GetData(data, "CatalogName") + "\r\n" +
                            //"Регион: " + App.dBcontext.GetData(data, "Region") + "\r\n" +
                            //"Название: " + App.dBcontext.GetData(data, "Name") + "\r\n" +
                            //"Количество заказано: " + App.dBcontext.GetData(data, "OrderCount") + "\r\n" +
                            //"Получено: " + App.dBcontext.GetData(data, "GetCount") + "\r\n" +
                            //"Комментарий: " + App.dBcontext.GetData(data, "Comment") + "\r\n" +
                            //"Заметка: " + App.dBcontext.GetData(data, "Note") + "\r\n" +
                            //"Дата получения: " + App.dBcontext.GetData(data, "DeliveryDate") + "\r\n" +
                            //"Дата статуса: " + App.dBcontext.GetData(data, "StatusDate") + "\r\n" +
                            //"Цена за единицу: " + App.dBcontext.GetData(data, "Price") + "\r\n" +
                            //"Сумма: " + App.dBcontext.GetData(data, "Sum") + "\r\n" +
                            //"Тип доставки: " + App.dBcontext.GetData(data, "DeliveryType") + "\r\n" +
                            //"Раздел: " + App.dBcontext.GetData(data, "Section") + "\r\n" +
                            //"Продавец (ID): " + App.dBcontext.GetData(data, "SellerId") + "\r\n" +
                            //"Штрихкод: " + App.dBcontext.GetData(data, "BarCode") + "\r\n" +
                            //"Информация: " + App.dBcontext.GetData(App.dBcontext.GetData(data, "Info"), "Text") + "\r\n" +
                            //"Приход: " + App.dBcontext.GetData(App.dBcontext.GetData(data, "Info"), "IsPrihod") + "\r\n" +
                            //"Заявка клиента: " + App.dBcontext.GetData(App.dBcontext.GetData(data, "Info"), "IsRequestClient") + "\r\n" +
                            //"Статус: " + App.dBcontext.GetData(App.dBcontext.GetData(data, "Status"), "Name") + "\r\n");

                            i++;
                        }
                        catch (Exception ex)
                        {

                        }
                        //"Код детали: " + App.dBcontext.GetData(data, "DetailUid") + "\r\n" +
                        //"Номер заказа: " + App.dBcontext.GetData(data, "Number") + "\r\n" +
                        //"Номер заменён на: " + App.dBcontext.GetData(data, "NumberChange") + "\r\n" +
                        //"Дата оформления: " + App.dBcontext.GetData(data, "OrderDate") + "\r\n" +
                        //"Каталог: " + App.dBcontext.GetData(data, "CatalogName") + "\r\n" +
                        //"Регион: " + App.dBcontext.GetData(data, "Region") + "\r\n" +
                        //"Название: " + App.dBcontext.GetData(data, "Name") + "\r\n" +
                        //"Количество заказано: " + App.dBcontext.GetData(data, "OrderCount") + "\r\n" +
                        //"Получено: " + App.dBcontext.GetData(data, "GetCount") + "\r\n" +
                        //"Комментарий: " + App.dBcontext.GetData(data, "Comment") + "\r\n" +
                        //"Заметка: " + App.dBcontext.GetData(data, "Note") + "\r\n" +
                        //"Дата получения: " + App.dBcontext.GetData(data, "DeliveryDate") + "\r\n" +
                        //"Дата статуса: " + App.dBcontext.GetData(data, "StatusDate") + "\r\n" +
                        //"Цена за единицу: " + App.dBcontext.GetData(data, "Price") + "\r\n" +
                        //"Сумма: " + App.dBcontext.GetData(data, "Sum") + "\r\n" +
                        //"Тип доставки: " + App.dBcontext.GetData(data, "DeliveryType") + "\r\n" +
                        //"Раздел: " + App.dBcontext.GetData(data, "Section") + "\r\n" +
                        //"Продавец (ID): " + App.dBcontext.GetData(data, "SellerId") + "\r\n" +
                        //"Штрихкод: " + App.dBcontext.GetData(data, "BarCode") + "\r\n" +
                        //"Информация: " + App.dBcontext.GetData(App.dBcontext.GetData(data, "Info"), "Text") + "\r\n" +
                        //"Приход: " + App.dBcontext.GetData(App.dBcontext.GetData(data, "Info"), "IsPrihod") + "\r\n" +
                        //"Заявка клиента: " + App.dBcontext.GetData(App.dBcontext.GetData(data, "Info"), "IsRequestClient") + "\r\n" +
                        //"Статус: " + App.dBcontext.GetData(App.dBcontext.GetData(data, "Status"), "Name") + "\r\n";
                    }
                }
            }


        }

    }
}

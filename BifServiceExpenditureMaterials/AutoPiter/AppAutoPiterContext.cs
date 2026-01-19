using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BifServiceExpenditureMaterials.AutoPiter
{
    class AppAutoPiterApp
    {
        public string GetValueToJson(string result)
        {
            

            // 1. Получаем XML-ответ (как строку)
            string xml = result;

            // 2. Загружаем в XDocument
            var doc = XDocument.Parse(xml);

            // 3. Получаем тело SOAP
            XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";
            var body = doc.Root.Element(soap + "Body");

            // 4. Получаем содержимое первого элемента (например: AuthorizationResponse)
            var content = body.Elements().FirstOrDefault();

            // 5. Преобразуем в JSON
            string json = JsonConvert.SerializeXNode(content, Formatting.Indented, omitRootObject: true);

            return json;
        }
        public async Task<List<JToken>> GetInvoiceOrderByDateTime(string cookie,string FirstDate,string LastDate)
        {
            var xml = @$"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <GetInvoiceOrderByDateTime xmlns=""http://www.autopiter.ru/"">
      <DateStart>{FirstDate}</DateStart>
      <DateFinish>{LastDate}</DateFinish>
    </GetInvoiceOrderByDateTime>
  </soap:Body>
</soap:Envelope>";

            var content = new StringContent(xml, Encoding.UTF8, "text/xml");
            content.Headers.Add("SOAPAction", "\"http://www.autopiter.ru/GetInvoiceOrderByDateTime\"");
            content.Headers.Add("Cookie", cookie);
            using var client = new HttpClient();
            var response = await client.PostAsync("http://service.autopiter.ru/v2/price?WSDL", content);
            string result = await response.Content.ReadAsStringAsync();
            var itog = GetValueToJson(result);
            return JObject.Parse(itog)["GetInvoiceOrderByDateTimeResult"]?.Values().ToList();

        }
        public string GetData(string json,string nameFiels)
        {
            return JObject.Parse(json)[nameFiels]?.ToString();
        }
        public async Task<string> Autorization()
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <Authorization xmlns=""http://www.autopiter.ru/"">
      <UserID>671700</UserID>
      <Password>tesla3311</Password>
      <Save>true</Save>
    </Authorization>
  </soap:Body>
</soap:Envelope>";

            var content = new StringContent(xml, Encoding.UTF8, "text/xml");
            content.Headers.Add("SOAPAction", "\"http://www.autopiter.ru/Authorization\"");

            using var client = new HttpClient();
            var response = await client.PostAsync("http://service.autopiter.ru/v2/price?WSDL", content);
            string cookieres = "";
            if (response.Headers.TryGetValues("Set-Cookie", out var cookieHeaders))
            {
                foreach (var cookie in cookieHeaders)
                {


                    // Можно сохранить в переменную
                    cookieres = cookie.Split(';')[0]; // например: ASP.NET_SessionId=abc123
                }
            }
            return cookieres;
        }
        public async Task<string> GetOrder(string cookie,string number)
        {
            var xml = @$"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <GetInvoiceOrderByNumberInvoice xmlns=""http://www.autopiter.ru/"">
      <OrderNumber>{number}</OrderNumber>
    </GetInvoiceOrderByNumberInvoice>
  </soap:Body>
</soap:Envelope>";

            var content = new StringContent(xml, Encoding.UTF8, "text/xml");
            content.Headers.Add("SOAPAction", "\"http://www.autopiter.ru/GetInvoiceOrderByNumberInvoice\"");
            content.Headers.Add("Cookie", cookie);
            using var client = new HttpClient();
            var response = await client.PostAsync("http://service.autopiter.ru/v2/price?WSDL", content);
            string result = await response.Content.ReadAsStringAsync();
            return result;
        }
        public async Task<string> GetBasket(string cookie)
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <GetBasket xmlns=""http://www.autopiter.ru/"" />
  </soap:Body>
</soap:Envelope>";

            var content = new StringContent(xml, Encoding.UTF8, "text/xml");
            content.Headers.Add("SOAPAction", "\"http://www.autopiter.ru/GetBasket\"");
            content.Headers.Add("Cookie", cookie);
            using var client = new HttpClient();
            var response = await client.PostAsync("http://service.autopiter.ru/v2/price?WSDL", content);
            string result = await response.Content.ReadAsStringAsync();
            return result;
        }
        public async Task<string> GetFullInvoiceOrder(string cookie,string number)
        {
            var xml = @$"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <GetFullInvoiceOrder xmlns=""http://www.autopiter.ru/"">
      <OrderNumber>{number}</OrderNumber>
    </GetFullInvoiceOrder>
  </soap:Body>
</soap:Envelope>";

            var content = new StringContent(xml, Encoding.UTF8, "text/xml");
            content.Headers.Add("SOAPAction", "\"http://www.autopiter.ru/GetFullInvoiceOrder\"");
            content.Headers.Add("Cookie", cookie);
            using var client = new HttpClient();
            var response = await client.PostAsync("http://service.autopiter.ru/v2/price?WSDL", content);
            string result = await response.Content.ReadAsStringAsync();
            
            var itog = GetValueToJson(result);
            itog = GetData(itog, "GetFullInvoiceOrderResult");
            itog = GetData(itog, "OrderInformationItemModel");
            return itog;
        }
        public async Task<bool> IsAuthorization(string cookie)
        {
            var xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <IsAuthorization xmlns=""http://www.autopiter.ru/"" />
  </soap:Body>
</soap:Envelope>";

            var content = new StringContent(xml, Encoding.UTF8, "text/xml");
            content.Headers.Add("SOAPAction", "\"http://www.autopiter.ru/IsAuthorization\"");
            content.Headers.Add("Cookie", cookie);
            using var client = new HttpClient();
            var response = await client.PostAsync("http://service.autopiter.ru/v2/price?WSDL", content);
            string result = await response.Content.ReadAsStringAsync();
            
            bool itog = Convert.ToBoolean(GetData(GetValueToJson(result), "IsAuthorizationResult"));
            return itog;
        }
    }
}

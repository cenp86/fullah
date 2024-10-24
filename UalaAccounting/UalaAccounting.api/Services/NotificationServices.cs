using System;
using System.Text;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
namespace UalaAccounting.api.Services
{
    public class NotificationServices : INotificationServices
    {
        private readonly ILogger<NotificationServices> _logger;
        
        private readonly IHttpClientFactory _client;
        private string _apiBase = "";
        

        // Constructor
        public NotificationServices(IHttpClientFactory client, ILogger<NotificationServices> logger)
        {
            _logger = logger;   
            _client = client;
        }

        public void SetApiBase(string apiBase)
        {
            this._apiBase = apiBase;
        }

        public async Task NotificationHttpPOST(string message, string code)
        {
            string result = string.Empty;
            HttpResponseMessage? response = new HttpResponseMessage();

            try
            {
                HttpClient client = _client.CreateClient();

                NotificationMessage requestBody = new NotificationMessage();
                requestBody.Message = message;
                requestBody.Code = code;

                var jsonBody = JsonConvert.SerializeObject(requestBody);

                client.BaseAddress = new Uri(this._apiBase);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Notification-ah");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                };

                response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                await response.Content.ReadAsStringAsync();
            }
            catch (Exception exc)
            {
                _logger.LogError($"ApiMambu PerformHttpPost(): {exc}");

                if (response != null)
                    _logger.LogError($"ResponseApiMambu PerformHttpPost(): {await response.Content.ReadAsStringAsync()}");

                throw exc;
            }
        }
    }

    public class NotificationMessage
    {
        public string Message { get; set; }         
        public string Code { get; set; }
    }
        
}
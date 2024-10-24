using System;
using Newtonsoft.Json;
using System.Text;
using UalaAccounting.api.Models;

namespace UalaAccounting.api.Services
{
	public class ApiMambuServices : IApiMambuServices
	{
        private readonly IHttpClientFactory _client;
        private readonly ILogger<ApiMambuServices> _logger;
        private string _apiCallback = "";
        private string _apiValue = "";
        private string _apiKey = "";
        private string _apiBase = "";
        private string _userAgent = "";
        private string _headerAcceptGet = "";
        private string _headerAcceptPost = "";
        private string _tablesBackup = string.Empty;

        public ApiMambuServices(IHttpClientFactory client, ILogger<ApiMambuServices> logger)
        {
            _client = client;
            _logger = logger;
        }

        public void SetTablesBackupk(string tablesBackup)
        {
            this._tablesBackup = tablesBackup;
        }

        public void SetApiCallback(string apiCallback)
        {
            this._apiCallback = apiCallback;
        }

        public void SetApiValue(string apiValue)
        {
            this._apiValue = apiValue;
        }

        public void SetApiKey(string apiKey)
        {
            this._apiKey = apiKey;
        }

        public void SetApiBase(string apiBase)
        {
            this._apiBase = apiBase;
        }

        public void SetUserAgent(string userAgent)
        {
            this._userAgent = userAgent;
        }

        public void SetHeaderAcceptGet(string headerAcceptGet)
        {
            this._headerAcceptGet = headerAcceptGet;
        }

        public void SetHeaderAcceptPost(string headerAcceptPost)
        {
            this._headerAcceptPost = headerAcceptPost;
        }


        public async Task<Stream> PerformBackupHttpGet()
        {
            HttpResponseMessage? response = new HttpResponseMessage();

            try
            {
                string apiKey_value = this._apiValue;
                string apiKey = this._apiKey;

                HttpClient client = _client.CreateClient();

                client.BaseAddress = new Uri(this._apiBase);
                client.DefaultRequestHeaders.Add(this._apiKey, this._apiValue);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(this._userAgent);
                client.DefaultRequestHeaders.Accept.ParseAdd(this._headerAcceptGet);
                client.Timeout = TimeSpan.FromMinutes(50);
                //client.DefaultRequestHeaders.Add("Accept", "application/vnd.mambu.v2+zip");

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{this._apiBase}/api/database/backup/LATEST")
                };

                response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStreamAsync();

            }
            catch (Exception exc)
            {
                _logger.LogError($"ApiMambu PerformHttpGet(): {exc}");

                if (response != null)
                    _logger.LogError($"ResponseApiMambu PerformHttpGet(): {await response.Content.ReadAsStringAsync()}");

                throw exc;
            }

        }

        public async Task<string> PerformBackupHttpPost()
        {
            string result = string.Empty;
            HttpResponseMessage? response = new HttpResponseMessage();

            try
            {
                string jsonBody = this._apiCallback;
                string apiKey_value = this._apiValue;
                string apiKey = this._apiKey;

                HttpClient client = _client.CreateClient();

                CustomerBackupRequest requestBody = new CustomerBackupRequest();

                requestBody.callback = this._apiCallback;
                if (!string.IsNullOrEmpty(this._tablesBackup))
                {
                    var tables = this._tablesBackup.Split(',').ToList();

                    requestBody.tables = tables;
                }

                jsonBody = JsonConvert.SerializeObject(requestBody);

                client.BaseAddress = new Uri(this._apiBase);
                client.DefaultRequestHeaders.Add(apiKey, apiKey_value);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(this._userAgent);
                client.DefaultRequestHeaders.Accept.ParseAdd(this._headerAcceptPost);

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{this._apiBase}/api/database/backup"),
                    Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
                };

                response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
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
}


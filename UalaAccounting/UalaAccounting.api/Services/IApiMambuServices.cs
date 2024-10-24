using System;
namespace UalaAccounting.api.Services
{
	public interface IApiMambuServices
	{
        void SetApiCallback(string apiCallback);
        void SetApiValue(string apiValue);
        void SetApiKey(string apiKey);
        void SetApiBase(string apiBase);
        void SetUserAgent(string userAgent);
        void SetHeaderAcceptGet(string headerAcceptGet);
        void SetHeaderAcceptPost(string headerAcceptPost);
        void SetTablesBackupk(string tablesBackup);
        Task<Stream> PerformBackupHttpGet();
        Task<string> PerformBackupHttpPost();
    }
}


using System;
namespace UalaAccounting.api.Services
{
    public interface INotificationServices
    {
        Task NotificationHttpPOST(string message, string code, string processId);
        void SetApiBase(string apiBase);
    }
}
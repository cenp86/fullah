using System;
namespace UalaAccounting.api.Services
{
    public interface INotificationServices
    {
        Task NotificationHttpPOST(string message, string code);
        void SetApiBase(string apiBase);
    }
}
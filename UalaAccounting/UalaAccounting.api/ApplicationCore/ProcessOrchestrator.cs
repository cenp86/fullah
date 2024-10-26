using System;
using Polly;
using Polly.Retry;
using UalaAccounting.api.Services;

namespace UalaAccounting.api.ApplicationCore
{
    public class ProcessOrchestration : IProcessOrchestration
    {
        private readonly ILogger<ProcessOrchestration> logger;
        private readonly IBackupProcess backupProcess;
        private readonly ILoanAccountHistoryProcess loanAccountHistoryProcess;
        private readonly IBusinessLogic businessLogic;
        private readonly IReclasificationProcess reclasificationProcess;
        private readonly INotificationServices notificationServices;
        private readonly IConfigurationData configurationData;
        public ProcessOrchestration(ILogger<ProcessOrchestration> _logger, 
        IBackupProcess _backupProcess,
        ILoanAccountHistoryProcess _loanAccountHistoryProcess,
        IBusinessLogic _businessLogic,
        IReclasificationProcess _reclasificationProcess,
        INotificationServices _notificationServices,
        IConfigurationData _configurationData)
        {
            logger = _logger;
            backupProcess = _backupProcess;
            loanAccountHistoryProcess = _loanAccountHistoryProcess;
            businessLogic = _businessLogic;
            reclasificationProcess = _reclasificationProcess;
            notificationServices = _notificationServices;
            configurationData = _configurationData;
        }


        // Methods
        public async Task Process()
        {
            var configurationList = await configurationData.GetConfigurationEnableAsync();
            var notificationUrl = configurationList.FirstOrDefault(x => x.Name == "NOTIFICATION_URL");
            
            if(configurationList!= null && notificationUrl != null && !string.IsNullOrEmpty(notificationUrl.Valueconfiguration))
            {
                notificationServices.SetApiBase(notificationUrl.Valueconfiguration);
            }

            var retryPolicy = Policy
            .Handle<Exception>() // Puedes especificar el tipo de excepción
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), 
                (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning($"{retryCount} failed. Retrying in {timeSpan.TotalSeconds} seconds...");
                });

            try
            {

            await retryPolicy.ExecuteAsync(async () => 
            {
                await backupProcess.GetAndProcessBackUp();
            });

            await retryPolicy.ExecuteAsync(async () => 
            {
                await loanAccountHistoryProcess.BuildLoanAccountHistory();
            });

            DateTime startDate = DateTime.Today.AddDays(-1); // Hoy a las 00:00:00
            DateTime endDate = DateTime.Today.AddSeconds(-1);          

            await retryPolicy.ExecuteAsync(async () => 
            {
                await businessLogic.Process(startDate, endDate);
            });   

            await retryPolicy.ExecuteAsync(async () => 
            {
                await reclasificationProcess.ExecuteReclasification(startDate, endDate);
            });     

            logger.LogInformation("Orchestration process completed successfully.");

            if(notificationUrl != null)
                await notificationServices.NotificationHttpPOST("SUCCESS", "OK");
            }
            catch(Exception ex) 
            {
                if(notificationUrl != null)
                    await notificationServices.NotificationHttpPOST("ERROR", "NOTOK");
                
                throw ex;
            }
        }
    }
}
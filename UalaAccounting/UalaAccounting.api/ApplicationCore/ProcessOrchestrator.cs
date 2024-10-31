using Polly;
using UalaAccounting.api.Services;
using UalaAccounting.api.EntitiesDB;
using System.Text.Json;

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
        private readonly IDbLogger dbLogger;

        public ProcessOrchestration(ILogger<ProcessOrchestration> _logger, 
        IBackupProcess _backupProcess,
        ILoanAccountHistoryProcess _loanAccountHistoryProcess,
        IBusinessLogic _businessLogic,
        IReclasificationProcess _reclasificationProcess,
        INotificationServices _notificationServices,
        IConfigurationData _configurationData,
        IDbLogger _dbLogger)
        {
            logger = _logger;
            backupProcess = _backupProcess;
            loanAccountHistoryProcess = _loanAccountHistoryProcess;
            businessLogic = _businessLogic;
            reclasificationProcess = _reclasificationProcess;
            notificationServices = _notificationServices;
            configurationData = _configurationData;
            dbLogger = _dbLogger;
        }

        public async Task<bool> CheckProcessInProgress()
        {
            logger.LogInformation($"Started check status Orchestrator Process");
            try
            {
                var processInProgress = await dbLogger.GetProcessInProgress();

                if(processInProgress != null && processInProgress.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                throw ex;
            }
        }

        // Methods
        public async Task Process(String processId)
        {
            DateTime? processStartDate = null;
            Configurationaccountinghub? notificationUrl = null;

            try
            {
                processStartDate = DateTime.Now;

                logger.LogInformation($"Started executing Orchestrator Process");

                await updateProcessStatus(processId, "IN_PROGRESS", "LOADING CONFIGURATION DATA", processStartDate, null, true);

                var configurationList = await configurationData.GetConfigurationEnableAsync();
                notificationUrl = configurationList.FirstOrDefault(x => x.Name == "NOTIFICATION_URL");
                
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


                await updateProcessStatus(processId, "IN_PROGRESS", "DOWNLOADING DB BACKUP", processStartDate, null, false);
                //await Task.Delay(TimeSpan.FromMinutes(2));
                await retryPolicy.ExecuteAsync(async () => 
                {
                    await backupProcess.GetAndProcessBackUp();
                });

                await updateProcessStatus(processId, "IN_PROGRESS", "BUILDING LOAN ACCOUNT HISTORIC DATA", processStartDate, null, false);
                await retryPolicy.ExecuteAsync(async () => 
                {
                    await loanAccountHistoryProcess.BuildLoanAccountHistory();
                });

                DateTime startDate = DateTime.Today.AddDays(-1); // Hoy a las 00:00:00
                DateTime endDate = DateTime.Today.AddSeconds(-1);          

                await updateProcessStatus(processId, "IN_PROGRESS", "BUILDING ACCOUNTING HUB ENTRY DATA", processStartDate, null, false);
                await retryPolicy.ExecuteAsync(async () => 
                {
                    await businessLogic.Process(startDate, endDate);
                });   

                await updateProcessStatus(processId, "IN_PROGRESS", "EXECUTING RECLASSIFICATION LOGIC", processStartDate, null, false);
                await retryPolicy.ExecuteAsync(async () => 
                {
                    await reclasificationProcess.ExecuteReclasification(startDate, endDate);
                });     

                await updateProcessStatus(processId, "COMPLETED", "PROCESS EXECUTED SUCCESFULLY", processStartDate, DateTime.Now, false);
                
                if(notificationUrl != null)
                    await notificationServices.NotificationHttpPOST("Success", "OK", processId);
            }
            catch(Exception ex) 
            {
                if(notificationUrl != null)
                    await notificationServices.NotificationHttpPOST("ERROR", "NOTOK", processId);
                
                await updateProcessStatus(processId, "FAILED", "PROCESS FAILED", processStartDate, DateTime.Now, false);
                
                logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<String> GetProcessStatus(String processId)
        {   
            String? jsonString = null;
            try
            {
                List<Accountinghubprocesscontrol> values = await dbLogger.GetOrchestrationProcessStatus(processId);

                if (values.Count > 0)
                    jsonString = JsonSerializer.Serialize(values).Replace("\"", "'");

                return jsonString;
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                throw ex;
            }

        }

        private async Task updateProcessStatus(string processId, string status, string currentStep, DateTime? startDate, DateTime? endDate, Boolean flagCreateRecord)
        {
            try
            {                
                var processStatus = new Accountinghubprocesscontrol
                {
                    Processuuid = processId,
                    Startdate = startDate,
                    Enddate = endDate,
                    Currentstep = currentStep,
                    Status = status
                };

                await dbLogger.OrchestrationProcessStatusUpdate(processStatus, flagCreateRecord);
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                throw ex;
            }
        }
    }
}
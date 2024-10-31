using System;
using UalaAccounting.api.EntitiesDB;
namespace UalaAccounting.api.Services
{
	public interface IDbLogger
	{
        Task LogActionsDbLevel(String logLine);
        Task OrchestrationProcessStatusUpdate(Accountinghubprocesscontrol log, Boolean flagCreateRecord);
        Task<List<Accountinghubprocesscontrol>> GetOrchestrationProcessStatus(String processId);
        Task<List<Accountinghubprocesscontrol>> GetProcessInProgress();
    }
}


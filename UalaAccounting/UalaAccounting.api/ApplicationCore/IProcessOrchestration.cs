using System;
using UalaAccounting.api.EntitiesDB;
namespace UalaAccounting.api.ApplicationCore
{
    public interface IProcessOrchestration 
    {        
        // Methods
        Task Process(String processid);
        Task<String> GetProcessStatus(string processId);
    }
}
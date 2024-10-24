using System;
namespace UalaAccounting.api.ApplicationCore
{
    public interface IProcessOrchestration
    {
        // Methods
        Task Process();
    }
}
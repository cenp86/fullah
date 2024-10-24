using System;
namespace UalaAccounting.api.Services
{
	public interface IFormedSqlExecution
	{
        void SetConnectionString(string connection);
        Task ExecuteDatabaseNativeCommand(string strQuery);
    }
}


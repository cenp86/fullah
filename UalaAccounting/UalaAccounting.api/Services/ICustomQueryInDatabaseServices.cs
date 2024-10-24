using System;
namespace UalaAccounting.api.Services
{
	public interface ICustomQueryInDatabaseServices
	{
        Task ExecuteDatabaseCommand(string strQuery);
    }
}


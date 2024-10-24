using System;
namespace UalaAccounting.api.Services
{
	public interface IShellServices
	{
        Task ImportRestore(string? fileForRestore);
        void SetMySqlPath(string pathOfmysql);
        Task FormatConnectionString(string connectionString = null);
    }
}


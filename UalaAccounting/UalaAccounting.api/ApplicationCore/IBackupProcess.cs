using System;
namespace UalaAccounting.api.ApplicationCore
{
	public interface IBackupProcess
	{
        Task RunImportAsync();
        Task GetAndProcessBackUp();
    }
}


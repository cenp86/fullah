using System;
namespace UalaAccounting.api.Services
{
	public interface IDbLogger
	{
        Task LogActionsDbLevel(String logLine);
    }
}


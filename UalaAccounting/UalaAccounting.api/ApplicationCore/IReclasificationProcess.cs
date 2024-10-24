using System;
namespace UalaAccounting.api.ApplicationCore
{
	public interface IReclasificationProcess
	{
        Task ExecuteReclasification(DateTime from, DateTime to);
    }
}
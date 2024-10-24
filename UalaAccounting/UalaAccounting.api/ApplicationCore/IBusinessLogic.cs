using System;

namespace UalaAccounting.api.ApplicationCore
{
	public interface IBusinessLogic
	{
        Task Process(DateTime from, DateTime to);

    }
}
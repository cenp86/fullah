using System;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public interface IAccountChartData
	{
        Task<List<string>> GetAccountChartByNameAsync();
        Task<List<Accountchart>> GetAccountChartByIdAsync(string accountChartId);
        Task SaveAccountChartFromList(List<Accountchart> list);
        Task UpdateAccountChartFromList(List<Accountchart> list);
    }
}


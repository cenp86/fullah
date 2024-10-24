using System;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public interface IProductAccountingHubData
	{
        Task<List<Productaccountinghub>> GetProducctByAccountChart(string accountChartId);
        Task UpdateProductsFromList(List<Productaccountinghub> list);
        Task SaveProductsFromList(List<Productaccountinghub> list);
    }
}


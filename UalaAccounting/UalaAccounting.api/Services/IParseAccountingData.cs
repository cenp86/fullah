using System;
namespace UalaAccounting.api.Services
{
	public interface IParseAccountingData
	{
        Task GetParseAccountingBreackdownDataAsync(DateTime from, DateTime to, string[] productKeys);
        Task GetParseAccountingGljournalentryDataAsync(DateTime from, DateTime to, string[] productKeys);
        Task DeleteAsync(DateTime from, DateTime to);
    }
}


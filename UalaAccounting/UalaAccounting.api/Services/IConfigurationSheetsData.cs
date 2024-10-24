using System;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public interface IConfigurationSheetsData 
	{
        Task<List<Configurationsheet>> GetConfigurationSheetsAsync();

    }
}


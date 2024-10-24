using System;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public interface IConfigurationSheetColumnsData
	{
        Task<List<Configurationsheetcolumn>> GetConfigurationBySheetAsync(string sheet);
        Task<List<Configurationsheetcolumn>> GetConfigurationByListSheetAsync(List<string> list);
    }
}


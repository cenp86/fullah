using System;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public interface IConfigurationData
	{
        Task<List<Configurationaccountinghub>> GetConfigurationEnableAsync();
    }
}


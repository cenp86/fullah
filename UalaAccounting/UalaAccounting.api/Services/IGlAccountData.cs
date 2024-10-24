using System;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public interface IGlAccountData
	{
        Task<List<Glaccount>> GetGlAccountByGlCodeListAsync(List<string> list);

    }
}


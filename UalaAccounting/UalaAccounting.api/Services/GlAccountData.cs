using System;
using Microsoft.EntityFrameworkCore;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public class GlAccountData : IGlAccountData
    {
        private readonly ContaContext _dbContext;
        private readonly ILogger<GlAccountData> _logger;

        public GlAccountData(ContaContext dbContext, ILogger<GlAccountData> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<Glaccount>> GetGlAccountByGlCodeListAsync(List<string> list)
        {
            try
            {
                return await _dbContext.Glaccounts.Where(x => list.Any(o => o == x.Glcode) &&  x.Activated == 1).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}


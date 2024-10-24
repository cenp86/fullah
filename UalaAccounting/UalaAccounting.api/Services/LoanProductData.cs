using System;
using Microsoft.EntityFrameworkCore;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public class LoanProductData : ILoanProductData
    {
        private readonly ContaContext _dbContext;
        private readonly ILogger<LoanProductData> _logger;

        public LoanProductData(ContaContext dbContext, ILogger<LoanProductData> logger)
		{
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<Loanproduct>> GetProductByEncodedKeyAsync(string encodedKey)
        {
            try
            {
                return await _dbContext.Loanproducts.Where(x => x.Encodedkey == encodedKey && x.Activated == 1).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Loanproduct>> GetProductByListOfEncodedKey(List<string> products)
        {
            try
            {
                return await _dbContext.Loanproducts.Where(x => products.Any(o => o == x.Encodedkey) && x.Activated == 1).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}


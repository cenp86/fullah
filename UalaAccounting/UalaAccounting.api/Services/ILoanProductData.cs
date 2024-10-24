using System;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public interface ILoanProductData
	{
        Task<List<Loanproduct>> GetProductByEncodedKeyAsync(string encodedKey);
        Task<List<Loanproduct>> GetProductByListOfEncodedKey(List<string> products);
    }
}


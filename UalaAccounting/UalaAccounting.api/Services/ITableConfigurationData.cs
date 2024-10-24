using System;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
    public interface ITableConfigurationData
    {
        Task<List<Sheetparametrization>> GetReclassificationRules();
        Task<List<Productaccountinghub>> GetProductList();
        
    }
}


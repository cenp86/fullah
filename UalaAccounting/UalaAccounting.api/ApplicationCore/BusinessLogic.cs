using System;
using UalaAccounting.api.Services;
using UalaAccounting.api.Models;

namespace UalaAccounting.api.ApplicationCore
{
    public class BusinessLogic : IBusinessLogic
    {
        private readonly ILogger<BusinessLogic> logger;
        private readonly IParseAccountingData parseAccountingData;
        private readonly TablesInMemoryModel tablesInMemoryModel;
        private readonly ITableConfigurationData tableConfigurationData;

        public BusinessLogic(ILogger<BusinessLogic> _logger, IParseAccountingData _parseAccountingData, TablesInMemoryModel _tablesInMemoryModel, ITableConfigurationData _iTableConfigurationData)
        {
            logger = _logger;
            parseAccountingData = _parseAccountingData;
            tablesInMemoryModel = _tablesInMemoryModel;
            tableConfigurationData = _iTableConfigurationData;
        }

        public async Task Process(DateTime from, DateTime to)
        {
            //string[] productKeys = ["8a5ca0548f62549b018f627490880492","8a5c85d582edddc90182f9548c355b0c"];
            try
            {
                await LoadConfigAsync();

                string[] productsArray = tablesInMemoryModel.ProductList.Select(p => p.Productencodedkey).ToArray();

                await parseAccountingData.DeleteAsync(from, to);

                var task1 = parseAccountingData.GetParseAccountingGljournalentryDataAsync(from, to, productsArray);
                var task2 = parseAccountingData.GetParseAccountingBreackdownDataAsync(from, to, productsArray);

                await Task.WhenAll(task1, task2);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw ex;
            }
        }

        private async Task LoadConfigAsync()
        {
            try
            {
                if (tablesInMemoryModel.ProductList == null)
                {
                    tablesInMemoryModel.ProductList = await tableConfigurationData.GetProductList();
                    logger.LogInformation($"Load Configuration Products count({tablesInMemoryModel.ProductList.Count})");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw ex;
            }
        }        
    }
}
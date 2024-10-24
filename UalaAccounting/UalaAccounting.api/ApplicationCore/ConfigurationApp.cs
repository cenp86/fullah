using System;
using UalaAccounting.api.EntitiesDB;
using UalaAccounting.api.Services;

namespace UalaAccounting.api.ApplicationCore
{
	public class ConfigurationApp : IConfigurationApp
    {
        private readonly ILogger<ConfigurationApp> logger;
        private readonly IDbLogger dbLogger;
        private readonly IConfigTemplateData configTemplateData;
        private readonly IAccountChartData _accountChartData;
        private readonly IProductAccountingHubData _productAccountingHubData;
        private readonly IConfigurationSheetsData _configurationSheets;
        private readonly ISheetParametrizationData _sheetParameterizationData;
        private readonly IConfigurationSheetColumnsData _configurationSheetColumnsData;

        public ConfigurationApp(ILogger<ConfigurationApp> _logger, IDbLogger _dbLogger, IConfigTemplateData _configTemplateData, IAccountChartData accountChartData, IProductAccountingHubData productAccountingHubData,
            IConfigurationSheetsData configurationSheets, ISheetParametrizationData sheetParameterizationData, IConfigurationSheetColumnsData configurationSheetColumnsData)
		{
            logger = _logger;
            configTemplateData = _configTemplateData;
            _accountChartData = accountChartData;
            _productAccountingHubData = productAccountingHubData;
            _configurationSheets = configurationSheets;
            _sheetParameterizationData = sheetParameterizationData;
            _configurationSheetColumnsData = configurationSheetColumnsData;
        }

        /// <summary>
        /// create new account chart
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task CreateConfigTemplate(IFormFile file)
        {
            try
            {
                var configuratioSheets = await _configurationSheets.GetConfigurationSheetsAsync();
                var listOfSheet = configuratioSheets.Select(x => x.Encodedkey).ToList();
                var configurationColumns = await _configurationSheetColumnsData.GetConfigurationByListSheetAsync(listOfSheet);
                await configTemplateData.CreateConfigurationTemplate(file, configuratioSheets, configurationColumns);


            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// update account chart
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task UpdateConfigTemplate(IFormFile file)
        {
            try
            {
                var configuratioSheets = await _configurationSheets.GetConfigurationSheetsAsync();
                var configurationOfAccountChart = configuratioSheets;
                var listOfSheet = configuratioSheets.Select(x => x.Encodedkey).ToList();
                var configurationColumns = await _configurationSheetColumnsData.GetConfigurationByListSheetAsync(listOfSheet);
                await configTemplateData.ProcessUpdateConfigurationTemplate(file, configurationOfAccountChart, configurationColumns);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// List of accountchart id
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetAccountChartNameAsync()
        {
            try
            {
                return await _accountChartData.GetAccountChartByNameAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// get account chart by id
        /// </summary>
        /// <param name="accountChartId"></param>
        /// <returns></returns>
        public async Task<MemoryStream> GetConfigTemplate(string accountChartId)
        {
            try
            {
                var configuratioSheets = await _configurationSheets.GetConfigurationSheetsAsync();
                var accountChartList = await _accountChartData.GetAccountChartByIdAsync(accountChartId);

                if(accountChartList.Count <= 0 )
                    throw new Exception("the account chart id doesn't exist");

                var configurationColumns = await _configurationSheetColumnsData.GetConfigurationByListSheetAsync(configuratioSheets.Select(x => x.Encodedkey).ToList());
                List<Productaccountinghub> productList;
                string accountChartIdFirstElement = string.Empty;

                if (accountChartList.Count > 0)
                {
                    accountChartIdFirstElement = accountChartList.First().Accountchartid;
                    productList = await _productAccountingHubData.GetProducctByAccountChart(accountChartIdFirstElement);
                }
                else
                {
                    throw new Exception("Error with productList");
                }

                var stagesName = configuratioSheets.Where(x => x.Reclassification == true).Select(x => x.Name).ToList();
                var stages = await _sheetParameterizationData.GetSheetbyAccountChartAsync(stagesName, accountChartIdFirstElement);

                if (stages == null)
                    throw new Exception("Error with stages configuration");

                return await configTemplateData.getConfigurationTemplate(configuratioSheets, configurationColumns, accountChartList, stagesName, stages);

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw ex;
            }
        }
    }
}


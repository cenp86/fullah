using System;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public interface ISheetParametrizationData
	{
        Task<List<Sheetparametrization>> GetSheetParamAsync(List<string> listStage);
        Task<List<Sheetparametrization>> GetSheetbyAccountChartAsync(List<string> listStage, string accountChart);
        Task<List<Sheetparametrization>> GetStagesByAccountchartId(string accountChart);
        Task UpdateStagesFromList(List<Sheetparametrization> list);
        Task SaveStagesFromList(List<Sheetparametrization> list);
    }
}


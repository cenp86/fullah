using System;
using Microsoft.EntityFrameworkCore;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public class SheetParametrizationData : ISheetParametrizationData
    {
        private readonly ContaContext _dbContext;
        private readonly ILogger<SheetParametrizationData> _logger;

        public SheetParametrizationData(ContaContext dbContext, ILogger<SheetParametrizationData> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<List<Sheetparametrization>> GetSheetParamAsync(List<string> listStage)
        {
            try
            {
                return await _dbContext.Sheetparametrizations.Where(x => listStage.Any(o => o == x.Currentstage)).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Sheetparametrization>> GetSheetbyAccountChartAsync(List<string> listStage, string accountChart)
        {
            try
            {
                return await _dbContext.Sheetparametrizations.Where(x => listStage.Any(o => o == x.Currentstage) && x.Accountchart == accountChart && x.Enable == true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Sheetparametrization>> GetStagesByAccountchartId(string accountChart)
        {
            try
            {
                return await _dbContext.Sheetparametrizations.Where(x => x.Accountchart == accountChart && x.Enable == true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateStagesFromList(List<Sheetparametrization> list)
        {
            try
            {
                foreach (var item in list)
                {
                    _dbContext.Entry(item).State = EntityState.Modified;
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task SaveStagesFromList(List<Sheetparametrization> list)
        {
            try
            {
                await _dbContext.Sheetparametrizations.AddRangeAsync(list);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}


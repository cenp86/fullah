using System;
using Microsoft.EntityFrameworkCore;
using UalaAccounting.api.EntitiesDB;
using UalaAccounting.api.Models;

namespace UalaAccounting.api.Services
{
    public class DbLogger : IDbLogger
    {
        private readonly ContaContext _dbContext;
        private readonly ILogger<DbLogger> _logger;
        private readonly IDbContextFactory<ContaContext> _contextFactory;

        public DbLogger(ContaContext dbContext, ILogger<DbLogger> logger, IDbContextFactory<ContaContext> contextFactory)
        {
            _dbContext = dbContext;
            _logger = logger;
            _contextFactory = contextFactory;
        }

        public async Task LogActionsDbLevel(String logLine)
        {
            try
            {
                using var _dbContext = _contextFactory.CreateDbContext();

                var logEntry = new Accountinghublog
                {
                    Logline = logLine,
                    Creationdate = DateTime.Now
                };

                _dbContext.Accountinghublogs.Add(logEntry);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task OrchestrationProcessStatusUpdate(Accountinghubprocesscontrol log, Boolean flagCreateRecord)
        {
            try
            {
                using var _dbContext = _contextFactory.CreateDbContext();
                
                if(flagCreateRecord)
                {
                    _dbContext.Accountinghubprocesscontrols.Add(log);
                }
                else
                {
                    _dbContext.Accountinghubprocesscontrols.Update(log);
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }            
        }

        public async Task<List<Accountinghubprocesscontrol>> GetOrchestrationProcessStatus(String processId){
            try
            {
                using var _dbContext = _contextFactory.CreateDbContext();

                return await _dbContext.Accountinghubprocesscontrols.Where(x => x.Processuuid == processId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;                
            }
        }


        public async Task<List<Accountinghubprocesscontrol>> GetProcessInProgress(){
            try
            {
                using var _dbContext = _contextFactory.CreateDbContext();

                return await _dbContext.Accountinghubprocesscontrols.Where(x => x.Enddate == null && x.Status == "IN_PROGRESS").ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;                
            }
        }
    }
}
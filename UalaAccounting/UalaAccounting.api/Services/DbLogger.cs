using System;
using Microsoft.EntityFrameworkCore;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
    public class DbLogger : IDbLogger
    {
        private readonly ContaContext _dbContext;
        private readonly ILogger<DbLogger> _logger;

        public DbLogger(ContaContext dbContext, ILogger<DbLogger> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task LogActionsDbLevel(String logLine)
        {
            try
            {
                var logEntry = new Accountinghublog
                {
                    Logline = logLine,
                    Creationdate = DateTime.Now
                };

                // Agregar la entidad al DbSet
                _dbContext.Accountinghublogs.Add(logEntry);

                // Guardar los cambios en la base de datos
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }
    }
}


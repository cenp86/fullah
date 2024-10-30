using System;
using System.Data;
using System.Security.Principal;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using UalaAccounting.api.EntitiesDB;
using MySql.Data;

namespace UalaAccounting.api.Services
{
    public class ParseAccountingData : IParseAccountingData
    {
        private readonly ContaContext _dbContext;
        private readonly ILogger<ParseAccountingData> _logger;
        private readonly IDbContextFactory<ContaContext> _contextFactory;

        public ParseAccountingData(ContaContext dbContext, ILogger<ParseAccountingData> logger, IDbContextFactory<ContaContext> contextFactory)
        {
            _dbContext = dbContext;
            _logger = logger;
            _contextFactory = contextFactory;
        }

        public async Task GetParseAccountingGljournalentryDataAsync(DateTime from, DateTime to, string[] productKeys)
        {
            using var _dbContext = _contextFactory.CreateDbContext();

            try
            {
                if(productKeys.Length > 0){
                    string sql = @"INSERT INTO accountinghubentry
                                SELECT 
                                    la.ID AS 'LOANID',
                                    lp.ENCODEDKEY AS 'PRODUCTENCODEDKEY',
                                    b.NAME AS 'BRANCHNAME',
                                    cfv_actual.VALUE AS 'ACTUALSTAGE',
                                    cfv_last.VALUE AS 'LASTSTAGECHANGE',
                                    ga.GLCODE,
                                    ga.name AS 'GLNAME',
                                    gje.TRANSACTIONID,
                                    gje.ENTRYID,
                                    ROUND(gje.AMOUNT, 2) AS 'AMOUNT',
                                    gje.CREATIONDATE,
                                    gje.entrydate AS 'BOOKINGDATE',
                                    gje.TYPE,
                                    lt.TYPE AS 'LOANTRANSACTIONTYPE',
                                    IFNULL((lh.PRINCIPALDUE),0) 'PRINCIPALDUE',
                                    IFNULL((lh.PRINCIPALBALANCE),0) 'PRINCIPALBALANCE',
                                    rv.TRANSACTIONID 'REVERSALTRANSACTIONID',
                                    CASE WHEN cftx.VALUE = 'True' THEN true ELSE false END 'IS_PAYOFF',
                                    CASE WHEN IFNULL((lh.PRINCIPALDUE),0) > 0 THEN true ELSE false END 'IS_PREPAYMENT',
                                    CASE WHEN ISNULL(lt.USERKEY) THEN true ELSE false END 'IS_OVERDUE',
                                    tc.NAME as 'TRANSACTIONCHANNEL'
                                FROM 
                                    conta.gljournalentry gje
                                LEFT JOIN
                                    conta.branch b ON b.ENCODEDKEY = gje.ASSIGNEDBRANCHKEY
                                INNER JOIN 
                                    conta.glaccount ga ON ga.ENCODEDKEY = gje.GLACCOUNT_ENCODEDKEY_OID
                                INNER JOIN 
                                    conta.loanaccount la ON la.ENCODEDKEY = gje.ACCOUNTKEY
                                INNER JOIN 
                                    conta.loantransaction lt ON gje.TRANSACTIONID = lt.transactionid
                                LEFT JOIN 
                                    conta.loantransaction rv ON lt.TYPE like '%ADJUSTMENT%' AND rv.REVERSALTRANSACTIONKEY = lt.ENCODEDKEY
                                INNER JOIN 
                                    loanproduct lp ON la.PRODUCTTYPEKEY = lp.ENCODEDKEY
                                LEFT JOIN 
                                    customfieldvalue cfv_actual ON cfv_actual.PARENTKEY = la.ENCODEDKEY AND cfv_actual.CUSTOMFIELDKEY = (SELECT ENCODEDKEY FROM customfield WHERE ID = '_ACTUAL_STAGE')
                                LEFT JOIN 
                                    customfieldvalue cfv_last ON cfv_last.PARENTKEY = la.ENCODEDKEY AND cfv_last.CUSTOMFIELDKEY = (SELECT ENCODEDKEY FROM customfield WHERE ID = '_LAST_STAGE_CHANGE')
                                LEFT JOIN 
	                                customfieldvalue cftx ON cftx.PARENTKEY = lt.ENCODEDKEY AND cftx.CUSTOMFIELDKEY = (SELECT ENCODEDKEY FROM customfield WHERE ID = 'Is_PayOff')    
                                LEFT JOIN 
                                    loanaccounthistory lh ON lh.ENCODEDKEY = la.ENCODEDKEY AND lh.SNAPSHOTDATE = DATE(gje.CREATIONDATE)
                                LEFT JOIN 
                                    transactiondetails td on lt.DETAILS_ENCODEDKEY_OID = td.ENCODEDKEY
                                LEFT JOIN 
                                    transactionchannel tc on td.TRANSACTIONCHANNELKEY = tc.ENCODEDKEY                                    
                                WHERE 
                                    gje.CREATIONDATE >= @from AND gje.CREATIONDATE < @to AND gje.PRODUCTKEY IN (   
                                ";

                    // Add parameters for product keys
                    for (int i = 0; i < productKeys.Length; i++)
                    {
                        if (i > 0) sql += ", ";
                        sql += $"@productKey{i}";
                    }

                    sql += ");";

                    // Prepare the parameters
                    var parameters = new List<MySqlParameter>
                    {
                        new MySqlParameter("from", SqlDbType.DateTime) { Value = from },
                        new MySqlParameter("to", SqlDbType.DateTime) { Value = to }
                    };

                    // Add product key parameters
                    for (int i = 0; i < productKeys.Length; i++)
                    {
                        parameters.Add(new MySqlParameter($"productKey{i}", SqlDbType.VarChar) { Value = productKeys[i] });
                    }

                    var result = _dbContext.Database.ExecuteSqlRaw(sql, parameters);
                    _logger.LogInformation("amount of rows:" + result.ToString());   
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task GetParseAccountingBreackdownDataAsync(DateTime from, DateTime to, string[] productKeys)
        {
            using var _dbContext = _contextFactory.CreateDbContext();
            
            try
            {
                if(productKeys.Length > 0){
                    string sql = @"INSERT INTO accountinghubentry
                                SELECT 
                                    acc.ID AS 'ACCOUNTID',
                                    lp.ENCODEDKEY AS 'PRODUCTENCODEDKEY',
                                    b.NAME,
                                    cfv_actual.VALUE AS 'ACTUALSTAGE',
                                    cfv_last.VALUE AS 'LASTSTAGECHANGE',
                                    ga.GLCODE,
                                    ga.name AS 'GLNAME',
                                    aib.TRANSACTIONID,
                                    aib.ENTRYID,
                                    ROUND(aib.AMOUNT, 2) AS 'AMOUNT',
                                    aib.CREATIONDATE,
                                    aib.BOOKINGDATE,
                                    aib.ENTRYTYPE,
                                    'ACCRUED_INTEREST',
                                    0 'PRINCIPALDUE',
                                    0 'PRINCIPALBALANCE',
                                    NULL 'REVERSALTRANSACTIONID',
                                    0 'IS_PAYOFF',
                                    0 'IS_PREPAYMENT',
                                    0 'IS_OVERDUE',
                                    NULL 'TRANSACTIONCHANNEL'
                                FROM 
                                    conta.accountinginterestaccrualbreakdown aib
                                LEFT JOIN 
                                    conta.branch b ON b.ENCODEDKEY = aib.BRANCHENCODEDKEY
                                INNER JOIN 
                                    conta.glaccount ga ON ga.ENCODEDKEY = aib.GLACCOUNTENCODEDKEY
                                INNER JOIN 
                                    conta.loanaccount acc ON acc.ID = aib.ACCOUNTID
                                INNER JOIN 
                                    loanproduct lp ON acc.PRODUCTTYPEKEY = lp.ENCODEDKEY
                                LEFT JOIN 
                                    customfieldvalue cfv_actual ON cfv_actual.PARENTKEY = acc.ENCODEDKEY AND cfv_actual.CUSTOMFIELDKEY = (SELECT ENCODEDKEY FROM customfield WHERE ID = '_ACTUAL_STAGE')
                                LEFT JOIN 
                                    customfieldvalue cfv_last ON cfv_last.PARENTKEY = acc.ENCODEDKEY AND cfv_last.CUSTOMFIELDKEY = (SELECT ENCODEDKEY FROM customfield WHERE ID = '_LAST_STAGE_CHANGE')
                                WHERE 
                                    aib.CREATIONDATE >= @from AND aib.CREATIONDATE < @to AND PRODUCTENCODEDKEY IN (
                                ";

                    // Add parameters for product keys
                    for (int i = 0; i < productKeys.Length; i++)
                    {
                        if (i > 0) sql += ", ";
                        sql += $"@productKey{i}";
                    }

                    sql += ");";

                    // Prepare the parameters
                    var parameters = new List<MySqlParameter>
                    {
                        new MySqlParameter("from", SqlDbType.DateTime) { Value = from },
                        new MySqlParameter("to", SqlDbType.DateTime) { Value = to }
                    };

                    // Add product key parameters
                    for (int i = 0; i < productKeys.Length; i++)
                    {
                        parameters.Add(new MySqlParameter($"productKey{i}", SqlDbType.VarChar) { Value = productKeys[i] });
                    }

                    var result = _dbContext.Database.ExecuteSqlRaw(sql, parameters);
                    _logger.LogInformation("amount of rows:" + result.ToString());
                }                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task DeleteAsync(DateTime from, DateTime to)
        {
            using var _dbContext = _contextFactory.CreateDbContext();
            
            try
            {
                var originalEntriesToDelete = await _dbContext.Accountinghubentries.Where(x => x.Creationdate >= from && x.Creationdate < to).ToListAsync();

                _dbContext.Accountinghubentries.RemoveRange(originalEntriesToDelete);

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
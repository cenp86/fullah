using System;
using Microsoft.EntityFrameworkCore;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.SS.Formula.Functions;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
    public class ParseLoanAccountHistoryData : IParseLoanAccountHistoryData
    {
        private readonly ContaContext _dbContext;
        private readonly ILogger<ParseAccountingData> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IDbContextFactory<ContaContext> _contextFactory;

        public ParseLoanAccountHistoryData(ContaContext dbContext, ILogger<ParseAccountingData> logger, IServiceScopeFactory serviceScopeFactory, IDbContextFactory<ContaContext> contextFactory)
        {
            _dbContext = dbContext;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _contextFactory = contextFactory;
        }

        public async Task GetParseLoanAccountHistoryDataAsync(DateOnly today)
        {
            try
            {
                _logger.LogInformation($"Get Loan Account from date {today}");
                
                // using var scope = _serviceScopeFactory.CreateAsyncScope();                   
                // var _dbContext = scope.ServiceProvider.GetRequiredService<ContaContext>(); 
                using var _dbContext = _contextFactory.CreateDbContext();

                //TODO VALIDAR SI SE NECESITAN PASAR TODAS LAS CUENTAS O SOLO LAS ACTIVAS Y EN ATRASO
                var loanAccountRecords = _dbContext.Loanaccounts.ToList();
                var loanAccountHistoryRecords = new List<Loanaccounthistory>();

                foreach (var item in loanAccountRecords)
                {
                    loanAccountHistoryRecords.Add(new Loanaccounthistory
                    {
                        Encodedkey = item.Encodedkey,
                        Accountholderkey = item.Accountholderkey,
                        Accountholdertype = item.Accountholdertype,
                        Accountstate = item.Accountstate,
                        Accountsubstate = item.Accountsubstate,
                        Rescheduledaccountkey = item.Rescheduledaccountkey,
                        Assignedbranchkey = item.Assignedbranchkey,
                        Assigneduserkey = item.Assigneduserkey,
                        Closeddate = item.Closeddate,
                        Lastlockeddate = item.Lastlockeddate,
                        Creationdate = item.Creationdate,
                        Approveddate = item.Approveddate,
                        Feesdue = item.Feesdue,
                        Feespaid = item.Feespaid,
                        Graceperiod = item.Graceperiod,
                        Graceperiodtype = item.Graceperiodtype,
                        Id = item.Id,
                        Interestcalculationmethod = item.Interestcalculationmethod,
                        Interesttype = item.Interesttype,
                        Repaymentschedulemethod = item.Repaymentschedulemethod,
                        Interestapplicationmethod = item.Interestapplicationmethod,
                        Paymentmethod = item.Paymentmethod,
                        Interestchargefrequency = item.Interestchargefrequency,
                        Interestbalance = item.Interestbalance,
                        Interestpaid = item.Interestpaid,
                        Interestrate = item.Interestrate,
                        Lastmodifieddate = item.Lastmodifieddate,
                        Lastsettoarrearsdate = item.Lastsettoarrearsdate,
                        Loanamount = item.Loanamount,
                        Periodicpayment = item.Periodicpayment,
                        LoangroupEncodedkeyOid = item.LoangroupEncodedkeyOid,
                        Loanname = item.Loanname,
                        Notes = item.Notes,
                        Penaltydue = item.Penaltydue,
                        Penaltypaid = item.Penaltypaid,
                        Principalbalance = item.Principalbalance,
                        Principalpaid = item.Principalpaid,
                        Producttypekey = item.Producttypekey,
                        Repaymentinstallments = item.Repaymentinstallments,
                        Repaymentperiodcount = item.Repaymentperiodcount,
                        Repaymentperiodunit = item.Repaymentperiodunit,
                        AccountsIntegerIdx = item.AccountsIntegerIdx,
                        Migrationeventkey = item.Migrationeventkey,
                        Assignedcentrekey = item.Assignedcentrekey,
                        Lastaccountappraisaldate = item.Lastaccountappraisaldate,
                        Principalrepaymentinterval = item.Principalrepaymentinterval,
                        Principaldue = item.Principaldue,
                        Interestdue = item.Interestdue,
                        Lastinterestreviewdate = item.Lastinterestreviewdate,
                        Accruelateinterest = item.Accruelateinterest,
                        Interestspread = item.Interestspread,
                        Interestratesource = item.Interestratesource,
                        Interestratereviewunit = item.Interestratereviewunit,
                        Interestratereviewcount = item.Interestratereviewcount,
                        Accruedinterest = item.Accruedinterest,
                        Lastinterestapplieddate = item.Lastinterestapplieddate,
                        Feesbalance = item.Feesbalance,
                        Penaltybalance = item.Penaltybalance,
                        Scheduleduedatesmethod = item.Scheduleduedatesmethod,
                        Hascustomschedule = item.Hascustomschedule,
                        Fixeddaysofmonth = item.Fixeddaysofmonth,
                        Shortmonthhandlingmethod = item.Shortmonthhandlingmethod,
                        Taxrate = item.Taxrate,
                        Lasttaxratereviewdate = item.Lasttaxratereviewdate,
                        Penaltyrate = item.Penaltyrate,
                        Loanpenaltycalculationmethod = item.Loanpenaltycalculationmethod,
                        Accruedpenalty = item.Accruedpenalty,
                        Activationtransactionkey = item.Activationtransactionkey,
                        Lineofcreditkey = item.Lineofcreditkey,
                        Lockedoperations = item.Lockedoperations,
                        Interestcommission = item.Interestcommission,
                        Defaultfirstrepaymentduedateoffset = item.Defaultfirstrepaymentduedateoffset,
                        Principalpaymentsettingskey = item.Principalpaymentsettingskey,
                        Interestbalancecalculationmethod = item.Interestbalancecalculationmethod,
                        Disbursementdetailskey = item.Disbursementdetailskey,
                        Arrearstoleranceperiod = item.Arrearstoleranceperiod,
                        Accrueinterestaftermaturity = item.Accrueinterestaftermaturity,
                        Prepaymentrecalculationmethod = item.Prepaymentrecalculationmethod,
                        Principalpaidinstallmentstatus = item.Principalpaidinstallmentstatus,
                        Elementsrecalculationmethod = item.Elementsrecalculationmethod,
                        Latepaymentsrecalculationmethod = item.Latepaymentsrecalculationmethod,
                        Applyinterestonprepaymentmethod = item.Applyinterestonprepaymentmethod,
                        Allowoffset = item.Allowoffset,
                        Futurepaymentsacceptance = item.Futurepaymentsacceptance,
                        Redrawbalance = item.Redrawbalance,
                        Prepaymentacceptance = item.Prepaymentacceptance,
                        Interestfromarrearsaccrued = item.Interestfromarrearsaccrued,
                        Interestfromarrearsdue = item.Interestfromarrearsdue,
                        Interestfromarrearspaid = item.Interestfromarrearspaid,
                        Interestfromarrearsbalance = item.Interestfromarrearsbalance,
                        Interestroundingversion = item.Interestroundingversion,
                        Accountarrearssettingskey = item.Accountarrearssettingskey,
                        Holdbalance = item.Holdbalance,
                        Snapshotdate = today
                    });
                }

                _logger.LogInformation($"Load Loan Account History Entries({loanAccountHistoryRecords.Count})");

                await _dbContext.Loanaccounthistories.AddRangeAsync(loanAccountHistoryRecords);

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task DeleteLoanAccountHistoryRecords(DateOnly today)
        {
            try
            {
                _logger.LogInformation($"Executing DeleteLoanAccountHistoryRecords....");
                               
                using var _dbContext = _contextFactory.CreateDbContext();
                
                var recordsToDelete = await _dbContext.Loanaccounthistories.Where(x => x.Snapshotdate == today).ToListAsync();

                _dbContext.Loanaccounthistories.RemoveRange(recordsToDelete);

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
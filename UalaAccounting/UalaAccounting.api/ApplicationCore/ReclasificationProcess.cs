using Microsoft.AspNetCore.Mvc.ViewFeatures;
using UalaAccounting.api.EntitiesDB;
using UalaAccounting.api.Models;
using UalaAccounting.api.Services;

namespace UalaAccounting.api.ApplicationCore
{
    public class ReclasificationProcess : IReclasificationProcess
    {
        private readonly ILogger<BusinessLogic> logger;
        private readonly TablesInMemoryModel tablesInMemoryModel;
        private readonly ITableConfigurationData tableConfigurationData;
        private readonly IAccountingEntriesData accountingEntriesData;

        public ReclasificationProcess(ILogger<BusinessLogic> _logger, TablesInMemoryModel _tablesInMemoryModel, ITableConfigurationData _iTableConfigurationData, IAccountingEntriesData _iAccountingEntriesData)
        {
            logger = _logger;
            tablesInMemoryModel = _tablesInMemoryModel;
            tableConfigurationData = _iTableConfigurationData;
            accountingEntriesData = _iAccountingEntriesData;
        }

        public async Task ExecuteReclasification(DateTime from, DateTime to)
        {
            try
            {
                await LoadConfigAsync();

                await accountingEntriesData.DeleteNewEntriesAsync(from, to);

                //RECLASIFICACION DE ASIENTOS Y DEVENGOS EXCEPTUANDO AJUSTES
                List<Accountinghubentry> accountingEntriesList = await accountingEntriesData.GetAccountingEntriesAsync(from, to);
                logger.LogInformation($"Load Accounting Entries({accountingEntriesList.Count})");
                await journalEntriesReclassification(accountingEntriesList);

                //RECLASIFICACION DE AJUSTESS
                List<Accountinghubentry> adjustmentEntriesList = await accountingEntriesData.GetAdjustmentEntriesAsync(from, to);
                logger.LogInformation($"Load Adjustments Entries({adjustmentEntriesList.Count})");
                await adjustmentsReclassification(adjustmentEntriesList);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw ex;
            }
        }

        private async Task journalEntriesReclassification(List<Accountinghubentry> accountingEntriesList)
        {
            List<Accountinghubexit> newAccountingEntriesList = new();

            try
            {
                var reclassificationLogicList = tablesInMemoryModel.reclassificationRuleList;

                foreach (var item in accountingEntriesList)
                {
                    var sendOriginalEntry = true;
                    var sendNewEntry = true;
                    var newAmount = item.Amount;

                    var applicableRules = reclassificationLogicList.Where(rule =>
                        (item.Branchname == rule.Currentstage && item.Loantransactiontype == rule.Currentloantrxtype && 
                        (item.Glcode == rule.Currentglcode && rule.Currentjetype == String.Empty ||
                        item.Type == rule.Currentjetype && rule.Currentglcode == String.Empty ||
                        item.Type == rule.Currentjetype && item.Glcode == rule.Currentglcode))).ToList();

                    foreach (var rule in applicableRules)
                    {
                        Accountinghubexit newEntry = await buildNewAccountingEntry(item, newAmount);
                        newEntry.Mambuglcode = item.Glcode;
                        newEntry.Glcode = rule.Outputglcode;
                        newEntry.Glname = rule.Outputglname;
                        newEntry.Type = rule.Outputjetype != null ? rule.Outputjetype : item.Type;
                        sendOriginalEntry = false;

                        // if(item.Loanid=="SOOR9961" && item.Loantransactiontype=="WRITE_OFF" && item.Principaldue==0)   
                        // {
                        //     Console.WriteLine("ENTRE");
                        // }
                                                    
                        //CONDICION PARA GENERAR SEGUNDO ASIENTO POR CONCEPTO DE IVA -- SE DEBE GENERAR PARAMETRO PARA EL VALOR DEL IVA TODO
                        if (rule.Taxentry)
                        {
                            newEntry.Amount = item.Amount * (decimal)0.16;
                        }
                        
                        //CONDICION PARA GENERAR SEGUNDO ASIENTO POR CAPITAL EXIGIBLE
                        else if (rule.Principal)
                        {                     
                            if(item.Principaldue > 0){
                                //PARA WRITEOFF
                                if(item.Loantransactiontype != "REPAYMENT")
                                    newAmount = (decimal)(item.Pricinpalbalance - item.Principaldue);
                                else if(item.Loantransactiontype == "REPAYMENT"){
                                    newAmount = item.Amount;
                                }                                    
                                else
                                    newAmount = (decimal)(item.Amount - item.Principaldue); 

                                //newEntry.Amount = item.Principaldue;

                                if(newAmount > 0)
                                    sendOriginalEntry = true;                                    
                            }
                            else{
                                sendOriginalEntry = true;
                                sendNewEntry = false;   
                            }                                                         
                        }                        

                        //CONDICION PARA GENERAR SEGUNDO ASIENTO POR CAPITAL EXIGIBLE EN VENCIMIENTO DE CUOTA
                        else if (rule.Overdueppal && item.Loantransactiontype == "INTEREST_APPLIED" && item.Principaldue > 0 && item.IsOverdue)
                        {
                            newEntry.Amount = item.Principaldue;
                        }

                        //CONDONACIONES VALIDACION DE CANAL TRANSACCIONAL
                        //else if(item.tra)

                        if(item.Loantransactiontype == "PENALTIES_DUE_REDUCED")
                        {
                            if(newEntry.Glcode == "2401070100000004" || newEntry.Glcode == "5105870201220000")
                            {
                                Accountinghubexit newEntry2 = await buildNewAccountingEntry(item, item.Amount);
                                newEntry2.Glcode = newEntry.Glcode == "2401070100000004" ? "1302080221060000" : "1302080221050000";
                                newEntry2.Glname = newEntry.Glcode == "2401070100000004" ? "IVA A COBRAR DE CARTERA DE CREDITO" : "PERSONAL NO REVOLVENTE INTERES MORATORIOS EXIGIBLE ETAPA 1";
                                newEntry2.Type = "CREDIT";
                                newAccountingEntriesList.Add(newEntry2);
                            }                            
                        }

                        //GENERA SEGUNDO - PARA ASIENTO CAPITAL EXIGIBLE EN LA TABLA LOANACCOUNTHISTORY DEBE HABER PRINCIPALDUE
                        if ((!rule.Overdueppal && !rule.Adjust && sendNewEntry) || (rule.Overdueppal && item.IsOverdue && item.Principaldue > 0))
                        {
                            newAccountingEntriesList.Add(newEntry);
                        }
                    }

                    if (sendOriginalEntry)
                    {
                        newAccountingEntriesList.Add(await buildNewAccountingEntry(item, newAmount));
                    }

                    applicableRules.Clear();
                }

                logger.LogInformation($"New Accounting Entries List({newAccountingEntriesList.Count})");

                await accountingEntriesData.AccountingEntriesInsertBatchAsync(newAccountingEntriesList);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw ex;
            }
        }

        private async Task adjustmentsReclassification(List<Accountinghubentry> adjustmentEntriesList)
        {
            List<Accountinghubexit> newAdjustmentEntriesList = new();

            try
            {
                var reclassificationLogicList = tablesInMemoryModel.reclassificationRuleList;

                foreach (var item in adjustmentEntriesList)
                {
                    List<Accountinghubexit> originalEntries = await accountingEntriesData.getOriginalEntriesFromAdjustedAsync(item.Reversaltransactionid, item.Glcode);

                    foreach (var originalEntry in originalEntries)
                    {
                        originalEntry.Type = originalEntry.Type == "DEBIT" ? "CREDIT" : "DEBIT";
                        originalEntry.Loantransactiontype = item.Loantransactiontype;
                        originalEntry.Entryidrev = originalEntry.Entryid;
                        originalEntry.Transactionidrev = originalEntry.Transactionid;
                        originalEntry.Entryid = item.Entryid;
                        originalEntry.Transactionid = item.Transactionid;
                        originalEntry.Creationdate = item.Creationdate;
                        originalEntry.BookingDate = item.BookingDate;

                        newAdjustmentEntriesList.Add(originalEntry);
                    }
                }

                logger.LogInformation($"New Adjustment Entries List({newAdjustmentEntriesList.Count})");

                await accountingEntriesData.AccountingEntriesInsertBatchAsync(newAdjustmentEntriesList);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw ex;
            }
        }

        private async Task<Accountinghubexit> buildNewAccountingEntry(Accountinghubentry item, decimal newAmount)
        {
            Accountinghubexit newEntry = new Accountinghubexit();

            try
            {
                newEntry.Actualstage = item.Actualstage;
                newEntry.Amount = newAmount;
                newEntry.BookingDate = item.BookingDate;
                newEntry.Branchname = item.Branchname;
                newEntry.Creationdate = item.Creationdate;
                newEntry.Entryid = item.Entryid;
                newEntry.Glcode = item.Glcode;
                newEntry.Glname = item.Glname;
                newEntry.Laststagechange = item.Laststagechange;
                newEntry.Loanid = item.Loanid;
                newEntry.Loantransactiontype = item.Loantransactiontype;
                newEntry.Productencodedkey = item.Productencodedkey;
                newEntry.Transactionid = item.Transactionid;
                newEntry.Type = item.Type;
                newEntry.Transactionidrev = String.Empty;
                newEntry.Entryidrev = 0;
                newEntry.Mambuglcode = item.Glcode;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw ex;
            }

            return newEntry;
        }

        private async Task LoadConfigAsync()
        {
            try
            {
                if (tablesInMemoryModel.reclassificationRuleList == null)
                {
                    tablesInMemoryModel.reclassificationRuleList = await tableConfigurationData.GetReclassificationRules();
                    logger.LogInformation($"Load Configuration({tablesInMemoryModel.reclassificationRuleList.Count})");
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
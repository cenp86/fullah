using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NPOI.Util;
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
        private readonly IParseLoanAccountHistoryData parseLoanAccountHistoryData;
        private readonly IConfigurationData configurationData;
        private readonly IDbLogger dbLogger;

        public ReclasificationProcess(ILogger<BusinessLogic> _logger, TablesInMemoryModel _tablesInMemoryModel, ITableConfigurationData _iTableConfigurationData, IAccountingEntriesData _iAccountingEntriesData, IParseLoanAccountHistoryData _parseLoanAccountHistoryData, IConfigurationData _configurationData, IDbLogger _dbLogger)
        {
            logger = _logger;
            tablesInMemoryModel = _tablesInMemoryModel;
            tableConfigurationData = _iTableConfigurationData;
            accountingEntriesData = _iAccountingEntriesData;
            parseLoanAccountHistoryData = _parseLoanAccountHistoryData;
            configurationData = _configurationData;
            dbLogger = _dbLogger;
        }

        public async Task ExecuteReclasification(DateTime from, DateTime to)
        {
            try
            {
                await LoadConfigAsync();

                //ELIMINAR DATA DE LAS TABLAS ACCOUNTINGHUBENTRY Y ACCOUNTINGBALANCESTAGE3                
                await accountingEntriesData.DeleteNewEntriesAsync(from, to);
                await accountingEntriesData.DeleteBalancesEntriesAsync(from, to);

                //RECLASIFICACION DE ASIENTOS Y DEVENGOS EXCEPTUANDO AJUSTES
                List<Accountinghubentry> accountingEntriesList = await accountingEntriesData.GetAccountingEntriesAsync(from, to);
                logger.LogInformation($"Load Accounting Entries({accountingEntriesList.Count})");
                await journalEntriesReclassification(accountingEntriesList);

                //RECLASIFICACION DE AJUSTES
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
            var validBalanceS3values = new[] {
                "InterestS3",
                "InterestMA",
                "PenaltyS3",
                "PenaltyMA"
            }; 

            List<Accountinghubexit> newAccountingEntriesList = new();
            List<Accountingbalancestage3> newBalanceEntriesList = new();
            List<Accountinghubexit> orderAccountBalanceModelList = new();            
            List<String> orderAccountSource = new List<String>();
            List<OrderAccountModel> orderAccountDetails = new List<OrderAccountModel>();
            Boolean orderAccountLogic=false;

            try
            {
                var configurationList = await configurationData.GetConfigurationEnableAsync();

                if(configurationList!= null)
                {                    
                    var orderAccountLogicConfig = configurationList.FirstOrDefault(x => x.Name == "ORDER_ACCOUNT_LOGIC");
                    var orderAccountSourceConfig = configurationList.FirstOrDefault(x => x.Name == "ORDER_ACCOUNT_SOURCE");
                    var orderAccountSourceDetails = configurationList.FirstOrDefault(x => x.Name == "ORDER_ACCOUNT_DETAILS_JSON");                

                    if(orderAccountLogicConfig != null && orderAccountSourceConfig != null && orderAccountSourceDetails != null && IsValidJson(orderAccountSourceDetails.Valueconfiguration))
                    {
                        orderAccountLogic = orderAccountLogicConfig.Enable ?? false;
                        orderAccountSource = (orderAccountSourceConfig?.Valueconfiguration ?? string.Empty).Split(',').ToList();

                        orderAccountDetails = JsonSerializer.Deserialize<List<OrderAccountModel>>(orderAccountSourceDetails.Valueconfiguration);

                        logger.LogInformation($"Order Account Logic parameters configured correctly.");
                    }
                    else
                    {
                        logger.LogError($"Order Account Logic parameters not configured correctly. Please review them on the table configurationaccountinghub.");
                    }
                }

                var reclassificationLogicList = tablesInMemoryModel.reclassificationRuleList;

                foreach (var item in accountingEntriesList)
                {                    
                    try
                    {
                        var storeMambuOriginalEntry = true; //FLAG PARA CONTROLAR EL ALMACENAMIENTO DEL ASIENTO NATIVO DE MAMBU
                        var storeNewEntry = true; //FLAG PARA CONTROLAR EL ALMACENAMIENTO DEL ASIENTO GENERADO POR EL AH
                        var newAmount = item.Amount; //VARIABLE AUXILIAR PARA MODIFICAR EL MONTO DEL ASIENTO NATIVO DE MAMBU EN CASO DE SER NECESARIO

                        //LOGICA PARA CALCULAR EN MEMORIA EL MONTO A POSTEAR PARA LAS CUENTAS DE ORDEN
                        if(item.Loantransactiontype == "WRITE_OFF" && orderAccountLogic)
                        {                            
                            var existingItem = orderAccountBalanceModelList.FirstOrDefault(o => o.Loanid == item.Loanid);
                                                                                    
                            if(orderAccountSource.Contains(item.Glcode))
                            {                               
                                if (existingItem == null)
                                {
                                    Accountinghubexit newEntry = await buildNewAccountingEntry(item, item.Amount); 
                                    var glAccountDetails = orderAccountDetails.FirstOrDefault(o => o.type == item.Type);

                                    newEntry.Glcode = glAccountDetails.glAccountCode;
                                    newEntry.Glname = glAccountDetails.glAccountName;    

                                    orderAccountBalanceModelList.Add(newEntry);
                                } 
                                else
                                {
                                    existingItem.Amount += item.Amount;                                                                   
                                }   
                            }                               
                        }  

                        //OBTENER REGLAS QUE APLICAN PARA EL ASIENTO
                        var applicableRules = reclassificationLogicList.Where(rule =>
                            item.Branchname == rule.Currentstage && item.Loantransactiontype == rule.Currentloantrxtype && 
                            (item.Glcode == rule.Currentglcode && String.IsNullOrEmpty(rule.Currentjetype) ||
                            item.Type == rule.Currentjetype && String.IsNullOrEmpty(rule.Currentglcode) ||
                            item.Type == rule.Currentjetype && item.Glcode == rule.Currentglcode)).ToList();                    

                        foreach (var rule in applicableRules)
                        {
                            Accountinghubexit newEntry = await buildNewAccountingEntry(item, newAmount);
                            newEntry.Mambuglcode = item.Glcode;
                            newEntry.Glcode = rule.Outputglcode;
                            newEntry.Glname = rule.Outputglname;
                            newEntry.Type = rule.Outputjetype != null ? rule.Outputjetype : item.Type;
                            storeMambuOriginalEntry = false;
                            
                            //CONDICION PARA GENERAR SEGUNDO ASIENTO POR CONCEPTO DE IVA
                            if (rule.Taxentry)
                            {
                                //TODO INCLUIR LOGICA PARA STAGE 3 - LINEA 635 SP
                                newEntry.Amount = item.Amount * (item.Taxrate/100);

                                if(item.Branchname == "STAGE1" )
                                    storeMambuOriginalEntry = true;
                            }
                            
                            //CONDICION PARA APLICAR LOGICA DE CAPITAL EXIGIBLE Y NO EXIGIBLE
                            else if (rule.Principal)
                            {              
                                if(item.Loantransactiontype == "REPAYMENT" || item.Loantransactiontype == "BRANCH_CHANGED")
                                {
                                    if(item.Principaldue > 0)
                                    {
                                        if(item.Amount - item.Principaldue <= 0)
                                        {
                                            newEntry.Amount = item.Amount;
                                        }
                                        else if(item.Amount - item.Principaldue > 0)
                                        {
                                            newEntry.Amount = item.Principaldue;
                                            newAmount = (decimal)(item.Amount - item.Principaldue);
                                            storeMambuOriginalEntry = true;
                                        }
                                    }
                                    else                          
                                    {                                    
                                        storeMambuOriginalEntry = true;
                                        storeNewEntry = false;
                                    }
                                }
                                else if(item.Loantransactiontype == "WRITE_OFF")
                                {
                                    if(item.Principaldue > 0)
                                    {
                                        newAmount = (decimal)(item.Principalbalance - item.Principaldue);
                                        newEntry.Amount = item.Principaldue;
                                        storeMambuOriginalEntry = true;
                                    }
                                    else{
                                        storeMambuOriginalEntry = true;
                                        storeNewEntry = false;
                                    }
                                }                                                     
                            }
                                                                        
                            //CONDICION PARA LOGICA DE EXIGIBILIDAD DE CAPITAL AL VENCIMIENTO DE CUOTA
                            else if (rule.Overdueppal && item.Loantransactiontype == "INTEREST_APPLIED" && item.Principaldue > 0 && item.IsOverdue)
                            {
                                Loanaccounthistory previousDayRecord = await parseLoanAccountHistoryData.GetLoanAccountHistoryRecord(item.Loanid, DateOnly.FromDateTime(item.Creationdate).AddDays(-1));
                                newEntry.Amount = previousDayRecord != null ? item.Principaldue - previousDayRecord.Principaldue + item.Principalpaid - previousDayRecord.Principalpaid : item.Principaldue - item.Principalpaid;                           
                            }
                            //LOGICA PARA ALMACENAR SALDOS STAGE 3 - PRUEBAS
                            if (!String.IsNullOrEmpty(rule.Observaciones) && rule.Observaciones.Contains(","))
                            {             
                                List<string> ruleOptions = new List<string>(rule.Observaciones.Split(","));                    
                                string entryAmountType = ruleOptions[0]; // "0 monto del asiento, 1 monto reverso"
                                string amountToStore = ruleOptions[1];

                                if(item.Branchname == "STAGE3" && validBalanceS3values.Contains(amountToStore))
                                {
                                    Decimal? tempAmount = item.Amount;
                                    Decimal? amountReversed = 0;
                                    Decimal? penaltyMA = 0;
                                    Decimal? penaltyS3 = 0;

                                    //LOGICA PARA DETERMINAR MONTO DEL ASIENTO
                                    if(entryAmountType == "1")
                                    {
                                        if(item.Penaltyma != 0)
                                        {
                                            tempAmount -= item.Penaltyma;
                                            amountReversed = item.Penaltyma;  
                                            penaltyMA = 0;                                      
                                        }
                                        else
                                        {
                                            item.Penaltyma -= tempAmount;
                                            amountReversed = tempAmount;
                                            penaltyS3 = 0;
                                        }   

                                        penaltyS3 += amountReversed;                                     
                                    }

                                    //LOGICA PARA ALMACENAR/ACTUALIZAR MONTOS EN ACCOUNTINGBALANCESTAGE3
                                    var existingItem = newBalanceEntriesList.FirstOrDefault(o => o.Loanid == item.Loanid && o.Creationdate == item.Creationdate.Date);

                                    if (existingItem == null)
                                    {
                                        Accountingbalancestage3 newBalancesEntry = await buildNewBalanceEntry(item);   
                                        updateBalanceS3amounts(newBalancesEntry, item, amountToStore);                                      
                                        newBalanceEntriesList.Add(newBalancesEntry);
                                    }
                                    else{
                                        updateBalanceS3amounts(existingItem, item, amountToStore);
                                    }                            
                                }                          
                            }                                          

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

                            //VALIDACIÓN PARA GENERAR SEGUNDO ASIENTO
                            if ((!rule.Overdueppal && !rule.Adjust && storeNewEntry) || (rule.Overdueppal && item.IsOverdue && item.Principaldue > 0))
                            {
                                newAccountingEntriesList.Add(newEntry);
                            }
                        }

                        if (storeMambuOriginalEntry)
                        {
                            newAccountingEntriesList.Add(await buildNewAccountingEntry(item, newAmount));
                        }

                        applicableRules.Clear();

                    }
                    catch(Exception ex)
                    {
                        await logMessagesInDb($"Error: {ex.Message} - Account ID {item.Loanid} - Entry ID {item.Entryid}");
                        logger.LogError($"Error: {ex.Message} - Account ID {item.Loanid} - Entry ID {item.Entryid}");
                        throw ex;
                    }                    
                }
                
                //LOGICA PARA AGREGAR ASIENTOS A LA SALIDA
                if(orderAccountBalanceModelList.Count > 0)
                {
                    foreach (var orderAccountEntry in orderAccountBalanceModelList)
                    {
                        Accountinghubexit secondOrderAcctEntry = orderAccountEntry.Copy();
                        var glAccountDetails = orderAccountDetails.FirstOrDefault(o => o.type == (secondOrderAcctEntry.Type == "DEBIT" ? "CREDIT" : "DEBIT"));

                        secondOrderAcctEntry.Type = glAccountDetails.type;
                        secondOrderAcctEntry.Glcode = glAccountDetails.glAccountCode;
                        secondOrderAcctEntry.Glname = glAccountDetails.glAccountName;

                        newAccountingEntriesList.Add(secondOrderAcctEntry);
                        newAccountingEntriesList.Add(orderAccountEntry);
                    }
                }

                logger.LogInformation($"New Accounting Entries List({newAccountingEntriesList.Count})");
                await accountingEntriesData.AccountingEntriesInsertBatchAsync(newAccountingEntriesList);
                await accountingEntriesData.BalancesEntriesInsertBatchAsync(newBalanceEntriesList);
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
                    try
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
                    catch (Exception ex)
                    {                
                        await logMessagesInDb($"Error: {ex.Message} - Account ID {item.Loanid} - Entry ID {item.Entryid}");
                        logger.LogError($"Error: {ex.Message} - Account ID {item.Loanid} - Entry ID {item.Entryid}");
                        throw ex;
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

        private bool IsValidJson(String jsonText)
        {

            if (string.IsNullOrWhiteSpace(jsonText))
            {
                return false;
            }

            try
            {
                using JsonDocument doc = JsonDocument.Parse(jsonText);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false; 
            }
    
        }

        private void updateBalanceS3amounts(Accountingbalancestage3 balanceEntry, Accountinghubentry currentEntry, String updateType)
        {
            try
            {
                if(updateType.Equals("InterestS3"))
                {                    
                    balanceEntry.VarInterestS3 = currentEntry.Type == "DEBIT" ? currentEntry.Interests3 + currentEntry.Amount : currentEntry.Interests3 - currentEntry.Amount;
                }
                else if(updateType.Equals("InterestMA")) 
                {
                    balanceEntry.VarInterestMa = currentEntry.Type == "DEBIT" ? currentEntry.Interestma + currentEntry.Amount : currentEntry.Interestma - currentEntry.Amount;
                }
                else if(updateType.Equals("PenaltyS3"))
                {
                    balanceEntry.VarPenaltyS3 = currentEntry.Type == "DEBIT" ? currentEntry.Penaltys3 + currentEntry.Amount  : currentEntry.Penaltys3 - currentEntry.Amount;
                }     
                else if(updateType.Equals("PenaltyMA"))
                {
                    balanceEntry.VarPenaltyMa = currentEntry.Type == "DEBIT" ? currentEntry.Penaltyma + currentEntry.Amount  : currentEntry.Penaltyma - currentEntry.Amount;
                }                  
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

        private async Task<Accountingbalancestage3> buildNewBalanceEntry(Accountinghubentry item)
        {                
            Accountingbalancestage3 newBalanceEntry = new Accountingbalancestage3();

            try
            {
                newBalanceEntry.Loanid = item.Loanid;
                newBalanceEntry.VarInterestS3 = item.Interests3;
                newBalanceEntry.VarInterestMa = item.Interestma;
                newBalanceEntry.VarPenaltyS3 = item.Penaltys3;
                newBalanceEntry.VarPenaltyMa = item.Penaltyma;                
                newBalanceEntry.Creationdate = item.Creationdate.Date;                                       
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw ex;
            }  

            return newBalanceEntry;          
        }

        private async Task logMessagesInDb(string message)
        {
            try
            {                
                await dbLogger.LogActionsDbLevel(message);
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message);
                throw ex;
            }
        }                
    }
}
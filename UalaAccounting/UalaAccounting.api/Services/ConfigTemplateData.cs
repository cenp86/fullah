using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Principal;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Org.BouncyCastle.Crypto.IO;
using UalaAccounting.api.EntitiesDB;
using UalaAccounting.api.Models;

namespace UalaAccounting.api.Services
{
    public class ConfigTemplateData : IConfigTemplateData
    {
        private readonly ContaContext _dbContext;
        private readonly ILogger<DbLogger> _logger;
        private readonly IProductAccountingHubData _productAccountingHubData;
        private readonly ISheetParametrizationData _sheetParametrizationData;
        private readonly ILoanProductData _loanproductData;
        private readonly IGlAccountData _glAccountData;
        private readonly IAccountChartData _accountChartData;

        public ConfigTemplateData(ContaContext dbContext, ILogger<DbLogger> logger, IProductAccountingHubData productAccountingHubData, ISheetParametrizationData SheetParametrizationData, ILoanProductData loanproductData, IGlAccountData glAccountData, IAccountChartData accountChartData)
        {
            _dbContext = dbContext;
            _logger = logger;
            _productAccountingHubData = productAccountingHubData;
            _sheetParametrizationData = SheetParametrizationData;
            _loanproductData = loanproductData;
            _glAccountData = glAccountData;
            _accountChartData = accountChartData;
        }

        public async Task CreateConfigurationTemplate(IFormFile file, List<Configurationsheet> structures, List<Configurationsheetcolumn> structureColumns)
        {
            try
            {
                _logger.LogInformation("Creating configuration template");
                SheetparametrizationModel fileConfiguration = new SheetparametrizationModel();
                fileConfiguration = await GetInformationFromFileToCreate(file, structures, structureColumns);
                var accountCharts = await _accountChartData.GetAccountChartByIdAsync(fileConfiguration.AccountChartItemModel.AccountChartId);

                if (accountCharts.Count == 0)
                {
                    var productsToSave = fileConfiguration.ProductItemModel.productList;
                    var stagesToSave = fileConfiguration.stages;
                    var accountChartToSave = fileConfiguration.AccountChartItemModel.AccountChartItems;

                    await _accountChartData.SaveAccountChartFromList(accountChartToSave);
                    await _productAccountingHubData.SaveProductsFromList(productsToSave);
                    await _sheetParametrizationData.SaveStagesFromList(stagesToSave);
                }
                else
                {
                    throw new Exception("");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task ProcessUpdateConfigurationTemplate(IFormFile file, List<Configurationsheet> structures, List<Configurationsheetcolumn> structureColumns)
        {
            try
            {
                _logger.LogInformation("Updating configuration template");

                SheetparametrizationModel fileConfiguration = new SheetparametrizationModel();

                fileConfiguration = await GetInformationFromFile(file, structures, structureColumns);

                var accountCharts = await _accountChartData.GetAccountChartByIdAsync(fileConfiguration.AccountChartItemModel.AccountChartId);
                List<Productaccountinghub> listToAddProducts = new List<Productaccountinghub>();
                List<Productaccountinghub> listToRemoveProducts = new List<Productaccountinghub>();

                List<Sheetparametrization> listToAddStages = new List<Sheetparametrization>();
                List<Sheetparametrization> listToRemoveStages = new List<Sheetparametrization>();
                List<Sheetparametrization> updatesStages = new List<Sheetparametrization>();

                List<Accountchart> listToAddAccountChart = new List<Accountchart>();
                List<Accountchart> listToRemoveAccountChart = new List<Accountchart>();

                if (accountCharts.Count > 0)
                {
                    var products = await _productAccountingHubData.GetProducctByAccountChart(accountCharts.First().Accountchartid);
                    var stages = await _sheetParametrizationData.GetStagesByAccountchartId(accountCharts.First().Accountchartid);

                    foreach (var item in fileConfiguration.AccountChartItemModel.AccountChartItems)
                    {
                        if (!accountCharts.Any(x => x.Glcode == item.Glcode )) //&& x.Glname == item.Glname && x.Type == item.Type && x.Enable == item.Enable
                        {
                            listToAddAccountChart.Add(item);
                        }
                    }

                    foreach (var item in accountCharts)
                    {
                        if (!fileConfiguration.AccountChartItemModel.AccountChartItems.Any(x => x.Glcode == item.Glcode)) //&& x.Glname == item.Glname && x.Type == item.Type && x.Enable == item.Enable
                        {
                            item.Enable = false;
                            listToRemoveAccountChart.Add(item);
                        }
                    }

                    var updateAccountChart = (from item1 in fileConfiguration.AccountChartItemModel.AccountChartItems
                                              join item2 in accountCharts on item1.Glcode equals item2.Glcode
                                              where item2.Glname != item1.Glname || item2.Type != item1.Type || item2.Enable != item1.Enable
                                              select new Accountchart()
                                              { Accountchartid = item1.Accountchartid,
                                                  Enable = item1.Enable,
                                                  Encodedkey = item2.Encodedkey,
                                                  Glcode = item1.Glcode,
                                                  Glname = item1.Glname,
                                                  Owner = item1.Owner,
                                                  Type = item1.Type
                                              }
                                              ).ToList();

                    

                    foreach (var item in fileConfiguration.ProductItemModel.productList)
                    {
                        if (!products.Any(x => x.Productencodedkey == item.Productencodedkey && x.Productid == item.Productid && x.Enable == item.Enable))
                        {
                            listToAddProducts.Add(item);
                        }
                    }

                    foreach (var item in products)
                    {
                        if (!fileConfiguration.ProductItemModel.productList.Any(x => x.Productencodedkey == item.Productencodedkey && x.Productid == item.Productid && x.Enable == item.Enable))
                        {
                            item.Enable = false;
                            listToRemoveProducts.Add(item);
                        }
                    }

                    var updateProducts = (from item1 in fileConfiguration.ProductItemModel.productList
                                          join item2 in products on item1.Productencodedkey equals item2.Productencodedkey
                                          where item2.Productid != item1.Productid || item2.Accountchart != item1.Accountchart || item1.Enable != item2.Enable
                                          select item1).ToList();


                    listToRemoveProducts.AddRange(updateProducts);

                    foreach (var item in listToRemoveAccountChart)
                    {

                        listToRemoveStages = stages.Where(x => x.Currentglcode == item.Glcode || x.Outputglcode == item.Glcode).ToList();
                        listToRemoveStages.ForEach(x => x.Enable = false);
                    }

                    foreach (var item in listToAddAccountChart)
                    {
                        listToAddStages = fileConfiguration.stages.Where(x => x.Currentglcode == item.Glcode || x.Outputglcode == item.Glcode).ToList();
                    }


                    updatesStages = (from item1 in fileConfiguration.stages
                                         join item2 in stages on item1.Encodedkey equals item2.Encodedkey
                                         where item2.Currentstage != item1.Currentstage || item2.Currentglcode != item1.Currentglcode ||
                                         item2.Currentloantrxtype != item1.Currentloantrxtype || item2.Currentjetype != item1.Currentjetype ||
                                         item2.Outputglcode != item1.Outputglcode || item2.Outputglname != item1.Outputglname ||
                                         item2.Exclusionglcodes != item1.Exclusionglcodes || item2.Adjust != item1.Adjust ||
                                         item2.Observaciones != item1.Observaciones || item2.Enable != item1.Enable ||
                                         item2.Taxentry != item1.Taxentry || item2.Principal != item1.Principal
                                         select item1).ToList();

                    listToRemoveAccountChart.AddRange(updateAccountChart);

                }
                else
                {
                    throw new Exception("This accountChart not exist. We can not update de file");
                }

                if (listToAddAccountChart.Count > 0)
                    await _accountChartData.SaveAccountChartFromList(listToAddAccountChart);

                if (listToRemoveAccountChart.Count > 0)
                    await _accountChartData.UpdateAccountChartFromList(listToRemoveAccountChart);

                if (listToAddProducts.Count > 0)
                    await _productAccountingHubData.SaveProductsFromList(listToAddProducts);

                if(listToRemoveProducts.Count > 0)
                    await _productAccountingHubData.UpdateProductsFromList(listToRemoveProducts);

                if(listToAddStages.Count > 0)
                    await _sheetParametrizationData.SaveStagesFromList(listToAddStages);

                if(listToRemoveStages.Count > 0)
                    await _sheetParametrizationData.UpdateStagesFromList(listToRemoveStages);

                if(updatesStages.Count > 0)
                    await _sheetParametrizationData.UpdateStagesFromList(updatesStages);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        public async Task<MemoryStream> getConfigurationTemplate(List<Configurationsheet> structures, List<Configurationsheetcolumn> structureColumns, List<Accountchart> accountChart, List<string> stagesName, List<Sheetparametrization> stages)
        {
            try
            {
                IWorkbook workbook = new XSSFWorkbook();

                Configurationsheet accountChartSheet = structures.FirstOrDefault(x => x.Name == "ACCOUNTCHART");
                var accountChartSheetColumn = structureColumns.Where(x => x.Configurationsheetencodedkey == accountChartSheet.Encodedkey).ToList();
                await GetAccountChartAsync(accountChartSheet, accountChartSheetColumn, accountChart, workbook);

                var accountChartItem = accountChart.FirstOrDefault();

                if (accountChartItem == null)
                {
                    throw new Exception("Error with account chart configuration");
                }

                Configurationsheet productSheet = structures.FirstOrDefault(x => x.Name == "PRODUCT");
                var productSheetColumn = structureColumns.Where(x => x.Configurationsheetencodedkey == productSheet.Encodedkey).ToList();
                await GetProductByAccountChartAsync(accountChartItem.Accountchartid, productSheet, productSheetColumn, workbook);

                await GetStagesAsync(structures, stages, structureColumns, workbook);

                var memoryStream = new MemoryStream();
                workbook.Write(memoryStream);

                return memoryStream;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }


        private async Task GetAccountChartAsync(Configurationsheet accountChartSheet, List<Configurationsheetcolumn> accountChartSheetColumn, List<Accountchart> accountChart, IWorkbook workbook)
        {
            try
            {
                _logger.LogInformation("Downloading configuration template");


                if (accountChartSheet == null)
                {
                    throw new Exception("Error with ACCOUNTCHART sheet");
                }

                ISheet sheet = workbook.CreateSheet("ACCOUNTCHART");

                int row = 0;
                if (accountChartSheet.Requiredid == true)
                {
                    //Add header row
                    IRow id = sheet.CreateRow(row);
                    id.CreateCell(0).SetCellValue("ACOUNTCHARTID");

                    var accountIdItem = accountChart.First().Accountchartid;

                    id.CreateCell(1).SetCellValue(accountIdItem);

                    row = row + 1;
                }

                IRow headerRow = sheet.CreateRow(row);

                //cabecera
                //string[] headers = new string[] { "GLCODE", "GLNAME", "TYPE", "OWNER", "ENABLE" };
                string[] headers = accountChartSheetColumn.OrderBy(x => x.Columnindex).Select(x => x.Name).ToArray();

                for (int i = 0; i < headers.Length; i++)
                {
                    ICell cell = headerRow.CreateCell(i);
                    cell.SetCellValue(headers[i]);
                }

                row = row + 1;
                foreach (var item in accountChart)
                {
                    IRow dataRow = sheet.CreateRow(row);

                    Configurationsheetcolumn column =  accountChartSheetColumn.First(x => x.Name == "GLCODE");
                    dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Glcode);

                    column = accountChartSheetColumn.First(x => x.Name == "GLNAME");
                    dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Glname);

                    column = accountChartSheetColumn.First(x => x.Name == "TYPE");
                    dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Type);

                    column = accountChartSheetColumn.First(x => x.Name == "OWNER");
                    dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Owner);

                    column = accountChartSheetColumn.First(x => x.Name == "ENABLE");
                    dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Enable.Value);

                    row = row + 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task GetProductByAccountChartAsync(string accountChart, Configurationsheet accountChartSheet, List<Configurationsheetcolumn> productSheetColumn, IWorkbook workbook)
        {
            try
            {
                if (accountChartSheet == null)
                {
                    throw new Exception("Error with PRODUCT sheet");
                }

                ISheet sheet = workbook.CreateSheet("PRODUCT");


                var productList = await _productAccountingHubData.GetProducctByAccountChart(accountChart);


                int row = 0;
                if (accountChartSheet.Requiredid == true)
                {
                    //Add header row
                    //TODO : Definir si pueden tener IDs los otros sheets

                    row = row + 1;
                }

                IRow headerRow = sheet.CreateRow(row);

                //string[] headers = new string[] { "PRODUCTENCODEDKEY", "PRODUCTID", "ENABLE" };

                string[] headers = productSheetColumn.OrderBy(x => x.Columnindex).Select(x => x.Name).ToArray();

                for (int i = 0; i < headers.Length; i++)
                {
                    ICell cell = headerRow.CreateCell(i);
                    cell.SetCellValue(headers[i]);
                }

                row = row + 1;
                foreach (var item in productList)
                {
                    IRow dataRow = sheet.CreateRow(row);

                    Configurationsheetcolumn column = productSheetColumn.First(x => x.Name == "PRODUCTENCODEDKEY");
                    dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Productencodedkey);

                    column = productSheetColumn.First(x => x.Name == "PRODUCTID");
                    dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Productid);

                    column = productSheetColumn.First(x => x.Name == "ENABLE");
                    dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Enable.Value);

                    row = row + 1;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task GetStagesAsync(List<Configurationsheet> structures, List<Sheetparametrization> stages, List<Configurationsheetcolumn> stagesColumns, IWorkbook workbook)
        {
            try
            {
                var accountChartSheet = structures.Where(x => x.Reclassification == true).ToList();
                if (accountChartSheet == null)
                {
                    throw new Exception("Error with Stage configurations");
                }

                //string[] headers = new string[] { "ENCODEDKEY", "ENCODEDKEY", "CURRENTSTAGE", "CURRENTGLCODE", "CURRENTLOANTRXTYPE", "CURRENTJETYPE", "OUTPUTGLCODE", "OUTPUTGLNAME", "EXCLUSIONGLCODES", "ADJUST", "OBSERVATIONS", "ENABLE" };
                
                var group = stages.GroupBy(e => e.Currentstage);

                foreach (var currentStage in group)
                {
                    string sheetName = currentStage.Key;
                    var sheetConfig = accountChartSheet.FirstOrDefault(x => x.Name == sheetName);
                    var columns = stagesColumns.Where(x => x.Configurationsheetencodedkey == sheetConfig.Encodedkey).ToList();
                    string[] headers = columns.OrderBy(x => x.Columnindex).Select(x => x.Name).ToArray();

                    if (sheetConfig == null)
                    {
                        throw new Exception("The stage is incorrect");
                    }

                    ISheet sheet = workbook.CreateSheet(sheetName);

                    //ID
                    int row = 0;
                    if (accountChartSheet.First().Requiredid == true)
                    {
                        row = row + 1;
                    }

                    //HEADERS
                    IRow headerRow = sheet.CreateRow(row);
                    for (int i = 0; i < headers.Length; i++)
                    {
                        ICell cell = headerRow.CreateCell(i);
                        cell.SetCellValue(headers[i]);
                    }
                    row = row + 1;

                    //DATA
                    foreach (var item in currentStage)
                    {
                        IRow dataRow = sheet.CreateRow(row);

                        Configurationsheetcolumn column = columns.First(x => x.Name == "ENCODEDKEY");
                        dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Encodedkey);

                        column = stagesColumns.First(x => x.Name == "CREATIONDATE");
                        dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Creationdate.ToString());

                        column = stagesColumns.First(x => x.Name == "CURRENTSTAGE");
                        dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Currentstage);

                        column = stagesColumns.First(x => x.Name == "CURRENTGLCODE");
                        dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Currentglcode);

                        column = stagesColumns.First(x => x.Name == "CURRENTLOANTRXTYPE");
                        dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Currentloantrxtype);

                        column = stagesColumns.First(x => x.Name == "CURRENTJETYPE");
                        dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Currentjetype);

                        column = stagesColumns.First(x => x.Name == "OUTPUTGLCODE");
                        dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Outputglcode);

                        column = stagesColumns.First(x => x.Name == "OUTPUTGLNAME");
                        dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Outputglname);

                        column = stagesColumns.First(x => x.Name == "EXCLUSIONGLCODES");
                        dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Exclusionglcodes);

                        column = stagesColumns.First(x => x.Name == "ADJUST");
                        dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Adjust);

                        column = stagesColumns.First(x => x.Name == "OBSERVACIONES");
                        dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Observaciones);

                        column = stagesColumns.First(x => x.Name == "ENABLE");
                        dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Enable);

                        column = stagesColumns.First(x => x.Name == "TAXENTRY");
                        dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Taxentry);

                        column = stagesColumns.First(x => x.Name == "PRINCIPAL");
                        dataRow.CreateCell(column.Columnindex.Value - 1).SetCellValue(item.Principal);

                        row = row + 1;
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<MemoryStream> getConfigurationTemplate()
        {
            try
            {
                _logger.LogInformation("Downloading configuration template");

                var configTable = await _dbContext.Accountinghublogs.ToListAsync();

                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Hoja1");

                //Add header row
                IRow headerRow = sheet.CreateRow(0);
                headerRow.CreateCell(0).SetCellValue("logID");
                headerRow.CreateCell(1).SetCellValue("logLine");
                headerRow.CreateCell(2).SetCellValue("creationDate");

                //Add data from DB table to rows
                int rowIndex = 1;
                foreach (var row in configTable)
                {
                    IRow excelRow = sheet.CreateRow(rowIndex++);

                    excelRow.CreateCell(0).SetCellValue(row.Logid);
                    excelRow.CreateCell(1).SetCellValue(row.Logline);
                    excelRow.CreateCell(2).SetCellValue(row.Creationdate.ToString());
                }

                var ms = new MemoryStream();
                workbook.Write(ms);

                return ms;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        private async Task<SheetparametrizationModel> GetInformationFromFileToCreate(IFormFile file, List<Configurationsheet> structures, List<Configurationsheetcolumn> structureColumns)
        {
            try
            {
                SheetparametrizationModel fileConfiguration = new SheetparametrizationModel();
                fileConfiguration.stages = new List<Sheetparametrization>();
                var stream = new MemoryStream();

                await file.CopyToAsync(stream);
                stream.Position = 0;
                IWorkbook workbook = new XSSFWorkbook(stream);

                ISheet sheetAccountChart = workbook.GetSheet("ACCOUNTCHART");
                var accountChartStruct = structures.FirstOrDefault(x => x.Name == "ACCOUNTCHART");
                var accountChartStructureColumns = structureColumns.Where(x => x.Configurationsheetencodedkey == accountChartStruct.Encodedkey).ToList();
                var accountChartItem = await GetAccountChartFromExcelAsync(accountChartStruct, sheetAccountChart, accountChartStructureColumns);
                fileConfiguration.AccountChartItemModel = accountChartItem;

                if(!await ValidateAccountchartIdToCreate(fileConfiguration.AccountChartItemModel))
                {
                    throw new Exception("The AccountChartId exist!");                   
                }

                ISheet sheetProduct = workbook.GetSheet("PRODUCT");
                var productStruct = structures.FirstOrDefault(x => x.Name == "PRODUCT");
                var productStructureColumns = structureColumns.Where(x => x.Configurationsheetencodedkey == productStruct.Encodedkey).ToList();
                var productModel = await GetProductFromExcelAsync(productStruct, sheetProduct, productStructureColumns);
                fileConfiguration.ProductItemModel = productModel;

                ISheet sheetStage1 = workbook.GetSheet("STAGE1");
                var stage1Structure = structures.FirstOrDefault(x => x.Name == "STAGE1");
                var stage1StructureColumns = structureColumns.Where(x => x.Configurationsheetencodedkey == stage1Structure.Encodedkey).ToList();
                var stage1 = await GetStagesFromExcelAsync(stage1Structure, sheetStage1, stage1StructureColumns);

                if(stage1 != null)
                    fileConfiguration.stages.AddRange(stage1);

                ISheet sheetStage2 = workbook.GetSheet("STAGE2");
                var stage2Structure = structures.FirstOrDefault(x => x.Name == "STAGE2");
                var stage2StructureColumns = structureColumns.Where(x => x.Configurationsheetencodedkey == stage2Structure.Encodedkey).ToList();
                var stage2 = await GetStagesFromExcelAsync(stage2Structure, sheetStage2, stage2StructureColumns);

                if(stage2 != null)
                    fileConfiguration.stages.AddRange(stage2);

                ISheet sheetStage3 = workbook.GetSheet("STAGE3");
                var stage3Structure = structures.FirstOrDefault(x => x.Name == "STAGE3");
                var stage3StructureColumns = structureColumns.Where(x => x.Configurationsheetencodedkey == stage3Structure.Encodedkey).ToList();
                var stage3 = await GetStagesFromExcelAsync(stage3Structure, sheetStage3, stage3StructureColumns);

                if(stage3 != null)
                    fileConfiguration.stages.AddRange(stage3);

                await UpdateAccountChartIdInSheetparametrizationModel(fileConfiguration);

                await ValidateFileToProcess(fileConfiguration, structures);

                return fileConfiguration;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        private async Task<bool> ValidateAccountchartIdToCreate(AccountChartModel accountChartItemModel)
        {
            bool result = false;

            var listFromDb = await _accountChartData.GetAccountChartByIdAsync(accountChartItemModel.AccountChartId);

            if(listFromDb.Count > 0)
            {
                result = false;
            }
            else
            {
                result = true;
            }

            return result;
        }

        private async Task<SheetparametrizationModel> GetInformationFromFile(IFormFile file, List<Configurationsheet> structures, List<Configurationsheetcolumn> structureColumns)
        {
            try
            {
                SheetparametrizationModel fileConfiguration = new SheetparametrizationModel();
                fileConfiguration.stages = new List<Sheetparametrization>();
                var stream = new MemoryStream();

                await file.CopyToAsync(stream);
                stream.Position = 0;
                IWorkbook workbook = new XSSFWorkbook(stream);

                ISheet sheetAccountChart = workbook.GetSheet("ACCOUNTCHART");
                var accountChartStruct = structures.FirstOrDefault(x => x.Name == "ACCOUNTCHART");
                var accountChartStructureColumns = structureColumns.Where(x => x.Configurationsheetencodedkey == accountChartStruct.Encodedkey).ToList();
                var accountChartItem = await GetAccountChartFromExcelAsync(accountChartStruct, sheetAccountChart, accountChartStructureColumns);
                fileConfiguration.AccountChartItemModel = accountChartItem;


                ISheet sheetProduct = workbook.GetSheet("PRODUCT");
                var productStruct = structures.FirstOrDefault(x => x.Name == "PRODUCT");
                var productStructureColumns = structureColumns.Where(x => x.Configurationsheetencodedkey == productStruct.Encodedkey).ToList();
                var productModel = await GetProductFromExcelAsync(productStruct, sheetProduct, productStructureColumns);
                fileConfiguration.ProductItemModel = productModel;

                ISheet sheetStage1 = workbook.GetSheet("STAGE1");
                var stage1Structure = structures.FirstOrDefault(x => x.Name == "STAGE1");
                var stage1StructureColumns = structureColumns.Where(x => x.Configurationsheetencodedkey == stage1Structure.Encodedkey).ToList();
                var stage1 = await GetStagesFromExcelAsync(stage1Structure, sheetStage1, stage1StructureColumns);

                if(stage1 != null)
                    fileConfiguration.stages.AddRange(stage1);

                ISheet sheetStage2 = workbook.GetSheet("STAGE2");
                var stage2Structure = structures.FirstOrDefault(x => x.Name == "STAGE2");
                var stage2StructureColumns = structureColumns.Where(x => x.Configurationsheetencodedkey == stage2Structure.Encodedkey).ToList();
                var stage2 = await GetStagesFromExcelAsync(stage2Structure, sheetStage2, stage2StructureColumns);

                if(stage2 != null)
                    fileConfiguration.stages.AddRange(stage2);

                ISheet sheetStage3 = workbook.GetSheet("STAGE3");
                var stage3Structure = structures.FirstOrDefault(x => x.Name == "STAGE3");
                var stage3StructureColumns = structureColumns.Where(x => x.Configurationsheetencodedkey == stage3Structure.Encodedkey).ToList();
                var stage3 = await GetStagesFromExcelAsync(stage3Structure, sheetStage3, stage3StructureColumns);

                if(stage3 != null)
                    fileConfiguration.stages.AddRange(stage3);

                await UpdateAccountChartIdInSheetparametrizationModel(fileConfiguration);

                await ValidateFileToProcess(fileConfiguration, structures);

                return fileConfiguration;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw ex;
            }
        }

        private async Task UpdateAccountChartIdInSheetparametrizationModel(SheetparametrizationModel item)
        {
            try
            {
                string accountChartId = item.AccountChartItemModel.AccountChartId;
                item.ProductItemModel.productList.ForEach(x => x.Accountchart = accountChartId);

                item.stages.ForEach(x => x.Accountchart = accountChartId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task ValidateFileToProcess(SheetparametrizationModel fileConfiguration, List<Configurationsheet> structures)
        {
            var countOfSateges = fileConfiguration.stages.Select(x => x.Currentstage).Distinct().ToList();

            if (countOfSateges.Count != structures.Where(x => x.Reclassification == true).ToList().Count)
            {
                throw new Exception("Error in the number of \"stages\" configured and defined");
            }

            List<Accountchart> notFoundTransactions = new List<Accountchart>();
            var listGlCodesInFields = fileConfiguration.stages.Select(x => x.Currentglcode).ToList();
            listGlCodesInFields.AddRange(fileConfiguration.stages.Select(x => x.Outputglcode).ToList());

            if (fileConfiguration.AccountChartItemModel != null && fileConfiguration.stages != null)
            {
                foreach (var item in fileConfiguration.AccountChartItemModel.AccountChartItems)
                {
                    bool notFoundCurrent = listGlCodesInFields.Any(x => x == item.Glcode);

                    if (!notFoundCurrent)
                    {
                        notFoundTransactions.Add(item);
                    }
                }
            }

            if (notFoundTransactions.Count > 0)
            {
                StringBuilder messageBuilder = new StringBuilder();

                foreach (var item in notFoundTransactions)
                {
                    messageBuilder.AppendLine("Error with Glocode of the AccountChart : " + item.Glcode);
                }

                throw new Exception(messageBuilder.ToString());
            }

            var notFound = new List<string>();
            foreach (var item in listGlCodesInFields)
            {
                bool notFoundCurrent = fileConfiguration.AccountChartItemModel.AccountChartItems.Any(x => x.Glcode == item);

                if (!notFoundCurrent)
                {
                    notFound.Add(item);
                }
            }

            if (notFound.Count > 0)
            {
                StringBuilder messageBuilder = new StringBuilder();

                foreach (var item in notFound)
                {
                    messageBuilder.AppendLine("Error with the glcode configured in the stage detail : " + item);
                }

                throw new Exception(messageBuilder.ToString());
            }

            List<string> productList = new List<string>();
            if (fileConfiguration != null && fileConfiguration.ProductItemModel != null)
            {
                productList = fileConfiguration.ProductItemModel.productList.Select(x => x.Productencodedkey).ToList();
            }
            else
            {
                throw new Exception("Error with configuration file in Sheet PRODUCT");
            }

            var products = await _loanproductData.GetProductByListOfEncodedKey(productList);

            if (products.Count != productList.Count)
            {
                throw new Exception("All products are not defined in Mambu");
            }

            List<string> accountChartOfMambu = new List<string>();

            if (fileConfiguration != null && fileConfiguration.AccountChartItemModel != null)
            {
                accountChartOfMambu = fileConfiguration.AccountChartItemModel.AccountChartItems.Where(x => x.Owner == "MAMBU").Select(x => x.Glcode).ToList();  
            }
            else
            {
                throw new Exception("Error with configuration file");
            }

            var accountChartFromMambu = await _glAccountData.GetGlAccountByGlCodeListAsync(accountChartOfMambu);
            if (accountChartOfMambu.Count != accountChartFromMambu.Count)
            {
                throw new Exception("The \"GLCode\" defined in the \"chart of accounts\" in quantity do not match those in the Mambu database. Error with configuration file in sheet ACCOUNTCHART");
            }

        }

        private async Task<List<Sheetparametrization>> GetStagesFromExcelAsync(Configurationsheet stageStruct, ISheet sheetStage, List<Configurationsheetcolumn> columnsConfiguration)
        {
            string fieldToProcess = string.Empty;
            int rowToProcess = 0;

            try
            {
                List<Sheetparametrization> list = new List<Sheetparametrization>();
                int startPosition = stageStruct.Startposition.Value;
                int i = startPosition;
                

                while (HasData(sheetStage.GetRow(i)) && i < sheetStage.PhysicalNumberOfRows)
                {
                    rowToProcess = i;

                    IRow row = sheetStage.GetRow(i);
                    if (row == null)
                        throw new Exception("Error with Accountchart");

                    int cantColumn = stageStruct.Numbercolumns.Value;

                    Sheetparametrization stageItem = new Sheetparametrization();

                    fieldToProcess = "ENCODEDKEY";
                    Configurationsheetcolumn column = columnsConfiguration.FirstOrDefault(x => x.Name == "ENCODEDKEY");
                    ICell cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true ? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    stageItem.Encodedkey = cell.ToString();

                    fieldToProcess = "CREATIONDATE";
                    column = columnsConfiguration.FirstOrDefault(x => x.Name == "CREATIONDATE");
                    cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true ? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    var creationDateString = cell.ToString();

                    DateTime ParseDateTime = new DateTime();
                    if (DateTime.TryParse(creationDateString, out ParseDateTime))
                    {
                        stageItem.Creationdate = ParseDateTime;
                    }

                    fieldToProcess = "CURRENTSTAGE";
                    column = columnsConfiguration.FirstOrDefault(x => x.Name == "CURRENTSTAGE");
                    cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true ? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    stageItem.Currentstage = cell.ToString();

                    fieldToProcess = "CURRENTGLCODE";
                    column = columnsConfiguration.FirstOrDefault(x => x.Name == "CURRENTGLCODE");
                    cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true ? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    stageItem.Currentglcode = cell.ToString();

                    fieldToProcess = "CURRENTLOANTRXTYPE";
                    column = columnsConfiguration.FirstOrDefault(x => x.Name == "CURRENTLOANTRXTYPE");
                    cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true ? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    stageItem.Currentloantrxtype = cell.ToString();

                    fieldToProcess = "Currentjetype";
                    column = columnsConfiguration.FirstOrDefault(x => x.Name == "CURRENTJETYPE");
                    cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true ? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    stageItem.Currentjetype = cell.ToString();

                    fieldToProcess = "OUTPUTGLCODE";
                    column = columnsConfiguration.FirstOrDefault(x => x.Name == "OUTPUTGLCODE");
                    cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true ? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    stageItem.Outputglcode = cell.ToString();

                    fieldToProcess = "EXCLUSIONGLCODES";
                    column = columnsConfiguration.FirstOrDefault(x => x.Name == "EXCLUSIONGLCODES");
                    cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true ? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    stageItem.Exclusionglcodes = cell.ToString();


                    fieldToProcess = "ADJUST";
                    column = columnsConfiguration.FirstOrDefault(x => x.Name == "ADJUST");
                    cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true ? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    if (string.IsNullOrEmpty(cell.ToString()))
                    {
                        stageItem.Adjust = false;
                    }
                    else
                    {
                        stageItem.Adjust = cell.BooleanCellValue;

                    }

                    fieldToProcess = "OBSERVACIONES";
                    column = columnsConfiguration.FirstOrDefault(x => x.Name == "OBSERVACIONES");
                    cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true ? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    stageItem.Observaciones = cell.ToString();

                    fieldToProcess = "ENABLE";
                    column = columnsConfiguration.FirstOrDefault(x => x.Name == "ENABLE");
                    cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true ? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    if (cell == null || string.IsNullOrEmpty(cell.ToString()))
                    {
                        stageItem.Enable = false;
                    }
                    else
                    {
                        stageItem.Enable = cell.BooleanCellValue;

                    }

                    fieldToProcess = "TAXENTRY";
                    column = columnsConfiguration.FirstOrDefault(x => x.Name == "TAXENTRY");
                    cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true ? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    if (cell == null || string.IsNullOrEmpty(cell.ToString()))
                    {
                        stageItem.Taxentry = false;
                    }
                    else
                    {
                        stageItem.Taxentry = cell.BooleanCellValue;

                    }

                    fieldToProcess = "PRINCIPAL";
                    column = columnsConfiguration.FirstOrDefault(x => x.Name == "PRINCIPAL");
                    cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true ? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    if (cell == null || string.IsNullOrEmpty(cell.ToString()))
                    {
                        stageItem.Principal = false;
                    }
                    else
                    {
                        stageItem.Principal = cell.BooleanCellValue;

                    }

                    list.Add(stageItem);

                    i++;
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Sheet {0}. Error in row {1} with the {2} field ", stageStruct.Name, rowToProcess, fieldToProcess), ex);
            }
        }

        private async Task<ProductModel> GetProductFromExcelAsync(Configurationsheet productStruct, ISheet sheetProduct, List<Configurationsheetcolumn> columnsConfiguration)
        {
            string fieldToProcess = string.Empty;
            int rowToProcess = 0;

            try
            {
                ProductModel item = new ProductModel();
                item.productList = new List<Productaccountinghub>();

                if (productStruct == null)
                    throw new Exception("Error with Product configuration");

                if (productStruct.Requiredid.Value)
                {
                    int idIndex = productStruct.Startpositionid.Value;
                    rowToProcess = idIndex;
                    fieldToProcess = "id";
                    IRow row = sheetProduct.GetRow(idIndex);
                    if (row == null)
                        throw new Exception("Error with Accountchart");

                    ICell cell = row.GetCell(1);
                    item.ProductId = cell.ToString();
                }

                int startPosition = productStruct.Startposition.Value;

                int i = startPosition;
                
                while (HasData(sheetProduct.GetRow(i)) && i < sheetProduct.PhysicalNumberOfRows)
                {
                    rowToProcess = i;

                    IRow row = sheetProduct.GetRow(i);
                    if (row == null)
                        throw new Exception("Error with Accountchart");

                    int cantColumn = productStruct.Numbercolumns.Value;

                    Productaccountinghub productItem = new Productaccountinghub();

                    fieldToProcess = "Productencodedkey";
                    Configurationsheetcolumn column = columnsConfiguration.FirstOrDefault(x => x.Name == "PRODUCTENCODEDKEY");
                    ICell cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true ? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    productItem.Productencodedkey = cell.ToString();

                    fieldToProcess = "Productid";
                    column = columnsConfiguration.FirstOrDefault(x => x.Name == "PRODUCTID");
                    cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true ? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    productItem.Productid = cell.ToString();

                    fieldToProcess = "Enable";
                    column = columnsConfiguration.FirstOrDefault(x => x.Name == "ENABLE");
                    cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true ? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    productItem.Enable = cell.BooleanCellValue;

                    item.productList.Add(productItem);

                    i++;
                }

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Sheet {0}. Error in row {1} with the {2} field ", productStruct.Name, rowToProcess, fieldToProcess), ex);
            }

        }

        private async Task<AccountChartModel> GetAccountChartFromExcelAsync(Configurationsheet accountChartStruct,
            ISheet sheetAccountChart, List<Configurationsheetcolumn> columnsConfiguration)
        {
            string fieldToProcess = string.Empty;
            int rowToProcess = 0;

            try
            {
                AccountChartModel item = new AccountChartModel();
                
                if (accountChartStruct == null)
                    throw new Exception("Error with Accountchart configuration");


                if (accountChartStruct.Requiredid.Value)
                {
                    fieldToProcess = "id";
                    int idIndex = accountChartStruct.Startpositionid.Value;
                    rowToProcess = idIndex;
                    IRow row = sheetAccountChart.GetRow(idIndex);
                    if (row == null)
                        throw new Exception("Error with Accountchart");

                    //TODO: cambiar la forma en como obtenemos los identificadores de la columna
                    ICell cell = row.GetCell(1);
                    item.AccountChartId = cell.ToString();

                }

                int startPosition = accountChartStruct.Startposition.Value;


                int i = startPosition;
                item.AccountChartItems = new List<Accountchart>();
                while (HasData(sheetAccountChart.GetRow(i)) && i < sheetAccountChart.PhysicalNumberOfRows)
                {
                    rowToProcess = i;
                    IRow row = sheetAccountChart.GetRow(i);
                    if (row == null)
                        throw new Exception("Error with Accountchart");

                    int cantColumn = accountChartStruct.Numbercolumns.Value;

                    Accountchart accountchartItem = new Accountchart();

                    fieldToProcess = "Accountchartid";
                    accountchartItem.Accountchartid = item.AccountChartId;

                    fieldToProcess = "Glcode";
                    Configurationsheetcolumn column = columnsConfiguration.FirstOrDefault(x => x.Name == "GLCODE");
                    ICell cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    accountchartItem.Glcode = cell.ToString();

                    fieldToProcess = "Glname";
                    column = columnsConfiguration.FirstOrDefault(x => x.Name == "GLNAME");
                    cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true ? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    accountchartItem.Glname = cell.ToString();

                    fieldToProcess = "Type";
                    column = columnsConfiguration.FirstOrDefault(x => x.Name == "TYPE");
                    cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true ? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    accountchartItem.Type = cell.ToString();

                    fieldToProcess = "Owner";
                    column = columnsConfiguration.FirstOrDefault(x => x.Name == "OWNER");
                    cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true ? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    accountchartItem.Owner = cell.ToString();

                    //TODO:revisar esto en test
                    fieldToProcess = "Enable";
                    column = columnsConfiguration.FirstOrDefault(x => x.Name == "ENABLE");
                    cell = row.GetCell(column.Columnindex.Value - 1, column.Required == true ? MissingCellPolicy.RETURN_BLANK_AS_NULL : MissingCellPolicy.RETURN_NULL_AND_BLANK);
                    accountchartItem.Enable = cell.BooleanCellValue;

                    accountchartItem.Encodedkey = Guid.NewGuid().ToString();

                    item.AccountChartItems.Add(accountchartItem);

                    i++;
                }

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Sheet {0}. Error in row {1} with the {2} field ", accountChartStruct.Name, rowToProcess, fieldToProcess), ex);
            }
        }

        private string ObtenerNombreDeAtributo<T>(Expression<Func<T, object>> expression)
        {
            MemberExpression memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null)
            {
                UnaryExpression unaryExpression = expression.Body as UnaryExpression;
                memberExpression = unaryExpression?.Operand as MemberExpression;
            }

            if (memberExpression != null)
            {
                return memberExpression.Member.Name;
            }

            throw new ArgumentException("La expresión no es un atributo.");
        }

        private bool HasData(IRow row)
        {
            if(row == null)
                return false;

            return row.Cells.Any(c => c.CellType != CellType.Blank &&
                                      (c.CellType == CellType.String && !string.IsNullOrWhiteSpace(c.StringCellValue) ||
                                       c.CellType != CellType.String));
        }



    }
}
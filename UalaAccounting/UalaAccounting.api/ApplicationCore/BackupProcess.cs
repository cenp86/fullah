using System;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using NPOI.Util;
using UalaAccounting.api.Models;
using UalaAccounting.api.Services;

namespace UalaAccounting.api.ApplicationCore
{
    public class BackupProcess : IBackupProcess
    {
        private readonly ILogger<BackupProcess> _logger;
        private readonly IAmazonS3Bucket _amazonS3Bucket;
        private readonly IConfigurationData _configurationData;
        private readonly IApiMambuServices _apiMambu;
        private readonly IConfiguration _config;
        private readonly IFormedSqlExecution _IFormedSqlExecution;
        private readonly ICustomQueryInDatabaseServices _customQueryInDatabaseServices;
        private readonly IShellServices _shellServices;

        private readonly TablesInMemoryModel _tablesInMemory;

        private string? _amazonS3BucketName = "";
        private string? _folderSepa = "\\";
        private bool _batchImportUploadToS3;
        private string? importMainFolder;

        private string importBkpFolder = string.Empty;
        private string importZipFolder = string.Empty;
        private string fileName = string.Empty;
        private string mysqlPath = string.Empty;

        public BackupProcess(ILogger<BackupProcess> logger, IAmazonS3Bucket amazonS3Bucket,
            IConfigurationData configurationData, IApiMambuServices apiMambu,
            IFormedSqlExecution formedSqlExecution, IConfiguration config,
            TablesInMemoryModel tablesInMemory, ICustomQueryInDatabaseServices customQueryInDatabaseServices,
            IShellServices shellServices)
        {
            _logger = logger;
            _amazonS3Bucket = amazonS3Bucket;
            _configurationData = configurationData;
            _apiMambu = apiMambu;
            _config = config;
            _IFormedSqlExecution = formedSqlExecution;
            _tablesInMemory = tablesInMemory;
            _customQueryInDatabaseServices = customQueryInDatabaseServices;
            _shellServices = shellServices;
        }

        private async Task LoadConfiguration()
        {
            string errorInConfiguration = string.Empty;
            try
            {
                var configuration = await _configurationData.GetConfigurationEnableAsync();
                
                if (configuration == null || configuration.Count == 0)
                {
                    throw new Exception("Error in configuration. You dont have any configurations");
                }

                _tablesInMemory.ConfigurationList = configuration;

                errorInConfiguration = "API_CALLBACK";
                _apiMambu.SetApiCallback(_tablesInMemory.ConfigurationList.Where(x => x.Name == "API_CALLBACK").First().Valueconfiguration);

                errorInConfiguration = "API_KEY";
                _apiMambu.SetApiKey(_tablesInMemory.ConfigurationList.Where(x => x.Name == "API_KEY").First().Valueconfiguration);

                errorInConfiguration = "MAMBU_APIKEY";
                _apiMambu.SetApiValue(_config["MAMBU_APIKEY"]);

                errorInConfiguration = "API_BASE";
                _apiMambu.SetApiBase(_tablesInMemory.ConfigurationList.Where(x => x.Name == "API_BASE").First().Valueconfiguration);

                errorInConfiguration = "USER_AGENT";
                _apiMambu.SetUserAgent(_tablesInMemory.ConfigurationList.Where(x => x.Name == "USER_AGENT").First().Valueconfiguration);

                errorInConfiguration = "HEADER_ACCEPT_GET";
                _apiMambu.SetHeaderAcceptGet(_tablesInMemory.ConfigurationList.Where(x => x.Name == "HEADER_ACCEPT_GET").First().Valueconfiguration);

                errorInConfiguration = "HEADER_ACCEPT_POST";
                _apiMambu.SetHeaderAcceptPost(_tablesInMemory.ConfigurationList.Where(x => x.Name == "HEADER_ACCEPT_POST").First().Valueconfiguration);

                errorInConfiguration = "TABLES_BACKUP";
                _apiMambu.SetTablesBackupk(_tablesInMemory.ConfigurationList.Where(x => x.Name == "TABLES_BACKUP").First().Valueconfiguration);

                errorInConfiguration = "MYSQL_PATH";
                mysqlPath = _tablesInMemory.ConfigurationList.Where(x => x.Name == "MYSQL_PATH").First().Valueconfiguration;
                _shellServices.SetMySqlPath(mysqlPath);

                errorInConfiguration = "ACHUB";
                _IFormedSqlExecution.SetConnectionString(_config["ACHUB"]);

                errorInConfiguration = "BATCH_IMPORT_UPLOAD_TO_S3";
                var configUploadToS3 = _tablesInMemory.ConfigurationList.Where(x => x.Name == "BATCH_IMPORT_UPLOAD_TO_S3").First().Valueconfiguration;
                this._batchImportUploadToS3 = this.AnalizeBoolean(configUploadToS3);


                errorInConfiguration = "FOLDER_SEPARATOR";
                _folderSepa = _tablesInMemory.ConfigurationList.Where(x => x.Name == "FOLDER_SEPARATOR").First().Valueconfiguration;

                errorInConfiguration = "IMPORT_FOLDER";
                importMainFolder = _tablesInMemory.ConfigurationList.Where(x => x.Name == "IMPORT_FOLDER").First().Valueconfiguration;

                //errorInConfiguration = "BKP_FOLDER";
                //importBkpFolder = _tablesInMemory.ConfigurationList.Where(x => x.Name == "BKP_FOLDER").First().Valueconfiguration;

                errorInConfiguration = "ZIP_FOLDER";
                importZipFolder = _tablesInMemory.ConfigurationList.Where(x => x.Name == "ZIP_FOLDER").First().Valueconfiguration;

                errorInConfiguration = "CORE_FILE_NAME";
                fileName = _tablesInMemory.ConfigurationList.Where(x => x.Name == "CORE_FILE_NAME").First().Valueconfiguration;

                if (_batchImportUploadToS3)
                {
                    errorInConfiguration = "AMAZON_S3_BUCKET";
                    this._amazonS3BucketName = _tablesInMemory.ConfigurationList.Where(x => x.Name == "AMAZON_S3_BUCKET").First().Valueconfiguration;

                    _amazonS3Bucket.SetS3BucketName(this._amazonS3BucketName);

                    //errorInConfiguration = "S3-access-key";
                    //_amazonS3Bucket.SetAmazonApiKey(_config["S3-access-key"]);

                    //errorInConfiguration = "S3-secret-key";
                    //_amazonS3Bucket.SetAmazonApiSecret(_config["S3-secret-key"]);
                }
            }
            catch (Exception ex)
            {
                string messageError = "Error in configuration: " + errorInConfiguration;
                throw new Exception(messageError, ex);
            }
        }

        public bool AnalizeBoolean(string? strBoolean)
        {
            if (strBoolean.ToLower() == "true") return true;
            if (strBoolean.ToLower() == "false") return false;
            return false;

        }

        public async Task RunImportAsync()
        {
            List<string> insertCommands = new List<string>();
            try
            {
                _logger.LogInformation($"Started executing RunImportAsync({System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")})");
                await LoadConfiguration();
                var requestBackup = await _apiMambu.PerformBackupHttpPost();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task GetAndProcessBackUp()
        {
            try
            {
                await LoadConfiguration();

                string sqlFileName = fileName + ".sql";
                string zipFileName = importZipFolder + this._folderSepa + fileName + ".zip";
                string unzipFileName = importZipFolder + this._folderSepa + sqlFileName;
                string unzipFolderName = importZipFolder + this._folderSepa + fileName;

                if (!Directory.Exists(importMainFolder))
                {
                    Directory.CreateDirectory(importMainFolder);
                }

                importBkpFolder = Path.Combine(importMainFolder, importBkpFolder);
                if (!Directory.Exists(importBkpFolder))
                {
                    Directory.CreateDirectory(importBkpFolder);
                }

                importZipFolder = Path.Combine(importMainFolder, importZipFolder);
                if (!Directory.Exists(importZipFolder))
                {
                    Directory.CreateDirectory(importZipFolder);
                }

                unzipFolderName = Path.Combine(importMainFolder, unzipFolderName);
                if (Directory.Exists(unzipFolderName))
                {
                    Directory.Delete(unzipFolderName, true);
                }

                var responseBackUp = await _apiMambu.PerformBackupHttpGet();

                zipFileName = Path.Combine(importMainFolder, zipFileName);

                using (var fileStream = new FileStream(zipFileName, FileMode.Create))
                {
                    await responseBackUp.CopyToAsync(fileStream);
                }

                _logger.LogInformation($"Extracting Files from {zipFileName} -- {System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")})");
                ZipFile.ExtractToDirectory(zipFileName, unzipFolderName);

                string[] files = Directory.GetFiles(unzipFolderName);
                //string outputFile = Path.Combine(unzipFolderName, "aux.sql");

                _logger.LogInformation("the sql validation process is about to start");
                //ValidateSqlFile(files[0], outputFile);
                _logger.LogInformation("The validation process is ended.");
                
                _logger.LogInformation("the FormatConnectionString process is about to start");
                await _shellServices.FormatConnectionString();
                _logger.LogInformation("The FormatConnectionString is ended.");

                _logger.LogInformation("the ImportRestore process is about to start");
                await _shellServices.ImportRestore(files[0]);
                _logger.LogInformation("The ImportRestore process is ended.");
                

                if (this._batchImportUploadToS3)
                {
                    _logger.LogInformation($"Perform Backup of ZIP -- {System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")})");
                    await BackupAllFileAsync(zipFileName, importBkpFolder, fileName, ".zip");
                }


                _logger.LogInformation($"Finished executing GetAndProcessBackUp({System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")})");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ValidateSqlFile(string filePath, string outputFile)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                using (StreamReader reader = new StreamReader(filePath))
                using (StreamWriter writer = new StreamWriter(outputFile))
                {
                    //writer.WriteLine("SET FOREIGN_KEY_CHECKS = 0;");
                    //writer.Write(reader.ReadToEnd());
                    //writer.WriteLine("SET FOREIGN_KEY_CHECKS = 1;");
                }

                File.Copy(outputFile, filePath, true);
                if (File.Exists(outputFile))
                    {
                         File.Delete(outputFile);
                    }
            }
            catch(Exception ex)
            {
                throw ex;                
            }
        }

        // public void ValidateSqlFile(string filePath, string outputFile)
        // {
        //     try
        //     {
        //         string[] lines = File.ReadAllLines(filePath);
        //         bool inCreateTable = false;
        //         string createTableBlock = string.Empty;

        //         using (StreamReader reader = new StreamReader(filePath))
        //         using (StreamWriter writer = new StreamWriter(outputFile))
        //         {
        //             string line;
        //             while ((line = reader.ReadLine()) != null)
        //             {
        //                 if (Regex.IsMatch(line, @"^\s*CREATE\s+TABLE", RegexOptions.IgnoreCase))
        //                 {
        //                     if (inCreateTable)
        //                     {
        //                         throw new Exception("A CREATE TABLE block was found inside another CREATE TABLE block. This is invalid.");
        //                     }
        //                     inCreateTable = true;
        //                     createTableBlock = line + "\n";
        //                 }
        //                 else if (inCreateTable)
        //                 {
        //                     if (!Regex.IsMatch(line, @"CONSTRAINT.*?FOREIGN KEY", RegexOptions.IgnoreCase))
        //                     {
        //                         createTableBlock += line + "\n";

        //                         if (line.Trim().StartsWith(") ENGINE=InnoDB"))
        //                         {
        //                             // El bloque CREATE TABLE ha terminado
        //                             inCreateTable = false;

        //                             // Validar que el bloque no tenga errores
        //                             line = ValidateCreateTableBlock(createTableBlock);

        //                             // Reiniciar el bloque
        //                             createTableBlock = string.Empty;
        //                         }
        //                     }
        //                 }

        //                 if (!inCreateTable && !Regex.IsMatch(line, @"^\/\*\!\d+", RegexOptions.IgnoreCase))
        //                     writer.WriteLine(line);
        //             }

        //             writer.WriteLine("SET foreign_key_checks=1;");

        //         }

        //         File.Copy(outputFile, filePath, true);

        //         if (File.Exists(outputFile))
        //         {
        //             File.Delete(outputFile);
        //         }

        //         if (inCreateTable)
        //         {
        //             throw new Exception("The file ended without closing a CREATE TABLE block.");
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         throw ex;
        //     }
        // }

        public string ValidateCreateTableBlock(string block)
        {
            // Validar que no haya comas antes de ); si hay "ENGINE"
            if (Regex.IsMatch(block, @",\s*\)\s*ENGINE", RegexOptions.IgnoreCase))
            {
                block = Regex.Replace(block, @",\s*(\)\s*ENGINE)", "$1", RegexOptions.IgnoreCase);
            }

            // Validar que no haya FOREIGN KEY o CONSTRAINT no deseadas
            if (Regex.IsMatch(block, @"CONSTRAINT.*?FOREIGN KEY", RegexOptions.IgnoreCase))
            {
                string pattern = @"CONSTRAINT.*?FOREIGN KEY.*?,";
                string cleanedTableDefinition = Regex.Replace(block, pattern, string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);

                pattern = @"CONSTRAINT.*?FOREIGN KEY.*?\)";
                cleanedTableDefinition = Regex.Replace(cleanedTableDefinition, pattern, ")", RegexOptions.IgnoreCase | RegexOptions.Singleline);

                block = cleanedTableDefinition;
                //throw new Exception("Se encontró una restricción FOREIGN KEY en el bloque CREATE TABLE.");
            }

            return block;
        }

        private async Task BackupAllFileAsync(string zipFileName, string outputFolder, string outputFile, string outputExtension)
        {
            try
            {
                string destFileName = outputFile + DateTime.Now.ToString("yyyyMMddHHmmss") + outputExtension;
                string destFullPath = outputFolder + this._folderSepa + destFileName;

                // UPLOAD TO S3
                bool result = await _amazonS3Bucket.UploadFileToS3(zipFileName,destFileName);

                if (!result)
                {
                    string msgS3 = $"Could not upload {zipFileName} to {this._amazonS3BucketName}.";
                    _logger.LogError(msgS3);
                    throw new Exception(msgS3);
                }

                File.Move(zipFileName, destFullPath);
            }
            catch (Exception exc)
            {
                _logger.LogError($"An error occurred during the process 'BackupAllFileAsync' module: {exc}");
                throw exc;
            }
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore;
using UalaAccounting.api.ApplicationCore;
using UalaAccounting.api.EntitiesDB;
using UalaAccounting.api.Models;
using UalaAccounting.api.Services;
//using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Extensions
{
    public static class GroupExtensionServices
    {
        public static IServiceCollection AddServicesMambu(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IParseAccountingData, ParseAccountingData>();
            services.AddScoped<IBusinessLogic, BusinessLogic>();
            services.AddScoped<IConfigurationApp, ConfigurationApp>();
            services.AddScoped<IDbLogger, DbLogger>();
            services.AddScoped<IConfigTemplateData, ConfigTemplateData>();
            services.AddScoped<IReclasificationProcess, ReclasificationProcess>();
            services.AddScoped<ITableConfigurationData, TableConfigurationData>();
            services.AddScoped<IAccountingEntriesData, AccountingEntriesData>();
            services.AddScoped<IProductAccountingHubData, ProductAccountingHubData>();
            services.AddScoped<IConfigurationSheetsData, ConfigurationSheetsData>();
            services.AddScoped<ISheetParametrizationData, SheetParametrizationData>();

            services.AddScoped<IProcessOrchestration, ProcessOrchestration>();

            services.AddScoped<ILoanProductData, LoanProductData>();
            services.AddScoped<IGlAccountData, GlAccountData>();
            services.AddScoped<IAccountChartData, AccountChartData>();
            services.AddScoped<IConfigurationData, ConfigurationData>();
            services.AddScoped<IFormedSqlExecution, FormedSqlExecution>();

            services.AddScoped<IAmazonS3Bucket, AmazonS3Bucket>();
            services.AddScoped<IApiMambuServices, ApiMambuServices>();
            services.AddScoped<IBackupProcess, BackupProcess>();
            services.AddScoped<ICustomQueryInDatabaseServices, CustomQueryInDatabaseServices>();
            services.AddScoped<IConfigurationSheetColumnsData, ConfigurationSheetColumnsData>();
            services.AddScoped<TablesInMemoryModel>();

            services.AddScoped<IParseLoanAccountHistoryData, ParseLoanAccountHistoryData>();
            services.AddScoped<ILoanAccountHistoryProcess, LoanAccountHistoryProcess>();
            services.AddScoped<IShellServices, ShellServices>();
            services.AddScoped<INotificationServices, NotificationServices>();

            services.AddDbContextPool<ContaContext>((serviceProvider, options) =>
            {
                var connectionString = configuration["ACHUB"];
                options.UseMySql(connectionString,
                    ServerVersion.AutoDetect(connectionString), options => options.CommandTimeout(12000));

            });

            return services;
        }
    }
}

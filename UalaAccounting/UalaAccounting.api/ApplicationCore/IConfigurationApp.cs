using System;
namespace UalaAccounting.api.ApplicationCore
{
	public interface IConfigurationApp
	{
        Task<MemoryStream> GetConfigTemplate(string accountChartId);
        Task UpdateConfigTemplate(IFormFile file);
        Task<List<string>> GetAccountChartNameAsync();
        Task CreateConfigTemplate(IFormFile file);
    }
}


using System;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Services
{
	public interface IConfigTemplateData
    {
        Task<MemoryStream> getConfigurationTemplate();
        Task CreateConfigurationTemplate(IFormFile file, List<Configurationsheet> structures, List<Configurationsheetcolumn> structureColumns);
        Task<MemoryStream> getConfigurationTemplate(List<Configurationsheet> structures, List<Configurationsheetcolumn> structureColumns, List<Accountchart> accountChart, List<string> stagesName, List<Sheetparametrization> stages);
        Task ProcessUpdateConfigurationTemplate(IFormFile file, List<Configurationsheet> structures, List<Configurationsheetcolumn> structureColumns);
    }
}
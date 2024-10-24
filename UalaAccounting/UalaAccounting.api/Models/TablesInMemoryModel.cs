using System;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Models
{
	public class TablesInMemoryModel
    {
        public List<Sheetparametrization> reclassificationRuleList { get; set; }
        public List<Configurationaccountinghub> ConfigurationList { get; set; }
        public List<Productaccountinghub> ProductList { get; set; }
    }
}


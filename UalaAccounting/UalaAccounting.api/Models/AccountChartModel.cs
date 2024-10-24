using System;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Models
{
	public class AccountChartModel
	{
        public string AccountChartId { get; set; }
        public List<Accountchart> AccountChartItems { get; set; }
    }
}


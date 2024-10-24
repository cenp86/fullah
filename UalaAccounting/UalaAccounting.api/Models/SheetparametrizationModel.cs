using System;
using UalaAccounting.api.EntitiesDB;

namespace UalaAccounting.api.Models
{
	public class SheetparametrizationModel
	{
		public List<Sheetparametrization> stages { get; set; }
		public AccountChartModel AccountChartItemModel { get; set; }
		public ProductModel ProductItemModel { get; set; }
	}
}


using System;
namespace UalaAccounting.api.Models
{
	public class ConfigurationSheetModel
	{
		public string Name { get; set; }
		public int StartPositicion { get; set; }
		public int StartPositionColumName { get; set; }
		public int  NumberColumn{ get; set; }
		public bool RequiredId { get; set; }
		public int StartPositionID { get; set; }
	}
}


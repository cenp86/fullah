using System;
namespace UalaAccounting.api.Models
{
	public class BackupRequestModel
	{
        public string TenantId { get; set; }
        public string Result { get; set; }
        public string Domain { get; set; }
        public string FileName { get; set; }
    }
}


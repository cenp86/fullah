using System;
using Newtonsoft.Json;

namespace UalaAccounting.api.Models
{
    public class CustomerBackupRequest
    {
        public string callback { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? createBackupFromDate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string>? tables { get; set; }
    }
}


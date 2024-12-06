using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UalaAccounting.api.Models
{
    public class OrderAccountModel
    {
        public string type { get; set; }
        public string glAccountCode { get; set; }
        public string glAccountName { get; set; }        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFY.Fatura.Models
{
    public class CreatedInvoiceModel
    {
        public FoundDraftInvoiceModel data { get; set; }
        public string uuid { get; set; }
        public bool signed { get; set; }
    }
}

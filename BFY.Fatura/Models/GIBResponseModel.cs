using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFY.Fatura.Models
{
    public class GIBResponseModel<T>
    {
        public T data { get; set; }
        public string message { get; set; }
        public bool status { get; set; }
        public MetaData metadata { get; set; }
    }

    public class MetaData
    {
        public string optime { get; set; }
    }
}

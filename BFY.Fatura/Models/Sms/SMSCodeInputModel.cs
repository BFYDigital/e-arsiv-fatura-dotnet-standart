using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFY.Fatura.Models.Sms
{
    public class SMSCodeInputModel
    {
        public string SIFRE { get; set; }
        public string OID { get; set; }
        public int OPR { get; set; }
        public List<SmsData> DATA { get; set; }
    }

    public class SmsData
    {
        public string belgeNumarasi { get; set; }
        public string aliciVknTckn { get; set; }
        public string aliciUnvanAdSoyad { get; set; }
        public string saticiVknTckn { get; set; }
        public string saticiUnvanAdSoyad { get; set; }
        public string belgeTarihi { get; set; }
        public string belgeTuru { get; set; }
        public string onayDurumu { get; set; }
        public string ettn { get; set; }
        public string talepDurumColumn { get; set; }
        public int talepDurum { get; set; }
        public int iptalItiraz { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BFY.Fatura.Helpers
{
    class UnicodeHelper
    {
        public static string TurkishCharFix(string text)
        {
            text = text.Replace("İ", "I");
            text = text.Replace("Ç", "C");
            text = text.Replace("Ö", "O");
            text = text.Replace("Ü", "U");
            text = text.Replace("Ş", "S");
            text = text.Replace("Ğ", "G");
            text = text.Replace("ü", "u");
            text = text.Replace("ö", "ö");
            text = text.Replace("ç", "c");
            text = text.Replace("ı", "i");
            text = text.Replace("ş", "s");
            text = text.Replace("ğ", "g");

            return text;
        }
    }
}

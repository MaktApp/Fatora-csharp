using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaymentTempalte.Models
{
    public class Currency
    {
        public int CurrencyId { set; get; }
        public string CurrencyCode { set; get; }

        public string Country { set; get; }
    }
}
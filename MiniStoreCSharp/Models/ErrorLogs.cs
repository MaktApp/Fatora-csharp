using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PaymentTempalte.Models
{
    public class ErrorLogs
    {
        public int ID { get; set; }    
        public DateTime Date { get; set; }
        public string ErrorDescription { get; set; }
       
        public string Controller { get; set; }

    }
}
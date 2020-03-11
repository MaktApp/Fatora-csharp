using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PaymentTempalte.Models
{
    public class Activity
    {
        [Key]
        public int ActivityID { set; get; }
        public string ActivityDescription { set; get; }
        public DateTime ActivityDate { set; get; }
        public string Controller { set; get; }
        public string Paramters { set; get; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PaymentTempalte.Models
{
    public class FatoraMerchants
    {

        [Key]
        public int id { set; get; }     
        public Guid Token { get; set; }      
        public string MerchantName { get; set; }       
        public string MerchantEmail { set; get; }      
        public string CompanyName { set; get; }
        public string Phone { set; get; }              
        public string Country { set; get; }
        public bool IsActive { set; get; }                  
        public int PaymentType { set; get; }
        public string Logo { set; get; }
        public bool IsTest { set; get; }
        public List<Payments> Payments { set; get; }
        
        

    }
}
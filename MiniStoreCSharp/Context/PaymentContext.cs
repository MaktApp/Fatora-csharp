using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PaymentTempalte.Models;
using System.Data.Entity;
//using MakPayement.Migrations;
namespace PaymentTempalte.Context
{
   
        public class PaymentContext : DbContext
        {
            public PaymentContext() : base("name=PaymentConnstring")
            {            
           //Database.SetInitializer(new   MigrateDatabaseToLatestVersion<PaymentContext, Configuration>());
           
        }
         // public DbSet<Merchants> Merchants { set; get; }
         public DbSet<Payments> Payments { set; get; }
        public DbSet<ErrorLogs> Errors { set; get; }
        public DbSet<Activity> Activities { set; get; }
       
        public DbSet<FatoraMerchants> FatoraMerchants { set; get; }
       
        public DbSet<Currency> Currencies { set; get; }
       
    }
    
}
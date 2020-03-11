using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using PaymentTempalte.Helpers;
namespace PaymentTempalte.Models
{
    public class Payments
    {
        [Key]
        public int ID { set; get; }
        public string orderID { set; get; }
             
        public double PaymentAmount { get; set; }
        public string CurrencyCode { set; get; }
        public string CustomerEmail { set; get; }
        public string CustomerCountry { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Paymentstate { get; set; }
        public string PaymentDescription { set; get; }
        public string TransactionId { get; set; }
        public string TrackingId { get; set; }
        public string ResponseToken { get; set; }
        public bool Isrecurring { get; set; }
      //  public int RecurringPaymentID { set; get; }
        public int? RecurringID { set; get; }
        [ForeignKey("RecurringID")]
        public Payments FPayment { set; get; }


        public List<Payments> ChildPayments { set; get; }
        public string Note { set; get; }
        public bool  Refund{ set; get; }
        public string Lang { set; get; }
        public bool OrderState { set; get; }
        public double InterchangeFees { set; get; }
       
        //0 Makshop
        //1 Fatora
        //2 External
        //3FatoraMe
        public Helper. PaymantSource From { set; get; }
        
        public string AuthCode { set; get; }
       

        public int? FMerchantID { get; set; }
        [ForeignKey("FMerchantID")]
        public FatoraMerchants FMerchant { get; set; }


       

        public bool IsTest { set; get; }
        public string FcmToken { set; get; }
        public int FromVistaCount { set; get; }
        public double ExchangeRate { set; get; }
        public double PaidAmount { set; get;  }
       
        public bool IsAutoSave { set; get; }
        public string PayId { set; get; }
      
        public DateTime? PaymentRequestDate { set; get; }
        public Helper.PaymentType PaymentType { set; get; }
        public string RequestInfo { set; get; }
    }
    
}
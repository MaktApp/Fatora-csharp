using PaymentTempalte.Context;
using PaymentTempalte.Manager;
using PaymentTempalte.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;


namespace PaymentTempalte.Helpers
{

    public class Helper
    {
        //payment gate way type
        public enum PaymentType
        {
            vistamoney = 0,
            paypal = 1,
            stripe = 2,
            vapulus=3,
            templateType1=4,
            templateType2 = 5,
                 templateType3 = 6

        }       
        public static PaymentType GetPaymentType(string paymentType)
        {
            if (paymentType == "vistamoney")
                return PaymentType.vistamoney;
            else if (paymentType == "paypal")
                return PaymentType.paypal;
            else if (paymentType == "stripe")
                return PaymentType.stripe;
            else if (paymentType == "vapulus")
                return PaymentType.vapulus;
            else
                return PaymentType.vistamoney;

        }
        public enum PaymantSource
        {
            Maktshop = 0,
            Fatora = 1,
            External = 2,
            Me = 3,
            Demo1 = 4,
            Demo2 = 5,
            POS = 6,
            Maktapp = 7,
            product=8

        }

        public enum MarchantType
        {
            GateWay = 1,
            Fatora = 3,
            MakShop = 4,
            MTaskat = 2,
            MFatora = 5
        }

        public static Payments GetNewpayment(Payments _payment, FatoraMerchants _merchant, double amount, string orderID)
        {
            string tracID = Helper.GetTrackingID();
            string result = "";

            PaymentManager _paymentManger = new PaymentManager();
            string _code = _payment.CurrencyCode;
           
          //get new payment using 
            #region AddPayment
            Payments newPayment = new Payments()
            {
                Isrecurring = false,
                From = _payment.From,
                PaymentAmount = amount,
                PaymentDate = DateTime.Now,
                PaymentRequestDate = DateTime.Now,
                TrackingId = tracID,
               
                PaymentType = PaymentType.vistamoney,
               
                RecurringID = _payment.ID,
                CustomerCountry = _payment.CustomerCountry,
                CustomerEmail = _payment.CustomerEmail,
                CurrencyCode = _payment.CurrencyCode,
                orderID = orderID,
                FMerchantID = _merchant.id,
               
                
                Lang = _payment.Lang,
                OrderState = true,
               
                IsTest = _payment.IsTest,

            };
            #endregion
            return newPayment;
        }
        public static string GetView(string controllerName, string viewName, ViewDataDictionary viewData)
        {
            StringWriter writer = new StringWriter();
            
                return writer.ToString();
            

        }
       
        private static void SMTPSendEmail(MailMessage mailMsg)
        {
            try
            {

               

                SmtpClient smtpClient = new SmtpClient("", Convert.ToInt32(587));
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("", "");
                smtpClient.Credentials = credentials;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = true;

                smtpClient.Send(mailMsg);
            }
            catch (Exception cc)
            {
                PaymentContext _context = new PaymentContext();
                ErrorLogs error = new ErrorLogs();

                error.Date = DateTime.Now;
                error.ErrorDescription = "Exception" + cc.Message;
                error.Controller = "";

                _context.Errors.Add(error);
                _context.SaveChanges();
            }
        }
      
        public static void SendEmail(List<string> To, string subject, string Body, List<string> attchment = null)
        {
            try
            {
                MailMessage mailMsg = new MailMessage();

                mailMsg.To.Add(new MailAddress(To[0], "User"));

                mailMsg.From = new MailAddress("dont_replay@maktApp.com", "MaktApp Payment ");
                foreach (string cc in To)
                {
                    if (cc != To[0])
                        mailMsg.CC.Add(cc);
                }


                mailMsg.Subject = subject;
                mailMsg.BodyEncoding = System.Text.Encoding.UTF8;
                mailMsg.SubjectEncoding = System.Text.Encoding.UTF8;

                if (attchment != null)
                    foreach (string att in attchment)
                    {
                        mailMsg.Attachments.Add(new Attachment(att));
                    }
                string html = "";

                html = Body;




                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));
                mailMsg.IsBodyHtml = true;

                SMTPSendEmail(mailMsg);

            }
            catch (Exception e)
            {
                PaymentContext _context = new PaymentContext();
                ErrorLogs error = new ErrorLogs();

                error.Date = DateTime.Now;
                error.ErrorDescription = "Exception" + e.Message;
                error.Controller = "";

                _context.Errors.Add(error);
                _context.SaveChanges();
            }
        }
        public static bool ValidateCurrencyCode(string currencyCode)
        {
            PaymentContext _context = new PaymentContext();
            Currency _code = _context.Currencies.FirstOrDefault(c => c.CurrencyCode.ToLower() == currencyCode);
            if (_code!=null)               
                return true;
            return false;
        }
       
     
      
        public static string FormatNumber(double num)
        {
            return string.Format("{0:n}", num).Replace(".00", "");
        }
        
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
       
        public static double ConvertToQAR(double amount)
        {
            return amount * 3.62;
        }
       
        public static string GetTrackingID(int length = 6)
        {
            Random r = new Random();
            string tracID = "";

            for (int i = 0; i < 5; i++)
            {

                string newtracID = RandomString(length);

                PaymentManager paymentManager = new PaymentManager();
                Payments payment = paymentManager.GetPaymentByTrackID(tracID);
                if (payment == null)
                {
                    tracID = newtracID;
                    break;
                }

            }
            if (string.IsNullOrEmpty(tracID))
            {
                return RandomString(7);
            }

            return tracID;
        }
       
      
       

       
     
        
      
    }
   
    public class PaymentsParam
    {

        public string token { set; get; }
        public double amount { set; get; }
        public string currencyCode { set; get; }
        public string orderId { set; get; }
        public string note { set; get; }
        public bool isRecurring { set; get; }
        public string customerEmail { set; get; }
        public string customerCountry { set; get; }
        public string customerName { set; get; }
        public string customerPhone { set; get; }
        public string lang { set; get; }
        public string FcmToken { set; get; }
        public string AutoSave { set; get; }
        public int from { set; get; }
        


    }


    public class PaymentClient
    {
        public string Name { set; get; }
        public string Email { set; get; }
        public string Phone { set; get; }
    }

    public class SuccessPayParameter
    {
        public string Trackid { set; get; }
        public string cvv { set; get; }

    }
    public class RecurringParam
    {
        public string token { set; get; }
        public string cardToken { set; get; }
        public double amount { set; get; }
        public string orderID { set; get; }
    }
    public class OrderTransaction
    {
        public string transactionID { set; get; }
        public string orderId { set; get; }
        public string token { set; get; }
    }


}
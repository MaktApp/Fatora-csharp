using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PaymentTempalte.Context;
using PaymentTempalte.Models;
using PaymentTempalte.Manager;
using PaymentTempalte.Helpers;
using System.Globalization;
using System.Threading;
using System.Web.Configuration;


namespace PaymentTempalte.Controllers
{
    public class PayController : Controller
    {
        // GET: Pay
        

        public ActionResult MCPaymentPage(string paymentID)
        {
            PaymentManager paymentManager = new PaymentManager();
             MarchentManager _merchantManager = new MarchentManager();
            Payments _payment = paymentManager.getByPaymentID(paymentID);
            if(_payment!=null)
            {              
                FatoraMerchants _fmrchent = null;                                            
                   //get Merchant by payment type
                    _fmrchent = _merchantManager.GetFMerchantById((int)_payment.FMerchantID,Helper.PaymentType.templateType1);
               
                if (_payment.Paymentstate.ToLower().Equals("success"))
                {
                    _payment.FMerchant = _fmrchent;                    
                    return View("SuccessPage", _payment);

                }
                else if (_payment.Paymentstate.ToLower().Equals("failure"))
                {
                    #region generate trackID for transaction
                    

                    string tracID = Helper.GetTrackingID();
                    Payments payment = new Payments
                    {
                        PaymentAmount = _payment.PaymentAmount,
                        TrackingId = tracID,
                        PaymentDate = DateTime.Now,
                        CurrencyCode = _payment.CurrencyCode,
                        Isrecurring = _payment.Isrecurring,
                        CustomerEmail = _payment.CustomerEmail,
                        CustomerCountry = _payment.CustomerCountry,
                        orderID = _payment.orderID,
                        Note = _payment.orderID,
                        IsTest = _payment.IsTest,
                        Lang = _payment.Lang,
                        IsAutoSave = _payment.IsAutoSave,
                        FMerchantID = _payment.FMerchantID,
                    
                        From = _payment.From,
                        Paymentstate = "PENDING",
                        OrderState = false,
                        Refund = false,
                        FcmToken = _payment.FcmToken,
                        PaymentType = _payment.PaymentType,
                    
                    };
                    paymentManager.AddPayment(payment);
                    _payment = payment;
                    #endregion
                }
                else if (_payment.Paymentstate.ToLower().Equals("pending"))
                {
                    
                   
                        paymentManager.updatePayment(_payment);
                    }                            

                if (_payment.PaymentType == Helper.PaymentType.templateType1)
                {
                    if(   _fmrchent.IsTest)
                        
                    {
                        ViewBag.Merchant = _fmrchent;
                        ViewBag.MerchantName = _fmrchent.CompanyName;
                        ViewBag.Test = _payment.IsTest;
                        if (!_payment.CurrencyCode.Equals("QAR") && !_payment.CurrencyCode.Equals("USD"))
                        {
                            _payment.PaymentAmount = _payment.PaidAmount;
                            _payment.CurrencyCode = "USD";
                        }
                        var ci = new CultureInfo(_payment.Lang);
                        Session["Culture"] = ci;
                        Thread.CurrentThread.CurrentUICulture = ci;
                        _payment.FMerchant = _fmrchent;
                        return View("PaymentPage", _payment);
                    }
                    else

                    {
                       
                        string url = "";
                        //generate url of payment page 
                        return Redirect(url);
                    }
                }
                else if (_payment.PaymentType == Helper.PaymentType.templateType2)

                {
                    ViewBag.Merchant = _fmrchent;
                    ViewBag.MerchantName = _fmrchent.CompanyName;
                    ViewBag.Test = _payment.IsTest;
                    //get payemnt gate way data to add it in payment page and pass it AS viewBag data
                    var ci = new CultureInfo(_payment.Lang);
                    Session["Culture"] = ci;
                    Thread.CurrentThread.CurrentUICulture = ci;
                    _payment.FMerchant = _fmrchent;
                    
                    return View("PaymentPage", _payment);
                }
            }
            return Json(new { result = "Payment not Found" });
        }
        public JsonResult ConatctUs(string Message, int paymentID)
        {
            



            return Json(new
            {
                status = true,
                msg = ""
            });
        }
       
    }
    
}
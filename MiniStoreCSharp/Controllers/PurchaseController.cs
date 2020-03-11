using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;
using PaymentTempalte.Models;
using PaymentTempalte.Manager;
using PaymentTempalte.Context;
using System.Web;
using System.Collections.Specialized;
using PaymentTempalte.Helpers;

namespace PaymentTempalte.Controllers
{
    [RoutePrefix("Purchase")]
    public class PurchaseController : ApiController
    {

     

        [HttpPost, Route("SuccessPay")]
        public IHttpActionResult SuccessPay(SuccessPayParameter _param)
        {
            Activity activity;
            PaymentManager _PaymentManager = new PaymentManager();
            MarchentManager _merchantManager = new MarchentManager();
            ActivityManager _activityManager = new ActivityManager();
           
            
            ErrorManager _errorManager = new ErrorManager();
            try
            {
                Payments payment = _PaymentManager.GetPaymentByTrackID(_param.Trackid);
                if (payment != null)
                {
                    FatoraMerchants _marchent = null;
                    
                        _marchent = _merchantManager.GetFMerchantById((int)payment.FMerchantID);
                    
                  
                    
                    payment.PaymentDate = DateTime.Now;
                   
                    payment.TransactionId = "Test " + payment.orderID.Substring(payment.orderID.LastIndexOf(',') + 1)+payment.ID;
                   
                    payment.AuthCode = "Test " + payment.orderID.Substring(payment.orderID.LastIndexOf(',') + 1)+ payment.ID;
                    payment.Paymentstate = _param.cvv=="123"?  "SUCCESS":"FAILURE";
                    payment.PaymentDescription = "Test Payment";
                    payment.OrderState = true;
                    payment.PaidAmount = payment.PaymentAmount;
                    payment.FromVistaCount = 1;
                  
                    
                    if (((payment.Isrecurring) || payment.IsAutoSave==true) && _param.cvv=="123")
                        payment.ResponseToken = payment.TrackingId + "test";
                    _PaymentManager.updatePayment(payment);
                    if(_param.cvv=="123")

                    {


                        #region redirect to Suscces Url of Merchant

                        string successUrl = "";
                        return Json(new { result = 1, url = successUrl });                       
                        #endregion
                    }
                    else
                    {
                        string FailUrl = "";
                        FailUrl = "";
                        return Json(new { result = 1, url = FailUrl });
                    }
                    
                }
                else
                {
                    #region AddActivity
                    // PaymentContext _context = new PaymentContext();
                    activity = new Activity()
                    {
                        ActivityDate = DateTime.Now,
                        ActivityDescription = "Error: the Payment isn't in database",
                        Controller = Request.RequestUri.AbsoluteUri.ToString()
                    };

                    _activityManager.AddActivity(activity);
                    #endregion
                    return Json(new { result = -2 });

                }
            }
            catch (Exception ex)
            {
                #region Add to Error LOg
                // PaymentContext _context = new PaymentContext();
                ErrorLogs error = new ErrorLogs()
                {
                    Date = DateTime.Now,
                    ErrorDescription = "Exception" + ex.Message + " ,StackTrace: " + ex.StackTrace,

                    Controller = Request.RequestUri.AbsoluteUri.ToString()
                };
                _errorManager.AddError(error);
                #endregion
                #region SendEmail
                string desc = "Exception in Payment, Message:  " + ex.Message + "<br/>";
                desc += "return Url: " + error.ErrorDescription + "<br/>";
                desc += "Url From vistamoney: " + error.Controller + "<br/>";
                List<string> listOfMails = new List<string>();
                listOfMails.Add("dalia@maktapp.com");
                listOfMails.Add("waleed@maktapp.com");
                Helper.SendEmail(listOfMails, "Execption in Payment After retutn from vistamoney", desc);
                //Helper.SendEmail(, "Execption in Payment", desc);
                #endregion

                return Json(new { result = -4 });
            }

        }

        

        }
    }

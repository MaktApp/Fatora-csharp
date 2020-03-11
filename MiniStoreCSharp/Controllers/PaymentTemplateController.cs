using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using PaymentTempalte.Manager;
using PaymentTempalte.Helpers;

using PaymentTempalte.Models;

namespace PaymentTempalte.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("PaymentTypeName/v1")]
    //PaymentTypeName:Name of Payment gateway exp:vapulus, paypal,payfort
    public class PaymentTemplateController : ApiController
    {
        [HttpPost]
        [Route("PaymentTempalate1")]
        public IHttpActionResult PaymentTempalate1(PaymentsParam _param)
        {
            MarchentManager _merchentManager = new MarchentManager();
            FatoraMerchants _fmerchant = null;
            //first step validate paramter and check required paramater, 

            string _token = _param.token;
            Guid token;
            bool isGuid = Guid.TryParse(_token, out token);
            if (!isGuid)
            {
                return Ok(new { result = -8 });
            }
            string resultT = "";
            if (_param.orderId == null)
                resultT += "&OrderID is null";
            if (_param.customerEmail == null)
                resultT += "&customerEmail is null";
          

            Activity activity = null;
            ActivityManager _activityManager = new ActivityManager();

            //defaul paramaters, check if they have values
            if (resultT.Length > 0)
                return Ok(new { result = -10, code = resultT });
            if (_param.AutoSave == null)
                _param.AutoSave = "no";
            if (_param.lang == null)
                _param.lang = "ar";

            if (_param.customerCountry == null)
                _param.customerCountry = "qatar";

            if (_param.currencyCode == null)
                _param.currencyCode = "QAR";

            if (_param.currencyCode.Equals("ريال"))
                _param.currencyCode = "QAR";

            if (_param.currencyCode.Equals("دولار"))
                _param.currencyCode = "USD";
                //third step check currency code if is supported
            
            bool cuurencyCode = Helper.ValidateCurrencyCode(_param.currencyCode);
            if (cuurencyCode)
            {
                //4Step get Merchant with paymentgate way data
                #region GetMarchent                
                        _fmerchant = _merchentManager.GetFatoraMerchant(token);                     
                #endregion
                if (_fmerchant != null)
                {
                    string url = "";


                  // get payment gateway application app for currency

                    //if (_app == null)
                    //    return Ok(new { result = -20 });


                    //5 step add new payment to database
                    #region generate trackID for transaction
                    Random r = new Random();
                    string tracID = Helper.GetTrackingID();
                    #endregion
                    #region AddPayment
                    PaymentManager paymentManager = new PaymentManager();


                    Payments _payment = new Payments
                    {
                        PaymentAmount = _param.amount,
                        TrackingId = tracID,
                        From = (Helper.PaymantSource)_param.from,
                        PaymentDate = DateTime.Now,
                        PaymentRequestDate = DateTime.Now,
                        CurrencyCode = _param.currencyCode,
                        Isrecurring = _param.isRecurring,
                        IsAutoSave = (_param.AutoSave.ToLower().Equals("yes") ? true : false),
                        //CustomerEmail = _param.EmailID,
                        CustomerCountry = _param.customerCountry,
                        orderID = _param.orderId,
                        Note = _param.note,
                        Lang = _param.lang,
                        Paymentstate = "PENDING",
                        OrderState = false,
                        PayId = Helper.GetTrackingID(12),
                        Refund = false,
                        FcmToken = (string.IsNullOrEmpty(_param.FcmToken) ? "" : _param.FcmToken),
                        IsTest = _fmerchant.IsTest,
                        PaymentType = Helper.PaymentType.templateType1
                    };
                    
                        PaymentClient _client = new PaymentClient()
                        {
                            Email = _param.customerEmail
                                                                     ,
                            Name = _param.customerName
                                                                    ,
                            Phone = _param.customerPhone
                        };
                        _payment.CustomerEmail = Newtonsoft.Json.JsonConvert.SerializeObject(_client);
                       
                       
                        _payment.FMerchantID = _fmerchant.id;
                    
                   
                    paymentManager.AddPayment(_payment);
                    #region AddActivity

                    activity = new Activity()
                    {
                        ActivityDate = DateTime.Now,
                        ActivityDescription = "Add Payment from Integration 3, Token:" + token.ToString() +
                                                                       "amount: " + _param.amount.ToString() +
                                                                       "currencyCode  :" + _param.currencyCode
                                                                        + "orderId: " + _param.orderId.ToString() +
                                                                       "customerEmail: " + _param.customerEmail +
                                                                       ",note :" + _param.note +
                                                                       ",isRecurring: " + _param.isRecurring.ToString() +

                                                                        ",autoSave: " + _param.AutoSave +
                                                                       ",lang: " + _param.lang,
                        Controller = Request.RequestUri.AbsoluteUri.ToString()

                    };



                    _activityManager.AddActivity(activity);

                    #endregion

                    #endregion
                    #region ReDirectToVistamoney




                   
                        return Redirect(PaymentTempalte.Helpers.Setting.maktappCreditUrl + "pay/MCPaymentPage?paymentID=" + _payment.PayId);


                    #endregion

                    //return Ok(new { result = url });
                }
                else//the marchent is not in the database
                {
                    #region AddActivity

                    activity = new Activity()
                    {
                        ActivityDate = DateTime.Now,
                        ActivityDescription = "Error: the Merchent isn't in the database, AddPayment",
                        Controller = Request.RequestUri.AbsoluteUri.ToString()
                    };
                    _activityManager.AddActivity(activity);

                    #endregion
                    return Ok(new { result = -1 });
                }
            }


            else if (cuurencyCode == false && isGuid == false)
            {
                #region AddActivity

                activity = new Activity()
                {
                    ActivityDate = DateTime.Now,
                    ActivityDescription = "Error:  the currency code isn't supported in MaktapPayment,  and the token is not vaild GUId, AddPayment",
                    Controller = Request.RequestUri.AbsoluteUri.ToString()
                };
                _activityManager.AddActivity(activity);

                #endregion
                return Ok(new { result = -4 });
            }


            else if (isGuid == false)// currency code is not valide
            {
                #region AddActivity

                activity = new Activity()
                {
                    ActivityDate = DateTime.Now,
                    ActivityDescription = "Error:  token is not valid GUID, AddPayment",
                    Controller = Request.RequestUri.AbsoluteUri.ToString()
                };



                _activityManager.AddActivity(activity);

                #endregion
                return Ok(new { result = -8 });
            }
            else if (cuurencyCode == false)// currency code is not valide
            {
                #region AddActivity

                activity = new Activity()

                {
                    ActivityDate = DateTime.Now,
                    ActivityDescription = "Error:  Currenct code  is not Supported, AddPayment",
                    Controller = Request.RequestUri.AbsoluteUri.ToString()
                };

                _activityManager.AddActivity(activity);

                #endregion
                return Ok(new { results = -9 });

            }
            return Ok();
        }
        [HttpPost]
        [Route("PaymentTempalate2")]
        public IHttpActionResult PaymentTempalate2(PaymentsParam _param)
        {
            MarchentManager _merchentManager = new MarchentManager();
            FatoraMerchants _fmerchant = null;
            //first step validate paramter and check required paramater, 

            string _token = _param.token;
            Guid token;
            bool isGuid = Guid.TryParse(_token, out token);
            if (!isGuid)
            {
                return Ok(new { result = -8 });
            }
            string resultT = "";
            if (_param.orderId == null)
                resultT += "&OrderID is null";
            if (_param.customerEmail == null)
                resultT += "&customerEmail is null";


            Activity activity = null;
            ActivityManager _activityManager = new ActivityManager();

            //defaul paramaters, check if they have values
            if (resultT.Length > 0)
                return Ok(new { result = -10, code = resultT });
            if (_param.AutoSave == null)
                _param.AutoSave = "no";
            if (_param.lang == null)
                _param.lang = "ar";

            if (_param.customerCountry == null)
                _param.customerCountry = "qatar";

            if (_param.currencyCode == null)
                _param.currencyCode = "QAR";

            if (_param.currencyCode.Equals("ريال"))
                _param.currencyCode = "QAR";

            if (_param.currencyCode.Equals("دولار"))
                _param.currencyCode = "USD";
            //third step check currency code if is supported

            bool cuurencyCode = Helper.ValidateCurrencyCode(_param.currencyCode);
            if (cuurencyCode)
            {
                //4Step get Merchant with paymentgate way data
                #region GetMarchent                
                _fmerchant = _merchentManager.GetFatoraMerchant(token);
                #endregion
                if (_fmerchant != null)
                {
                    string url = "";


                    // get payment gateway application app for currency

                    //if (_app == null)
                    //    return Ok(new { result = -20 });


                    //5 step add new payment to database
                    #region generate trackID for transaction
                    Random r = new Random();
                    string tracID = Helper.GetTrackingID();
                    #endregion
                    #region AddPayment
                    PaymentManager paymentManager = new PaymentManager();


                    Payments _payment = new Payments
                    {
                        PaymentAmount = _param.amount,
                        TrackingId = tracID,
                        From = (Helper.PaymantSource)_param.from,
                        PaymentDate = DateTime.Now,
                        PaymentRequestDate = DateTime.Now,
                        CurrencyCode = _param.currencyCode,
                        Isrecurring = _param.isRecurring,
                        IsAutoSave = (_param.AutoSave.ToLower().Equals("yes") ? true : false),
                        //CustomerEmail = _param.EmailID,
                        CustomerCountry = _param.customerCountry,
                        orderID = _param.orderId,
                        Note = _param.note,
                        Lang = _param.lang,
                        Paymentstate = "PENDING",
                        OrderState = false,
                        PayId = Helper.GetTrackingID(12),
                        Refund = false,
                        FcmToken = (string.IsNullOrEmpty(_param.FcmToken) ? "" : _param.FcmToken),
                        IsTest = _fmerchant.IsTest,
                        PaymentType = Helper.PaymentType.templateType2
                    };

                    PaymentClient _client = new PaymentClient()
                    {
                        Email = _param.customerEmail
                                                                 ,
                        Name = _param.customerName
                                                                ,
                        Phone = _param.customerPhone
                    };
                    _payment.CustomerEmail = Newtonsoft.Json.JsonConvert.SerializeObject(_client);


                    _payment.FMerchantID = _fmerchant.id;


                    paymentManager.AddPayment(_payment);
                    #region AddActivity

                    activity = new Activity()
                    {
                        ActivityDate = DateTime.Now,
                        ActivityDescription = "Add Payment from Paymenttempalate, Token:" + token.ToString() +
                                                                       "amount: " + _param.amount.ToString() +
                                                                       "currencyCode  :" + _param.currencyCode
                                                                        + "orderId: " + _param.orderId.ToString() +
                                                                       "customerEmail: " + _param.customerEmail +
                                                                       ",note :" + _param.note +
                                                                       ",isRecurring: " + _param.isRecurring.ToString() +

                                                                        ",autoSave: " + _param.AutoSave +
                                                                       ",lang: " + _param.lang,
                        Controller = Request.RequestUri.AbsoluteUri.ToString()

                    };



                    _activityManager.AddActivity(activity);

                    #endregion

                    #endregion
                    #region ReDirectToVistamoney





                    return Redirect(PaymentTempalte.Helpers.Setting.maktappCreditUrl + "pay/MCPaymentPage?paymentID=" + _payment.PayId);


                    #endregion

                    //return Ok(new { result = url });
                }
                else//the marchent is not in the database
                {
                    #region AddActivity

                    activity = new Activity()
                    {
                        ActivityDate = DateTime.Now,
                        ActivityDescription = "Error: the Merchent isn't in the database, AddPayment",
                        Controller = Request.RequestUri.AbsoluteUri.ToString()
                    };
                    _activityManager.AddActivity(activity);

                    #endregion
                    return Ok(new { result = -1 });
                }
            }


            else if (cuurencyCode == false && isGuid == false)
            {
                #region AddActivity

                activity = new Activity()
                {
                    ActivityDate = DateTime.Now,
                    ActivityDescription = "Error:  the currency code isn't supported in MaktapPayment,  and the token is not vaild GUId, AddPayment",
                    Controller = Request.RequestUri.AbsoluteUri.ToString()
                };
                _activityManager.AddActivity(activity);

                #endregion
                return Ok(new { result = -4 });
            }


            else if (isGuid == false)// currency code is not valide
            {
                #region AddActivity

                activity = new Activity()
                {
                    ActivityDate = DateTime.Now,
                    ActivityDescription = "Error:  token is not valid GUID, AddPayment",
                    Controller = Request.RequestUri.AbsoluteUri.ToString()
                };



                _activityManager.AddActivity(activity);

                #endregion
                return Ok(new { result = -8 });
            }
            else if (cuurencyCode == false)// currency code is not valide
            {
                #region AddActivity

                activity = new Activity()

                {
                    ActivityDate = DateTime.Now,
                    ActivityDescription = "Error:  Currenct code  is not Supported, AddPayment",
                    Controller = Request.RequestUri.AbsoluteUri.ToString()
                };

                _activityManager.AddActivity(activity);

                #endregion
                return Ok(new { results = -9 });

            }
            return Ok();
        }

        [Route("PaymentTempalate3")]
        [HttpPost]
        public IHttpActionResult PaymentTempalate3(PaymentsParam _param)
        {
            if (_param.lang == null)
                _param.lang = "ar";

            if (_param.currencyCode == null)
                _param.currencyCode = "QAR";

            if (_param.customerCountry == null)
                _param.customerCountry = "qatar";

            if (_param.from == 0)
                _param.from = 1;

            if (_param.currencyCode.Trim() == "ريال")
                _param.currencyCode = "QAR";
            if (_param.currencyCode.Trim() == "$")
                _param.currencyCode = "USD";
            string _token = _param.token;
            Guid token;
            bool isGuid = Guid.TryParse(_token, out token);
            if (!isGuid)
            {
                return Ok(new { result = -8 });
            }
            MarchentManager _merchentManager = new MarchentManager();
            FatoraMerchants _fmerchant = null;
            try
            {
                bool cuurencyCode = Helper.ValidateCurrencyCode(_param.currencyCode);

                string url = "";

                Activity _activity = null;
                ActivityManager _activityManager = new ActivityManager();
                if (cuurencyCode)
                {
                    #region GetMarchent   

                    _fmerchant = _merchentManager.GetFatoraMerchant(token);
                    #endregion
                    // get payment gateway application app for currency

                    //if (_app == null)
                    //    return Ok(new { result = -20 });

                    if (_fmerchant.IsActive)
                    {




                        #region generate trackID for transaction

                        string tracID = Helper.GetTrackingID();
                        #endregion
                        #region AddPayment                      
                        PaymentManager paymentManager = new PaymentManager();
                        Payments payment = new Payments
                        {
                            PaymentAmount = _param.amount,
                            TrackingId = tracID,
                            From = (Helper.PaymantSource)_param.from,
                            PaymentDate = DateTime.Now,
                            PaymentRequestDate = DateTime.Now,
                            CurrencyCode = _param.currencyCode,
                            Isrecurring = _param.isRecurring,
                            CustomerEmail = _param.customerEmail,
                            //CustomerEmail = (_param.from!=3? _param.customerEmail:_param.customerName),
                            CustomerCountry = _param.customerCountry,
                            orderID = _param.orderId,
                            Note = _param.note,
                            Lang = _param.lang,
                            FMerchantID = _fmerchant.id,

                            Paymentstate = "PENDING",
                            OrderState = false,
                            Refund = false,
                            IsTest = _fmerchant.IsTest,
                            PayId = Helper.GetTrackingID(12),
                            FromVistaCount = (_fmerchant.IsTest == true ? 1 : 0)
                            ,
                            PaidAmount = _param.amount
                            ,
                            PaymentType = Helper.PaymentType.templateType3
                            // IsAutoSave = (_param.AutoSave.ToLower().Equals("yes") ? true : false),
                        };

                        PaymentClient _client = new PaymentClient()
                        {
                            Name = _param.customerName,
                            Email = _param.customerEmail
                        };
                        payment.CustomerEmail = Newtonsoft.Json.JsonConvert.SerializeObject(_client);

                        paymentManager.AddPayment(payment);
                        #endregion
                        #region ReDirectToVistamoney
                        // url = Setting.maktappCreditUrl + "pay/MCPaymentPage?paymentID=" + payment.PayId;

                        _activity = new Activity()
                        {

                            ActivityDate = DateTime.Now,
                            ActivityDescription = "Open  Payment Page",

                            Controller = "Requesting URL:" + Request.RequestUri.AbsoluteUri.ToString(),


                            Paramters = ",Paramters:{ Token:" + _param.token.ToString() + ",amount:" + _param.amount.ToString() +
                                                    ",currencyCode:" + _param.currencyCode + ",orderId:" + _param.orderId
                                                   + ",note:" + _param.note + ",isRecurring:" + _param.isRecurring.ToString()
                                                   + "customerEmail:" + _param.customerEmail + ",lang:" + _param.lang + "}, Resulting URL:" + url,
                        };
                        _activityManager.AddActivity(_activity);
                        return Ok(new
                        {
                            amount = payment.PaidAmount,
                            //payment gateway data required to opem payment page

                            trackid = payment.TrackingId
                        });
                        #endregion
                    }
                    else//the marchent is not in the database
                    {
                        #region AddActivity                       
                        _activity = new Activity()
                        {
                            ActivityDate = DateTime.Now,
                            ActivityDescription = "Open PaymentPage, No Merchant for Sending token",
                            Controller = Request.RequestUri.AbsoluteUri.ToString(),
                            Paramters = "Paramter:{token:" + _param.token.ToString() + "}"
                        };
                        _activityManager.AddActivity(_activity);
                        #endregion
                        return Redirect(Setting.maktappCreditUrl + "/pay/InactiveMerchant?lang=" + _param.lang);
                        // return Ok("-1");
                    }
                }

                else if (cuurencyCode == false)// the country of customer is not supported
                {
                    #region AddActivity                   
                    _activity = new Activity()
                    {
                        ActivityDate = DateTime.Now,
                        ActivityDescription = "Open Payment Page, Error in  Currency code",
                        Controller = Request.RequestUri.AbsoluteUri.ToString(),
                        Paramters = "Parameter:{CurrencyCode:" + _param.currencyCode + "}"
                    };
                    _activityManager.AddActivity(_activity);
                    #endregion
                    return Ok("-3");
                }

                return Ok();
            }
            catch (Exception e)
            {
                #region Add Error             
                ErrorManager _errorManager = new ErrorManager();
                ErrorLogs _error = new ErrorLogs()
                {
                    Date = DateTime.Now,
                    ErrorDescription = "Exception" + e.Message + " ,stacktrace: " + e.StackTrace,
                    Controller = Request.RequestUri.AbsoluteUri.ToString() +
                               ",Paramters:{ Token:" + _param.token.ToString() +
                                             ",amount:" + _param.amount.ToString() +
                                             ",currencyCode:" + _param.currencyCode +
                                             ",orderId:" + _param.orderId
                                             + ",note:" + _param.note +
                                             ",isRecurring:" + _param.isRecurring.ToString()
                                            + "customerEmail:" + _param.customerEmail +
                                            ",lang:" + _param.lang + "}",
                };
                _errorManager.AddError(_error);
                #endregion
                return Ok("-5");
            }
        }

        // to validate payment in payment gateway if it is success , failure pending
        [Route("Validate")]
        [HttpPost]
        public IHttpActionResult ValidateOrder()
        {
            // return Redirect(ReturnHelper.GetSuccessURl(_payment, _fMerchant, merchanttype, Request.RequestUri.AbsoluteUri.ToString()));
            return Ok();
        }

        [HttpPost]
        [Route("ValidateTransaction")]
        public IHttpActionResult ValidateTransaction(OrderTransaction param)
        {
            Activity activity = null;
            ActivityManager _activitymanager = new ActivityManager();
            try
            {
                string resultT = "";
                if (param.orderId == null)
                    resultT = "Order Id Is null";
                else if (param.token == null)
                    resultT += "Token is Null";
                else if (param.transactionID == null)
                    resultT += "transactionID is Null";

                if (resultT.Length > 0)
                    return Ok(new { result = -1 });

                Guid token;
                if (Guid.TryParse(param.token, out token))
                {
                    PaymentManager manager = new PaymentManager();
                    Payments payment = manager.GetGatewayPaymentByTransID(param.orderId, token, param.transactionID,Helper.PaymentType.templateType1);
                    #region AddActivity
                    activity = new Activity()
                    {
                        ActivityDate = DateTime.Now,
                        ActivityDescription = "ValidatePayment, OrderID:" + param.orderId + " ,Token: " + param.token.ToString(),
                        Controller = Request.RequestUri.AbsoluteUri.ToString()
                    };

                    _activitymanager.AddActivity(activity);

                    #endregion
                    #region Update OrderState
                    if (payment != null)
                    {
                        if ((payment.Paymentstate.ToUpper().Equals("SUCCESS") || payment.Paymentstate.ToLower().Equals("successful")) && (payment.OrderState == false))
                        {
                            payment.OrderState = true;
                            manager.updatePayment(payment);
                        }
                    }
                    #endregion
                    if (payment == null)
                    {
                        return Ok(new { result = -2 });
                    }
                    else
                    {
                        var Foundpayment = new
                        {
                            transactionID = payment.TransactionId,
                            paymentAmount = payment.PaymentAmount,
                            currencyCode = payment.CurrencyCode,
                            customerEmail = payment.CustomerEmail,
                            paymentDate = payment.PaymentDate,
                            paymentstate = payment.Paymentstate,
                            auth = payment.AuthCode,
                           
                            Mode = (payment.IsTest ? "test" : "live"),
                            token = (payment.ResponseToken != null ? payment.ResponseToken : "")

                        };
                        return Ok(new { result = 1, payment = Foundpayment });
                    }



                }
                else
                {
                    return Ok(new { result = -3 });
                }
            }
            catch (Exception ex)
            {
                #region Add Error 

                ErrorLogs error = new ErrorLogs()
                {
                    Date = DateTime.Now,
                    ErrorDescription = "Exception" + ex.Message + " ,stackTrace:  " + ex.StackTrace,

                    Controller = Request.RequestUri.AbsoluteUri.ToString() +
                             "?token :" + param.token ?? "" + ",Order ID: " + param.orderId

                };
                ErrorManager _errorManager = new ErrorManager();
                _errorManager.AddError(error);
                #endregion
                return Ok(new { result = -5 });
            }
        }
        public IHttpActionResult ReturnURl(string result,string trackid, string transactionid,string authcode,double amount,string token="")
        {
           
            try
            {
                PaymentManager paymentManager = new PaymentManager();
                Payments payment = paymentManager.GetPaymentByTrackID(trackid);
                Activity _activity = null;
                ActivityManager _activityManager = new ActivityManager();
                MarchentManager marhcentManager = new MarchentManager();
               
                if (payment != null)
                {
                    FatoraMerchants _marchent = null;
                    
                        _marchent = marhcentManager.GetFMerchantById((int)payment.FMerchantID,Helper.PaymentType.templateType1);
                    
                    
                    #region update Payemnt
                    payment.FromVistaCount = (payment.Paymentstate == "PENDING" || payment.Paymentstate.ToUpper().Equals("FAILURE")) ? 0 : payment.FromVistaCount;
                    payment.TransactionId = (payment.Paymentstate == "PENDING" || payment.Paymentstate.ToUpper().Equals("FAILURE")) ? transactionid : payment.TransactionId;
                    
                    payment.PaymentDate = DateTime.Now;

                    payment.PaidAmount = amount;
                   
                    payment.AuthCode = (payment.Paymentstate == "PENDING" || payment.Paymentstate.ToUpper().Equals("FAILURE")) ? authcode : payment.AuthCode;
                    
                    payment.Paymentstate = (payment.Paymentstate == "PENDING" || payment.Paymentstate.ToUpper().Equals("FAILURE")) ? ((result.ToLower().Trim().Equals("successful") || (result.ToUpper().Trim().Equals("SUCCESS"))) ? "SUCCESS" : "FAILURE") : payment.Paymentstate;
                    payment.FromVistaCount += 1;
                   

                    if (((payment.Isrecurring) && !string.IsNullOrEmpty(token))
                        || ((payment.IsAutoSave) && !string.IsNullOrEmpty(token)))
                    {

                        payment.ResponseToken = token;
                       
                    }


                    paymentManager.updatePayment(payment);

                    #endregion
                    #region AddActivity                 
                    _activity = new Activity()
                    {
                        ActivityDate = DateTime.Now,
                        ActivityDescription = "Update Payment status",
                        Paramters = "{result:" + result + ",trackid:" + trackid + ",token:" + (payment.ResponseToken != null ? payment.ResponseToken : "") + "}"
                    };
                    _activityManager.AddActivity(_activity);
                    #endregion
                    if (result == "SUCCESS")
                        return Redirect("");//SUCCESS URL
                    else
                        return Redirect("");//Failure URL



                }
                else//payment is not in database 
                {
                    #region AddActivity

                    _activity = new Activity()
                    {
                        ActivityDate = DateTime.Now,
                        ActivityDescription = "No Payment In DataBase",
                        Controller = Request.RequestUri.AbsoluteUri.ToString()


                    };



                    _activityManager.AddActivity(_activity);

                    #endregion
                    return Ok(-11);
                    // return null;
                }

            }
            catch (Exception e)
            {

                #region Add Error             
                ErrorManager _errorManager = new ErrorManager();
                ErrorLogs _error = new ErrorLogs()
                {
                    Date = DateTime.Now,
                    ErrorDescription = "Exception" + e.Message + " ,stacktrace: " + e.StackTrace + "paymentId:" +
                                    "tranid:" + (transactionid == null ? "" : transactionid) +
                                    
                                    " result:" + (result == null ? "" : result) + " trackid: " + (trackid == null ? "" : trackid) ,
                                     
                                  
                                        
                    Controller = Request.RequestUri.AbsoluteUri.ToString()


                };
                _errorManager.AddError(_error);
                #endregion
                #region SendEmail
                string desc = "Exception in Payment, Message:  " + e.Message + "<br/>";
                desc += "return Url: " + _error.ErrorDescription + "<br/>";
                desc += "Url From vistamoney: " + _error.Controller + "<br/>";
                List<string> listOfMails = new List<string>();
                listOfMails.Add("dalia@maktapp.com");
                listOfMails.Add("waleed@maktapp.com");
                Helper.SendEmail(listOfMails, "Execption in Payment After retutn from vistamoney", desc);
                // Helper.SendEmail(", "Execption in Payment", desc);
                #endregion

                return Ok("-5");

            }

        }
        public IHttpActionResult SUCCESSURL( string trackid, string transactionid, string authcode, double amount, string token = "")
        {

            try
            {
                PaymentManager paymentManager = new PaymentManager();
                Payments payment = paymentManager.GetPaymentByTrackID(trackid);
                Activity _activity = null;
                ActivityManager _activityManager = new ActivityManager();
                MarchentManager marhcentManager = new MarchentManager();

                if (payment != null)
                {
                    FatoraMerchants _marchent = null;

                    _marchent = marhcentManager.GetFMerchantById((int)payment.FMerchantID, Helper.PaymentType.templateType1);


                    #region update Payemnt
                    payment.FromVistaCount = (payment.Paymentstate == "PENDING" || payment.Paymentstate.ToUpper().Equals("FAILURE")) ? 0 : payment.FromVistaCount;
                    payment.TransactionId = (payment.Paymentstate == "PENDING" || payment.Paymentstate.ToUpper().Equals("FAILURE")) ? transactionid : payment.TransactionId;

                    payment.PaymentDate = DateTime.Now;

                    payment.PaidAmount = amount;

                    payment.AuthCode = (payment.Paymentstate == "PENDING" || payment.Paymentstate.ToUpper().Equals("FAILURE")) ? authcode : payment.AuthCode;

                    payment.Paymentstate = (payment.Paymentstate == "PENDING" || payment.Paymentstate.ToUpper().Equals("FAILURE")) ?  "SUCCESS" : payment.Paymentstate;
                    payment.FromVistaCount += 1;


                    if (((payment.Isrecurring) && !string.IsNullOrEmpty(token))
                        || ((payment.IsAutoSave) && !string.IsNullOrEmpty(token)))
                    {

                        payment.ResponseToken = token;

                    }


                    paymentManager.updatePayment(payment);

                    #endregion
                    #region AddActivity                 
                    _activity = new Activity()
                    {
                        ActivityDate = DateTime.Now,
                        ActivityDescription = "Update Payment status",
                        Paramters =  ",trackid:" + trackid + ",token:" + (payment.ResponseToken != null ? payment.ResponseToken : "") + "}"
                    };
                    _activityManager.AddActivity(_activity);
                    #endregion
                    
                        return Redirect("");//SUCCESS URL
                  



                }
                else//payment is not in database 
                {
                    #region AddActivity

                    _activity = new Activity()
                    {
                        ActivityDate = DateTime.Now,
                        ActivityDescription = "No Payment In DataBase",
                        Controller = Request.RequestUri.AbsoluteUri.ToString()


                    };



                    _activityManager.AddActivity(_activity);

                    #endregion
                    return Ok(-11);
                    // return null;
                }

            }
            catch (Exception e)
            {

                #region Add Error             
                ErrorManager _errorManager = new ErrorManager();
                ErrorLogs _error = new ErrorLogs()
                {
                    Date = DateTime.Now,
                    ErrorDescription = "Exception" + e.Message + " ,stacktrace: " + e.StackTrace + "paymentId:" +
                                    "tranid:" + (transactionid == null ? "" : transactionid) +

                                     " trackid: " + (trackid == null ? "" : trackid),



                    Controller = Request.RequestUri.AbsoluteUri.ToString()


                };
                _errorManager.AddError(_error);
                #endregion
                #region SendEmail
                string desc = "Exception in Payment, Message:  " + e.Message + "<br/>";
                desc += "return Url: " + _error.ErrorDescription + "<br/>";
                desc += "Url From vistamoney: " + _error.Controller + "<br/>";
                List<string> listOfMails = new List<string>();
                listOfMails.Add("dalia@maktapp.com");
                listOfMails.Add("waleed@maktapp.com");
                Helper.SendEmail(listOfMails, "Execption in Payment After retutn from vistamoney", desc);
                // Helper.SendEmail(", "Execption in Payment", desc);
                #endregion

                return Ok("-5");

            }

        }

        public IHttpActionResult FailureURl(string trackid, string transactionid, string authcode, double amount, string token = "")
        {

            try
            {
                PaymentManager paymentManager = new PaymentManager();
                Payments payment = paymentManager.GetPaymentByTrackID(trackid);
                Activity _activity = null;
                ActivityManager _activityManager = new ActivityManager();
                MarchentManager marhcentManager = new MarchentManager();

                if (payment != null)
                {
                    FatoraMerchants _marchent = null;

                    _marchent = marhcentManager.GetFMerchantById((int)payment.FMerchantID, Helper.PaymentType.templateType1);


                    #region update Payemnt
                    payment.FromVistaCount = (payment.Paymentstate == "PENDING" || payment.Paymentstate.ToUpper().Equals("FAILURE")) ? 0 : payment.FromVistaCount;
                    payment.TransactionId = (payment.Paymentstate == "PENDING" || payment.Paymentstate.ToUpper().Equals("FAILURE")) ? transactionid : payment.TransactionId;

                    payment.PaymentDate = DateTime.Now;

                    payment.PaidAmount = amount;

                    payment.AuthCode = (payment.Paymentstate == "PENDING" || payment.Paymentstate.ToUpper().Equals("FAILURE")) ? authcode : payment.AuthCode;

                    payment.Paymentstate = (payment.Paymentstate == "PENDING" || payment.Paymentstate.ToUpper().Equals("FAILURE")) ? "FAILURE" : payment.Paymentstate;
                    payment.FromVistaCount += 1;


                    if (((payment.Isrecurring) && !string.IsNullOrEmpty(token))
                        || ((payment.IsAutoSave) && !string.IsNullOrEmpty(token)))
                    {

                        payment.ResponseToken = token;

                    }


                    paymentManager.updatePayment(payment);

                    #endregion
                    #region AddActivity                 
                    _activity = new Activity()
                    {
                        ActivityDate = DateTime.Now,
                        ActivityDescription = "Update Payment status",
                        Paramters = ",trackid:" + trackid + ",token:" + (payment.ResponseToken != null ? payment.ResponseToken : "") + "}"
                    };
                    _activityManager.AddActivity(_activity);
                    #endregion

                    return Redirect("");//Failure URL




                }
                else//payment is not in database 
                {
                    #region AddActivity

                    _activity = new Activity()
                    {
                        ActivityDate = DateTime.Now,
                        ActivityDescription = "No Payment In DataBase",
                        Controller = Request.RequestUri.AbsoluteUri.ToString()


                    };



                    _activityManager.AddActivity(_activity);

                    #endregion
                    return Ok(-11);
                    // return null;
                }

            }
            catch (Exception e)
            {

                #region Add Error             
                ErrorManager _errorManager = new ErrorManager();
                ErrorLogs _error = new ErrorLogs()
                {
                    Date = DateTime.Now,
                    ErrorDescription = "Exception" + e.Message + " ,stacktrace: " + e.StackTrace + "paymentId:" +
                                    "tranid:" + (transactionid == null ? "" : transactionid) +

                                     " trackid: " + (trackid == null ? "" : trackid),



                    Controller = Request.RequestUri.AbsoluteUri.ToString()


                };
                _errorManager.AddError(_error);
                #endregion
                #region SendEmail
                string desc = "Exception in Payment, Message:  " + e.Message + "<br/>";
                desc += "return Url: " + _error.ErrorDescription + "<br/>";
                desc += "Url From vistamoney: " + _error.Controller + "<br/>";
                List<string> listOfMails = new List<string>();
                listOfMails.Add("dalia@maktapp.com");
                listOfMails.Add("waleed@maktapp.com");
                Helper.SendEmail(listOfMails, "Execption in Payment After retutn from vistamoney", desc);
                // Helper.SendEmail(", "Execption in Payment", desc);
                #endregion

                return Ok("-5");

            }

        }


        [HttpPost]
        [Route("GetPament")]
        public IHttpActionResult GetNextPayment(RecurringParam _param)
        {
            MarchentManager marchentManager = new MarchentManager();
            PaymentManager _paymentManger = new PaymentManager();
            Activity _activity = new Activity();
            ActivityManager _activityManager = new ActivityManager();
            
            Guid newtoken;
            try
            {
                if (Guid.TryParse(_param.token, out newtoken))
                {
                    FatoraMerchants marchent = marchentManager.GetFatoraMerchant(newtoken);
                    Payments _payment = _paymentManger.GetPaymentByResponseToken(_param.cardToken, newtoken, Helper.PaymentType.templateType1);
                    if (_payment == null)
                        return Ok(new { result = 7 });
                    
                    
                    if (_payment != null && (_payment.IsAutoSave == true || _payment.Isrecurring) )
                    {
                        #region AddPayment
                        Payments newPayment = Helper.GetNewpayment(_payment, marchent, _param.amount > 0 ? _param.amount : _payment.PaymentAmount, !string.IsNullOrEmpty(_param.orderID) && !string.IsNullOrWhiteSpace(_param.orderID) ? _param.orderID : _payment.orderID);
                        #endregion
                        if (newPayment.Paymentstate.ToLower().Equals("success") || newPayment.Paymentstate.ToLower().Equals("successful"))
                        {
                            #region add Invoice to Fatora Account 
                            if ((_payment.From == Helper.PaymantSource.External || _payment.From == Helper.PaymantSource.Maktshop))
                            {
                                _activity = new Activity()
                                {
                                    ActivityDate = DateTime.Now,
                                    ActivityDescription = "Sending request to fatora Account to adding Payment Gateway, orderID:" + newPayment.orderID,
                                    Controller = Request.RequestUri.AbsoluteUri.ToString()
                                };
                                _activityManager.AddActivity(_activity);
                              


                            }
                            #endregion
                           
                            #region addPayment in marchernDatabase


                            _activity = new Activity()
                            {
                                ActivityDate = DateTime.Now,
                                ActivityDescription = "Adding new payment to recurring payment " + _param.cardToken,
                                Controller = "Integration Controller",
                                Paramters = ""
                            };
                            #endregion
                            return Json(new
                            {
                                result = 1,
                                transactionid = newPayment.TransactionId,
                                authcode = newPayment.AuthCode,
                                mode = (newPayment.IsTest ? "test" : "live"),

                            });//

                        }
                        else//there is error
                        {
                            //newPayment.Paymentstate = "FAILURE";
                            //_paymentManger.AddPayment(newPayment);
                            return Json(new
                            {
                                result = 0,
                                transactionid = newPayment.TransactionId,
                                description = newPayment.PaymentDescription
                            });
                        }

                    }
                    return Json(new { result = -1 });
                }
                return Json(new { result = -2 });
            }
            catch (Exception ex)
            {
                #region Add Error             
                ErrorManager _errorManager = new ErrorManager();
                ErrorLogs _error = new ErrorLogs()
                {
                    Date = DateTime.Now,
                    ErrorDescription = "Exception" + ex.Message + " ,stacktrace: " + ex.StackTrace,
                    Controller = Request.RequestUri.AbsoluteUri.ToString() +
                               ",Paramters:{ Token:" + _param.token.ToString() +

                                             ",orderId:" + _param.cardToken
                                             + "}"
                };
                _errorManager.AddError(_error);
                #endregion
                return Json(new { result = -5 });
            }


        }



        [HttpPost]
        [Route("StopRecurring")]
        public IHttpActionResult StopRecurring(RecurringParam _param)
        {
            ActivityManager _ActivtyManager = new ActivityManager();
           
            try
            {

                Activity activity = new Activity()
                {
                    ActivityDate = DateTime.Now,
                    ActivityDescription = "Cancel Request From Account :" + (_param.token == null ? " " : _param.token) + " for Order ID :" + (_param.cardToken == null ? " " : _param.cardToken),
                    Controller = Request.RequestUri.AbsoluteUri.ToString()

                };
                _ActivtyManager.AddActivity(activity);
                if (_param.cardToken != null && _param.token != null)
                {
                    Guid newToken;
                    bool isGuid = Guid.TryParse(_param.token, out newToken);
                    if (isGuid)
                    {
                        PaymentManager paymentManager = new PaymentManager();
                        Payments payment = paymentManager.GetPaymentByResponseToken(_param.cardToken, newToken,Helper.PaymentType.templateType1);
                        if (payment != null && payment.Isrecurring == true)
                        {
                            payment.Isrecurring = false;
                            paymentManager.updatePayment(payment);

                            return Ok(new { result = 1 });
                        }
                        else
                            return Ok(new { result = -1 });

                    }
                    else
                    {
                        return Ok(new { result = -2 });
                    }

                }
                else
                {
                    return Ok(new { result = -3 });
                }

            }
            catch (Exception e)
            {


                ErrorLogs error = new ErrorLogs()
                {
                    Date = DateTime.Now,
                    ErrorDescription = "Exception" + e.Message + " From Cancel Request From Account :" + (_param.token == null ? " " : _param.token) + " for Order ID :" + (_param.cardToken == null ? " " : _param.cardToken),
                    Controller = Request.RequestUri.AbsoluteUri.ToString()

                };
                ErrorManager _errorManager = new ErrorManager();
                _errorManager.AddError(error);
                return Ok(-5);
            }
        }

    }
}

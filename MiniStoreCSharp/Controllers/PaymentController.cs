using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Store.Controllers
{
    public class PaymentController : Controller
    {
         
        /*
            This action is requested by Fator gateway after payment has been done,

            This action sends a request to ensure that Fatora gateway which issued it,
            If the result of the response was successfully, the action processes the following:
            1- converting The customer's order into a paid status
            2- returns success page
            If the result of the response was failed, the action
            1- returns the error page with failure message
        */
        public ActionResult Success(string transid, string orderId,string mode)
        {
            if (string.IsNullOrEmpty(transid))
            {
                return HttpNotFound();
            }
            string checkUrl = "https://maktapp.credit/v3/CheckStatus";//?transactionID="+ transid;

            string myParameters = "transactionID=" + transid;
            var resValidate = "";
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                System.Collections.Specialized.NameValueCollection postData =
                    new System.Collections.Specialized.NameValueCollection()
                    {
                        {"transactionID", transid },
                        {"orderId","" },
                        {"token","" }
                    };

                var res = wc.UploadValues(checkUrl, postData);
                resValidate = Encoding.UTF8.GetString(res);
                /*
                
                The above request will returns JSON structured if Fatora gateway find the payment:

                { 
                    "result": 1,
                    "payment":
                    {
                        "transactionID": "XXXXXXX",
                        "amount": XXXX,
                        "currencyCode":  "XXX",
                        "customerEmail": "XXXX",
                        "customerPhone": "XXXX",
                        "customerName":  "XXXX",
                        "paymentDate":   "XXXX",
                        "paymentstate":  "XXXX" [SUCCESS, PENDING,FAILURE], 
                        "auth" : "XXXX",
                        "mode": "XXX" [Live, Test] ,
                        "ExchangeRate":0,
                        "token":null,
                        "description":"Transation Successfull",
                        "refundState":false,
                        "refundstatus":null,
                        "refundDescription":null,
                        "refundTransactionId":null
                    } 
                }
                Response if Fatora gatway dos not find the payment:
                { 
                    "result": 0
                }
                Response in case one of the request parameters is null or not valid:

                { 
                    "result": X
                } [-1, -2, -3, -6, -8, -10, -20, -21 ]
                                
                */
                ValidateResponse result = Newtonsoft.Json.JsonConvert.DeserializeObject<ValidateResponse>(resValidate);
                if (result.result == 1)
                {
                    if (result.payment.Paymentstate == "SUCCESS")
                    {
                        // confirm  customer`s order status as paid
                        // order = getOrderById(orderId);
                        // order.status = "done";
                        // save(order);

                        //return success page
                        return View("Success");

                    }
                }

            }


            return View("Failure");

          
        }
        public ActionResult Failure(string transid, string orderId, string mode,string Failerdescription)
        {
            return View("Failure");
        }

    }
class ValidateResponse
{
    public int result { set; get; }
    public PaymentDetails payment { set; get; }
}
public class PaymentDetails
{
    public string orderID { set; get; }
    public string paymentID { set; get; }
    public decimal PaymentAmount { set; get; }
    public string CurrencyCode { set; get; }
    public string CustomerEmail { set; get; }
    public string PaymentDate { set; get; }
    public string Paymentstate { set; get; }
    public string Mode { set; get; }
    public string PaymentType { set; get; }
    public string AuthCode { set; get; }
    public decimal ExchangeRate { set; get; }
    public decimal PaidAmount { set; get; }
}
}
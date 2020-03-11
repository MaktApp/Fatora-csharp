using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PaymentTempalte.Models;
using PaymentTempalte.Context;
using System.Data.Entity;
using PaymentTempalte.Helpers;
namespace PaymentTempalte.Manager
{
    public class PaymentManager
    {
        private PaymentContext _context;
        
        public PaymentManager()
        {
            this._context = new PaymentContext();
        }

        public Payments GetPaymentByTrackID(string trackid)
        {
            return _context.Payments.FirstOrDefault(p => p.TrackingId == trackid);
        }
        public void AddPayment(Payments payment)
        {
            _context.Payments.Add(payment);
            _context.SaveChanges();
        }

        public Payments getByPaymentID(string paymentID)
        {
            return _context.Payments.FirstOrDefault(payment => payment.PayId == paymentID);
        }

        public void updatePayment(Payments payment)
        {
            _context.Entry(payment).State = EntityState.Modified;
            _context.SaveChanges();
        }



        public Payments GetPaymentByResponseToken(string responseToken, Guid token,Helper.PaymentType paymentType)
        {
            return _context.Payments.FirstOrDefault(p => p.ResponseToken.Equals(responseToken)
                                                    && p.FMerchant.Token == token
                                                    && (p.Paymentstate.Equals("SUCCESS") || p.Paymentstate.Equals("Successful"))
                                                    && p.IsTest == false
                                                    && p.Isrecurring == true
                                                    && p.PaymentType==paymentType);
        }



        public Payments GetGatewayPaymentByTransID(string orderID, Guid token, string transID, Helper.PaymentType _payemnttype)
        {
            return _context.Payments.Include(p => p.FMerchant).
                          OrderByDescending(pay => pay.ID).
                          FirstOrDefault(p => p.orderID == orderID
                                            && p.FMerchant.Token == token
                                            && p.TransactionId != null && p.TransactionId == transID
                                            && p.PaymentType==_payemnttype);
        }

    }
}
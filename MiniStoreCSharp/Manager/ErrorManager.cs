using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PaymentTempalte.Context;
using PaymentTempalte.Models;
namespace PaymentTempalte.Manager
{
    public class ErrorManager
    {
        private PaymentContext _paymentContext;
        public ErrorManager()
        {
            _paymentContext = new PaymentContext();
        }
        public void AddError(ErrorLogs error)
        {
            _paymentContext.Errors.Add(error);
            _paymentContext.SaveChanges();
        }
    }
}
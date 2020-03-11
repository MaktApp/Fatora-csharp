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
    public class MarchentManager
    {
        private PaymentContext _context;

        
        public MarchentManager()
        {
            _context = new PaymentContext();
        }

        public FatoraMerchants  GetFatoraMerchant(Guid token)
        {
            // get merchat with payment gate waydata (ex: paypal Application data)
            return _context.FatoraMerchants.FirstOrDefault(f => f.Token == token);
        }

        public FatoraMerchants GetFMerchantById(int id, Helper.PaymentType _paymentType = Helper.PaymentType.vistamoney)
        {
            //get Merchant with paymentGateway data
           if(_paymentType==Helper.PaymentType.templateType1)
                return _context.FatoraMerchants
                     
                     .FirstOrDefault(m => m.id == id);
            return null;
        }



    }
}
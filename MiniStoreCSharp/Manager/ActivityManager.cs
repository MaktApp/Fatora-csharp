using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PaymentTempalte.Models;
using PaymentTempalte.Context;
namespace PaymentTempalte.Manager
{
    public class ActivityManager
    {
        private PaymentContext _context;
        public ActivityManager ()
        {
            _context = new PaymentContext();

        }
        public void AddActivity(Activity activity)
        {
            _context.Activities.Add(activity);
            _context.SaveChanges();
        }
    }
}
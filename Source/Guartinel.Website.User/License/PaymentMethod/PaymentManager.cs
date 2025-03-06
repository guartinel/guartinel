using Guartinel.Website.User.License.PaymentMethod.MyPayPal;
using Guartinel.Website.User.License.PaymentMethod.MyPayPal.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Guartinel.Website.User.License.PaymentMethod {
   public class PaymentManager {
   public PaymentManager(IPayment instance) {
         _paymentInstance = instance;
   }
      private IPayment _paymentInstance;
      public  IPayment USE {
         get {
            return _paymentInstance;
         }
      }   
   }
}
using Guartinel.Website.User.License.Invoicing.SzamlazzDotHu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Guartinel.Website.User.License.Invoicing {
   public class InvoiceManager {
      public InvoiceManager (IInvoiceAgent agent) {
         this._invoiceAgent = agent;
      }
      private IInvoiceAgent _invoiceAgent;
      public IInvoiceAgent USE {
         get {
            return _invoiceAgent;
         }
      }
   }
}
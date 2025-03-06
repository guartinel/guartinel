using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Guartinel.Kernel.Logging;

namespace Guartinel.Website.User.License {
   public class LicenseOrder {
      public static class LicenseOrderStatusValue {
         public static string PAYMENT_CREATED = "PAYMENT_CREATED";
         public static string PAYMENT_STARTED = "PAYMENT_STARTED";
         public static string PAID = "PAID";
         public static string PAYMENT_FAILED = "PAYMENT_FAILED";
         public static string INVOICE_FAILED = "INVOICE_FAILED";
         public static string PAID_AND_INVOICED = "PAID_AND_INVOICED";
      }
      public LicenseOrder () {
         Statuses = new List<Status>();
      }
      List<Order> _orders = new List<Order>();
      public string GetStatus () {
         return Statuses.Last().Value;
      }
      public void SetCurrentStatus (string status, string message = "") {
         string key = DateTime.UtcNow.ToString();
         Logger.Log($"LicenseOrder.Status adding new status key {key} value {status}");
         Statuses.Add(new License.LicenseOrder.Status() {
            TimeStamp = DateTime.UtcNow.ToString(),
            Value = status,
            Message = message
         }
         );
      }
      public List<Status> Statuses { get; set; }

      public string ID { get; set; }
      public List<Order> Orders { get { return _orders; } set { _orders = value; } }
      public Payment LicensePayment { get; set; }
      public string GuartinelUserAccountEmail { get; set; }
      public BuyerDetails BuyerDetail { get; set; }

      public double GetTotalOrderPrice () {
         double sum = 0;
         foreach ( LicenseOrder.Order order in Orders ) {
            sum += order.Price;
         }
         return sum;
      }

      public class Status {
         public string TimeStamp { get; set; }
         public string Value { get; set; }
         public string Message { get; set; }
      }

      public class Order {
         public string LicenseId { get; set; }
         public double Price { get; set; }
         public string ItemName { get; set; }
         public string Description { get { return $"Payment for: {ItemName} from {StartDate.ToString("yyyy.MM.d.")} to {ExpiryDate.ToString("yyyy.MM.d.")}"; } }
         public int SelectedInterval { get; set; }
         public DateTime StartDate { get; set; }
         public DateTime ExpiryDate {
            get {
               return StartDate.AddMonths(SelectedInterval);
            }
         }
      }

      public class BuyerDetails {
         public string PaymentEmail { get; set; }
         public string FirstName { get; set; }
         public string LastName { get; set; }
         public int ZIPCode { get; set; }
         public string Address { get; set; }
         public string City { get; set; }
      }

      public class Payment {
         public string StartPaymentResult { get; set; }
         public string FinishedPaymentResult { get; set; }
      }
      public override string ToString () {
         return Newtonsoft.Json.JsonConvert.SerializeObject(this);
      }

   }
}

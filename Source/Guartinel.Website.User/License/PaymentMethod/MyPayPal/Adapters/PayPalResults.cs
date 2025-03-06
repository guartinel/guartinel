namespace Guartinel.Website.User.License.PaymentMethod.MyPayPal.Adapters {
   public class PayPalResults {
      public class PayPalResult {
         public PayPalResult() {
            IsSuccess = true ;
         }

         public string ResponseRaw {get ; set ;}
         public bool IsSuccess {get ; set ;}
         public string Error {get ; set ;}
      }

      public class CreatePaymentResult : PayPalResult {
       public string RedirectURL { get; set; }
      }

      public class GetPaymentResult : PayPalResult {
         public string LicenseOrderId {get ; set ;}       
         public string PayerID {get ; set ;}          
      }

      public class ExecutePaymentResult : PayPalResult {
       //  public PayPal.Api.Payment Payment{get;set;}
      }
   }
}

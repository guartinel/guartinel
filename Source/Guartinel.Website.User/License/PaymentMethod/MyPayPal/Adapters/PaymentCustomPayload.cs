using Newtonsoft.Json.Linq ;

namespace Guartinel.Website.User.License.PaymentMethod.MyPayPal.Adapters {
   public class PaymentCustomPayload {
      public PaymentCustomPayload (string customPayload) {
         JObject customDatePayload = JObject.Parse (customPayload) ;
         LicenseId = (string) customDatePayload.GetValue ("id") ;
         StartDate = (string) customDatePayload.GetValue ("startDate") ;
         ExpiryDate = (string) customDatePayload.GetValue ("expiryDate") ;
      }

      public string LicenseId {get ; set ;}
      public string StartDate {get ; set ;}
      public string ExpiryDate {get ; set ;}

      public override string ToString() {
         JObject summary = new JObject() ;

         summary.Add ("expiryDate", ExpiryDate) ;
         summary.Add ("startDate", StartDate) ;
         summary.Add ("id", LicenseId) ;

         return summary.ToString() ;
      }
   }
}

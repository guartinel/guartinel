
using Guartinel.Website.User.License.PaymentMethod.MyPayPal.Adapters ;
using static Guartinel.Website.User.License.PaymentMethod.MyPayPal.Adapters.PayPalResults;

namespace Guartinel.Website.User.License.PaymentMethod {
   public interface IPayment {
        PayPalResults.CreatePaymentResult CreatePayment (LicenseOrder order);
        PayPalResults.ExecutePaymentResult ExecutePayment (string token, string payerId, LicenseOrder licenseOrder);
        PayPalResults.GetPaymentResult GetPayment (string token);      
   }
}
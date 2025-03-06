using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel.Utility ;

namespace Guartinel.Website.User.License.PaymentMethod.MyPayPal.Adapters {
   public  class PayPalNVP :IPayment{
      public static class Constants {
         public const string PAYPAL_LIVE_END_POINT = "https://api-3t.paypal.com/nvp";
         public const string PAYPAL_SAND_BOX_END_POINT = "https://api-3t.sandbox.paypal.com/nvp";
         public const int REQUEST_TIMEOUT = 15000;
         public const string CURRENCY = "USD";
      }  

      private string BuildRedirectURL(string token) {
        return "https://www.paypal.com/cgi-bin/webscr?cmd=_express-checkout&token=" + token;
      }

      private PayPalResults.CreatePaymentResult SetExpressCheckout (LicenseOrder order) {
         Logger.Log("Starting Set Express Checkout");
         PayPalResults.CreatePaymentResult result = new PayPalResults.CreatePaymentResult();

         /* NameValueCollection requestNVP = new NameValueCollection() {
             {"METHOD", "SetExpressCheckout"},
             {"CANCELURL", Utils.GetGuartinelLicensePageAddress() + "/?error=true"},
             {"RETURNURL", Utils.GetGuartinelLicensePageAddress()},
             {"ALLOWNOTE", "1"},
             {"PAYMENTREQUEST_0_PAYMENTACTION", "Sale"},
             {"PAYMENTREQUEST_0_CURRENCYCODE", Constants.CURRENCY},
             {"PAYMENTREQUEST_0_AMT", Convert.ToString (order.Price)},
             {"PAYMENTREQUEST_0_DESC", order.Description},
             {"PAYMENTREQUEST_0_ITEMAMT", Convert.ToString (order.Price)},
             {"L_PAYMENTREQUEST_0_QTY0", "1"},
             {"L_PAYMENTREQUEST_0_AMT0", Convert.ToString (order.Price)},
             {"L_PAYMENTREQUEST_0_NAME0", order.ItemName},
             {"L_PAYMENTREQUEST_0_NUMBER0", "0"},
             {"PAYMENTREQUEST_0_CUSTOM", order.Summary()}
          };

          NameValueCollection resultNVP = new NameValueCollection();
          try {
             resultNVP = SendNVPRequest(requestNVP);
             result.RedirectURL = BuildRedirectURL(resultNVP.Get("TOKEN"));
             string ACK = resultNVP.Get("ACK");
             if ( ACK.Equals("Success") ) result.IsSuccess = true;
             else result.IsSuccess = false;
          } catch ( Exception e ) {
             result.IsSuccess = false;
             result.Error = e.Message;
          }

          result.ResponseRaw = resultNVP.ToString();
          Logger.Log("SetExpressCheckout result:" + result.ResponseRaw);
          return result;*/
         throw new NotImplementedException();
      }

      private PayPalResults.GetPaymentResult GetExpressCheckout (string token) {
         Logger.Log("Starting Get Express Checkout");
         /*
                  PayPalResults.GetPaymentResult result = new PayPalResults.GetPaymentResult();

                  NameValueCollection resultNVP = new NameValueCollection();

                  try {
                     resultNVP = SendNVPRequest(new NameValueCollection() {
                        {"METHOD", "GetExpressCheckoutDetails"},
                        {"TOKEN", token},
                     });

                     result.Email = resultNVP.Get("EMAIL");
                     result.PayerID = resultNVP.Get("PAYERID");
                     result.PaymentAmmount = Convert.ToDouble(resultNVP.Get("PAYMENTREQUEST_0_ITEMAMT"));
                     result.LoadCustomPayLoad(resultNVP.Get("PAYMENTREQUEST_0_CUSTOM"));
                  } catch ( Exception e ) {
                     result.IsSuccess = false;
                     result.Error = e.Message;
                  }
                  Logger.Log("Stopping Get Express Checkout");
                  result.ResponseRaw = resultNVP.ToString();
                  return result;*/
         throw new NotImplementedException();
      }

      private   PayPalResults.ExecutePaymentResult DoExpressCheckout (string token,
            string payerID,
            LicenseOrder order) {
         /*  Logger.Log("Starting Do Express Checkout");

           PayPalResults.ExecutePaymentResult result = new PayPalResults.ExecutePaymentResult();
           NameValueCollection resultNVP = new NameValueCollection();
           ;
           try {
              resultNVP = SendNVPRequest(new NameValueCollection() {
                 {"METHOD", "DoExpressCheckoutPayment"},
                 {"TOKEN", token},
                 {"PAYERID", payerID},
                 {"PAYMENTREQUEST_0_PAYMENTACTION", "Sale"},
                 {"PAYMENTREQUEST_0_CURRENCYCODE", Constants.CURRENCY},
                 {"PAYMENTREQUEST_0_AMT", Convert.ToString (paymentAmount)},
              });
              result.MsgSubID = resultNVP.Get("MSGSUBID");
              string ACK = resultNVP.Get("ACK");
              if ( ACK.Equals("Success") ) result.IsSuccess = true;
              else result.IsSuccess = false;
           } catch ( Exception e ) {
              result.IsSuccess = false;
              result.Error = e.Message;
           }
           result.ResponseRaw = resultNVP.ToString();
           Logger.Log("DoExpressCheckout result:" + result.ResponseRaw);

           return result;*/
         throw new NotImplementedException();
      }

      private   NameValueCollection SendNVPRequest (NameValueCollection nameValueCollection) {
         ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
         nameValueCollection = AddCredentialsToNVP(nameValueCollection);

         string logMessage = "";
         foreach ( string key in nameValueCollection ) {
            logMessage += key + ":" + nameValueCollection[key] + "\n";
         }
         Logger.Log(string.Format("PayPalRequest: {0} ", logMessage));

         string nameValueCollectionString = String.Join("&", nameValueCollection.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(nameValueCollection[a], Encoding.Default)));

         string requestURL = Constants.PAYPAL_LIVE_END_POINT + '?' + nameValueCollectionString;

         HttpWebRequest objRequest = (HttpWebRequest) WebRequest.Create(requestURL);
         objRequest.Timeout = Constants.REQUEST_TIMEOUT;
         objRequest.Method = "POST";
         objRequest.ContentLength = nameValueCollectionString.Length;
         try {
            using ( StreamWriter myWriter = new StreamWriter(objRequest.GetRequestStream(), new UTF8Encoding(false)) ) {
               myWriter.Write(nameValueCollectionString);
            }
         } catch ( Exception e ) {
            Logger.Error($"Cannot send NVP request to PayPal. Error: {e.GetAllMessages()}");
         }

         //Retrieve the Response returned from the NVP API call to PayPal.
         HttpWebResponse objResponse = (HttpWebResponse) objRequest.GetResponse();
         string result;
         using ( StreamReader sr = new StreamReader(objResponse.GetResponseStream()) ) {
            result = sr.ReadToEnd();
         }
         return HttpUtility.ParseQueryString(HttpUtility.UrlDecode(result, Encoding.Default));
         ;
      }

      private   NameValueCollection AddCredentialsToNVP (NameValueCollection nameValueCollection) {
         nameValueCollection.Add(new NameValueCollection() {
            {"USER", "admin_api1.sysment.hu"}, //"zoltan.szabototh-facilitator_api1.sysment.hu"},
            {"PWD", "HBXH6MEAY4CGY4GQ"}, //"524TB7YCK8FTJRLR"},
            {"SIGNATURE", "AFcWxV21C7fd0v3bYYYRCpSSRl31Aaj1jLx5G0LKsgQsBnLdg1XGmvAT"}, //"AFcWxV21C7fd0v3bYYYRCpSSRl31AE2qiWKQwdTawYxqWZ2OpGw3HV5U"},
            {"VERSION", "98.8"}
         });
         return nameValueCollection;
      }

      public PayPalResults.CreatePaymentResult CreatePayment (LicenseOrder order) {
         return SetExpressCheckout(order);
      }

      public PayPalResults.ExecutePaymentResult ExecutePayment (string token,string payerId,LicenseOrder order) {
        return DoExpressCheckout(token, payerId, order);
      }

      public PayPalResults.GetPaymentResult GetPayment (string token) {
         return GetExpressCheckout(token);
      }
   }
}

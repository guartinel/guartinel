using System;
using System.Collections.Generic;
using Guartinel.Kernel.Logging;
using Guartinel.Website.User.Misc;
using PayPal;
using PayPal.Api;
using static Guartinel.Website.User.License.PaymentMethod.MyPayPal.Adapters.PayPalResults;

namespace Guartinel.Website.User.License.PaymentMethod.MyPayPal.Adapters {
   public class PayPalRest : IPayment {
      private static class Auth {
         public static class Live {
            public static string ClientID { get { return @"AUNm4wobKcyGSjaR_EKSbTsQ9lIyatPP_f7LXsn9f63tPzC1Ps5FgBWfnYqjDq8vfwpTWqD0s36LFSn_"; } }
            public static string ClientSecret { get { return @"EJYxV4F2Ws_79kBs0hF6aTjIeH_7t7-GF_SGqt-g38zFww0X-9niXttT2XSEH-QNtIX_xH594haNlHHQ"; } }
         }
         public static class SandBox {
            public static string ClientID { get { return @"ATdmIRQYc6lPWD7SRIdz7CKP3WeVRqimP9EDAwHunWlwlN7KDjUm6aHtXntFJZ8eFoFOh4aeDBn7i3XM"; } }
            public static string ClientSecret { get { return @"ELye_K3nb-1uwLMPdLhMjHbEVyu_Gkm9p1wel3fdtJ4Q1Qb-PQAn1upzPdCet9WZFr1OZqmckewRj3Jv"; } }
         }
      }

      APIContext apiContext;
      Dictionary<string, string> sdkConfig = new Dictionary<string, string>();
      PayPalMode mode;
      public PayPalRest (PayPalMode mode) {
         this.mode = mode;

         if ( mode.Equals(PayPalMode.SANDBOX) ) {
            sdkConfig.Add("mode", "sandbox");
         }
         else {
            sdkConfig.Add("mode", "live");
         }
         Logger.Log($"Using REST PayPal API. In {sdkConfig["mode"]} mode");

         AuthenticatePayPal();
      }
      private void AuthenticatePayPal () {
         string clientID = "";
         string clientSecret = "";

         if ( mode.Equals(PayPalMode.SANDBOX) ) {
            clientID = Auth.SandBox.ClientID;
            clientSecret = Auth.SandBox.ClientSecret;
         }

         if ( mode.Equals(PayPalMode.LIVE) ) {
            clientID = Auth.Live.ClientID;
            clientSecret = Auth.Live.ClientSecret;
         }
         if ( clientID.Length == 0 || clientSecret.Length == 0 ) {
            throw new Exception("AuthenticationPayPal auth data is empty!");
         }
         string accessToken;
         try {
            accessToken = new OAuthTokenCredential(
                 clientID,
                 clientSecret,
                 sdkConfig).GetAccessToken();
         } catch ( PaymentsException e ) {
            Logger.Error("Error while Getting access token: " + e.Response);
            throw;
         }
         Logger.Log($"Authenticated to PP. Access token : {accessToken}");
         apiContext = new APIContext(accessToken) { Config = sdkConfig };
      }

      public PayPalResults.CreatePaymentResult CreatePayment (LicenseOrder licenseOrder) {
         Logger.Log("CreatePayment starting.");
         AuthenticatePayPal();
         Payment paymentToCreate = new Payment {
            intent = "sale",
            payer = new Payer {
               payment_method = "paypal"
            },
            transactions = new List<Transaction>
        {
        new Transaction
        {
            description = "Guartinel license payment",
            invoice_number = Guid.NewGuid().ToString(),
            amount = new Amount
            {
                currency = "USD",
                total = Convert.ToString( licenseOrder.GetTotalOrderPrice()),
                details = new Details
                {
                    tax = "0",
                    shipping = "0",
                    subtotal = Convert.ToString( licenseOrder.GetTotalOrderPrice()), // TODO what is the difference between total and subtotal???
                }
            },

            item_list = new ItemList
            {
                items = new List<Item>    (),

            },
            custom = licenseOrder.ID
        }
            },
            redirect_urls = new RedirectUrls {
               return_url = Utils.GetGuartinelLicensePageAddress(),
               cancel_url = Utils.GetGuartinelLicensePageAddress() + "/?error=true"
            }
         };
         foreach ( LicenseOrder.Order order in licenseOrder.Orders ) {
            paymentToCreate.transactions[0].item_list.items.Add(new Item {
               name = order.ItemName,
               currency = "USD",
               price = Convert.ToString(order.Price),
               quantity = "1",
               sku = "sku"
            });
         }

         Payment createdPayment = null;
         try {
            createdPayment = Payment.Create(apiContext, paymentToCreate);
         } catch ( PaymentsException e ) {
            Logger.Error("Error while CreatePayment: " + e.Response);
            throw;
         } catch ( PayPalException e ) {
            Logger.Error($"Error while CreatePayment: {Newtonsoft.Json.JsonConvert.SerializeObject(e)}");
            throw;
         }
         string paymentResult = Newtonsoft.Json.JsonConvert.SerializeObject(createdPayment);
         licenseOrder.LicensePayment = new License.LicenseOrder.Payment() {
            StartPaymentResult = paymentResult
         };
         Logger.Log($"Payment result: {paymentResult}");
         PayPalResults.CreatePaymentResult result = new PayPalResults.CreatePaymentResult();
         result.RedirectURL = createdPayment.id;
         result.IsSuccess = true;

         return result;
      }

      private string BuildRedirectURL (string approvalToken) {
         if ( mode.Equals(PayPalMode.SANDBOX) ) {
            return "https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_express-checkout&token=" + approvalToken;
         }
         if ( mode.Equals(PayPalMode.LIVE) ) {
            return "https://www.paypal.com/cgi-bin/webscr?cmd=_express-checkout&token=" + approvalToken;
         }
         throw new Exception("PaPal mode is not set correctly to live or sandbox.");
      }

      public PayPalResults.ExecutePaymentResult ExecutePayment (string paymentID, string payerId, LicenseOrder licenseOrder) {
         Logger.Log("ExecutePayment start");
         AuthenticatePayPal();

         Payment payment = new Payment();
         payment.id = paymentID;
         PaymentExecution paymentExecution = new PaymentExecution();
         paymentExecution.payer_id = payerId;
         Payment executedPayment = null;

         try {
            executedPayment = payment.Execute(apiContext, paymentExecution);
            string paymentResult = Newtonsoft.Json.JsonConvert.SerializeObject(executedPayment);
            Logger.Log($"ExecutePayment result: {paymentResult}");
            licenseOrder.LicensePayment.FinishedPaymentResult = paymentResult;
         } catch ( PaymentsException e ) {
            Logger.Error($"Error while ExecutePayment: {Newtonsoft.Json.JsonConvert.SerializeObject(e)}");
            throw;
         }
       
         licenseOrder.BuyerDetail = new LicenseOrder.BuyerDetails() {
            Address = executedPayment.payer.payer_info.shipping_address.line1,
            City = executedPayment.payer.payer_info.shipping_address.city,
            ZIPCode = Convert.ToInt16(executedPayment.payer.payer_info.shipping_address.postal_code),
            FirstName = executedPayment.payer.payer_info.first_name,
            LastName = executedPayment.payer.payer_info.last_name,
            PaymentEmail = executedPayment.payer.payer_info.email
         };
         ExecutePaymentResult result = new ExecutePaymentResult();
         result.IsSuccess = true; // TODO finish     
         return result;
      }

      public PayPalResults.GetPaymentResult GetPayment (string paymentId) {
         Logger.Log("GetPayment start");
         AuthenticatePayPal();

         Payment paymentDetails = null;
         try {
            paymentDetails = Payment.Get(apiContext, paymentId);
         } catch ( PaymentsException e ) {
            Logger.Error("Error while GetPayment: " + e.Response);
            throw;
         }
         string responseRaw = Newtonsoft.Json.JsonConvert.SerializeObject(paymentDetails);
         Logger.Log($"GetPayment result: {responseRaw}");
         GetPaymentResult result = new GetPaymentResult();
         result.ResponseRaw = responseRaw;
         result.LicenseOrderId = paymentDetails.transactions[0].custom;
         result.PayerID = paymentDetails.payer.payer_info.payer_id;
         return result;
      }
   }
}
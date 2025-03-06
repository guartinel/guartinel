using System.Web.Http;
using Guartinel.Communication;
using Guartinel.Website.Common.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Guartinel.Kernel.Logging;
using Guartinel.Website.User.License;
using Guartinel.Website.User.License.PaymentMethod.MyPayPal.Adapters;
using System;
using Guartinel.Kernel;
using Guartinel.Kernel.Utility ;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;

namespace Guartinel.Website.User.Controllers {
   [RoutePrefix(UserWebsiteAPI.License.URL)]
   public class LicenseController : ApiController {
      public class StartBuyingPackageModel {
         [JsonProperty(PropertyName = UserWebsiteAPI.License.StartBuyingLicense.Request.LICENSE)]
         public JObject License { get; set; }

         [JsonProperty(PropertyName = UserWebsiteAPI.License.StartBuyingLicense.Request.ACCOUNT)]
         public JObject Account { get; set; }

         [JsonProperty(PropertyName = ManagementServerAPI.Account.GetStatus.Request.TOKEN)]
         public string Token { get; set; }
      }

      [Route(UserWebsiteAPI.License.StartBuyingLicense.URL_PART)]
      public IHttpActionResult StartBuyingLicense (StartBuyingPackageModel startBuyingPackageModel) {
         Logger.Log($"LicenseController.StartBuyingLicense Model:{Newtonsoft.Json.JsonConvert.SerializeObject(startBuyingPackageModel)} ");
           string token = startBuyingPackageModel.Token;
         Logger.Log("Validating user login");
         // VALIDATE TOKEN
         Common.Connection.IManagementServer.Account.ValidateToken validateTokenRequest = new Common.Connection.IManagementServer.Account.ValidateToken(GuartinelApp.WebRequester, GuartinelApp.Settings.ManagementServer, token);

         if ( !validateTokenRequest.IsSuccess() ) return Json(MessageTool.CreateJObjectWithError(validateTokenRequest.GetError()));
       
           Logger.Log("User token is valid. Requesting all licenses");
         // GET ALL LICENSES FROM MS AND CREATE A LICENSE ORDER BASED ON IT
         IManagementServer.License.GetAvailable getAvailableLicensesRequest = new IManagementServer.License.GetAvailable(GuartinelApp.WebRequester, GuartinelApp.Settings.ManagementServer, token);
         LicenseOrder licenseOrder = LicenseFactory.CreateLicenseOrder(
                                    startBuyingPackageModel.Account,
                                    startBuyingPackageModel.License,
                                    getAvailableLicensesRequest.Licenses);
         Logger.Log($"License order content: {licenseOrder.ToString()}");
         IManagementServer.License.SaveLicenseOrder licenseOrderCreated = new IManagementServer.License.SaveLicenseOrder(GuartinelApp.WebRequester, GuartinelApp.Settings.ManagementServer, token, licenseOrder);
         licenseOrder.ID = licenseOrderCreated.ID;
         Logger.Log("Creating payment from license order");
         // CREATE PAYMENT
         PayPalResults.CreatePaymentResult createPaymentResult = GuartinelApp.PaymentManager.USE.CreatePayment(licenseOrder);
         Logger.Log($"License order content: {licenseOrder.ToString()}");
         licenseOrder.SetCurrentStatus( LicenseOrder.LicenseOrderStatusValue.PAYMENT_STARTED);
         IManagementServer.License.SaveLicenseOrder licenseOrderStarted = new IManagementServer.License.SaveLicenseOrder(GuartinelApp.WebRequester, GuartinelApp.Settings.ManagementServer, token, licenseOrder);

         if ( createPaymentResult.IsSuccess ) {
            JObject successResponse = MessageTool.CreateJObjectWithSuccess();
            successResponse.Add(AllParameters.REDIRECT_URL, createPaymentResult.RedirectURL);
            return Json(successResponse);
         }
         return Json(MessageTool.CreateJObjectWithError(createPaymentResult.Error, createPaymentResult.ResponseRaw));
      }

      public class FinalizeBuyingLicenseModel {
         [JsonProperty(PropertyName = UserWebsiteAPI.License.FinalizeBuyingLicense.Request.PAYER_ID)]
         public string PayerID { get; set; }

         [JsonProperty(PropertyName = UserWebsiteAPI.License.FinalizeBuyingLicense.Request.PAYPAL_TOKEN)]
         public string PaymentID { get; set; }

         [JsonProperty(PropertyName = ManagementServerAPI.Account.GetStatus.Request.TOKEN)]
         public string Token { get; set; }
      }

      [Route(UserWebsiteAPI.License.FinalizeBuyingLicense.URL_PART)]
      public IHttpActionResult FinalizeBuyingLicense (FinalizeBuyingLicenseModel buyPackageModel) {
         Logger.Log($"LicenseController.FinalizeBuyingLicense Model: PayerID{buyPackageModel.PayerID}  PayPalToken {buyPackageModel.PaymentID} Token {buyPackageModel.Token}");
         Logger.Log("Validating user token");
         // VALIDATE USER TOKEN
         string token = buyPackageModel.Token;
         Common.Connection.IManagementServer.Account.ValidateToken validateTokenRequest = new Common.Connection.IManagementServer.Account.ValidateToken(GuartinelApp.WebRequester, GuartinelApp.Settings.ManagementServer, token);
         if ( !validateTokenRequest.IsSuccess() ) return Json(MessageTool.CreateJObjectWithError(validateTokenRequest.GetError()));
         Logger.Log("User token is valid");


         Logger.Log("Getting paymentinfo");
         // GET PAYMENT 
         PayPalResults.GetPaymentResult getPaymentResult = GuartinelApp.PaymentManager.USE.GetPayment(buyPackageModel.PaymentID);

         Logger.Log($"Getting LicenseOrder for license order id: {getPaymentResult.LicenseOrderId}");
         //GET LICENSE ORDER FROM MS
         IManagementServer.License.GetLicenseOrder licenseOrderRequest = new IManagementServer.License.GetLicenseOrder(GuartinelApp.WebRequester, GuartinelApp.Settings.ManagementServer, token, getPaymentResult.LicenseOrderId);
         LicenseOrder licenseOrder = licenseOrderRequest.LicenseOrder;
         Logger.Log($"License order content: {licenseOrder.ToString()}");

         Logger.Log("Checking if payer id is equal to the provided from the client side.");
         //CHECK IF PAYMENT IS VALID
         if ( getPaymentResult.PayerID != buyPackageModel.PayerID ) {
            //request is suspicious reject it
            return Json(MessageTool.CreateInternalSystemErrorJObject("FinalizeBuyLicense PayerID is not equal with the one got from GetExpressCheckoutResult"));
         }
         Logger.Log("Executing the payment");
         // EXECUTE THE PAYMENT
         PayPalResults.ExecutePaymentResult executePaymentResult = GuartinelApp.PaymentManager.USE.ExecutePayment(buyPackageModel.PaymentID, getPaymentResult.PayerID, licenseOrder);
         if ( !executePaymentResult.IsSuccess ) {
            Logger.Error($"Cannot execute payment. Error: {executePaymentResult.Error}");
             licenseOrder.SetCurrentStatus(LicenseOrder.LicenseOrderStatusValue.PAYMENT_FAILED, executePaymentResult.Error);
            Logger.Log("Saving license order with new status.");
            IManagementServer.License.SaveLicenseOrder savePaymentFailed = new IManagementServer.License.SaveLicenseOrder(GuartinelApp.WebRequester, GuartinelApp.Settings.ManagementServer, token, licenseOrder);
            Logger.Log($"License order content: {licenseOrder.ToString()}");

            return Json(MessageTool.CreateJObjectWithError(executePaymentResult.Error));
         }

         Logger.Log($"License order content: {licenseOrder.ToString()}");
         Logger.Log("Adding new license to the account");
         // ADD NEW LICENSE TO USER BY MS  
         IManagementServer.License.AddToAccount addLicenseToAccountRequest = new IManagementServer.License.AddToAccount(GuartinelApp.WebRequester, GuartinelApp.Settings.ManagementServer,
               token,
               licenseOrder.Orders[0].LicenseId,
               licenseOrder.Orders[0].StartDate.ToString(),
               licenseOrder.Orders[0].ExpiryDate.ToString(),
               licenseOrder.Orders[0].Price,
               "License Order ID "+ licenseOrder.ID
               );

         if ( !addLicenseToAccountRequest.IsSuccess() ) {
            Logger.Error("Cannot add license to account..");
            return Json(MessageTool.CreateJObjectWithError(validateTokenRequest.GetError()));
         }

         Logger.Log("Savinng license order as paid");
         // SAVE LICENSE ORDER AS PAID
         licenseOrder.SetCurrentStatus(LicenseOrder.LicenseOrderStatusValue.PAID);
         IManagementServer.License.SaveLicenseOrder savePaid = new IManagementServer.License.SaveLicenseOrder(GuartinelApp.WebRequester, GuartinelApp.Settings.ManagementServer, token, licenseOrder);
         Logger.Log($"License order content: {licenseOrder.ToString()}");

         Logger.Log("Sending invoice");
         // SEND INVOICE
         try {
            GuartinelApp.InvoiceManager.USE.CreateInvoice(licenseOrder);
            Logger.Log($"License order content: {licenseOrder.ToString()}");

         } catch ( Exception e ) {
            Logger.Error($"Cannot send invoice. Err: {e.GetAllMessages()}");
            licenseOrder.SetCurrentStatus(LicenseOrder.LicenseOrderStatusValue.INVOICE_FAILED, e.GetAllMessages());
            Logger.Log($"License order content: {licenseOrder.ToString()}");

            Logger.Log("Saving license order with failed invoicing state.");
            IManagementServer.License.SaveLicenseOrder saveInvoiceFailed = new IManagementServer.License.SaveLicenseOrder(GuartinelApp.WebRequester, GuartinelApp.Settings.ManagementServer, token, licenseOrder);
            throw e;
         }
         Logger.Log("Saving license order as paid and invoiced");
         // SAVE LICENSE ORDER INVOICED AND PAID
         licenseOrder.SetCurrentStatus(LicenseOrder.LicenseOrderStatusValue.PAID_AND_INVOICED);
         IManagementServer.License.SaveLicenseOrder savePaidAndInvoiced = new IManagementServer.License.SaveLicenseOrder(GuartinelApp.WebRequester, GuartinelApp.Settings.ManagementServer, token, licenseOrder);
         Logger.Log($"License order content: {licenseOrder.ToString()}");

         Logger.Log("Payment executing finished returning now.");
         JObject successResponse = MessageTool.CreateJObjectWithSuccess();
         return Json(successResponse);
      }

      public class LicenseGetAvailableModel {
         [JsonProperty(PropertyName = ManagementServerAPI.License.GetAvailable.Request.TOKEN)]
         public string Token { get; set; }
      }

      [Route(UserWebsiteAPI.License.GetAvailable.URL_PART)]
      public IHttpActionResult G3tAvailable (LicenseGetAvailableModel getAvailableModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.License.GetAvailable.FULL_URL, getAvailableModel);
         return Json(result);
      }

      public class ActivateLicenseModel {
         [JsonProperty(PropertyName = ManagementServerAPI.License.ActivateLicense.Request.TOKEN)]
         public string Token { get; set; }

         [JsonProperty(PropertyName = ManagementServerAPI.License.ActivateLicense.Request.LICENSE_ID)]
         public string LicenseId { get; set; }
      }

      [Route(UserWebsiteAPI.License.ActivateLicense.URL_PART)]
      public IHttpActionResult ActivateLicense (ActivateLicenseModel activateLicenseModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.License.ActivateLicense.FULL_URL, activateLicenseModel);
         return Json(result);
      }
   }
}

using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using Guartinel.Communication;
using Guartinel.Website.User.Misc;
using Guartinel.Website.User.Models.Account;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Guartinel.Website.User.Controllers {
   public class Models {
      public class SendNewPasswordModel {
         [JsonProperty(PropertyName = ManagementServerAPI.Account.SendNewPassword.Request.EMAIL)]
         [Required(ErrorMessage = ErrorMessages.EMAIL_REQUIRED)]
         [EmailAddress(ErrorMessage = ErrorMessages.EMAIL_INVALID)]
         public string Email { get; set; }

         [JsonProperty(PropertyName = ManagementServerAPI.Account.SendNewPassword.Request.ADDRESS)]
         public JObject Address { get; set; }
      }

      public class VerifySendNewPasswordModel {
         [JsonProperty(PropertyName = ManagementServerAPI.Account.VerifySendNewPassword.Request.EMAIL)]
         [Required(ErrorMessage = ErrorMessages.EMAIL_REQUIRED)]
         [EmailAddress(ErrorMessage = ErrorMessages.EMAIL_INVALID)]
         public string Email { get; set; }

         [JsonProperty(PropertyName = ManagementServerAPI.Account.VerifySendNewPassword.Request.VERIFICATION_CODE)]
         public string VeriicationCode { get; set; }
      }

      public class DeletAccountModel {
         [JsonProperty(PropertyName = ManagementServerAPI.Account.Delete.Request.EMAIL)]
         [Required(ErrorMessage = ErrorMessages.EMAIL_REQUIRED)]
         [EmailAddress(ErrorMessage = ErrorMessages.EMAIL_INVALID)]
         public string Email { get; set; }

         [JsonProperty(PropertyName = ManagementServerAPI.Account.Delete.Request.PASSWORD)]
         [Required(ErrorMessage = ErrorMessages.PASSWORD_REQUIRED)]
         public string Password { get; set; }
      }

      public class FreezeAccountModel {
         [JsonProperty(PropertyName = ManagementServerAPI.Account.Delete.Request.EMAIL)]
         [Required(ErrorMessage = ErrorMessages.EMAIL_REQUIRED)]
         [EmailAddress(ErrorMessage = ErrorMessages.EMAIL_INVALID)]
         public string Email { get; set; }

         [JsonProperty(PropertyName = ManagementServerAPI.Account.Delete.Request.PASSWORD)]
         [Required(ErrorMessage = ErrorMessages.PASSWORD_REQUIRED)]
         public string Password { get; set; }
      }
   }

   [RoutePrefix(UserWebsiteAPI.Account.URL)]
   public class AccountController : ApiController {
      [System.Web.Http.AllowAnonymous]
      [Route(UserWebsiteAPI.Account.Create.URL_PART)]
      public IHttpActionResult Create (AccountCreateModel accountCreateModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Account.Create.FULL_URL, accountCreateModel);
         return Json(result);
      }

      [System.Web.Http.AllowAnonymous]
      [Route(UserWebsiteAPI.Account.Login.URL_PART)]
      public IHttpActionResult Login (AccountLoginModel accountLoginModel) {
         // Logger.LogDebug("GuartinelApp.Settings.ManagementServer is null: " + (GuartinelApp.Settings.ManagementServer == null));
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Account.Login.FULL_URL, accountLoginModel);
         return Json(result);
      }


      public class UnsubscribeAllEmailModel {
         [JsonProperty(PropertyName = ManagementServerAPI.Alert.UnsubscribeAllEmail.Request.BLACK_LIST_TOKEN)]
         public string BlackListToken { get; set; }
      }

      [System.Web.Http.AllowAnonymous]
      [Route(UserWebsiteAPI.Account.UnsubscribeAllEmail.URL_PART)]
      public IHttpActionResult UnsubscribeAllEmail (UnsubscribeAllEmailModel unsubscribeAllEmailModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Alert.UnsubscribeAllEmail.FULL_URL, unsubscribeAllEmailModel);
         return Json(result);
      }


      public class UnsubscribeFromPackageEmailModel {
         [JsonProperty(PropertyName = ManagementServerAPI.Alert.UnSubscribeFromPackageEmail.Request.BLACK_LIST_TOKEN)]
         public string BlackListToken { get; set; }

         [JsonProperty(PropertyName = ManagementServerAPI.Alert.UnSubscribeFromPackageEmail.Request.PACKAGE_ID)]
         public string PackageID { get; set; }
      }
      [System.Web.Http.AllowAnonymous]
      [Route(UserWebsiteAPI.Account.UnsubscribeFromPackageEmail.URL_PART)]
      public IHttpActionResult UnsubscribeFromPackageEmail (UnsubscribeFromPackageEmailModel unsubscribeFromPackageEmailModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Alert.UnSubscribeFromPackageEmail.FULL_URL, unsubscribeFromPackageEmailModel);
         return Json(result);
      }

      [System.Web.Http.AllowAnonymous]
      [Route(UserWebsiteAPI.Account.SendNewPassword.URL_PART)]
      public IHttpActionResult SendNewPassword (Models.SendNewPasswordModel sendNewPasswordModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Account.SendNewPassword.FULL_URL, sendNewPasswordModel);
         return Json(result);
      }

      [System.Web.Http.AllowAnonymous]
      [Route(UserWebsiteAPI.Account.VerifySendNewPassword.URL_PART)]
      public IHttpActionResult VerifySendNewPassword (Models.VerifySendNewPasswordModel verifySendNewPasswordModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Account.VerifySendNewPassword.FULL_URL, verifySendNewPasswordModel);
         return Json(result);
      }

      [Route(UserWebsiteAPI.Account.Delete.URL_PART)]
      public IHttpActionResult D3leteAccount (Models.DeletAccountModel deletAccountModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Account.Delete.FULL_URL, deletAccountModel);
         return Json(result);
      }

      [Route(UserWebsiteAPI.Account.Freeze.URL_PART)]
      public IHttpActionResult FreezeAccount (Models.FreezeAccountModel freezeAccountModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Account.Freeze.FULL_URL, freezeAccountModel);
         return Json(result);
      }

      [Route(UserWebsiteAPI.Account.Logout.URL_PART)]
      public IHttpActionResult Logout (AuthenticationModel authenticationModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Account.Logout.FULL_URL, authenticationModel);
         return Json(result);
      }

      [Route(UserWebsiteAPI.Account.ValidateToken.URL_PART)]
      public IHttpActionResult ValidateToken (AuthenticationModel authenticationModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Account.ValidateToken.FULL_URL, authenticationModel);
         return Json(result);
      }

      [Route(UserWebsiteAPI.Account.GetAccountInfo.URL_PART)]
      public IHttpActionResult G3tAccountInfo (AccountGetStatusModel accountGetStatusModel) {
         //cannot rename to start with get because MVC will restrict this route to only HTTP GET METHODS
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Account.GetStatus.FULL_URL, accountGetStatusModel);
         return Json(result);
      }

      [Route(UserWebsiteAPI.Account.Update.URL_PART)]
      public IHttpActionResult Update (AccountUpdateModel accountUpdateModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Account.Update.FULL_URL, accountUpdateModel);
         return Json(result);
      }

      [Route(UserWebsiteAPI.Account.Activate.URL_PART)]
      public IHttpActionResult Activate (AccountActivateModel accountActivateModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Account.Activate.FULL_URL, accountActivateModel);
         return Json(result);
      }

      [Route(UserWebsiteAPI.Account.ResendActivationCode.URL_PART)]
      public IHttpActionResult ResendActivationCode (AccountResendActivationCodeModel accountResendActivationCodeModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Account.ResendActivationCode.FULL_URL, accountResendActivationCodeModel);
         return Json(result);
      }
   }
}

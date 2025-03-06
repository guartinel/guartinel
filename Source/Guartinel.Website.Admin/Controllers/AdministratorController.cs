using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Http;
using Guartinel.Communication;
using Guartinel.Kernel.Utility;
using Guartinel.Website.Admin.Misc;
using Guartinel.Website.Common.Error;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Guartinel.Kernel.Logging;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;
using AllSuccessValues = Guartinel.Communication.Strings.Strings.AllSuccessValues;

namespace Guartinel.Website.Admin.Controllers {
   [RoutePrefix (AdminWebsiteAPI.Administrator.URL)]
   public class AdministratorController : ApiController {
      public class AdministratorLoginModel {
         [JsonProperty (PropertyName = AdminWebsiteAPI.Administrator.Login.Request.USER_NAME)]
         [Required (ErrorMessage = ErrorMessages.USERNAME_REQUIRED)]
         public string User_Name {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.Administrator.Login.Request.PASSWORD)]
         [Required (ErrorMessage = ErrorMessages.PASSWORD_REQUIRED)]
         [MinLength (5, ErrorMessage = ErrorMessages.ADMIN_PASSWORD_TOO_SHORT)]
         public string Password {get ; set ;}
      }

      [Route (AdminWebsiteAPI.Administrator.Login.URL_PART)]
      public IHttpActionResult Login (AdministratorLoginModel adminLoginModel) {
         if (GuartinelApp.Settings.AdminAccount.Username != adminLoginModel.User_Name) {
            Logger.Log ("Invalid user name attempt") ;
            throw new CustomException.InvalidUserNameOrPasswordException() ;
         }
         var doubleHash = Hashing.GenerateHash (adminLoginModel.Password, adminLoginModel.User_Name) ;
         if (!doubleHash.Equals (GuartinelApp.Settings.AdminAccount.PasswordHash)) {
            Logger.Log("Invalid password attempt: " + adminLoginModel.Password + " Correct password: " + GuartinelApp.Settings.AdminAccount.PasswordHash) ;
            throw new CustomException.InvalidUserNameOrPasswordException() ;
         }

         GuartinelApp.Settings.AdminAccount.LastLogin = DateTime.UtcNow ;
         GuartinelApp.Settings.AdminAccount.Token = Guid.NewGuid().ToString() ;
         GuartinelApp.SaveSettings() ;

         JObject response = new JObject() ;
         response.Add (AllParameters.TOKEN, GuartinelApp.Settings.AdminAccount.Token) ;
         response.Add (AllParameters.SUCCESS, AllSuccessValues.SUCCESS) ;
         return Json (response) ;
      }

      public class AdminidstratorGetStatusModel : Models.AdminTokenModel {}

      [Route (AdminWebsiteAPI.Administrator.GetStatus.URL_PART)] //cannot rename to GetStatus because MVC will restrict this route to only HTTP GET METHODS
      public IHttpActionResult G3tStatus (AdminidstratorGetStatusModel authenticationModel) {
         JObject response = new JObject() ;
         if (GuartinelApp.Settings.AdminAccount.Token != authenticationModel.Token) throw new CustomException.InvalidTokenException() ;
         response.Add (AdminWebsiteAPI.Administrator.GetStatus.Response.CONFIGURED, GuartinelApp.Settings.AdminAccount.IsFirstConfigurationDone) ;
         response.Add (AdminWebsiteAPI.Administrator.GetStatus.Response.FIRST_NAME, GuartinelApp.Settings.AdminAccount.FirstName) ;
         response.Add (AdminWebsiteAPI.Administrator.GetStatus.Response.LAST_NAME, GuartinelApp.Settings.AdminAccount.LastName) ;
         response.Add (AdminWebsiteAPI.Administrator.GetStatus.Response.SUCCESS, AdminWebsiteAPI.Administrator.GetStatus.Response.SuccessValues.SUCCESS) ;
         return Json (response) ;
      }

      public class AdministratorUpdateModel : Models.AdminTokenModel {
         [JsonProperty (PropertyName = AdminWebsiteAPI.Administrator.Update.Request.USER_NAME)]
         [Required (ErrorMessage = ErrorMessages.USERNAME_REQUIRED)]
         public string Username {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.Administrator.Update.Request.PASSWORD)]
         [Required (ErrorMessage = ErrorMessages.PASSWORD_REQUIRED)]
         [MinLength (ValidatorConstants.MINIMUM_ADMIN_PASSWORD_LENGTH, ErrorMessage = ErrorMessages.ADMIN_PASSWORD_TOO_SHORT)]
         public string Password {get ; set ;}
      }

      [Route (AdminWebsiteAPI.Administrator.Update.URL_PART)]
      public IHttpActionResult Update (AdministratorUpdateModel adminUpdateModel) {
         JObject response = new JObject() ;
         if (GuartinelApp.Settings.AdminAccount.Token != adminUpdateModel.Token) throw new CustomException.InvalidTokenException() ;
         string doubleHash = Hashing.GenerateHash (adminUpdateModel.Password, adminUpdateModel.Username) ;
         GuartinelApp.Settings.AdminAccount.PasswordHash = doubleHash ;
         GuartinelApp.Settings.AdminAccount.Username = adminUpdateModel.Username ;
         GuartinelApp.Settings.AdminAccount.IsFirstConfigurationDone = true ;
         GuartinelApp.SaveSettings() ;

         response.Add (AllParameters.SUCCESS, AllSuccessValues.SUCCESS) ;
         return Json (response) ;
      }

      public class AdministratorLogoutModel : Models.AdminTokenModel {}

      [Route (AdminWebsiteAPI.Administrator.Logout.URL_PART)]
      public IHttpActionResult Logout (AdministratorLogoutModel authenticationModel) {
         JObject response = new JObject() ;

         if (GuartinelApp.Settings.AdminAccount.Token != authenticationModel.Token) throw new CustomException.InvalidTokenException() ;

         GuartinelApp.Settings.AdminAccount.Token = null ;
         GuartinelApp.SaveSettings() ;
         response.Add (AllParameters.SUCCESS, AllSuccessValues.SUCCESS) ;
         return Json (response) ;
      }
   }
}

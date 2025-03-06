using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Http;
using Guartinel.Communication;
using Guartinel.Website.Common.Tools;
using Guartinel.Website.User.Misc;
using Guartinel.Website.User.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel.Utility ;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;
using AllSuccessValues = Guartinel.Communication.Strings.Strings.AllSuccessValues;

namespace Guartinel.Website.User.Controllers {
   [RoutePrefix (UserWebsiteAPI.Administrator.URL)]
   public class AdministratorController : ApiController {
      public class AdministratorRegisterModel : ModelBase {
         [JsonProperty (PropertyName = UserWebsiteAPI.Administrator.Register.Request.USER_NAME)]
         [Required (ErrorMessage = ErrorMessages.USERNAME_REQUIRED)]
         public string User_Name {get ; set ;}

         [JsonProperty (PropertyName = UserWebsiteAPI.Administrator.Register.Request.PASSWORD)]
         [Required (ErrorMessage = ErrorMessages.PASSWORD_REQUIRED)]
         [MinLength (5, ErrorMessage = ErrorMessages.ADMIN_PASSWORD_TOO_SHORT)]
         public string Password {get ; set ;}

         [JsonProperty (PropertyName = UserWebsiteAPI.Administrator.Register.Request.NEW_PASSWORD)]
         public string New_Password {get ; set ;} // already hashed

         [JsonProperty (PropertyName = UserWebsiteAPI.Administrator.Register.Request.NEW_USER_NAME)]
         public string New_User_Name {get ; set ;}

         [JsonProperty (PropertyName = UserWebsiteAPI.Administrator.Register.Request.MANAGEMENT_SERVER_ADDRESS)]
         public string Management_Server_Address {get ; set ;}

         [JsonProperty (PropertyName = UserWebsiteAPI.Administrator.Register.Request.MANAGEMENT_SERVER_PORT)]
         public int Management_Server_Port {get ; set ;}
      }

      [Route (UserWebsiteAPI.Administrator.Register.URL_PART)]
      public IHttpActionResult Register (AdministratorRegisterModel adminRegisterModel) {
         Logger.Log (UserWebsiteAPI.Administrator.Register.URL + " : " + adminRegisterModel.ToString()) ;

         if (GuartinelApp.Settings.AdminAccount.Username != adminRegisterModel.User_Name) {
            throw new Common.Error.CustomException.InvalidUserNameOrPasswordException() ;
            // JObject errorResponse = MessageTool.CreateJObjectWithError (UserWebsiteAPI.Administrator.Register.Response.INVALID_USER_NAME_OR_PASSWORD) ;
            //  return Json (errorResponse) ;
         }

         string doubleHash = Hashing.GenerateHash (adminRegisterModel.Password, adminRegisterModel.User_Name) ;
         if (!doubleHash.Equals (GuartinelApp.Settings.AdminAccount.PasswordHash)) {
            // JObject errorResponse = MessageTool.CreateJObjectWithError (UserWebsiteAPI.Administrator.Register.Response.INVALID_USER_NAME_OR_PASSWORD) ;
            //   return Json (errorResponse) ;
            throw new Common.Error.CustomException.InvalidUserNameOrPasswordException() ;
         }

         GuartinelApp.Settings.AdminAccount.LastLogin = DateTime.UtcNow ;
         GuartinelApp.Settings.AdminAccount.Token = Guid.NewGuid().ToString() ;
         GuartinelApp.Settings.AdminAccount.PasswordHash = adminRegisterModel.New_Password ; // this is already hashed on the admin page
         GuartinelApp.Settings.AdminAccount.Username = adminRegisterModel.New_User_Name ;

         GuartinelApp.Settings.ManagementServer = new Common.Configuration.Data.ManagementServer (
               "Associated MS",
               adminRegisterModel.Management_Server_Address,
               "MS",
               "") ;
         GuartinelApp.SaveSettings() ;

         var loginRequest = new Common.Connection.IManagementServer.Admin.Login (GuartinelApp.WebRequester, GuartinelApp.Settings.ManagementServer, GuartinelApp.Settings.AdminAccount.Username, GuartinelApp.Settings.AdminAccount.PasswordHash) ;
         loginRequest.ThrowExceptionIfError() ;
         string token = loginRequest.Token ;

         GuartinelApp.Settings.ManagementServer.Token = token ;
         GuartinelApp.SaveSettings() ;

         JObject response = new JObject() ;
         response.Add (AllParameters.TOKEN, GuartinelApp.Settings.AdminAccount.Token) ;
         response.Add (AllParameters.SUCCESS, AllSuccessValues.SUCCESS) ;
         return Json (response) ;
      }

      public class AdministratorGetStatusModel {}

      [Route (UserWebsiteAPI.Administrator.GetStatus.URL_PART)]
      public IHttpActionResult G3tStatus (AdministratorGetStatusModel authenticationModel) {
         //cannot rename to start with get because MVC will restrict this route to only HTTP GET METHODS
         JObject response = new JObject() ;
         float cpu = PerformanceMonitorTool.GetCPUUsage() ;
         float memory = PerformanceMonitorTool.GetMemoryUsage() ;
         float storage = PerformanceMonitorTool.GetStorageUsage() ;

         response.Add (AllParameters.CPU, cpu) ;
         response.Add (AllParameters.MEMORY, memory) ;
         response.Add (AllParameters.STORAGE, storage) ;
         response.Add (AllParameters.SUCCESS, AllSuccessValues.SUCCESS) ;
         return Json (response) ;
      }
   }
}

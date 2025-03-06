using System.Web.Http ;
using Guartinel.Communication ;
using Guartinel.Website.Admin.Attributes ;
using Guartinel.Website.Admin.Models ;
using Guartinel.Website.Common.Connection ;
using Guartinel.Website.Common.Error ;
using Guartinel.Website.Common.Tools ;
using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Website.Admin.Controllers {
   [RoutePrefix (AdminWebsiteAPI.UserWebServer.URL)]
   public class UserWebServerController : ApiController {
      public class UserWebServerRegisterModel : AdminTokenModel {
         [JsonProperty (PropertyName = AdminWebsiteAPI.UserWebServer.Register.Request.PASSWORD)]
         public string Password {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.UserWebServer.Register.Request.USER_NAME)]
         public string User_Name {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.UserWebServer.Register.Request.USER_WEB_SERVER_ADDRESS)]
         public string User_Web_Server_Address {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.UserWebServer.Register.Request.NAME)]
         public string Name {get ; set ;}
      }

      [Route (AdminWebsiteAPI.UserWebServer.Register.URL_PART)]
      [ValidateWebsiteToken]
      public IHttpActionResult Register (UserWebServerRegisterModel userWebServerRegisterModel) {
         if (GuartinelApp.Settings.ManagementServer == null) {
            //first we have to check the MS presence, if not found we must cancel the process because without MS the userwebserver cannot register itselft to it
            throw new CustomException.MissingManagementServerException() ;
         }

         JObject requestModel = new JObject() ;
         requestModel.Add (UserWebsiteAPI.Administrator.Register.Request.PASSWORD, userWebServerRegisterModel.Password) ;
         requestModel.Add (UserWebsiteAPI.Administrator.Register.Request.USER_NAME, userWebServerRegisterModel.User_Name) ;
         requestModel.Add (UserWebsiteAPI.Administrator.Register.Request.NEW_USER_NAME, GuartinelApp.Settings.AdminAccount.Username) ;
         requestModel.Add (UserWebsiteAPI.Administrator.Register.Request.NEW_PASSWORD, GuartinelApp.Settings.AdminAccount.PasswordHash) ;
         requestModel.Add (UserWebsiteAPI.Administrator.Register.Request.MANAGEMENT_SERVER_ADDRESS, GuartinelApp.Settings.ManagementServer.Address) ;

         Guartinel.Website.Common.Configuration.Data.UserWebServer tempUserWebServer = new Guartinel.Website.Common.Configuration.Data.UserWebServer (StringTool.EnsureStringEndsToBackSlash (userWebServerRegisterModel.User_Web_Server_Address), userWebServerRegisterModel.Name) ;
         JObject userWebServerResponse = GuartinelApp.WebRequester.SendRequestTo (tempUserWebServer,
               UserWebsiteAPI.Administrator.Register.URL,
               requestModel) ;
         if (MessageTool.IsSuccess (userWebServerResponse)) {
            //if the registration was success, than save it
            GuartinelApp.Settings.UserWebServer = tempUserWebServer ;
            GuartinelApp.SaveSettings() ;

            Common.Connection.IManagementServer.Admin.SetWebSiteAddress setWebsiteRequest = new Common.Connection.IManagementServer.Admin.SetWebSiteAddress ( GuartinelApp.WebRequester,GuartinelApp.Settings.ManagementServer,GuartinelApp.Settings.ManagementServer.Token, userWebServerRegisterModel.User_Web_Server_Address) ;
            setWebsiteRequest.ThrowExceptionIfError() ;

            return Json (MessageTool.CreateJObjectWithSuccess()) ;
         }
         return Json (userWebServerResponse) ;
      }

      public class UserWebServerGetStatusModel : AdminTokenModel {}

      [ValidateWebsiteToken]
      [Route (AdminWebsiteAPI.UserWebServer.GetStatus.URL_PART)]
      public IHttpActionResult G3tStatus (UserWebServerGetStatusModel userWebServerGetStatusModel) {
         if (GuartinelApp.Settings.UserWebServer == null) throw new CustomException.MissingUserWebSite() ;

         JObject requestModel = new JObject() ;
         requestModel.Add (UserWebsiteAPI.Administrator.GetStatus.Request.PASSWORD, GuartinelApp.Settings.AdminAccount.PasswordHash) ;
         requestModel.Add (UserWebsiteAPI.Administrator.GetStatus.Request.USER_NAME, GuartinelApp.Settings.AdminAccount.Username) ;

         JObject userWebServerResponse = GuartinelApp.WebRequester.SendRequestTo (GuartinelApp.Settings.UserWebServer, UserWebsiteAPI.Administrator.GetStatus.URL,
               requestModel) ;
         return Json (userWebServerResponse) ;
      }

      public class UserWebServerEditModel : AdminTokenModel {
         [JsonProperty (PropertyName = AdminWebsiteAPI.UserWebServer.Register.Request.USER_WEB_SERVER_ADDRESS)]
         public string User_Web_Server_Address {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.UserWebServer.Register.Request.NAME)]
         public string Name {get ; set ;}
      }

      [Route (AdminWebsiteAPI.UserWebServer.Update.URL_PART)]
      public IHttpActionResult Edit (UserWebServerEditModel userWebServerEditModel) {
         if (GuartinelApp.Settings.UserWebServer == null) throw new CustomException.MissingUserWebSite() ;
         GuartinelApp.Settings.UserWebServer.Name = userWebServerEditModel.Name ;
         GuartinelApp.Settings.UserWebServer.Address = StringTool.EnsureStringEndsToBackSlash (userWebServerEditModel.User_Web_Server_Address) ;

         return Json (MessageTool.CreateJObjectWithSuccess()) ;
      }

      public class UserWebServerGetAvailableModel : AdminTokenModel {}

      [ValidateWebsiteToken]
      [Route (AdminWebsiteAPI.UserWebServer.GetAvailable.URL_PART)]
      public IHttpActionResult G3tAvailable (UserWebServerGetAvailableModel userWebServerGetAvailable) {
         if (GuartinelApp.Settings.UserWebServer == null) throw new CustomException.MissingUserWebSite() ;
         JObject response = MessageTool.CreateJObjectWithSuccess() ;
         response [AdminWebsiteAPI.UserWebServer.GetAvailable.Response.ADDRESS] = GuartinelApp.Settings.UserWebServer.Address ;
         response [AdminWebsiteAPI.UserWebServer.GetAvailable.Response.NAME] = GuartinelApp.Settings.UserWebServer.Name ;

         return Json (response) ;
      }

      public class UserWebServerRemoveModel : AdminTokenModel {}

      [Route (AdminWebsiteAPI.UserWebServer.Remove.URL_PART)]
      [ValidateWebsiteToken]
      public IHttpActionResult Remove (UserWebServerRemoveModel userWebServerRemove) {
         if (GuartinelApp.Settings.UserWebServer == null) throw new CustomException.MissingUserWebSite() ;
         GuartinelApp.Settings.UserWebServer = null ;
         GuartinelApp.SaveSettings() ;
         return Json (MessageTool.CreateJObjectWithSuccess()) ;
      }
   }
}

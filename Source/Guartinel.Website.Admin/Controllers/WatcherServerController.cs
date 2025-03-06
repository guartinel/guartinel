using System.ComponentModel.DataAnnotations ;
using System.Web.Http ;
using Guartinel.Communication ;
using Guartinel.Website.Admin.Attributes ;
using Guartinel.Website.Common.Connection ;
using Guartinel.Website.Common.Error ;
using Newtonsoft.Json ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.Website.Admin.Controllers {
   [RoutePrefix (AdminWebsiteAPI.ManagementServer.WatcherServer.URL)]
   public class WatcherServerController : ApiController {
      public class WatcherServerGetExistingModel : Models.AdminTokenModel {}

      [Route (AdminWebsiteAPI.ManagementServer.WatcherServer.GetExisting.URL_PART)]
      [ValidateWebsiteToken]
      [IsManagementServerAvailable]
      public IHttpActionResult G3tExisting (WatcherServerGetExistingModel authenticationModel) {
         //cannot rename to start with get because MVC will restrict this route to only HTTP GET METHODS
         JObject requestModel = new JObject() ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.GetAvailable.Request.TOKEN, GuartinelApp.Settings.ManagementServer.Token) ;
         JObject result = GuartinelApp.WebRequester.SendRequestTo (GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Admin.WatcherServer.GetAvailable.FULL_URL, requestModel) ;
         return Json (result) ;
      }

      public class WatcherServerGetEventsModel : Models.AdminTokenModel {
         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.WatcherServer.GetStatus.Request.WATCHER_SERVER_ID)]
         [Required (ErrorMessage = ErrorMessages.WATCHER_SERVER_ID_REQUIRED)]
         public string WatcherServerId {get ; set ;}
      }

      [Route (AdminWebsiteAPI.ManagementServer.WatcherServer.GetEvents.URL_PART)]
      [ValidateWebsiteToken]
      [IsManagementServerAvailable]
      public IHttpActionResult G3tEvents (WatcherServerGetEventsModel watcherServerIdentifyModel) {
         //cannot rename to start with get because MVC will restrict this route to only HTTP GET METHODS
         JObject requestModel = new JObject() ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.GetEvents.Request.TOKEN, GuartinelApp.Settings.ManagementServer.Token) ;

         JObject result = GuartinelApp.WebRequester.SendRequestTo (GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Admin.WatcherServer.GetEvents.FULL_URL, requestModel) ;

         return Json (result) ;
      }

      public class WatcherServerGetStatusModel : Models.AdminTokenModel {
         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.WatcherServer.GetStatus.Request.WATCHER_SERVER_ID)]
         [Required (ErrorMessage = ErrorMessages.WATCHER_SERVER_ID_REQUIRED)]
         public string WatcherServerId {get ; set ;}
      }

      [Route (AdminWebsiteAPI.ManagementServer.WatcherServer.GetStatus.URL_PART)]
      [ValidateWebsiteToken]
      [IsManagementServerAvailable]
      public IHttpActionResult G3tStatus (WatcherServerGetStatusModel watcherServerIdentifyModel) {
         //cannot rename to start with get because MVC will restrict this route to only HTTP GET METHODS
         JObject requestModel = new JObject() ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.GetStatus.Request.TOKEN, GuartinelApp.Settings.ManagementServer.Token) ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.GetStatus.Request.WATCHER_SERVER_ID, watcherServerIdentifyModel.WatcherServerId) ;

         JObject result = GuartinelApp.WebRequester.SendRequestTo (GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Admin.WatcherServer.GetStatus.FULL_URL, requestModel) ;

         return Json (result) ;
      }

      public class WatcherServerRegisterModel : Models.AdminTokenModel {
         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.WatcherServer.Register.Request.NAME)]
         [Required (ErrorMessage = ErrorMessages.WATCHER_SERVER_ID_REQUIRED)]
         public string Name {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.WatcherServer.Register.Request.ADDRESS)]
         [Required (ErrorMessage = ErrorMessages.ADDRESS_REQUIRED)]
         public string Address {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.WatcherServer.Register.Request.PORT)]
         [Required (ErrorMessage = ErrorMessages.PORT_REQUIRED)]
         public string Port {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.WatcherServer.Register.Request.USER_NAME)]
         [Required (ErrorMessage = ErrorMessages.USERNAME_REQUIRED)]
         public string UserName {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.WatcherServer.Register.Request.PASSWORD)]
         [Required (ErrorMessage = ErrorMessages.PASSWORD_REQUIRED)]
         public string Password {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.WatcherServer.Register.Request.CATEGORIES)]
         public string[] Categories {get ; set ;}
      }

      [Route (AdminWebsiteAPI.ManagementServer.WatcherServer.Register.URL_PART)]
      [ValidateWebsiteToken]
      [IsManagementServerAvailable]
      public IHttpActionResult Register (WatcherServerRegisterModel watcherServerRegisterModel) {
         JObject requestModel = new JObject() ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.Register.Request.TOKEN, GuartinelApp.Settings.ManagementServer.Token) ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.Register.Request.ADDRESS, watcherServerRegisterModel.Address) ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.Register.Request.NAME, watcherServerRegisterModel.Name) ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.Register.Request.PASSWORD, watcherServerRegisterModel.Password) ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.Register.Request.NEW_USER_NAME, GuartinelApp.Settings.AdminAccount.Username) ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.Register.Request.NEW_PASSWORD, GuartinelApp.Settings.AdminAccount.PasswordHash) ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.Register.Request.CATEGORIES, new JArray (watcherServerRegisterModel.Categories)) ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.Register.Request.PORT, watcherServerRegisterModel.Port) ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.Register.Request.USER_NAME, watcherServerRegisterModel.UserName) ;

         JObject result = GuartinelApp.WebRequester.SendRequestTo (GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Admin.WatcherServer.Register.FULL_URL, requestModel) ;

         return Json (result) ;
      }

      public class WatcherServerRemoveModel : Models.AdminTokenModel {
         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.WatcherServer.GetStatus.Request.WATCHER_SERVER_ID)]
         [Required (ErrorMessage = ErrorMessages.WATCHER_SERVER_ID_REQUIRED)]
         public string WatcherServerId {get ; set ;}
      }

      [Route (AdminWebsiteAPI.ManagementServer.WatcherServer.Remove.URL_PART)]
      [ValidateWebsiteToken]
      [IsManagementServerAvailable]
      public IHttpActionResult Remove (WatcherServerRemoveModel watcherServerIdentifyModel) {
         JObject requestModel = new JObject() ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.Remove.Request.TOKEN, GuartinelApp.Settings.ManagementServer.Token) ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.Remove.Request.WATCHER_SERVER_ID, watcherServerIdentifyModel.WatcherServerId) ;

         JObject result = GuartinelApp.WebRequester.SendRequestTo (GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Admin.WatcherServer.Remove.FULL_URL, requestModel) ;

         return Json (result) ;
      }

      public class WatcherServerUpdateModel : Models.AdminTokenModel {
         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.WatcherServer.GetStatus.Request.WATCHER_SERVER_ID)]
         [Required (ErrorMessage = ErrorMessages.WATCHER_SERVER_ID_REQUIRED)]
         public string WatcherServerId {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.WatcherServer.Update.Request.NAME)]
         [Required (ErrorMessage = ErrorMessages.WATCHER_SERVER_ID_REQUIRED)]
         public string Name {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.WatcherServer.Update.Request.ADDRESS)]
         [Required (ErrorMessage = ErrorMessages.ADDRESS_REQUIRED)]
         public string Address {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.WatcherServer.Update.Request.PORT)]
         [Required (ErrorMessage = ErrorMessages.PORT_REQUIRED)]
         public string Port {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.WatcherServer.Register.Request.CATEGORIES)]
         public string[] Categories {get ; set ;}
      }

      [Route (AdminWebsiteAPI.ManagementServer.WatcherServer.Update.URL_PART)]
      [ValidateWebsiteToken]
      [IsManagementServerAvailable]
      public IHttpActionResult Update (WatcherServerUpdateModel watcherServerUpdateModel) {
         JObject requestModel = new JObject() ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.Update.Request.TOKEN, GuartinelApp.Settings.ManagementServer.Token) ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.Update.Request.ADDRESS, watcherServerUpdateModel.Address) ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.Update.Request.NAME, watcherServerUpdateModel.Name) ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.Update.Request.PORT, watcherServerUpdateModel.Port) ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.Update.Request.WATCHER_SERVER_ID, watcherServerUpdateModel.WatcherServerId) ;
         requestModel.Add (ManagementServerAPI.Admin.WatcherServer.Register.Request.CATEGORIES, new JArray (watcherServerUpdateModel.Categories)) ;
         JObject result = GuartinelApp.WebRequester.SendRequestTo (GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Admin.WatcherServer.Update.FULL_URL, requestModel) ;

         return Json (result) ;
      }
   }
}

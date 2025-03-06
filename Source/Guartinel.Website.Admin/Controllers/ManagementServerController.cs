using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Http;
using Guartinel.Communication;
using Guartinel.Kernel.Utility;
using Guartinel.Website.Admin.Attributes;
using Guartinel.Website.Common.Error;
using Guartinel.Website.Common.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Guartinel.Kernel.Logging;

namespace Guartinel.Website.Admin.Controllers {
   [RoutePrefix (AdminWebsiteAPI.ManagementServer.URL)]
   public class ManagementServerController : ApiController {
      public class ManagementServerAddModel : Models.AdminTokenModel {
         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.Add.Request.NAME)]
         [Required (ErrorMessage = ErrorMessages.NAME_REQUIRED)]
         public string Name {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.Add.Request.ADDRESS)]
         [Required (ErrorMessage = ErrorMessages.URL_REQUIRED)]
         public string Address {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.Add.Request.DESCRIPTION)]
         public string Description {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.Add.Request.USER_NAME)]
         public string User_Name {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.Add.Request.PASSWORD)]
         public string Password {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.Add.Request.PORT)]
         public int Port {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.Add.Request.EMAIL_PROVIDER)]
         public string Email_Provider {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.Add.Request.EMAIL_PASSWORD)]
         public string Email_Password {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.Add.Request.EMAIL_USER_NAME)]
         public string Email_User_Name {get ; set ;}
      }

      [Route (AdminWebsiteAPI.ManagementServer.Add.URL_PART)]
      [ValidateWebsiteToken]
      public IHttpActionResult Add (ManagementServerAddModel managementAddModel) {
         string doubleHashedPassword = Hashing.GenerateHash (managementAddModel.Password, managementAddModel.User_Name) ;
         Logger.Debug ( "Adding MS.") ;

         GuartinelApp.Settings.ManagementServer = new Common.Configuration.Data.ManagementServer  (
               managementAddModel.Name,
               managementAddModel.Address,
               managementAddModel.Description,
               "") ;
         GuartinelApp.SaveSettings() ;

         var loginRequest = new Common.Connection.IManagementServer.Admin.Login (GuartinelApp.WebRequester,GuartinelApp.Settings.ManagementServer, managementAddModel.User_Name, doubleHashedPassword) ;
         loginRequest.ThrowExceptionIfError() ;
         string token = loginRequest.Token ;
         string webSiteAddress = GuartinelApp.Settings.UserWebServer == null ? "localhost" : GuartinelApp.Settings.UserWebServer.Address ;

         var addRequest = new Common.Connection.IManagementServer.Admin.Update (GuartinelApp.WebRequester,GuartinelApp.Settings.ManagementServer,
               token,
               GuartinelApp.Settings.AdminAccount.Username,
               GuartinelApp.Settings.AdminAccount.PasswordHash,
               webSiteAddress,
               managementAddModel.Email_Password,
               managementAddModel.Email_Provider,
               managementAddModel.Email_User_Name) ;
         addRequest.ThrowExceptionIfError() ;

         Logger.Debug ( "Update successfull") ;
         loginRequest = new Common.Connection.IManagementServer.Admin.Login (GuartinelApp.WebRequester,GuartinelApp.Settings.ManagementServer, GuartinelApp.Settings.AdminAccount.Username, GuartinelApp.Settings.AdminAccount.PasswordHash) ;
         loginRequest.ThrowExceptionIfError() ;
         token = loginRequest.Token ;
         GuartinelApp.Settings.ManagementServer.Token = token ;
         GuartinelApp.SaveSettings() ;

         return Json (MessageTool.CreateJObjectWithSuccess()) ;
      }

      public class ManagementServerGetExistingModel : Models.AdminTokenModel {}

      [IsManagementServerAvailable]
      [ValidateWebsiteToken]
      [Route (AdminWebsiteAPI.ManagementServer.GetExisting.URL_PART)]
      public IHttpActionResult G3tExisting (ManagementServerGetExistingModel model) {
         //cannot rename to GetStatus because MVC will restrict this route to only HTTP GET METHODS

         JObject response = MessageTool.CreateJObjectWithSuccess() ;

         JObject getStatusReponse = new JObject() ;
         getStatusReponse.Add (AdminWebsiteAPI.ManagementServer.GetExisting.Response.ID, GuartinelApp.Settings.ManagementServer.Id) ;
         getStatusReponse.Add (AdminWebsiteAPI.ManagementServer.GetExisting.Response.NAME, GuartinelApp.Settings.ManagementServer.Name) ;
         getStatusReponse.Add (AdminWebsiteAPI.ManagementServer.GetExisting.Response.ADDRESS, GuartinelApp.Settings.ManagementServer.Address) ;
         getStatusReponse.Add (AdminWebsiteAPI.ManagementServer.GetExisting.Response.DESCRIPTION, GuartinelApp.Settings.ManagementServer.Description) ;
         response [AdminWebsiteAPI.ManagementServer.GetExisting.Response.MANAGEMENT_SERVER] = getStatusReponse ;

         return Json (response) ;
      }

      public class ManagementServerUpdateModel : Models.AdminTokenModel {
         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.Update.Request.NAME)]
         public string Name {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.Update.Request.ADDRESS)]
         public string Address {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.Update.Request.DESCRIPTION)]
         public string Description {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.Update.Request.USER_NAME)]
         public string UserName {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.Update.Request.PASSWORD)]
         public string Password {get ; set ;}
        
         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.Add.Request.EMAIL_PROVIDER)]
         public string Email_Provider {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.Add.Request.EMAIL_PASSWORD)]
         public string Email_Password {get ; set ;}

         [JsonProperty (PropertyName = AdminWebsiteAPI.ManagementServer.Add.Request.EMAIL_USER_NAME)]
         public string Email_User_Name {get ; set ;}
      }

      [Route (AdminWebsiteAPI.ManagementServer.Update.URL_PART)]
      [ValidateWebsiteToken]
      [IsManagementServerAvailable]
      public IHttpActionResult Update (ManagementServerUpdateModel managementUpdateModel) {
         GuartinelApp.Settings.ManagementServer.Address = managementUpdateModel.Address ;
        
         var loginRequest = new Common.Connection.IManagementServer.Admin.Login (GuartinelApp.WebRequester, GuartinelApp.Settings.ManagementServer, GuartinelApp.Settings.AdminAccount.Username, GuartinelApp.Settings.AdminAccount.PasswordHash) ;
         loginRequest.ThrowExceptionIfError() ;
         string token = loginRequest.Token ;
         string webSiteAddress = "localhost" ;
         if (GuartinelApp.Settings.UserWebServer != null) {
            webSiteAddress = GuartinelApp.Settings.UserWebServer.Address ;
         }
         var updateRequest = new Common.Connection.IManagementServer.Admin.Update (GuartinelApp.WebRequester,GuartinelApp.Settings.ManagementServer,
               token,
               GuartinelApp.Settings.AdminAccount.Username,
               GuartinelApp.Settings.AdminAccount.PasswordHash,
               webSiteAddress,
               managementUpdateModel.Email_Password,
               managementUpdateModel.Email_Provider,
               managementUpdateModel.Email_User_Name) ;
         updateRequest.ThrowExceptionIfError() ;

         GuartinelApp.Settings.ManagementServer.Name = managementUpdateModel.Name ;
         GuartinelApp.Settings.ManagementServer.Description = managementUpdateModel.Description ;
        
         loginRequest = new Common.Connection.IManagementServer.Admin.Login (GuartinelApp.WebRequester,GuartinelApp.Settings.ManagementServer, GuartinelApp.Settings.AdminAccount.Username, GuartinelApp.Settings.AdminAccount.PasswordHash) ;
         loginRequest.ThrowExceptionIfError() ;
         token = loginRequest.Token ;
         GuartinelApp.SaveSettings() ;
         GuartinelApp.Settings.ManagementServer.Token = token ;
         GuartinelApp.SaveSettings() ;

         return Json (MessageTool.CreateJObjectWithSuccess()) ;
      }

      public class ManagementServerRemoveModel : Models.AdminTokenModel {}

      [Route (AdminWebsiteAPI.ManagementServer.Remove.URL_PART)]
      [ValidateWebsiteToken]
      [IsManagementServerAvailable]
      public IHttpActionResult Remove (ManagementServerRemoveModel managementRemoveModel) {
         GuartinelApp.Settings.ManagementServer = null ;
         GuartinelApp.SaveSettings() ;
         return Json (MessageTool.CreateJObjectWithSuccess()) ;
      }
   }
}

using System.Web.Http;
using Guartinel.Communication;
using Guartinel.Website.Admin.Attributes;
using Guartinel.Website.Common.Error;
using Newtonsoft.Json.Linq;
using Guartinel.Kernel.Logging;

namespace Guartinel.Website.Admin.Controllers {
   [RoutePrefix (AdminWebsiteAPI.ManagementServer.Status.URL)]
   [ValidateWebsiteToken]
   public class ManagementServerStatusController : ApiController {
      public class ManagementServerGetStatusModel : Models.AdminTokenModel {}

      [Route (AdminWebsiteAPI.ManagementServer.Status.GetStatus.URL_PART)]
      [ValidateWebsiteToken]
      public IHttpActionResult G3tStatus (ManagementServerGetStatusModel authenticationModel) {
         //cannot rename to start with get because MVC will restrict this route to only HTTP GET METHODS
         if (GuartinelApp.Settings.ManagementServer == null) throw new CustomException.MissingManagementServerException() ;
         JObject requestModel = new JObject() ;
         requestModel.Add (ManagementServerAPI.Admin.Status.GetStatus.Request.TOKEN, GuartinelApp.Settings.ManagementServer.Token) ;
         JObject response = GuartinelApp.WebRequester.SendRequestTo (GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Admin.Status.GetStatus.FULL_URL, requestModel) ;

         return Json (response) ;
      }

      public class ManagementServerGetEventsModel : Models.AdminTokenModel {}

      [Route (AdminWebsiteAPI.ManagementServer.Status.GetEvents.URL_PART)]
      [ValidateWebsiteToken]
      public IHttpActionResult Events (ManagementServerGetEventsModel authenticationModel) {
         //cannot rename to start with get because MVC will restrict this route to only HTTP GET METHODS
         Logger.Debug("api/Management/Status/Events") ;

         JObject requestModel = new JObject() ;
         requestModel.Add (ManagementServerAPI.Admin.Status.GetEvents.Request.TOKEN, GuartinelApp.Settings.ManagementServer.Token) ;
         JObject response = GuartinelApp.WebRequester.SendRequestTo (GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Admin.Status.GetEvents.FULL_URL, requestModel) ;

         return Json (response) ;
      }

      public class ManagementServerGetLogModel : Models.AdminTokenModel {}

      [Route (AdminWebsiteAPI.ManagementServer.Status.GetLog.URL_PART)]
      [ValidateWebsiteToken]
      public IHttpActionResult Log (ManagementServerGetLogModel authenticationModel) {
         //cannot rename to start with get because MVC will restrict this route to only HTTP GET METHODS
         JObject requestModel = new JObject() ;
         requestModel.Add (ManagementServerAPI.Admin.Status.GetEvents.Request.TOKEN, GuartinelApp.Settings.ManagementServer.Token) ;
         JObject response = GuartinelApp.WebRequester.SendRequestTo (GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Admin.Status.GetLog.FULL_URL, requestModel) ;

         return Json (response) ;
      }

      public class ManagementServerGetInvalidRequestsModel : Models.AdminTokenModel {}

      [Route (AdminWebsiteAPI.ManagementServer.Status.GetInvalidRequests.URL_PART)]
      [ValidateWebsiteToken]
      public IHttpActionResult InvalidRequests (ManagementServerGetInvalidRequestsModel authenticationModel) {
         JObject requestModel = new JObject() ;
         requestModel.Add (ManagementServerAPI.Admin.Status.GetInvalidRequests.Request.TOKEN, GuartinelApp.Settings.ManagementServer.Token) ;
         JObject response = GuartinelApp.WebRequester.SendRequestTo (GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Admin.Status.GetInvalidRequests.FULL_URL, requestModel) ;

         return Json (response) ;
      }
   }
}

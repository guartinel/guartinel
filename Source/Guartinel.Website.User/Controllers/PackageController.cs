using System.Web.Http;
using Guartinel.Communication;
using Guartinel.Website.User.Models.Package;
using Newtonsoft.Json.Linq;
using Guartinel.Website.User.Models.Account;
using Newtonsoft.Json;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;

namespace Guartinel.Website.User.Controllers {
   [RoutePrefix(UserWebsiteAPI.Package.URL)]
   public class PackageController : ApiController {
      [Route(UserWebsiteAPI.Package.Save.URL_PART)]
      public IHttpActionResult Save (PackageModel packageModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Package.Save.FULL_URL, packageModel);
         return Json(result);
      }

      [Route(UserWebsiteAPI.Package.GetAvailable.URL_PART)]
      public IHttpActionResult G3tExisting (PackageExistingModel packageExistingModel) {
         //cannot rename to start with get because MVC will restrict this route to only HTTP GET METHODS
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Package.GetAvailable.FULL_URL, packageExistingModel);
         return Json(result);
      }

      public class PackageGetStatisticsModel : AuthenticationModel {
         [JsonProperty(PropertyName = AllParameters.PACKAGE_ID)]
         public string PackageID { get; set; }
      }
      [Route(UserWebsiteAPI.Package.GetStatistics.URL_PART)]
      public IHttpActionResult G3tStatistics (PackageGetStatisticsModel model) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Package.GetStatistics.FULL_URL, model);
         return Json(result);
      }
      [Route(UserWebsiteAPI.Package.TestEmail.URL_PART)]
      public IHttpActionResult TestEmail (PackageTestEmailModel packageTestEmailModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Package.SendTestEmail.FULL_URL, packageTestEmailModel);
         return Json(result);
      }
      [Route(UserWebsiteAPI.Package.RemoveAccess.URL_PART)]
      public IHttpActionResult R3moveAccess (PackageRemoveModel removeModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Package.RemoveAccess.FULL_URL, removeModel);
         return Json(result);
      }

      [Route(UserWebsiteAPI.Package.Delete.URL_PART)]
      public IHttpActionResult DoDelete (PackageDeleteModel packageDeleteModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Package.Delete.FULL_URL, packageDeleteModel);
         return Json(result);
      }
   }
}

using System.Web.Http;
using Guartinel.Communication;
using Guartinel.Website.User.Models.Account;
using Guartinel.Website.User.Models.Device;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;

namespace Guartinel.Website.User.Controllers {
   [RoutePrefix(UserWebsiteAPI.Device.URL)]
   public class DeviceController : ApiController {
      [Route(UserWebsiteAPI.Device.GetExisting.URL_PART)]
      public IHttpActionResult Existing (DeviceExistingModel deviceExistingModel) {
         //cannot rename to start with get because MVC will restrict this route to only HTTP GET METHODS
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Device.GetAvailable.FULL_URL, deviceExistingModel);
         return Json(result);
      }

      [Route(UserWebsiteAPI.Device.Delete.URL_PART)]
      public IHttpActionResult Remove (DeviceDeleteModel deviceDeleteModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Device.Delete.FULL_URL, deviceDeleteModel);
         return Json(result);
      }
        [Route(UserWebsiteAPI.Device.Disconnect.URL_PART)]
        public IHttpActionResult Disconnect(DeviceDeleteModel deviceDeleteModel)
        {
            JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Device.Disconnect.FULL_URL, deviceDeleteModel);
            return Json(result);
        }

        [Route(UserWebsiteAPI.Device.Test.URL_PART)]
      public IHttpActionResult Test (DeviceTestModel deviceTestModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Device.Android.Test.FULL_URL, deviceTestModel);
         return Json(result);
      }

      public class EditDeviceModel : AuthenticationModel {
         [JsonProperty(PropertyName = AllParameters.DEVICE_UUID)]
         public string DeviceID { get; set; }

         [JsonProperty(PropertyName = AllParameters.DEVICE_NAME)]
         public string DeviceName { get; set; }

         [JsonProperty(PropertyName = AllParameters.CATEGORIES)]
         public string[] Categories { get; set; }
      }

      [Route(UserWebsiteAPI.Device.Edit.URL_PART)]
      public IHttpActionResult Edit (EditDeviceModel editDeviceModel) {
         JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer, ManagementServerAPI.Device.Edit.FULL_URL, editDeviceModel);
         return Json(result);
      }
   }
}

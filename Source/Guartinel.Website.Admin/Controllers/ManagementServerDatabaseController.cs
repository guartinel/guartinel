using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using Guartinel.Communication;
using Guartinel.Website.Admin.Attributes;
using Guartinel.Website.Common.Error;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Guartinel.Website.Admin.Controllers {

    [RoutePrefix(AdminWebsiteAPI.ManagementServer.Database.URL)]
    public class ManagementDatabaseController : ApiController {

        public class ManagementServerDatabaseGetStatusModel : Models.AdminTokenModel { }
        [Route(AdminWebsiteAPI.ManagementServer.Database.GetStatus.URL_PART)]
        [ValidateWebsiteToken]
        public IHttpActionResult G3tStatus(ManagementServerDatabaseGetStatusModel authenticationModel) {//cannot rename to GetStatus because MVC will restrict this route to only HTTP GET METHODS
            JObject requestModel = new JObject();
            requestModel.Add(ManagementServerAPI.Admin.Database.GetStatus.Request.TOKEN, GuartinelApp.Settings.ManagementServer.Token);

            JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer,ManagementServerAPI.Admin.Database.GetStatus.FULL_URL, requestModel);

            return Json(result);
            }

        public class ManagementServerDatabaseRemoveModel : Models.AdminTokenModel { }
        [Route(AdminWebsiteAPI.ManagementServer.Database.Remove.URL_PART)]
        [ValidateWebsiteToken]
        public IHttpActionResult Remove(ManagementServerDatabaseRemoveModel authenticationModel) {//cannot rename to GetStatus because MVC will restrict this route to only HTTP GET METHODS
            JObject requestModel = new JObject();
            requestModel.Add(ManagementServerAPI.Admin.Database.Remove.Request.TOKEN, GuartinelApp.Settings.ManagementServer.Token);

            JObject result = GuartinelApp.WebRequester.SendRequestTo(GuartinelApp.Settings.ManagementServer,ManagementServerAPI.Admin.Database.Remove.FULL_URL, requestModel);

            return Json(result);
            }

        public class ManagementServerDatabaseRegisterModel : Models.AdminTokenModel {

            [JsonProperty(PropertyName = AdminWebsiteAPI.ManagementServer.Database.Update.Request.DATA_BASE_URL)]
            [Required(ErrorMessage = ErrorMessages.URL_REQUIRED)]
            public string DatabaseUrl { get; set; }

            [JsonProperty(PropertyName = AdminWebsiteAPI.ManagementServer.Database.Update.Request.USER_NAME)]
            public string UserName { get; set; }

            [JsonProperty(PropertyName = AdminWebsiteAPI.ManagementServer.Database.Update.Request.PASSWORD)]
            public string Password { get; set; }

            }
        [Route(AdminWebsiteAPI.ManagementServer.Database.Update.URL_PART)]
        [ValidateWebsiteToken]
        public IHttpActionResult Register(ManagementServerDatabaseRegisterModel databaseRegisterModel) {
            JObject requestModel = new JObject();
            requestModel.Add(ManagementServerAPI.Admin.Database.Update.Request.DATABASE_URL, databaseRegisterModel.DatabaseUrl);
            requestModel.Add(ManagementServerAPI.Admin.Database.Update.Request.USER_NAME, databaseRegisterModel.DatabaseUrl);
            requestModel.Add(ManagementServerAPI.Admin.Database.Update.Request.PASSWORD, databaseRegisterModel.DatabaseUrl);

            requestModel.Add(ManagementServerAPI.Admin.Database.Update.Request.TOKEN, GuartinelApp.Settings.ManagementServer.Token);

            JObject result = GuartinelApp.WebRequester.SendRequestTo (GuartinelApp.Settings.ManagementServer,ManagementServerAPI.Admin.Database.Update.FULL_URL, requestModel);

            return Json(result);
            }

        }
    }
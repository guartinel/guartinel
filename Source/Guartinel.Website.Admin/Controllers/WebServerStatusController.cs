using System.Web.Http ;
using Guartinel.Communication ;
using Guartinel.Website.Admin.Attributes ;
using Guartinel.Website.Common.Tools ;
using Newtonsoft.Json.Linq ;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;
using AllSuccessValues = Guartinel.Communication.Strings.Strings.AllSuccessValues;

namespace Guartinel.Website.Admin.Controllers {
    
    [RoutePrefix(AdminWebsiteAPI.WebSiteStatus.URL)]
    public class StatusController : ApiController {
        public class WebServerGetStatusModel : Models.AdminTokenModel { }

        [Route(AdminWebsiteAPI.WebSiteStatus.GetStatus.URL_PART)]
        [ValidateWebsiteToken]
        public IHttpActionResult G3tStatus(WebServerGetStatusModel authenticationModel) {//cannot rename to start with get because MVC will restrict this route to only HTTP GET METHODS
            JObject response = new JObject();
            float cpu = 0 ;//;WebsiteServerStatus.GetCPUUsage() ;
            float memory = PerformanceMonitorTool.GetMemoryUsage() ;
            float storage = PerformanceMonitorTool.GetStorageUsage() ;

            response.Add (AllParameters.CPU, cpu);
            response.Add (AllParameters.MEMORY, memory);
            response.Add (AllParameters.STORAGE, storage);
            response.Add (AllParameters.SUCCESS, AllSuccessValues.SUCCESS);
            return Json(response);
            }
        }
    }
using System;
using System.Linq;
using System.Web.Http ;
using Newtonsoft.Json.Linq ;
using Sysment.Watcher.Core.CommunicationInterface ;
using Sysment.Watcher.Website.Attributes ;
using Sysment.Watcher.Website.Misc ;
using Sysment.Watcher.Website.Models ;

namespace Sysment.Watcher.Website.Controllers.Website {

   [RoutePrefix ("api/Status")]
   public class StatusController : ApiController {

      [Route ("Info")]
      [ValidateWebsiteToken]
      public IHttpActionResult Info (AuthRequestModel authRequestModel) {
         JObject response = new JObject() ;
         response.Add ("cpu", WebsiteServerStatus.GetCPUUsage()) ;
         response.Add ("memory", WebsiteServerStatus.GetMemoryUsage()) ;
         response.Add ("storage", WebsiteServerStatus.GetStorageUsage()) ;
         response.Add (ConnectionVars.Parameter.CONTENT, ConnectionVars.Content.SUCCESS) ;
         return Json (response) ;
      }

   }
}
using System ;
using System.Linq ;
using System.Web.Http ;
using Newtonsoft.Json.Linq ;
using Sysment.Watcher.Core.CommunicationInterface ;
using Sysment.Watcher.Website.Attributes ;
using Sysment.Watcher.Website.Managers ;
using Sysment.Watcher.Website.Models ;
using Sysment.Watcher.Website.Models.Website ;

namespace Sysment.Watcher.Website.Controllers.Website {

   [RoutePrefix ("api/Management")]
   public class ManagementController : ApiController {

      [Route ("Add")]
      [ValidateWebsiteToken]
      public IHttpActionResult Add (ManagementAddModel managementAddModel) {
         JObject response = new JObject() ;
         ManagementManager.AddManagementServer (managementAddModel) ;
         ManagementManager.ReplaceDefaultValues();
         response.Add (ConnectionVars.Parameter.CONTENT, ConnectionVars.Content.SUCCESS) ;
         return Json (response) ;
      }

      [Route ("Existing")]
      [ValidateWebsiteToken]
      public IHttpActionResult Existing (AuthRequestModel authRequestModel) {
         JObject response = new JObject() ;
         ManagementManager.ConvertManagementServerToJson (response) ;
         response.Add (ConnectionVars.Parameter.CONTENT, ConnectionVars.Content.SUCCESS) ;
         return Json (response) ;
      }

      [Route ("Update")]
      [ValidateWebsiteToken]
      public IHttpActionResult Update (ManagementUpdateModel managementUpdateModel) {
         JObject response = new JObject() ;
         ManagementManager.UpdateManagementServer (managementUpdateModel) ;
         response.Add (ConnectionVars.Parameter.CONTENT, ConnectionVars.Content.SUCCESS) ;
         return Json (response) ;
      }

      [Route ("Remove")]
      [ValidateWebsiteToken]
      public IHttpActionResult Remove (ManagementIdentifyModel managementRemoveModel) {
         JObject response = new JObject() ;
         ManagementManager.RemoveManagementServer (managementRemoveModel) ;
         response.Add (ConnectionVars.Parameter.CONTENT, ConnectionVars.Content.SUCCESS) ;
         return Json (response) ;
      }
   }


}

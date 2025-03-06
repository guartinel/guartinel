using System ;
using System.Linq ;
using System.Web.Http ;
using Sysment.Watcher.Website.Attributes ;
using Sysment.Watcher.Website.Managers ;
using Sysment.Watcher.Website.Models ;
using Sysment.Watcher.Website.Server.Requests.Admin ;

namespace Sysment.Watcher.Website.Controllers.Management {

   [RoutePrefix ("api/Management/Status")]
   public class ManagementStatusController : ApiController {

      [Route ("Info")]
      [ValidateWebsiteToken]
      public IHttpActionResult Info (AuthRequestModel authRequestModel) {
         ManagementManager.ReplaceToken (authRequestModel) ;
         return Json (StatusRequests.Info (authRequestModel)) ;
      }

      [Route ("Events")]
      [ValidateWebsiteToken]
      public IHttpActionResult Events (AuthRequestModel authRequestModel) {
         ManagementManager.ReplaceToken (authRequestModel) ;
         return Json (StatusRequests.Events (authRequestModel)) ;
      }

      [Route ("Log")]
      [ValidateWebsiteToken]
      public IHttpActionResult Log (AuthRequestModel authRequestModel) {
         ManagementManager.ReplaceToken (authRequestModel) ;
         return Json (StatusRequests.Log (authRequestModel)) ;
      }

      [Route ("InvalidRequests")]
      [ValidateWebsiteToken]
      public IHttpActionResult InvalidRequests (AuthRequestModel authRequestModel) {
         ManagementManager.ReplaceToken (authRequestModel) ;
         return Json (StatusRequests.InvalidRequests (authRequestModel)) ;
      }

   }
}
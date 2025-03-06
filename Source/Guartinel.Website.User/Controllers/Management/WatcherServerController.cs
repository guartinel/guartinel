using System ;
using System.Linq ;
using System.Web.Http ;
using Sysment.Watcher.Website.Attributes ;
using Sysment.Watcher.Website.Managers ;
using Sysment.Watcher.Website.Models ;
using Sysment.Watcher.Website.Models.Management.WatcherServer ;
using Sysment.Watcher.Website.Server.Requests.Admin ;

namespace Sysment.Watcher.Website.Controllers.Management {

   [RoutePrefix ("api/Management/WatcherServer")]
   public class WatcherServerController : ApiController {

      [Route ("Existing")]
      [ValidateWebsiteToken]
      public IHttpActionResult Existing (AuthRequestModel authRequestModel) {
         ManagementManager.ReplaceToken (authRequestModel) ;
         return Json (WatcherServerRequests.Existing (authRequestModel)) ;
      }

      [Route ("Events")]
      [ValidateWebsiteToken]
      public IHttpActionResult Events (WatcherServerIdentifyModel watcherServerIdentifyModel) {
         ManagementManager.ReplaceToken (watcherServerIdentifyModel) ;
         return Json (WatcherServerRequests.Events (watcherServerIdentifyModel)) ;
      }

      [Route ("Info")]
      [ValidateWebsiteToken]
      public IHttpActionResult Info (WatcherServerIdentifyModel watcherServerIdentifyModel) {
         ManagementManager.ReplaceToken (watcherServerIdentifyModel) ;
         return Json (WatcherServerRequests.Info (watcherServerIdentifyModel)) ;
      }

      [Route ("Register")]
      [ValidateWebsiteToken]
      public IHttpActionResult Register (WatcherServerRegisterModel watcherServerRegisterModel) {
         ManagementManager.ReplaceToken (watcherServerRegisterModel) ;
         return Json (WatcherServerRequests.Register (watcherServerRegisterModel)) ;
      }

      [Route ("Remove")]
      [ValidateWebsiteToken]
      public IHttpActionResult Remove (WatcherServerIdentifyModel watcherServerIdentifyModel) {
         ManagementManager.ReplaceToken (watcherServerIdentifyModel) ;
         return Json (WatcherServerRequests.Remove (watcherServerIdentifyModel)) ;
      }

      [Route ("Update")]
      [ValidateWebsiteToken]
      public IHttpActionResult Update (WatcherServerUpdateModel watcherServerUpdateModel) {
         ManagementManager.ReplaceToken (watcherServerUpdateModel) ;
         return Json (WatcherServerRequests.Update (watcherServerUpdateModel)) ;
      }


   }
}
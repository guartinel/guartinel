using System ;
using System.Linq ;
using System.Web.Http ;
using Sysment.Watcher.Website.Attributes ;
using Sysment.Watcher.Website.Managers ;
using Sysment.Watcher.Website.Models ;
using Sysment.Watcher.Website.Models.Management.Database ;
using Sysment.Watcher.Website.Server.Requests.Admin ;

namespace Sysment.Watcher.Website.Controllers.Management {

   [RoutePrefix ("api/Management/Database")]
   public class ManagementDatabaseController : ApiController {

      [Route ("Info")]
      [ValidateWebsiteToken]
      public IHttpActionResult Info (AuthRequestModel authRequestModel) {
         ManagementManager.ReplaceToken (authRequestModel);
         return Json(DatabaseRequests.Info (authRequestModel)) ;
      }

      [Route ("Register")]
      [ValidateWebsiteToken]
      public IHttpActionResult Register (DatabaseRegisteModel databaseRegisteModel) {
         ManagementManager.ReplaceToken (databaseRegisteModel);
         return Json (DatabaseRequests.Register (databaseRegisteModel)) ;
      }

   }
}
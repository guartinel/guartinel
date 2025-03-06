using System ;
using System.Linq ;
using System.Web.Http ;
using Newtonsoft.Json.Linq ;
using Sysment.Watcher.Core.CommunicationInterface ;
using Sysment.Watcher.Website.DatabaseModels ;
using Sysment.Watcher.Website.Managers ;
using Sysment.Watcher.Website.Misc ;
using Sysment.Watcher.Website.Models ;
using Sysment.Watcher.Website.Models.Website.AdminAccount ;

namespace Sysment.Watcher.Website.Controllers.Website {

   [RoutePrefix ("api/Admin")]
   public class AdminController : ApiController {
      
      [Route ("Login")]
      public IHttpActionResult Login (AdminLoginModel adminLoginModel) {
         JObject response = new JObject() ;
         AdminAccount adminAccount = AdminManager.Login (adminLoginModel) ;
         ManagementManager.Login();
         response.Add (ConnectionVars.Parameter.TOKEN, adminAccount.Token) ;
         response.Add (ConnectionVars.Parameter.CONTENT, ConnectionVars.Content.SUCCESS) ;
         return Json (response) ;
      }

      [Route ("Info")]
      public IHttpActionResult Info (AuthRequestModel authRequestModel) {
         JObject response = new JObject() ;
         using (var database = new DataEntities()) {
            AdminAccount adminAccount = database.AdminAccounts.SingleOrDefault (a => a.Token == authRequestModel.Token) ;

            if (adminAccount == null)
               throw new CustomException.InvalidTokenException() ;

            response.Add (ApiParameters.CONFIGURED, adminAccount.Configured) ;
            response.Add (ConnectionVars.Parameter.FIRST_NAME, adminAccount.FirstName) ;
            response.Add (ConnectionVars.Parameter.LAST_NAME, adminAccount.LastName) ;
            response.Add (ConnectionVars.Parameter.CONTENT, ConnectionVars.Content.SUCCESS) ;
         }
         return Json (response) ;
      }

      [Route ("Update")]
      public IHttpActionResult Update (AdminUpdateModel adminUpdateModel) {
         JObject response = new JObject() ;
         AdminManager.Update (adminUpdateModel) ;
         response.Add (ConnectionVars.Parameter.CONTENT, ConnectionVars.Content.SUCCESS) ;
         return Json (response) ;
      }

      [Route ("Logout")]
      public IHttpActionResult Logout (AuthRequestModel authRequestModel) {
         JObject response = new JObject() ;
         AdminManager.Logout (authRequestModel) ;
         response.Add (ConnectionVars.Parameter.CONTENT, ConnectionVars.Content.SUCCESS) ;
         return Json (response) ;
      }



   }
}
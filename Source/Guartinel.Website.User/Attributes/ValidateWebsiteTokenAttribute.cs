using System ;
using System.Linq ;

namespace Guartinel.Website.User.Attributes {
   /*public class ValidateWebsiteTokenAttribute : ActionFilterAttribute {
      public override void OnActionExecuting (HttpActionContext filterContext) {
         KeyValuePair<string, object> apiModel = filterContext.ActionArguments.SingleOrDefault() ;
         string token = ((AuthenticationModel) apiModel.Value).Token ;

         if (GuartinelApp.Settings.AdminAccount.Token != token) {
            throw new CustomException.InvalidTokenException() ;
         }
         base.OnActionExecuting (filterContext) ;
      }
   }*/
}

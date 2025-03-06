using System ;
using System.Linq ;
using System.Web.Http.Filters ;

namespace Guartinel.Website.User.Attributes {
   public class IsManagementServerAvailable : ActionFilterAttribute {
      /*     public override void OnActionExecuting(HttpActionContext filterContext) {
          
            if (GuartinelApp.Settings.ManagementServer == null) {
                throw new CustomException.ManagementServerNotFoundException();
                }
            base.OnActionExecuting(filterContext);
            }
        }*/
   }
}

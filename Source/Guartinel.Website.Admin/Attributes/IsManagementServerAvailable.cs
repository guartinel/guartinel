using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using Guartinel.Website.Common.Error ;

namespace Guartinel.Website.Admin.Attributes {
    public class IsManagementServerAvailable : ActionFilterAttribute {
        public override void OnActionExecuting(HttpActionContext filterContext) {
          
            if (GuartinelApp.Settings.ManagementServer == null) {
                throw new CustomException.MissingManagementServerException();
                }
            base.OnActionExecuting(filterContext);
            }
        }
    }
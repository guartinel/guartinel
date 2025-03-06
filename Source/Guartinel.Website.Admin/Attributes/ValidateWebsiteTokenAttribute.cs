using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using Guartinel.Website.Common.Error ;

namespace Guartinel.Website.Admin.Attributes {

   public class ValidateWebsiteTokenAttribute : ActionFilterAttribute {
      public override void OnActionExecuting (HttpActionContext filterContext) {
         KeyValuePair<string, object> apiModel = filterContext.ActionArguments.SingleOrDefault() ;
         string token = ((Models.AdminTokenModel) apiModel.Value).Token ;

         if (GuartinelApp.Settings.AdminAccount.Token != token) {
            throw new CustomException.InvalidTokenException() ;
         }
         base.OnActionExecuting (filterContext) ;
      }
   }
}
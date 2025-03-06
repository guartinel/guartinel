using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json.Linq;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;

namespace Guartinel.Website.Admin.Attributes {

   public class ValidateModelStateAttribute : ActionFilterAttribute {

      public override void OnActionExecuting (HttpActionContext actionContext) {

         ModelStateDictionary modelState = actionContext.ModelState ;

         if (!modelState.IsValid) {

            string error = modelState.Keys.SelectMany (k => modelState [k].Errors).Select (m => m.ErrorMessage).ToArray().First() ;

            JObject response = new JObject() ;
            //response.Add (Common.AllParameters.ERROR, ErrorMessages.MODEL_ERROR) ;
            if (string.IsNullOrEmpty (error)) {
               error = " The request model is invalid" ;
            }
            response.Add (AllParameters.ERROR, error) ;

            actionContext.Response = actionContext.Request.CreateResponse (HttpStatusCode.OK,
                                                                           response, actionContext.ControllerContext.Configuration.Formatters.JsonFormatter) ;
         }
      }
   }
}
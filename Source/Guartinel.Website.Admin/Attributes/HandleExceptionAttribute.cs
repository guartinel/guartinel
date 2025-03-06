using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Guartinel.Website.Common.Error;
using Newtonsoft.Json.Linq;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel;
using Guartinel.Kernel.Utility ;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;
using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues;


namespace Guartinel.Website.Admin.Attributes {
   public class HandleExceptionAttribute : ExceptionFilterAttribute {
      public override void OnException (HttpActionExecutedContext actionExecutedContext) {
         Exception exception = actionExecutedContext.Exception ;
         Logger.Error  ($"HandleExceptionAttribute. Final closure for exception : OnException. E: {exception.GetAllMessages()}") ;
         JObject response = new JObject() ;

         if (exception is CustomException.ManagementServerInvalidTokenException) {
            var loginRequest = new Common.Connection.IManagementServer.Admin.Login (GuartinelApp.WebRequester,GuartinelApp.Settings.ManagementServer, GuartinelApp.Settings.AdminAccount.Username, GuartinelApp.Settings.AdminAccount.PasswordHash) ;
            loginRequest.ThrowExceptionIfError() ;
            string token = loginRequest.Token ;
            GuartinelApp.SaveSettings() ;
            GuartinelApp.Settings.ManagementServer.Token = token ;
            GuartinelApp.SaveSettings() ;

            AddErrorToResponse (response, AllErrorValues.INTERNAL_SYSTEM_ERROR) ;
         } else if (actionExecutedContext.Exception is CustomException) AddErrorToResponse (response, ((CustomException) exception).ErrorMessage) ;
         else AddErrorToResponse (response, AllErrorValues.INTERNAL_SYSTEM_ERROR) ;

         actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse (HttpStatusCode.OK,
               response, actionExecutedContext.ActionContext.ControllerContext.Configuration.Formatters.JsonFormatter) ;

         base.OnException (actionExecutedContext) ;
      }

      private void AddErrorToResponse (JObject response,
            string message) {
         response.Add (AllParameters.ERROR, message) ;
      }
   }
}

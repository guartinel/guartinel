using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Guartinel.Website.Common.Error;
using Newtonsoft.Json.Linq;
using Guartinel.Kernel.Logging;
using Guartinel.Kernel.Utility ;
using AllParameters = Guartinel.Communication.Strings.Strings.AllParameters;
using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues;

namespace Guartinel.Website.User.Attributes {
   public class HandleExceptionAttribute : ExceptionFilterAttribute {
      public override void OnException (HttpActionExecutedContext actionExecutedContext) {
         Exception exception = actionExecutedContext.Exception ;
         if (exception == null) {
            Logger.Error ("Cannot get exception from action context..") ;
            base.OnException (actionExecutedContext) ;
         }

         string errorUuid = Guid.NewGuid().ToString() ;
         Logger.Error ("Starting exception handling in HandleExceptionAttribute.OnException") ;

         Logger.Error ($"HandleExceptionAttribute catched exception.UUID: {errorUuid} Error: {exception.GetAllMessages()}") ;

         JObject response = new JObject() ;

         if (exception is CustomException.ManagementServerInvalidTokenException) {
            var loginRequest = new Common.Connection.IManagementServer.Admin.Login (GuartinelApp.WebRequester, GuartinelApp.Settings.ManagementServer, GuartinelApp.Settings.AdminAccount.Username, GuartinelApp.Settings.AdminAccount.PasswordHash) ;
            loginRequest.ThrowExceptionIfError() ;
            string token = loginRequest.Token ;
            GuartinelApp.Settings.ManagementServer.Token = token ;
            GuartinelApp.SaveSettings() ;
         }

         if (exception is CustomException) {
            string errorMessage = ((CustomException) exception).ErrorMessage ;
            AddErrorToResponse (response, errorMessage, errorUuid) ;
         } else AddErrorToResponse (response, AllErrorValues.INTERNAL_SYSTEM_ERROR, errorUuid) ;
         actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse (HttpStatusCode.OK,
                                                                                        response, actionExecutedContext.ActionContext.ControllerContext.Configuration.Formatters.JsonFormatter) ;

         base.OnException (actionExecutedContext) ;
      }

      private void AddErrorToResponse (JObject response,
                                       string message,
                                       string errorUuid) {
         response.Add (AllParameters.ERROR, message) ;
         response.Add (AllParameters.ERROR_UUID, errorUuid) ;
      }

      /*
      public void OnException2 (HttpActionExecutedContext actionExecutedContext) {
         Exception exception = actionExecutedContext.Exception ;
         Logger.Error ($"HandleExceptionAttribute catched exception. {e.GetAllMessages()}") ;
         JObject response = new JObject() ;

         if (exception is CustomException.ManagementServerInvalidTokenException) {
            var loginRequest = new Common.Connection.IManagementServer.Admin.Login (GuartinelApp.WebRequester, GuartinelApp.Settings.ManagementServer, GuartinelApp.Settings.AdminAccount.Username, GuartinelApp.Settings.AdminAccount.PasswordHash) ;
            loginRequest.ThrowExceptionIfError() ;
            string token = loginRequest.Token ;
            GuartinelApp.SaveSettings() ;
            GuartinelApp.Settings.ManagementServer.Token = token ;
            GuartinelApp.SaveSettings() ;
            AddErrorToResponse2 (response, Communication.Common.AllErrorValues.INTERNAL_SYSTEM_ERROR) ;
         } else if (actionExecutedContext.Exception is CustomException) AddErrorToResponse2 (response, ((CustomException) exception).ErrorMessage) ;
         else AddErrorToResponse2 (response, Communication.Common.AllErrorValues.INTERNAL_SYSTEM_ERROR) ;

         actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse (HttpStatusCode.OK,
               response, actionExecutedContext.ActionContext.ControllerContext.Configuration.Formatters.JsonFormatter) ;

         base.OnException (actionExecutedContext) ;
      }

      private void AddErrorToResponse2 (JObject response,
            string message) {
         response.Add (Communication.Common.AllParameters.ERROR, message) ;
      }*/
   }
}
using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Communication ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues;

namespace Guartinel.WatcherServer.Communication.Routes {
   public class LoginRoute : Route {

      //public static class Constants {         
      //   public const string ROUTE = "login" ;
      //}

      //public new static class ParameterNames {
      //   public const string USER_NAME = "user_name" ;
      //   public const string PASSWORD = "password" ;
      //}

      //public new static class ResultNames {
      //   public const string TOKEN = "token" ;         
      //}

      public override string Path => WatcherServerAPI.Admin.Login.FULL_URL ;

      protected override void ProcessRequest (Parameters parameters,
                                              Parameters results,
                                              TagLogger logger) {

         var loginPassword = parameters [WatcherServerAPI.Admin.Login.Request.PASSWORD] ;
         var loginPasswordHash = Kernel.Utility.Hashing.GenerateHash (loginPassword, ApplicationSettings.Use.ManagementServerID) ;

         // Check password
         if (Kernel.Utility.Hashing.GenerateHash (loginPassword, ApplicationSettings.Use.ManagementServerID) != loginPasswordHash) {
            throw new ServerException (AllErrorValues.INVALID_USER_NAME_OR_PASSWORD) ;
         }

         var token = Tokens.Use().GenerateToken() ;
         
         results [WatcherServerAPI.Admin.Login.Response.TOKEN] = token ;

         results.Success();

         logger.InfoWithDebug ("Server login done.", results.AsJObject.ConvertToLog()) ;
      }
   }
}
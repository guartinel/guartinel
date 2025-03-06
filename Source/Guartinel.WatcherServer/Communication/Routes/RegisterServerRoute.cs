using System;
using System.Linq;
using System.Text;
using Guartinel.Communication;
using Guartinel.Kernel.Logging ;
using AllErrorValues = Guartinel.Communication.Strings.Strings.AllErrorValues;

namespace Guartinel.WatcherServer.Communication.Routes {
   public class RegisterServerRoute : Route {

      //public static class Constants {         
      //   public const string ROUTE = @"admin/register" ;
      //}

      //public new static class ParameterNames {
      //   public const string CURRENT_USER_NAME = "init_user_name" ;
      //   public const string CURRENT_PASSWORD = "init_password" ;
      //   public const string NEW_USER_NAME = "user_name" ;
      //   public const string NEW_PASSWORD = "password" ;
      //   public const string MANAGEMENT_SERVER_ADDRESS = "management_server_address" ;
      //}

      //public static class ResultNames {
      //   public const string TOKEN = "token" ;
      //}

      public RegisterServerRoute() : base() { }

      public override string Path => WatcherServerAPI.Admin.RegisterServer.FULL_URL ;

      protected override void ProcessRequest (Parameters parameters,
                                              Parameters results,
                                              TagLogger logger) {
         var registrationUserName = parameters [WatcherServerAPI.Admin.RegisterServer.Request.USER_NAME] ;
         var registrationPassword = parameters [WatcherServerAPI.Admin.RegisterServer.Request.PASSWORD] ;
         var newRegistrationUserName = parameters [WatcherServerAPI.Admin.RegisterServer.Request.NEW_USER_NAME] ;
         var newRegistrationPassword = parameters [WatcherServerAPI.Admin.RegisterServer.Request.NEW_PASSWORD] ;
         var managementServerAddress = parameters [WatcherServerAPI.Admin.RegisterServer.Request.MANAGEMENT_SERVER_ADDRESS] ;
         var managementServerUID = parameters [WatcherServerAPI.Admin.RegisterServer.Request.UID] ;
         var managementServerRegistrationToken = parameters [WatcherServerAPI.Admin.RegisterServer.Request.ONE_TIME_REGISTRATION_TOKEN] ;
         var categories = parameters.AsStringArray (WatcherServerAPI.Admin.RegisterServer.Request.CATEGORIES) ;

         var managementServerID = Guid.NewGuid().ToString() ;
         string registrationPasswordHash = Kernel.Utility.Hashing.GenerateHash (registrationPassword, registrationUserName) ;

         logger.Info ($"Register server, password hash: {registrationPasswordHash}") ;
         logger.Info ($"Configuration password hash: {ApplicationSettings.Use.RegistrationPasswordHash}") ;

         // Authenticate user
         if (registrationUserName != ApplicationSettings.Use.RegistrationUserName) {
            throw new ServerException (AllErrorValues.INVALID_USER_NAME_OR_PASSWORD) ;
         }

         if (registrationPasswordHash != ApplicationSettings.Use.RegistrationPasswordHash) {
            throw new ServerException (AllErrorValues.INVALID_USER_NAME_OR_PASSWORD) ;
         }

         // Generate login data
         //   string loginPassword = Core.Utility.Hashing.GenerateHash(managementServerUID, managementServerID); // DTAP we dont store the login password instead we will generate it when neccesary 
         //  string loginPasswordHash = Core.Utility.Hashing.GenerateHash(loginPassword, managementServerID);

         // Store new admin data
         ApplicationSettings.Use.RegistrationUserName = newRegistrationUserName ;
         ApplicationSettings.Use.RegistrationPasswordHash = Kernel.Utility.Hashing.GenerateHash (newRegistrationPassword, newRegistrationUserName) ;

         // Store config
         ApplicationSettings.Use.ManagementServerAddress = managementServerAddress ;
         ApplicationSettings.Use.ManagementServerID = managementServerID ;
         ApplicationSettings.Use.ManagementServerUID = managementServerUID ;
         ApplicationSettings.Use.ManagementServerRegistrationToken = managementServerRegistrationToken ;
         // ApplicationSettings.Use.Save() ;

         results [WatcherServerAPI.Admin.RegisterServer.Response.TOKEN] = Tokens.Use().GenerateToken() ;
         results [WatcherServerAPI.Admin.RegisterServer.Response.MANAGEMENT_SERVER_ID] = managementServerID ;

         results.Success() ;
      }
   }
}
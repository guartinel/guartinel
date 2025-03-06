using System ;
using System.Linq ;
using System.Reflection ;
using System.Text ;
using Guartinel.Communication ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;

namespace Guartinel.WatcherServer.Communication.Routes {
   public class GetVersionRoute : Route {

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

      public GetVersionRoute() : base() { }

      public override string Path => WatcherServerAPI.Admin.GetVersion.FULL_URL ;

      protected override void ProcessRequest (Parameters parameters,
                                              Parameters results,
                                              TagLogger logger) {
         Parameters version = new Parameters() ;
         version [WatcherServerAPI.Admin.GetVersion.Response.Version.WATCHER_SERVER_VERSION] = Assembly.GetExecutingAssembly().GetName().Version.ToString() ;

         results.SetChild (WatcherServerAPI.Admin.GetVersion.Response.VERSION, version) ;
         
         results.Success();

         logger.InfoWithDebug ($"Server version returned.", results.AsJObject.ConvertToLog()) ;
      }
   }
}
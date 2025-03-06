using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Communication ;
using Guartinel.Kernel.Logging ;

namespace Guartinel.WatcherServer.Communication.Routes {
   /// <summary>
   /// Store a route: path with action to execute.
   /// </summary>
   public abstract class Route {
      
      //public static class ParameterNames {
      //   public const string TOKEN = "token" ;
      //}

      protected Route() {}      

      public abstract string Path {get ;}

      /// <summary>
      /// Process request for the route.
      /// </summary>
      /// <param name="parameters">Parameters coming from Management Server.</param>
      /// <param name="results">Results of call.</param>
      /// <param name="tags">Logger tags</param>
      /// <returns></returns>
      public void ProcessRequest (Parameters parameters,
                                  Parameters results,
                                  string[] tags) {
         var logger = new TagLogger() ;
         ProcessRequest (parameters, results, logger) ;
      }

      protected abstract void ProcessRequest (Parameters parameters,
                                              Parameters results,
                                              TagLogger logger) ;

      #region Registration
      private static readonly Dictionary<string, Route> _routes = new Dictionary<string, Route>() ;
      
      public static void Register (Route route) {
         if (route == null) return ;

         _routes.Add (route.Path, route) ;
      }

      public static Route Lookup (string path) {
         if (!_routes.ContainsKey (path)) {
            return new DefaultRoute();
         }

         return _routes [path] ;
      }
      #endregion

      protected void CheckToken (Parameters parameters) {
         Tokens.Use().CheckToken (parameters [WatcherServerAPI.GeneralRequest.TOKEN]) ;
      }
   }

   public class DefaultRoute : Route {
      public override string Path => "hello" ;

      protected override void ProcessRequest (Parameters parameters,
                                           Parameters results,
                                           TagLogger logger) {
         results.Data [WatcherServerAPI.Default.Response.CONTENT] = "Hello from Guartinel Watcher Server." ;
      }
   }
}
using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Runtime.InteropServices ;
using System.Text ;
using Guartinel.Communication ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;

namespace Guartinel.WatcherServer.Communication.Routes {
   public class GetEventsRoute : Route {

      //public static class Constants {         
      //   public const string ROUTE = @"admin/getEvents" ;
      //}

      //public new static class ParameterNames {
      //   public const string TOKEN = "token" ;
      //}

      //public static class ResultNames {
      //   public const string EVENTS = "events" ;

      //   public static class Events {
      //      public const string TIME_STAMP = "time_stamp" ;
      //      public const string EVENT = "event" ;
      //   }
      //}

      public override string Path => WatcherServerAPI.Admin.GetEvents.FULL_URL ;

      protected override void ProcessRequest (Parameters parameters,
                                              Parameters results,
                                              TagLogger logger) {
         CheckToken (parameters) ;

         // todo SZTZ: implement!
         var events = new List<Parameters>() ;

         var event1 = new Parameters() ;
         event1 [WatcherServerAPI.Admin.GetEvents.Response.Event.TIME_STAMP] = "2016-01-18T09:06:58.005Z" ; // ISOformat
         event1 [WatcherServerAPI.Admin.GetEvents.Response.Event.EVENT] = "The dog ate my homework" ;
         events.Add (event1) ;

         var event2 = new Parameters() ;
         event2 [WatcherServerAPI.Admin.GetEvents.Response.Event.TIME_STAMP] = "2016-01-18T09:07:58.005Z" ; // ISOformat
         event2 [WatcherServerAPI.Admin.GetEvents.Response.Event.EVENT] = "Nope, he didn't." ;
         events.Add (event2) ;

         results.SetChildren (WatcherServerAPI.Admin.GetEvents.Response.EVENTS, events.Select (x => x.ToString()).ToList()) ;

         results.Success() ;
      }
   }
}
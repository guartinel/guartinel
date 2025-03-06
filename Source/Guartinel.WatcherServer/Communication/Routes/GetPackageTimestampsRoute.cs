using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Communication ;
using Guartinel.Kernel.Entities ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Guartinel.WatcherServer.Packages ;

namespace Guartinel.WatcherServer.Communication.Routes {
   public class GetPackageTimestampsRoute : PackageRoute {

      public GetPackageTimestampsRoute (PackageController packageController) : base (packageController) { }

      public override string Path => WatcherServerAPI.Packages.GetAllWithTimeStamp.FULL_URL ;

      protected override void ProcessRequest (Parameters parameters,
                                              Parameters results,
                                              TagLogger logger) {

         CheckToken (parameters) ;

         IEnumerable<EntityTimestamp> watcherTimestamps = _packageController.GetPackageTimestamps() ;
         List<Parameters> timestamps = new List<Parameters>() ;

         foreach (var watcherTimestamp in watcherTimestamps) {
            Parameters timestamp = new Parameters() ;
            timestamp [WatcherServerAPI.Packages.GetAllWithTimeStamp.Response.Timestamp.PACKAGE_ID] = watcherTimestamp.ID ;
            timestamp [WatcherServerAPI.Packages.GetAllWithTimeStamp.Response.Timestamp.MODIFICATION_TIMESTAMP] =
                     Kernel.Utility.Converter.DateTimeToStringJson (watcherTimestamp.ModificationTimestamp) ;

            timestamps.Add (timestamp) ;
         }

         results.SetChildren (WatcherServerAPI.Packages.GetAllWithTimeStamp.Response.TIMESTAMPS, timestamps) ;

         results.Success() ;

         logger.InfoWithDebug ($"Package timestamps returned. Count: {timestamps.Count}", results.AsJObject.ConvertToLog()) ;
      }
   }
}
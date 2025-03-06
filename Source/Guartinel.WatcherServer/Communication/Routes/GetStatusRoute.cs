using System ;
using System.Globalization ;
using System.Linq ;
using System.Text ;
using Guartinel.Communication ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Guartinel.WatcherServer.Packages ;

namespace Guartinel.WatcherServer.Communication.Routes {
   public class GetStatusRoute : PackageRoute, IDisposable {
      protected PerformanceInfo _performanceInfo ;

      public GetStatusRoute (PackageController packageController) : base (packageController) {
         _performanceInfo = new PerformanceInfo() ;
      }

      private static double NormalizeValue (double value) {
         return Math.Round (value, 2) ;
      }

      private static int NormalizePercent (double value) {
         return (int) Math.Round(value, 0) ;
      }

      public override string Path => WatcherServerAPI.Admin.GetStatus.FULL_URL ;

      protected override void ProcessRequest (Parameters parameters,
                                              Parameters results,
                                              TagLogger logger) {

         CheckToken (parameters) ;
         
         Parameters status = new Parameters() ;

         var cpuLoad = _performanceInfo.GetCPULoad() ;
         var availableMemory = _performanceInfo.GetAvailableMemoryMBs() ;
         var availableMemoryPercents = _performanceInfo.GetAvailableMemoryPercents() ;

         status [WatcherServerAPI.Admin.GetStatus.Response.Status.CPU] = NormalizeValue (cpuLoad).ToString (CultureInfo.InvariantCulture) ;
         status [WatcherServerAPI.Admin.GetStatus.Response.Status.MEMORY] = NormalizeValue(availableMemory).ToString (CultureInfo.InvariantCulture) ;
         status [WatcherServerAPI.Admin.GetStatus.Response.Status.MEMORY_PERCENT] = NormalizePercent (availableMemoryPercents).ToString (CultureInfo.InvariantCulture) ;
         status [WatcherServerAPI.Admin.GetStatus.Response.Status.HDD_FREE_GB] = NormalizeValue(_performanceInfo.GetAvailableDiskGBs()).ToString (CultureInfo.InvariantCulture) ;
         status [WatcherServerAPI.Admin.GetStatus.Response.Status.PACKAGE_COUNT] = _packageController.PackageCount.ToString (CultureInfo.InvariantCulture) ;

         //status [WatcherServerAPI.Admin.GetStatus.Response.Status.AVERAGE_LATENCY] = "1" ;

         // Calculate stress level based on CPU and memory usage
         var memoryUsageFactor = _performanceInfo.GetTotalMemoryMBs() < 1 ? 0.5 : availableMemory / _performanceInfo.GetTotalMemoryMBs() ;
         status [WatcherServerAPI.Admin.GetStatus.Response.Status.STRESS_LEVEL] = ((cpuLoad + memoryUsageFactor) / 2.0).ToString (CultureInfo.InvariantCulture) ;

         results.SetChild (WatcherServerAPI.Admin.GetStatus.Response.STATUS, status) ;

         results.Success() ;

         logger.InfoWithDebug ($"Server status returned.", results.AsJObject.ConvertToLog()) ;
      }

      public void Dispose() {
         _performanceInfo.Dispose() ;
      }
   }
}
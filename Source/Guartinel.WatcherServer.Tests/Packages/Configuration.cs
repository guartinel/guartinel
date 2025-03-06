using System ;
using System.Linq ;
using System.Text ;
using Newtonsoft.Json.Linq ;

namespace Guartinel.WatcherServer.Tests.Packages {
   public static class Configuration {
      public static void ConfigureChecker (TestChecker checker,
                                           int minValue = 10,
                                           int maxValue = 20) {

         checker.Configure ("checker1", "packageID1", "instanceID1", false, minValue, maxValue) ;
      }

      public static void CreatePackageConfiguration (JObject configuration,
                                                     bool success = false,
                                                     int checkerCount = 1,
                                                     int minValue = 10,
                                                     int maxValue = 20) {
         if (configuration == null) return ;

         configuration [TestPackage.Constants.CHECKER_COUNT] = checkerCount ;
         configuration [TestPackage.Constants.SUCCESS] = success ;
         configuration [TestPackage.Constants.MIN_VALUE] = minValue ;
         configuration [TestPackage.Constants.MAX_VALUE] = maxValue ;
      }
   }
}
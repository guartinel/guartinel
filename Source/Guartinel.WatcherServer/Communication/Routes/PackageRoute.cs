using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Communication ;
using Guartinel.Kernel ;
using Guartinel.WatcherServer.Communication.ManagementServer ;
using Guartinel.WatcherServer.Packages ;

namespace Guartinel.WatcherServer.Communication.Routes {
   /// <summary>
   /// Store a route: path with action to execute.
   /// </summary>
   public abstract class PackageRoute : Route {

      //public static class ParameterNames {
      //   public const string TOKEN = "token" ;
      //}

      protected PackageRoute() { }

      #region Package Controller
      protected readonly PackageController _packageController ;

      protected PackageRoute (PackageController packageController) : this() {
         _packageController = packageController ;
      }
      #endregion

      #region Data Store
      protected PackageRoute (PackageController packageController,
                              IMeasuredDataStore measuredDataStore) : this (packageController) {
         _measuredDataStore = measuredDataStore ;
      }

      protected IMeasuredDataStore _measuredDataStore ;
      #endregion
   }
}
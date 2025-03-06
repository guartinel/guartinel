using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.WatcherServer.Alerts ;
using Guartinel.WatcherServer.Instances ;

namespace Guartinel.WatcherServer.Communication.ManagementServer {
   /// <summary>
   /// Access Management Server package-related functions.
   /// </summary>
   public interface IManagementServerPackages {
      /// <summary>
      /// Initiate synchronization on Management Server. Returns immediately, MS starts synchronization later.
      /// </summary>
      void RequestSynchronization() ;

      void StorePackageState (string packageID,
                              string state,
                              XString message,
                              XString messageDetails,
                              InstanceStateList instanceStates = null) ;

      /// <summary>
      /// Delete package states for specified package.
      /// </summary>
      /// <param name="packageID"></param>
      void DeletePackageStates (string packageID) ;

      /// <summary>
      /// Get all related agent IDs for the package.
      /// </summary>
      IList<string> GetApplicationInstanceIDs (string packageID) ;
   }
}
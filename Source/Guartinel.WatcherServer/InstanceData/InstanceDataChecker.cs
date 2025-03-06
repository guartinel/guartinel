using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Guartinel.WatcherServer.Checkers ;
using Guartinel.WatcherServer.CheckResults ;

namespace Guartinel.WatcherServer.InstanceData {
   public enum InstanceDataListCheckKind {
      FailIfLastFails,
      FailIfAnyFails,
      FailIfEveryOneFails
   }

   public abstract class InstanceDataChecker : Checker {
      #region Configuration
      public InstanceDataChecker Configure (string name,
                                            string packageID,
                                            string instanceID,
                                            string instanceName,
                                            Timeout timeout,
                                            List<InstanceData> instanceDataList,
                                            bool isHeartbeat,
                                            InstanceDataListCheckKind checkKind) {
         Configure (name, packageID, instanceID) ;

         InstanceName = instanceName ;
         var instanceDataListClone = new List<InstanceData>() ;
         if (instanceDataList != null) {
            foreach (var instanceData in instanceDataList) {
               instanceDataListClone.Add (instanceData.Duplicate()) ;
            }

            InstanceDataList = instanceDataListClone ;
         }

         _timeout = timeout ;
         _isHeartbeat = isHeartbeat ;
         CheckKind = checkKind ;

         return this ;
      }
      #endregion

      #region Properties
      public string InstanceName {get ; private set ;}

      #endregion

      private List<InstanceData> _instanceDataList = new List<InstanceData>() ;

      protected List<InstanceData> InstanceDataList {
         get => _instanceDataList ;
         set {
            _instanceDataList = value ?? new List<InstanceData>() ;
            if (_instanceDataList.Any()) {
               if (string.IsNullOrEmpty (InstanceID)) {
                  InstanceID = _instanceDataList.Last().ID ;
               }

               if (string.IsNullOrEmpty (InstanceName)) {
                  InstanceName = _instanceDataList.Last().Name ;
               }
            }
         }
      }

      protected Timeout _timeout ;
      protected bool _isHeartbeat ;

      public InstanceDataListCheckKind CheckKind {get ; set ;} = InstanceDataListCheckKind.FailIfLastFails ;

      protected bool CheckTimeout() {
         if (_timeout == null) return false ;

         if (!_timeout.IsStarted) _timeout.Reset() ;

         // No timeout
         if (_timeout.StillOK) return false ;

         Logger.Debug ($"'{InstanceID}' in package '{PackageID}' is timed out. Last life sign found at {Logger.AsString (_timeout.StartTime)}. Timeout is {_timeout.TimeoutInMilliSeconds / 1000} seconds.") ;

         // Timeout!
         // _timeout.Reset() ;
         return true ;
      }

      protected CheckResult CreateUndefined => CheckResult.CreateUndefined (InstanceName) ;
      protected abstract CheckResult CreateTimeout {get ;}

      protected sealed override IList<CheckResult> Check1(string[] tags) {
         var result = Check2(tags) ;
         return new List<CheckResult> {result} ;
      }

      protected virtual CheckResult CheckTimeout1 (string[] tags) {
         // Timeout
         if (CheckTimeout()) {
            var logger = new TagLogger (tags) ;
            logger.Debug ($"'{InstanceID}' is timed out.") ;
            return CreateTimeout ;
         }

         return null ;
      }

      protected CheckResult Check2 (string[] tags) {
         var logger = new TagLogger (tags) ;
         logger.Debug ($"Check request arrived for {InstanceID} in checker '{GetType().Name}'.") ;

         // Invalid
         if (string.IsNullOrEmpty (InstanceID)) {
            return CreateUndefined ;
         }

         // Timeout?
         var timeoutResult = CheckTimeout1(tags) ;
         if (timeoutResult != null) return timeoutResult ;

         // Check if any state is available
         if (InstanceDataList == null || !InstanceDataList.Any()) {
            Logger.Debug ($"No state found for {InstanceID}, return undefined.") ;
            return CreateUndefined ;
         }

         // Check last - if Kind says so
         if (CheckKind == InstanceDataListCheckKind.FailIfLastFails) {
            return Check3 (InstanceDataList.Last(), tags) ;
         }

         CheckResult result = CreateUndefined ;

         // Process all data
         foreach (var instanceData in InstanceDataList) {
            result = Check3 (instanceData, tags) ;
            if (CheckKind == InstanceDataListCheckKind.FailIfAnyFails && result.CheckResultKind.In (CheckResultKind.Fail,
                                                                                                    CheckResultKind.CriticalFail)) {
               return result ;
            }

            if (CheckKind == InstanceDataListCheckKind.FailIfEveryOneFails && result.CheckResultKind == CheckResultKind.Success) {
               return result ;
            }
         }

         return result ;
      }

      protected abstract CheckResult Check3 (InstanceData instanceData,
                                             string[] tags) ;
   }
}
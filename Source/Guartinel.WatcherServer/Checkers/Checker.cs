using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.Kernel.Utility ;
using Guartinel.WatcherServer.CheckResults ;
using Guartinel.WatcherServer.Communication.ManagementServer ;
using Strings = Guartinel.Communication.Strings.Strings ; 

namespace Guartinel.WatcherServer.Checkers {
   public abstract class Checker {
      #region Construction
      protected Checker() {
         //_lastResult = new CheckResult() ;

         Name = string.Empty ;
         // Enabled = true ;
      }

      #endregion

      #region Configuration
      public Checker Configure (string name,
                                string packageID,
                                string instanceID) {
         Name = name ;
         PackageID = packageID ;
         InstanceID = instanceID ;

         // _lastResult = new CheckResult().Configure (Name, CheckResultSuccess.Undefined) ;

         return this ;
      }
      #endregion

      #region Properties
      public string Name {get ; private set ;}
      public string PackageID {get ; private set ;}
      public string InstanceID { get; protected set; }

      //protected override void AddBasicSummary (List<string> result) {
      //   result.Add ($"{HTML.MakeLabel ("Type")}{GetType().Name}") ;
      //   // result.Add (HTML.MakeBold (Enabled ? "Enabled" : "Disabled")) ;
      //}

      public XConstantString.Parameter CreatePackageNameParameter (string packageID) =>
         new XConstantString.Parameter (Strings.Parameters.PackageName, Strings.Lookups.PackageNameFromID, packageID) ;
 
      #endregion

      #region Result
      protected DateTime? _lastCheckExecuted ;
      // private CheckResult _lastResult = new CheckResult().Configure (string.Empty, CheckResultSuccess.Undefined) ;

      // public CheckResult LastResult {get {return _lastResult ;}}
      #endregion

      /// <summary>
      /// Check the instance.
      /// </summary>
      /// <returns></returns>
      public IList<CheckResult> Check (string[] tags) {
         var logger = new TagLogger (tags, InstanceID) ;

         try {
            lock (_isCheckingLock) {
               if (IsChecking) {
                  logger.Debug ("Checker is already running...") ;
                  return new List<CheckResult> {CheckResult.CreateUndefined (Name)} ;
               }

               IsChecking = true ;
            }

            try {
               logger.Debug ("Checker is starting...") ;
               IList<CheckResult> results = Check1 (logger.Tags) ;
               _lastCheckExecuted = DateTime.UtcNow ;
               logger.Debug ("Checker is done.") ;

               return results ;
            } finally {
               lock (_isCheckingLock) {
                  IsChecking = false ;
               }
            }
         } catch (Exception e) {
            logger.Error ($"Error in executing check {Name}. Details: {e.GetAllMessages ()}") ;

            return new List<CheckResult> {
               new CheckResult().Configure (Name, CheckResultKind.Fail,
                                            new XConstantString (Strings.Use.Get (Strings.Messages.Use.ErrorInCheckMessage),
                                                                 new XConstantString.Parameter (Strings.Parameters.CheckName, Name)),
                                            new XSimpleString (e.Message),
                                            new XConstantString (Strings.Use.Get (Strings.Messages.Use.ErrorInCheckExtract)))
            } ;
         }
      }

      private readonly object _isCheckingLock = new object() ;

      public bool IsChecking { get; private set; }

      /// <summary>
      /// Check the instance.
      /// </summary>
      /// <returns></returns>
      protected virtual IList<CheckResult> Check1(string[] tags) {
         return new List<CheckResult> {new CheckResult().Configure (Name, CheckResultKind.Success, null, null, null)} ;
      }

      ///// <summary>
      ///// Define the estimated workload of the checker. By default it is 100.
      ///// </summary>
      ///// <returns></returns>
      //public virtual int CalculateWorkload() {
      //   return Constants.DEFAULT_WORKLOAD ;
      //}

      ///// <summary>
      ///// Event: check failed.
      ///// </summary>
      //public event CheckNotification Failed ;

      //protected virtual void FireFailed (CheckResult checkResult) {
      //   var handler = Failed ;
      //   if (handler != null) {
      //      handler.Invoke (checkResult) ;
      //   }
      //}

      ///// <summary>
      ///// Event: after check.
      ///// </summary>
      //public event CheckNotification AfterCheck ;

      //protected virtual void FireAfterCheck (CheckResult checkResult) {
      //   var handler = AfterCheck ;
      //   if (handler != null) {
      //      handler.Invoke (checkResult) ;
      //   }
      //}
      
      public bool AllowInstanceCheck (List<string> instanceIDs) {
         if (InstanceID == null) return false ;

         if (ForceAllowInstanceCheck1()) return true ;

         if (!instanceIDs.Contains (InstanceID)) return false ;

         return true ;
      }

      public virtual bool ForceAllowInstanceCheck1() {
         return false ;
      }
   }

   public abstract class CheckerWithMeasuredData : Checker {
      protected IMeasuredDataStore _measuredDataStore ;

      protected CheckerWithMeasuredData (IMeasuredDataStore measuredDataStore) {
         _measuredDataStore = measuredDataStore ;
      }
   }
}

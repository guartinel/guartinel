using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel.Entities ;
using Guartinel.Kernel.Logging ;
using Guartinel.WatcherServer.Instances ;

namespace Guartinel.WatcherServer.Alerts {

   public abstract class Alert : Entity {
      public static class Constants {
         public const string CAPTION = "Alert" ;
      }

      #region Construction
      protected Alert() {
         Name = string.Empty ;

         Enabled = true ;

         //_status = new Status<AlertStatusMessage> (new AlertStatusMessage.Ready (this)) ;
         //_status.StatusSet += StatusOnStatusSet ;
      }
      #endregion

      #region Configuration
      public Alert Configure (string name,
                              string packageID,
                              int dummy) {
         Name = name ;         
         PackageID = packageID ;

         // Make alertable!
         // SetState (new AlertState.Ready()) ;

         return this ;
      }

      //protected sealed override void Duplicate1 (Entity target) {
      //   Alert target1 = target.CastTo<Alert>() ;

      //   target1.Name = Name ;

      //   Duplicate2 (target1) ;
      //}

      // protected abstract void Duplicate2 (Alert target) ;
      #endregion
      
      #region Properties
      public string Name {get ; private set ;}
      public string PackageID { get; private set; }
      
      public bool Enabled {get ; set ;}
      #endregion

      public void Request (Instance instance,
                           AlertInfo alertInfo,
                           string[] tags) {
         var logger = new TagLogger (tags) ;

         if (!Enabled) {
            logger.Info( $"Alert {Name} ({GetType().Name}) not enabled.") ;
            return ;
         }

         logger.Info ($"Alert '{alertInfo?.CheckResult?.Name}' requested in package {alertInfo?.PackageID}. " +
                     $"Type: {GetType().Name}. " +
                     $"Message: { alertInfo?.Message}. " + 
                     $"Details: {alertInfo?.Details}. " +
                     $"Extract: {alertInfo?.Extract}. " +
                     $"Package state: {alertInfo?.PackageState}.") ;

         // Debug.WriteLine ($"Fired: {GetState().GetType().Name}, {GetType().Name}, {alertInfo.CheckResult.Message}") ;

         Fire (instance, alertInfo, tags) ;
         AfterFire (instance, alertInfo) ;
      }

      public AlertInfo LastAlertInfo {get ; set ;}

      public void Fire (Instance instance,
                        AlertInfo alertInfo,
                        string[] tags) {
         if (alertInfo == null) return ;

         var logger = new TagLogger(tags);

         Fire1 (instance, alertInfo) ;

         logger.InfoWithDebug ($"{Caption} alert sent. Package: {PackageID}. Instance: {instance?.Identifier}. Kind: {alertInfo.AlertKind}.",
                               $"Message: {alertInfo.Message?.ToJsonString()}") ;

         // Automatic confirm of delivery
         LastAlertInfo = alertInfo ;
         instance?.NotifyAlertDelivery (alertInfo.AlertID, alertInfo.Message) ;
      }

      protected abstract string Caption {get ;}

      protected abstract void Fire1 (Instance instance,
                                     AlertInfo alertInfo) ;

      public virtual void AfterFire (Instance instance,
                                     AlertInfo alertInfo) {}
   }
}

using System ;
using System.Linq ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.WatcherServer.Instances ;

namespace Guartinel.WatcherServer.Alerts {
   public interface IDeviceAlertSender {
      void SendAlertToDevice (string packageID,
                              string instanceID,
                              string alertID,
                              string alertDeviceID,
                              bool isRecovery,
                              bool forcedAlert,
                              string packageState,
                              XString message,
                              XString details) ;
   }

   public class DeviceAlert : Alert {
      public new static class Constants {
         public const string CAPTION = "Device Alert" ;
      }

      #region Construction
      public DeviceAlert() {
         _deviceAlertSender = IoC.Use.Single.GetInstance<IDeviceAlertSender>() ;
         if (_deviceAlertSender == null) {
            throw new Kernel.CoreException("No device alert sender specified.");
         }
      }

      //public static Creator GetCreator() {
      //   return new Creator (typeof (DeviceAlert), () => new DeviceAlert()) ;
      //}
      #endregion

      #region Configuration
      private readonly IDeviceAlertSender _deviceAlertSender ;
      public DeviceAlert Configure (string name,
                                    string packageID,
                                    string alertDeviceID,
                                    int dummy) {
                                    Logger.Log ($"Device alert created. AlertID: {ID} packageID: {packageID}") ;
      
         Configure (name, packageID, 0) ;

         AlertDeviceID = alertDeviceID ;

         return this ;
      }

      //protected override Entity Create() {
      //   return new DeviceAlert() ;
      //}

      //protected override void Duplicate2 (Alert target) {
      //   target.CastTo<DeviceAlert>().Configure (Name, PackageID, AlertDeviceID, 0) ;
      //}
      #endregion

      #region Properties
      public string AlertDeviceID {get ; set ;}
      #endregion

      protected override string Caption => Constants.CAPTION;

      protected override void Fire1 (Instance instance,
                                 AlertInfo alertInfo) {
         _deviceAlertSender.SendAlertToDevice (PackageID, instance?.Identifier, ID, AlertDeviceID,
                                               alertInfo.AlertKind == AlertKind.Recovery,
                                               alertInfo.ForcedDeviceAlert,
                                               alertInfo.PackageState,
                                               alertInfo.Message,
                                               alertInfo.Details) ;
      }
   }
}

using System ;
using Guartinel.Core.Alerts ;
using Guartinel.Core.Factories ;
using Guartinel.Core.Values ;
using Guartinel.WatcherServer.Communication ;
using Sysment.Watcher.WatcherServer ;

namespace Guartinel.WatcherServer.Watcher.Alerts {
   public class DeviceAlert : Alert {

      public new static class Constants {
         public const string CAPTION = "Device Alert" ;
      }

      public new static class PropertyNames {
         public const string DEVICE = "Device" ;
      }

      #region Construction

      public DeviceAlert() {
         DisableConfigure() ;
         try {
            Device = String.Empty ;

         } finally {
            EnableConfigure() ;
         }
      }

      public new static Creator GetCreator() {
         return new Creator (typeof (DeviceAlert), () => new DeviceAlert()) ;
      }

      #endregion

      #region Configuration

      public DeviceAlert Configure (string name,
                                    string device
            ) {
         StartConfigure() ;
         try {
            base.Configure (name) ;

            Device = device ;
         } finally {
            EndConfigure() ;
         }
         return this ;
      }

      #endregion

      #region Properties

      public string Device {
         get {return ((StringValue) Get (PropertyNames.DEVICE, new StringValue())).Value ;}
         set {Set (PropertyNames.DEVICE, new StringValue (value)) ;}
      }

      private IManagementServer _iIhttpRequest ;

      public IManagementServer IhttpRequest {
         set {_iIhttpRequest = value ;}
      }
      #endregion

      protected override void Fire1 (AlertRequest alertRequest) {
         //  MainForm.View.AddMsgToList ("Device alert fired!");
         if (_iIhttpRequest == null) {
            MainForm.View.AddMsgToList ("Cannot send GCM because ManagementServer is null") ;
            return ;
         }
         // todo SZTZ: _iIhttpRequest.SendAlertToDevice(Device, alertRequest.CheckResult.ExtraMessage);
      }
   }
}
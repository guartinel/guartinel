using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.WatcherServer.Instances ;

namespace Guartinel.WatcherServer.Alerts {
   public interface IMailAlertSender {
      void SendAlertMail (string packageID,
                          string instanceID,
                          string alertID,
                          string fromAddress,
                          string toAddress,
                          bool isRecovery,
                          string packageState,
                          XString message,
                          XString details) ;
   }

   public class MailAlert : Alert {
      public new static class Constants {
         public const string CAPTION = "Email Alert" ;
      }

      #region Construction
      public MailAlert() {
         FromAddress = string.Empty ;

         _mailAlertSender = IoC.Use.Single.GetInstance<IMailAlertSender>() ;
      }

      //public static Creator GetCreator() {
      //   return new Creator (typeof (MailAlert), () => new MailAlert()) ;
      //}
      #endregion

      #region Configuration
      private readonly IMailAlertSender _mailAlertSender ;

      public MailAlert Configure (string name,
                                  string packageID,
                                  string fromAddress,
                                  string toAddress) {
         base.Configure (name, packageID, 0) ;
         // @todo SzTZ: add tags
         Logger.Log ($"Mail alert created. AlertID: {ID}. PackageID: {packageID}.") ;
         
         PackageID = packageID ;
         FromAddress = fromAddress ;
         ToAddress = toAddress ;

         return this ;
      }

      public MailAlert Configure (string name,
                                  string packageID,
                                  string toAddress) {
         return Configure (name, packageID, string.Empty, toAddress) ;
      }

      //protected override Entity Create() {
      //   return new MailAlert() ;
      //}

      //protected override void Duplicate2 (Alert target) {
      //   target.CastTo<MailAlert>().Configure (Name, PackageID, FromAddress, ToAddress) ;
      //}
      #endregion

      #region Properties
      public new string PackageID {get ; private set ;}

      public string FromAddress {get ; private set ;}

      private string _toAddress ;
      public string ToAddress {get {return _toAddress ;} private set {_toAddress = value ;}}
      #endregion

      protected override string Caption => $"{Constants.CAPTION} - ({ToAddress})" ;

      protected override void Fire1 (Instance instance,
                                     AlertInfo alertInfo) {
         _mailAlertSender.SendAlertMail (PackageID,
                                     instance?.Identifier,
                                     ID,
                                     FromAddress,
                                     ToAddress,
                                     alertInfo.AlertKind == AlertKind.Recovery,
                                     alertInfo.PackageState,
                                     alertInfo.Message,
                                     alertInfo.Details) ;
      }

      public override void AfterFire (Instance instance,
                                      AlertInfo alertInfo) {
         // Consider as delivered
         // LastAlertInfo = alertInfo ;
         // instance?.NotifyAlertDelivery (alertInfo.AlertID, alertInfo.Message) ;
      }
   }
}

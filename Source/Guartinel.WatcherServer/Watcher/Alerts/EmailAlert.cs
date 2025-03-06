using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Core.Alerts ;
using Guartinel.Core.Factories ;
using Guartinel.Core.Values ;
using Guartinel.WatcherServer.Communication ;
using Sysment.Watcher.WatcherServer ;

namespace Guartinel.WatcherServer.Watcher.Alerts {
   public class EmailAlert : Alert {
      public new static class Constants {
         public const string CAPTION = "Email Alert" ;
      }

      public new static class PropertyNames {
         public const string EMAIL = "Email" ;
      }

      #region Construction

      public EmailAlert() {
         DisableConfigure() ;
         try {
            Email = String.Empty ;
         } finally {
            EnableConfigure() ;
         }
      }

      public new static Creator GetCreator() {
         return new Creator (typeof (EmailAlert), () => new EmailAlert()) ;
      }

      #endregion

      #region Configuration

      public EmailAlert Configure (string email
            ) {
         StartConfigure() ;
         try {
            base.Configure ("Email Alert") ;
            Email = email ;
         } finally {
            EndConfigure() ;
         }

         return this ;
      }

      #endregion

      #region Properties

      public string Email {
         get {return ((StringValue) Get (PropertyNames.EMAIL, new StringValue())).Value ;}
         set {Set (PropertyNames.EMAIL, new StringValue (value)) ;}
      }

      #endregion

      private IManagementServer _managementServer ;

      public IManagementServer ManagementServer {
         set {_managementServer = value ;}
      }

      protected override void Fire1 (AlertRequest alertRequest) {
         if (_managementServer == null) {
            MainForm.View.AddMsgToList ("Cannot send Email because ManagementServer is null") ;
            return ;
         }
         // todo SZTZ: _managementServer.SendEmailAlert(Email, alertRequest.CheckResult.ExtraMessage);
      }
   }
}
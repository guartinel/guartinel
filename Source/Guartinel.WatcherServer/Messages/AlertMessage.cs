using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.WatcherServer.Alerts ;

namespace Guartinel.WatcherServer.Messages {
   public class AlertMessage : Message {
      public new static class ParameterNames {
         public const string ALERT_REQUEST = "AlertRequest" ;
      }

      #region Construction
      public AlertMessage() {
         AlertInfo = new AlertInfo() ;
      }

      //public new static Creator GetCreator() {
      //   return new Creator (typeof (AlertMessage), () => new AlertMessage()) ;
      //}

      #endregion

      #region Configuration

      public AlertMessage Configure (AlertInfo alertInfo) {
         AlertInfo = alertInfo ;

         return this ;
      }

      #endregion
      
      #region Properties

      public AlertInfo AlertInfo {get ; set ;}

      #endregion
   }
}
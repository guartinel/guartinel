using Guartinel.Communication.Strings ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Entities ;
using Guartinel.WatcherServer.CheckResults ;

namespace Guartinel.WatcherServer.Alerts {
   public class AlertInfo : Entity {
      #region Construction
      public AlertInfo() {
         CheckResult = new CheckResult() ;
         Message = null ;
         Details = null;
         Extract = null;
         PackageID = string.Empty ;
         AlertID = string.Empty ;
         PackageState = string.Empty;
         AlertKind = AlertKind.None ;
      }

      //public static Creator GetCreator() {
      //   return new Creator (typeof (AlertInfo), () => new AlertInfo()) ;
      //}
      #endregion

      #region Configuration
      public AlertInfo Configure (CheckResult checkResult,
                                  XString message,
                                  XString details,
                                  XString extract,
                                  string packageID,
                                  string alertID,
                                  AlertKind alertKind,
                                  bool forcedDeviceAlert) {
         CheckResult = checkResult ;
         AlertKind = alertKind ;
         // Alert text
         bool isMessageMissing = message == null || message.IsEmpty ;
         if (isMessageMissing && checkResult?.Message != null) {
            Message = checkResult.Message ;
         } else {
            Message = message ;
         }

         // Alert details
         bool isDetailsMissing = details == null || details.IsEmpty;
         if (isDetailsMissing && checkResult?.Details != null) {
            Details = checkResult.Details;
         } else {
            Details = details ;
         }

         // Alert details
         bool isExtractMissing = extract == null || extract.IsEmpty ;
         if (isExtractMissing && checkResult?.Extract != null) {
            Extract = checkResult.Extract ;
         } else {
            Extract = extract ;
         }

         PackageID = packageID ;
         AlertID = alertID ;
         ForcedDeviceAlert = forcedDeviceAlert ;
         
         return this ;
      }

      //protected override void Configure1 (ConfigurationData configurationData) {
      //   Configure (new CheckResult().Go(x => x.Configure (configurationData [ParameterNames.CHECK_RESULT])),
      //              configurationData [ParameterNames.ALERT_TEXT]) ;
      //}

      //protected override void Duplicate1 (Entity target) {
      //   target.CastTo<AlertInfo>().Configure (CheckResult, AlertText, PackageID, AlertID, IsRecoveryX) ;
      //}

      //protected override Entity Create() {
      //   return new AlertInfo() ;
      //}
      #endregion

      #region Properties
      public CheckResult CheckResult {get ; private set; }
      public XString Message {get ; private set; }
      public XString Details { get; private set; }
      public XString Extract { get; private set; }
      public string PackageID {get ; private set; }
      public string AlertID {get ; private set; }
      public string PackageState { get; set; }
      public AlertKind AlertKind { get; set; }
      public bool ForcedDeviceAlert { get; set; }      
      #endregion
   }
}

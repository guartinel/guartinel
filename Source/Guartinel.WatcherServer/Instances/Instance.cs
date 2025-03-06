using System ;
using System.Collections.Generic ;
using System.Linq ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Logging ;
using Guartinel.WatcherServer.Alerts ;
using Guartinel.WatcherServer.CheckResults ;

namespace Guartinel.WatcherServer.Instances {
   public class Instance {
      public Instance (string identifier) {
         Identifier = identifier ;

         _logger = new TagLogger (identifier, TagLogger.CreateTag()) ;
      }

      public readonly TagLogger _logger ;
      public string Identifier {get ;}

      private readonly object _stateLock = new object() ;
      private InstanceState _state = new InstanceState.Unknown() ;

      public InstanceState State {
         get {
            lock (_stateLock) {
               return _state ;
            }
         }
         set {
            lock (_stateLock) {
               // if (_state.Equals (value)) return ;

               _logger.Debug ($"Set state for instance from {_state?.GetType().Name} to {value.GetType().Name}.") ;

               _state = value ;
            }
         }
      }

      public bool StateIsAlerting => State is InstanceState.Alerting ||
                                     State is InstanceState.Critical ||
                                     State is InstanceState.Warning ;

      public bool StateIsUnknown => State is InstanceState.Unknown ;

      public AlertKind GetAlertKind (string alertID,
                                     string packageID,
                                     CheckResultKind checkResultKind,
                                     XString alertText) {
         AlertKind result ;
         lock (_stateLock) {
            result = _state.GetAlertKind (checkResultKind) ;
         }

         if (result != AlertKind.Alert) return result ;

         // If alert, and already delivered, then do not alert again
         if (_deliveredAlerts.Contains (alertID, alertText)) {
            _logger.Info ($"Instance is already alerted on alert {alertID}. Package: {packageID} Alert text: {alertText}") ;
            return AlertKind.None ;
         }

         _logger.Info($"Instance is NOT alerted on alert {alertID} yet. Package: {packageID} Alert text: {alertText}") ;
         return result ;
      }

      public void AdjustAlertInfo (AlertInfo alertInfo) {
         lock (_stateLock) {
            _state.AdjustAlertInfo (Identifier, alertInfo) ;
         }         
      }

      public void RegisterCheckResult (CheckResult checkResult,
                                       string[] tags) {
         var logger = new TagLogger(_logger.Tags, tags);
         logger.Debug ($"Register check result called, state '{State.Name}', result '{checkResult.CheckResultKind.ToString().ToLowerInvariant()}'.") ;
         State = State.GetStateOnCheckResult (checkResult) ;
         logger.Debug ($"Register check result called, new state '{State.Name}'.") ;
      }

      // Delivered alerts
      private readonly DeliveredAlerts _deliveredAlerts = new DeliveredAlerts() ;

      protected class DeliveredAlerts {
         private readonly Dictionary<string, List<XString>> _data = new Dictionary<string, List<XString>>() ;

         public void Add (string alertID,
                          XString alertText) {
            if (string.IsNullOrEmpty(alertID)) return ;

            // _logger.Log ("DeliveredAlerts.Add  addig alertID" + alertID + " alertText" + alertText) ;
            if (!_data.ContainsKey (alertID)) {
               // _logger.Log ("DeliveredAlerts.Add Creating new alertID collection in the _data dictionary") ;
               _data.Add (alertID, new List<XString>()) ;
               //} else {
               //   _logger.Log ("DeliveredAlerts.Add Dictionary aleady has a collection for alertID") ;
            }
            _data [alertID].Add (alertText ?? new XSimpleString()) ;
         }

         public bool Contains (string alertID,
                               XString alertText) {
            if (string.IsNullOrEmpty (alertID)) return false ;

            if (!_data.ContainsKey (alertID)) {
               // _logger.Log ("DeliveredAlerts.Contains  _data not containing" + alertID + " alertText" + alertText) ;
               return false ;
            }

            if (_data [alertID] == null) return false ;

            XString alertTextToCheck = alertText ?? new XSimpleString() ;
            bool contains = _data [alertID].Any (x => x?.AreConstantPartsIdentical (alertTextToCheck) ?? false) ;

            return contains ;
         }

         public void Remove (string alertID,
                             XString alertText) {
            if (string.IsNullOrEmpty (alertID)) return ;
            if (alertText == null) return ;

            if (!_data.ContainsKey (alertID)) {
               // _logger.Log ("DeliveredAlerts.Remove Data not contains alert id") ;
               return;
            }

            if (!_data [alertID].Contains (alertText)) {
               // _logger.Log ($"DeliveredAlerts.Remove Cannot remove element because it is not existing in collection. AlertID  {alertID} Alert Text {alertText}") ;
               return;
            }

            // _logger.Log ("DeliveredAlerts.Remove  AlertID" + alertID + " alertText" + alertText) ;
            _data[alertID].Remove (alertText) ;
         }

         public void RemoveAlert (string alertID) {
            if (string.IsNullOrEmpty (alertID)) return ;
            if (!_data.ContainsKey (alertID)) {
               // _logger.Log ("Delivered alerts.RemoveAlert Data not contains alert id") ;
               return;
            }
            // _logger.Log ("DeliveredAlerts.RemoveAlert  AlertID " + alertID) ;
            _data.Remove (alertID) ;
         }
      }

      public void AfterAlert (AlertInfo alertInfo) {
         // Needed because this way only alerts which are resolved will be removed from the list
         if (alertInfo.CheckResult.CheckResultKind == CheckResultKind.Success) {
            _deliveredAlerts.RemoveAlert (alertInfo.AlertID) ;
         }
      }

      public void NotifyAlertDelivery (string alertID,
                                       XString message) {
         //  if (!(GetState() is InstanceState.Alerting)) return ;

         _logger.Debug ($"Alert delivery notification. Alert ID: {alertID}. Message: {message}") ;

         if (_deliveredAlerts.Contains (alertID, message)) return ;

         _deliveredAlerts.RemoveAlert (alertID) ;
         _deliveredAlerts.Add (alertID, message) ;
      }
   }
}
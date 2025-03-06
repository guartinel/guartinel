using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Communication ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Configuration ;
using Guartinel.WatcherServer.Alerts ;
using Guartinel.WatcherServer.CheckResults ;
using Strings = Guartinel.Communication.Strings.Strings ;

namespace Guartinel.WatcherServer.Instances {
   public abstract class InstanceState {
      //public static InstanceState Create (string name,
      //                                    XString message,
      //                                    XString details) {
      //   if (string.IsNullOrEmpty (name)) return new OK (message, details) ;

      //   if (string.Equals (name, new Unknown().Name, StringComparison.InvariantCultureIgnoreCase)) return new Unknown (message, details) ;
      //   if (string.Equals (name, new Alerting().Name, StringComparison.InvariantCultureIgnoreCase)) return new Alerting (FailLevel.Alerting, message, details) ;

      //   return new OK (message, details) ;
      //}

      protected InstanceState (ConfigurationData data) {
         Message = XString.FromJsonString (data [WatcherServerAPI.Packages.Save.Request.State.States.STATE_MESSAGE]) ;
         Details = XString.FromJsonString (data [WatcherServerAPI.Packages.Save.Request.State.States.STATE_DETAILS]) ;
         Extract = XString.FromJsonString (data [WatcherServerAPI.Packages.Save.Request.State.States.STATE_EXTRACT]) ;
      }

      protected InstanceState (XString message,
                               XString details,
                               XString extract) {
         Message = message ;
         Details = details ;
         Extract = extract ;
      }

      public static InstanceState Create (ConfigurationData data) {
         var name = data [WatcherServerAPI.Packages.Save.Request.State.States.STATE_NAME] ;

         return Create (name, data) ;
      }

      public static InstanceState Create (string name) {
         return Create (name, new ConfigurationData()) ;
      }

      protected static InstanceState Create (string name,
                                             ConfigurationData data) {
         if (string.IsNullOrEmpty (name)) return new OK (data) ;
         if (string.Equals (name, new Unknown().Name, StringComparison.InvariantCultureIgnoreCase)) return new Unknown (data) ;
         if (string.Equals (name, new Alerting().Name, StringComparison.InvariantCultureIgnoreCase)) return new Alerting (data) ;
         if (string.Equals (name, new Warning().Name, StringComparison.InvariantCultureIgnoreCase)) return new Warning (data) ;
         if (string.Equals (name, new Critical().Name, StringComparison.InvariantCultureIgnoreCase)) return new Critical (data) ;

         return new OK (data) ;
      }

      public abstract string Name {get ;}
      public XString Message {get ;}
      public XString Details {get ;}
      public XString Extract {get ;}

      public virtual AlertKind GetAlertKind (CheckResultKind checkResultKind) {
         switch (checkResultKind) {
            case CheckResultKind.Undefined:
               return AlertKind.None ;

            case CheckResultKind.Success:
               return AlertKind.None ;

            case CheckResultKind.Fail:
               return AlertKind.Alert ;

            case CheckResultKind.WarningFail:
               return AlertKind.Warning ;

            case CheckResultKind.CriticalFail:
               return AlertKind.Critical ;
         }

         return AlertKind.None ;
      }

      public virtual void AdjustAlertInfo (string instanceID,
                                           AlertInfo alertInfo) {
         // Nothing to do here
      }

      public virtual InstanceState GetStateOnCheckResult (CheckResult checkResult) {
         switch (checkResult.CheckResultKind) {
            case CheckResultKind.Undefined:
               return this ;

            case CheckResultKind.Success:
               return new OK (checkResult.Message,
                              checkResult.Details,
                              checkResult.Extract) ;

            case CheckResultKind.Fail:
               return new Alerting (checkResult.Message,
                                    checkResult.Details,
                                    checkResult.Extract) ;

            case CheckResultKind.WarningFail:
               return new Warning (checkResult.Message,
                                   checkResult.Details,
                                   checkResult.Extract) ;

            case CheckResultKind.CriticalFail:
               return new Critical (checkResult.Message,
                                    checkResult.Details,
                                    checkResult.Extract) ;
         }

         return this ;
      }

      public override string ToString() {
         return Name ;
      }

      public class Unknown : InstanceState {
         public Unknown (XString message,
                         XString details,
                         XString extract) : base (message ?? new XConstantString (Strings.Use.Get (Strings.Messages.Use.UnknownStatusMessage)),
                                                  details,
                                                  extract ?? new XConstantString(Strings.Use.Get(Strings.Messages.Use.UnknownStatusExtract))) { }

         public Unknown() : this (null, null, null) { }

         public Unknown (ConfigurationData data) : base (data) { }

         public override string Name => nameof(Unknown).ToLowerInvariant() ;
      }

      public class OK : InstanceState {
         public OK (ConfigurationData data) : base (data) { }

         public OK (XString message,
                    XString details,
                    XString extract) : base (message ?? new XConstantString (Strings.Use.Get (Strings.Messages.Use.OKStatusMessage)),
                                             details,
                                             extract ?? new XConstantString(Strings.Use.Get(Strings.Messages.Use.OKStatusExtract))) { }

         public override string Name => nameof(OK).ToLowerInvariant() ;

         public override InstanceState GetStateOnCheckResult (CheckResult checkResult) {
            switch (checkResult.CheckResultKind) {
               // SZTZ: 2018/06/13: if this runs, then the measured values do not refresh
               //case CheckResultSuccess.Success:
               //   return this ;

               case CheckResultKind.Undefined:
                  // DO NOT allow to go to unknown from OK, need an error - timeout - for that
                  return new Unknown() ;
            }

            return base.GetStateOnCheckResult (checkResult) ;
         }
      }

      public class Alerting : InstanceState {
         public Alerting() : this (null, null, null) { }

         public Alerting (XString message,
                          XString details,
                          XString extract) : base (message ?? new XConstantString (Strings.Use.Get (Strings.Messages.Use.OKStatusMessage)),
                                                   details,
                                                   extract ?? new XConstantString(Strings.Use.Get(Strings.Messages.Use.OKStatusExtract))) { }

         public Alerting (ConfigurationData data) : base (data) { }

         public override string Name => nameof(Alerting).ToLowerInvariant() ;

         public override AlertKind GetAlertKind (CheckResultKind checkResultKind) {
            switch (checkResultKind) {
               case CheckResultKind.Success:
                  return AlertKind.Recovery ;
            }

            return base.GetAlertKind (checkResultKind) ;
         }

         public override void AdjustAlertInfo (string instanceID,
                                               AlertInfo alertInfo) {
            if (alertInfo?.CheckResult?.CheckResultKind == CheckResultKind.Success) {
               alertInfo.AlertKind = AlertKind.Recovery ;
            }
         }
      }

      public class Warning : Alerting {
         public Warning() : this (null, null, null) { }

         public Warning (XString message,
                         XString details,
                         XString extract) : base (message ?? new XConstantString (Strings.Use.Get (Strings.Messages.Use.WarningStatusMessage)),
                                                  details,
                                                  extract ?? new XConstantString(Strings.Use.Get(Strings.Messages.Use.WarningStatusMessage))) { }

         public Warning (ConfigurationData data) : base (data) { }

         public override string Name => nameof(Warning).ToLowerInvariant() ;
      }

      public class Critical : Alerting {
         public Critical() : this (null, null, null) { }

         public Critical (XString message,
                          XString details,
                          XString extract) : base (message ?? new XConstantString (Strings.Use.Get (Strings.Messages.Use.CriticalStatusMessage)),
                                                   details,
                                                   extract ?? new XConstantString(Strings.Use.Get(Strings.Messages.Use.CriticalStatusExtract))) { }

         public Critical (ConfigurationData data) : base (data) { }

         public override string Name => nameof(Critical).ToLowerInvariant() ;
      }
   }
}
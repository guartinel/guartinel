using System.Collections.Generic ;
using Guartinel.Communication ;
using Guartinel.Communication.Strings ;
using Guartinel.Kernel ;
using Guartinel.WatcherServer.Instances ;

namespace Guartinel.WatcherServer.Packages {
   public abstract class PackageState {
      public abstract string StateName {get ;}
      public abstract XString Message {get ;}
      public abstract XString Details { get; }

      public override string ToString() {
         return $"State {StateName}, message: {Message}, details: {Details}" ;
      }

      public static PackageState Create (string name) {
         if (string.IsNullOrEmpty (name)) return new OK();

         if (name.ToLowerInvariant() == new Alerting().StateName) return new Alerting();
         if (name.ToLowerInvariant() == new Unknown().StateName) return new Unknown();
         return new OK() ;
      }

      public class OK : PackageState {
         public override string StateName => ManagementServerAPI.Package.StoreState.Request.State.StateNames.OK ;
         public override XString Message => new XConstantString (Strings.Use.Get (Strings.Messages.Use.OKStatusMessage)) ;
         public override XString Details => new XConstantString(Strings.Use.Get(Strings.Messages.Use.OKStatusDetails));
      }

      public class Unknown : PackageState {
         public override string StateName => ManagementServerAPI.Package.StoreState.Request.State.StateNames.UNKNOWN ;
         public override XString Message => new XConstantString (Strings.Use.Get (Strings.Messages.Use.UnknownStatusMessage)) ;
         public override XString Details => new XConstantString (Strings.Use.Get (Strings.Messages.Use.UnknownStatusDetails)) ;
      }

      public class Alerting : PackageState {
         public Alerting() {
            AggregatedMessages = new XSimpleString (string.Empty) ;
         }

         public override string StateName => ManagementServerAPI.Package.StoreState.Request.State.StateNames.ALERTING ;
         public override XString Message => new XConstantString (Strings.Use.Get (Strings.Messages.Use.AlertStatusMessage)) ;
         public override XString Details => AggregatedMessages ;
         private XString AggregatedMessages {get ; set ;}
         public Alerting AggregateAlertMessages (List<Instance> instances) {
            XString aggregatedAlertMessage = new XConstantString() ;
            foreach (Instance instance in instances) {
               if (instance.StateIsAlerting) {
                  InstanceState.Alerting currentState = (InstanceState.Alerting) instance.State ;
                  aggregatedAlertMessage = XStrings.Append (aggregatedAlertMessage, currentState.Message, true) ;
               }
            }
            AggregatedMessages = XStrings.Append (AggregatedMessages, aggregatedAlertMessage, true) ;
            return this ;
         }
      }
   }
}
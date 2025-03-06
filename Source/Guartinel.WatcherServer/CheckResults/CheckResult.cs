using System ;
using System.Linq ;
using System.Text ;
using Guartinel.Kernel ;
using Guartinel.Kernel.Entities ;
using Guartinel.WatcherServer.Checkers ;

namespace Guartinel.WatcherServer.CheckResults {
   public class CheckResult : Entity, IDuplicable<CheckResult> {
      //public new static class ParameterNames {
      //   public const string NAME = "name" ;
      //   public const string SUCCESS = "success" ;
      //   public const string TIMESTAMP = "timestamp" ;
      //   public const string ALERT_MESSAGE = "message" ;
      //}

      #region Construction
      public CheckResult() {
         Name = string.Empty ;
         CheckResultKind = CheckResultKind.Undefined;
         TimeStamp = DateTime.MinValue ;
         MeasuredData = null ;
      }

      //public static Creator GetCreator() {
      //   return new Creator (typeof (CheckResult), () => new CheckResult()) ;
      //}
      #endregion

      #region Configuration
      public CheckResult Configure (string name,
                                    CheckResultKind checkResultKind,
                                    XString message,
                                    XString details,
                                    XString extract,
                                    DateTime? timeStamp = null,
                                    MeasuredData measuredData = null) {
         Configure (Name, checkResultKind) ;
         
         Message = message ;
         Details = details ;
         Extract = extract ;
         TimeStamp = timeStamp.GetValueOrDefault (DateTime.UtcNow) ;
         MeasuredData = measuredData ;

         return this ;
      }

      public CheckResult Configure (string name,
                                    CheckResultKind checkResultKind) {
         Name = name ;
         CheckResultKind = checkResultKind ;

         return this ;
      }

      public static CheckResult CreateUndefined (string name) {
         return new CheckResult().Configure(name, CheckResultKind.Undefined, null, null, null) ;
      }

      public CheckResult Duplicate() {
         return new CheckResult().Configure (Name, CheckResultKind, Message, Details, Extract, TimeStamp, MeasuredData) ;
      }

      //protected override void Duplicate1 (Entity target) {
      //   target.CastTo<CheckResult>().Configure (Name, Success, TimeStamp, Message, Data) ;
      //}

      //protected override Entity Create() {
      //   return new CheckResult() ;
      //}
      #endregion

      #region Properties
      public string Name {get ; set ;}

      public CheckResultKind CheckResultKind { get ; set ;}
      public DateTime TimeStamp {get ; set ;}
      public XString Message {get ; set ;}
      public XString Details { get; set; }
      public XString Extract { get; set; }
      public MeasuredData MeasuredData {get ; set ;}
      public override string ToString() {
         return $"({TimeStamp}): {CheckResultKind}" ;
      }      
      #endregion
   }
}
